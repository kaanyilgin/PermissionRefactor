using System;
using System.Collections.Generic;
using PermissionModule.Permissions;
using PermissionModule.Rule;

namespace PermissionModule
{
    public class PermissionFactory : IPermissionFactory
    {
        private readonly IPermissionAuthorizer permissionAuthorizer;
        private PrivatePermissionRule privatePermissionRule;
        private PropertyStatusPermissionRule propertyStatusRule;
        private CostumerPermissionRule costumerPermissionRule;
        private BiddingLockedPermissionRule biddingLockedPermissionRule;

        public PermissionFactory(IPermissionAuthorizer permissionAuthorizer)
        {
            this.permissionAuthorizer = permissionAuthorizer;
            this.privatePermissionRule = new PrivatePermissionRule();
            this.propertyStatusRule = new PropertyStatusPermissionRule();
            this.costumerPermissionRule = new CostumerPermissionRule(this.permissionAuthorizer);
            this.biddingLockedPermissionRule = new BiddingLockedPermissionRule();
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
                    rules.Add(this.propertyStatusRule);
                    rules.Add(this.costumerPermissionRule);
                    break;    
                case ActionEnum.AddPhoto:
                    rules.Add(this.propertyStatusRule);
                    break;    
                case ActionEnum.DeleteProperty:
                case ActionEnum.CheckBiddingStatus:
                    rules.Add(this.privatePermissionRule);
                    rules.Add(this.propertyStatusRule);
                    rules.Add(this.costumerPermissionRule);
                    break;
                case ActionEnum.ChangeBiddingPrice:
                case ActionEnum.MakeOffer:
                    rules.Add(this.privatePermissionRule);
                    rules.Add(this.propertyStatusRule);
                    rules.Add(this.costumerPermissionRule);
                    rules.Add(this.biddingLockedPermissionRule);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(permissionSettings.Action), permissionSettings.Action, "Action enum is out of range");
            }
            
            return new RestrictedPermission(permissionSettings, rules);
        }
    }
}