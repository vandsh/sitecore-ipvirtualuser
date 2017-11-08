using System.Collections.Generic;
using System.Linq;
using Sitecore.Security.Accounts;

/// <summary>
/// Assisted on this control with: http://sitecoreblog.blogspot.com/2012/04/how-to-create-custom-multilist-field.html
/// </summary>
namespace IPVirtualUser.Custom.Controls
{
    public static class RoleSelectorExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePair(this IEnumerable<Role> roles)
        {
            return roles.Select(r => new KeyValuePair<string, string>(r.LocalName, r.DisplayName + (r.IsEveryone ? " - (everyone)" : string.Empty)));
        }
    }

    public class RoleSelector : MultilistExBase
    {
        protected override void InitRendering()
        {
            // You can Initialise some variables here.
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetNonSelectedItems()
        {
            // Return here your unselected items. First value is the ID you will store into your field, the second one is the display text.
            var retVal = RolesInRolesManager.GetAllRoles().ToKeyValuePair();
            return retVal;
        }

        protected override IEnumerable<KeyValuePair<string, string>> GetSelectedItems()
        {
            // Return here your selected items. First value is the ID you will store into your field, the second one is the display text.
            var selectedRoles = Value.Split('|');
            var retVal = RolesInRolesManager.GetAllRoles().Where(rl => selectedRoles.Contains(rl.LocalName)).ToKeyValuePair();
            return retVal;
        }
    }
}