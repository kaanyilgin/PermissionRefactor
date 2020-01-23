using System;
using PermissionModule.Permissions;

namespace PermissionModule
{
    public class PermissionFactory : IPermissionFactory
    {
        public Permission GetPermission(ActionEnum action, bool isPrivate)
        {
            switch (action)
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
                    var restrictedPermission = new RestrictedPermission();
                    restrictedPermission.IsEnabled = !isPrivate;
                    restrictedPermission.IsVisible = !isPrivate;
                    return restrictedPermission;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "Action enum is out of range");
            }
        }
    }
}