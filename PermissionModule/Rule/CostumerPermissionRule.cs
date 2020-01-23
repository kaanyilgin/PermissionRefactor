using System.Collections.Generic;

namespace PermissionModule.Rule
{
    public class CostumerPermissionRule : IPermissionRule
    {
        private readonly IPermissionAuthorizer permissionAuthorizer;

        public CostumerPermissionRule()
        {
			
        }
		
        public CostumerPermissionRule(IPermissionAuthorizer permissionAuthorizer)
        {
            this.permissionAuthorizer = permissionAuthorizer;
        }

        public bool IsEnabled(PermissionSettings permissionSettings)
        {
            return this.IsVisibleAndEnabled(permissionSettings);
        }

        public bool IsVisible(PermissionSettings permissionSettings)
        {
            return this.IsVisibleAndEnabled(permissionSettings);
        }

        internal virtual bool IsVisibleAndEnabled(PermissionSettings permissionSettings)
        {
            var privilegeIds = GetActionPrivilegeIds(permissionSettings);

            foreach (var privilegeId in privilegeIds)
            {
                var isAuthorized = this.permissionAuthorizer.IsAuthorized(permissionSettings.CallContext, permissionSettings.PrivilegesByUserRoom, privilegeId);
				
                if (isAuthorized)
                {
                    return true;
                }
            }

            return false;
        }

        internal static IList<int> GetActionPrivilegeIds(PermissionSettings permissionSettings)
        {
            var privilegeIds = new List<int>();

            if (permissionSettings.Action == ActionEnum.DeleteProperty)
            {
                privilegeIds.Add((int) CostumerPrivilege.DeleteProperty);
            }
            else if (permissionSettings.Action == ActionEnum.SendingDocuments)
            {
                privilegeIds.Add((int) PropertyPrivilege.SendDocument);
            }
            else if (permissionSettings.Action == ActionEnum.CheckBiddingStatus)
            {
                privilegeIds.Add((int) CostumerPrivilege.ScheduleViewing);
                privilegeIds.Add((int) CostumerPrivilege.MakeOffer);
            }
            else if (permissionSettings.Action == ActionEnum.ChangeBiddingPrice)
            {
                privilegeIds.Add((int) AgentPrivilege.ChangeBiddingPrice);
            }
            else if (permissionSettings.Action == ActionEnum.MenuScheduleViewing)
            {
                privilegeIds.Add((int) CostumerPrivilege.ScheduleViewing);
            }
            else if (permissionSettings.Action == ActionEnum.MakeOffer)
            {
                privilegeIds.Add((int) CostumerPrivilege.MakeOffer);
            }

            return privilegeIds;
        }
    }
}