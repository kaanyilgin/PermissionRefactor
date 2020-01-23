using System.Collections.Generic;
using PermissionModule.Rule;

namespace PermissionModule.Permissions
{
    public class RestrictedPermission : Permission
    {
        private readonly PermissionSettings values;
        private readonly IEnumerable<IPermissionRule> rules;

        /// <summary>
        /// Test purpose only
        /// </summary>
        internal IEnumerable<IPermissionRule> Rules => this.rules;

        public RestrictedPermission(PermissionSettings values, IEnumerable<IPermissionRule> rules)
        {
            this.values = values;
            this.rules = rules;
        }

        internal void ApplyRules()
        {
            foreach (var permissionRule in this.rules)
            {
                if (this.IsEnabled == false)
                {
                    break;
                }

                this.IsEnabled = permissionRule.IsEnabled(this.values);
            }

            foreach (var permissionRule in this.rules)
            {
                if (this.IsVisible == false)
                {
                    break;
                }

                this.IsVisible = permissionRule.IsVisible(this.values);
            }
        }
    }
}