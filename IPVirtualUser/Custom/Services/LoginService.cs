using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using NetTools;
using Sitecore.Security.Accounts;
using Sitecore.StringExtensions;

namespace IPVirtualUser.Custom.Services
{
    public class LoginService
    {
        public NameValueCollection AccessList { get; set; }
        public string VirtualUserRoles { get; set; }

        public LoginService(NameValueCollection accessList, string virtualuserRoles)
        {
            AccessList = accessList;
            VirtualUserRoles = virtualuserRoles;
        }

        /// <summary>
        /// Takes the current request IP and compares it to the list in the InternalAccessItem
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, string> InternalMatchCheck()
        {
            
            var context = HttpContext.Current;
            if (context == null || context.Request == null || context.Request.UserHostAddress == null)
            {
                throw new NullReferenceException("HttpContext.Current, HttpContext.Current.Request, or HttpContext.Current.Request.UserHostAddress is null.");
            }
            var currentIpAddress = IPAddress.Parse(context.Request.UserHostAddress);
            //Use HTTP_X_FORWARDED_FOR if available else use HostAddress
            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                currentIpAddress = IPAddress.Parse(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                Sitecore.Diagnostics.Log.Info("User HTTP_X_FORWARDED_FOR: " + currentIpAddress, "LoginService");
            }


            var retVal = new KeyValuePair<string, string>();
            
            Sitecore.Diagnostics.Log.Info("User IP address: " + currentIpAddress, "LoginService");

            // get the internal access setting item
            //var internalAccessSettingItem = new SitecoreContext().GetItem<Internal_Access_Setting_Item>(ItemIds.InternalAccessList.ToString());
            if (AccessList != null)
            {
                var parsedIpDictionary = AccessList.AllKeys.ToDictionary(k => k, k => IPAddressRange.Parse(AccessList[k]));
                var matchedIpRange = parsedIpDictionary.FirstOrDefault(ipr => ipr.Value.Contains(currentIpAddress));
                if (!matchedIpRange.Key.IsNullOrEmpty())
                {
                    retVal = new KeyValuePair<string, string>(matchedIpRange.Key, currentIpAddress.ToString());
                }
            }

            // if IP is in the settings item, return true
            return retVal;
        }

        /// <summary>
        /// Pulls the selected Virtual User Roles from the InternalAccessItem 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public IEnumerable<Role> GetVirtualUserRoles()
        {
            var retVal = new List<Role>();
            if (VirtualUserRoles != null && !VirtualUserRoles.IsNullOrEmpty())
            {
                var selectedRoles = VirtualUserRoles.Split('|');
                retVal = RolesInRolesManager.GetAllRoles().Where(rl => selectedRoles.Contains(rl.LocalName)).ToList();
            }

            return retVal;
        }
    }
}