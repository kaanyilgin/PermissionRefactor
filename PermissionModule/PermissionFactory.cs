using System;
using System.Collections.Generic;
using PermissionModule.Permissions;
using PermissionModule.Rule;

namespace PermissionModule
{
    public class PermissionFactory : IPermissionFactory
    {
        private PrivatePermissionRule privatePermissionRule;
        private PropertyStatusPermissionRule propertyStatusRule;
        
        public PermissionFactory()
        {
            this.privatePermissionRule = new PrivatePermissionRule();
            this.propertyStatusRule = new PropertyStatusPermissionRule();
        }

        public Permission GetPermission(PermissionSettings permissionSettings)
        {
            var rules = new List<IPermissionRule>();
            
            switch (permissionSettings.Action)
            {
                case ActionEnum.MenuViewProperty:
                case ActionEnum.ShareProperty:
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
                    return new NoRestriction();
                case ActionEnum.SendingDocuments:
                case ActionEnum.MenuScheduleViewing:
                case ActionEnum.AddPhoto:
                    rules.Add(propertyStatusRule);
                    break;    
                case ActionEnum.DeleteProperty:
                case ActionEnum.CheckBiddingStatus:
                case ActionEnum.ChangeBiddingPrice:
                case ActionEnum.MakeOffer:
                    rules.Add(privatePermissionRule);
                    rules.Add(propertyStatusRule);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(permissionSettings.Action), permissionSettings.Action, "Action enum is out of range");
            }
            
            return new RestrictedPermission(permissionSettings, rules);
        }
    }
}