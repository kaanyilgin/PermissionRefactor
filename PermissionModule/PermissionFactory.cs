using System;
using System.Collections.Generic;
using PermissionModule.Permissions;
using PermissionModule.Rule;

namespace PermissionModule
{
    public class PermissionFactory : IPermissionFactory
    {
        private PrivatePermissionRule privatePermissionRule;

        public PermissionFactory()
        {
            this.privatePermissionRule = new PrivatePermissionRule();
        }

        public Permission GetPermission(PermissionSettings permissionSettings)
        {
            var rules = new List<IPermissionRule>();
            
            switch (permissionSettings.Action)
            {
                case ActionEnum.MenuViewProperty:
                case ActionEnum.ShareProperty:
                case ActionEnum.AddPhoto:
                case ActionEnum.MenuAskQuestion:
                case ActionEnum.MenuReplyQuestion:
                case ActionEnum.ContextMenuMoreInformation:
                case ActionEnum.MenuAddFavourites:
                case ActionEnum.MessageToAgent:
                case ActionEnum.ViewPhoneCallOfAgent:
                case ActionEnum.DownloadBrochure:
                case ActionEnum.ViewLocation:
                case ActionEnum.RequestEnergyLabel:
                case ActionEnum.Request360Video:
                case ActionEnum.MenuScheduleViewing:
                case ActionEnum.SendingDocuments:
                    return new NoRestriction();
                case ActionEnum.DeleteProperty:
                case ActionEnum.CheckBiddingStatus:
                case ActionEnum.ChangeBiddingPrice:
                case ActionEnum.MakeOffer:
                    rules.Add(privatePermissionRule);
                    var restrictedPermission = new RestrictedPermission(permissionSettings, rules);
                    return restrictedPermission;
                default:
                    throw new ArgumentOutOfRangeException(nameof(permissionSettings.Action), permissionSettings.Action, "Action enum is out of range");
            }
        }
    }
}