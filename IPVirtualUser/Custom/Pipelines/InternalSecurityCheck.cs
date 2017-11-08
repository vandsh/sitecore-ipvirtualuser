using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using IPVirtualUser.Custom.Services;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderLayout;
using Sitecore.Security.Authentication;
using Sitecore.Text;
using Sitecore.Web;
using Log = Sitecore.Diagnostics.Log;
using Sitecore.Pipelines;

using MvcPipeline = Sitecore.Mvc.Pipelines.Request.RequestBegin;
using HttpPipeline = Sitecore.Pipelines.HttpRequest;

namespace IPVirtualUser.Custom.Pipelines
{
    public class InternalSecurityCheck : SecurityCheck
    {
        public string InternalAccessItem { get; set; }
        public string Database { get; set; }

        /*
         * If the current user doesn't have access or is anonymous attempts to create a virtual user, otherwise returns the login page
         * Website will fail if login page doesn't exist
         * 
         */
        public virtual void Process(MvcPipeline.RequestBeginArgs args)
        {
            _process(args);
        }

        public virtual void Process(HttpPipeline.HttpRequestArgs args)
        {
            _process(args);
        }

        private void _process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Profiler.StartOperation("Check security access to page.");

            if (!HasAccess() || Context.User.LocalName.ToLower().Equals("anonymous"))
            {
                CreateVirtualUserIfInternal();
            }

            if (!HasAccess())
            {
                args.AbortPipeline();
                var loginPage = GetLoginPage(Context.Site);
                if (loginPage.Length > 0)
                {
                    var urlString = new UrlString(loginPage);
                    if (Settings.Authentication.SaveRawUrl)
                    {
                        urlString.Append("url", HttpUtility.UrlEncode(Context.RawUrl));
                    }

                    var absolutePath = HttpContext.Current.Request.Url.AbsolutePath;
                    if (!string.IsNullOrEmpty(absolutePath))
                    {
                        urlString["returnUrl"] = absolutePath;
                    }

                    Tracer.Info("Redirecting to login page \"" + loginPage + "\".");

                    WebUtil.Redirect(urlString.ToString(), false);
                }
                else
                {
                    Tracer.Info("Redirecting to error page as no login page was found.");
                    WebUtil.RedirectToErrorPage(
                        "Login is required, but no valid login page has been specified for the site (" + Context.Site.Name +
                        ").", false);
                }
            }

            Profiler.EndOperation();
        }

        /*
         * Utility function for checking if a user is internal. If they are on the internal IP list they get to skip login
         * The internal ip list must have defined roles
         */
        private void CreateVirtualUserIfInternal()
        {
            var internalAccessItem = Sitecore.Data.Database.GetDatabase(Database).GetItem(InternalAccessItem);
            NameValueCollection accessList = WebUtil.ParseUrlParameters(internalAccessItem.Fields["Access List"].Value);
            var virtualUserRoles = internalAccessItem.Fields["Virtual User Roles"].Value;
            var loginService = new LoginService(accessList, virtualUserRoles);
            var internalRangeMatch = loginService.InternalMatchCheck();
            if (!internalRangeMatch.Equals(default(KeyValuePair<string, string>)))
            {
                var virtualUsername = string.Format("vUser\\{0}_{1}", internalRangeMatch.Key, internalRangeMatch.Value.Replace(".", "_"));
                var virtualUser = AuthenticationManager.BuildVirtualUser(virtualUsername, true);

                var virtualRoles = loginService.GetVirtualUserRoles();
                if (!virtualRoles.Any())
                {
                    Log.Error("No Virtual Roles defined", this);
                }

                foreach (var virtualUserRole in virtualRoles)
                {
                    virtualUser.Roles.Add(virtualUserRole);
                }

                AuthenticationManager.LoginVirtualUser(virtualUser);
            }
        }
    }
}