using System;
using System.Collections.Generic;
using System.Linq;

namespace PermissionModule
{
    public class PermissionManager
	{
		private readonly IPropertyService propertyService;
		private readonly IAuthorizationService authorizationService;

		public PermissionManager(IPropertyService propertyService, IAuthorizationService authorizationService)
		{
			this.propertyService = propertyService;
			this.authorizationService = authorizationService;
		}

		public List<PermissionModel> GetPermissions(CallContext cc)
		{
			// var property = this.propertyService.GetPropertyById(cc.PropertyId);
			var permissionModel = this.CreatePermissionModel(cc);
			// this.ApplyPropertyStatusRelatedPermissions(property.StatusId, permissionModel);
			// this.ApplyCostumerRelatedPermissions(cc, permissionModel);
			// this.ApplyBiddingRelatedPermission(property, permissionModel);
			return permissionModel;
		}

		private void ApplyCostumerRelatedPermissions(CallContext cc, List<PermissionModel> permissionModel)
		{
			var loggedInUserPrivilegeList = this.authorizationService.GetPrivilegesByUserProperty(cc.LoginName, cc.PropertyId, PrivilegeCategoryEnum.All);

			// when we are going to apply change bidding we need to check the "isChangeBiddingRelatedProperty" as well
			var isChangeBiddingPriceGranted = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)AgentPrivilege.ChangeBiddingPrice);
			var isChangeBiddingRelatedProperty = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)AgentPrivilege.ChangeBiddingRelatedProperty);

			foreach (var item in permissionModel)
			{
				switch (item.Action)
				{
					case ActionEnum.MenuViewProperty:
						break;
					case ActionEnum.ShareProperty:
						break;
					case ActionEnum.DeleteProperty:
						var isAuthorizedToDeleteProperty = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)CostumerPrivilege.DeleteProperty);
						item.IsEnabled &= isAuthorizedToDeleteProperty;
						item.IsVisible &= isAuthorizedToDeleteProperty;
						break;
					case ActionEnum.CheckBiddingStatus:
						var isScheduleViewingAvailable = (this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)CostumerPrivilege.ScheduleViewing) |
									 this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)CostumerPrivilege.MakeOffer));
						item.IsEnabled &= isScheduleViewingAvailable;
						item.IsVisible &= isScheduleViewingAvailable;
						break;
					case ActionEnum.SendingDocuments:
						var isAuthorizedToSendingDocument = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)PropertyPrivilege.SendDocument);
						item.IsEnabled &= isAuthorizedToSendingDocument;
						item.IsVisible &= isAuthorizedToSendingDocument;
						break;
					case ActionEnum.ChangeBiddingPrice:
						item.IsEnabled &= isChangeBiddingPriceGranted;
						item.IsVisible &= isChangeBiddingPriceGranted;
						break;
					case ActionEnum.MenuScheduleViewing:
						var isAuthorizedToScheduleViewing = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)CostumerPrivilege.ScheduleViewing);
						item.IsEnabled &= isAuthorizedToScheduleViewing;
						item.IsVisible &= isAuthorizedToScheduleViewing;
						break;
					case ActionEnum.ContextMenuMoreInformation:
						break;
					case ActionEnum.MakeOffer:
						var isAuthorizedToMakeOffer = this.IsAuthorized(cc, loggedInUserPrivilegeList, (int)CostumerPrivilege.MakeOffer);
						item.IsEnabled &= isAuthorizedToMakeOffer;
						item.IsVisible &= isAuthorizedToMakeOffer;
						break;
					case ActionEnum.AddPhoto:
						break;
					case ActionEnum.MenuAskQuestion:
						break;
					case ActionEnum.MenuReplyQuestion:
						break;
					case ActionEnum.MenuAddFavourites:
						break;
					case ActionEnum.MessageToAgent:
						break;
					case ActionEnum.ViewPhoneCallOfAgent:
						break;
					case ActionEnum.DownloadBrochure:
						break;
					case ActionEnum.ViewLocation:
						break;
					case ActionEnum.RequestEnergyLabel:
						break;
					case ActionEnum.Request360Video:
						break;
				}
			}
		}

		private bool IsAuthorized(CallContext cc, List<PropertyUserPrivilege> userPrivilegeList, int privilegeId)
		{
			var isAuthorized = false;

			if (cc.IsSpecialCustomer)
			{
				isAuthorized = userPrivilegeList.Any(privilege => (privilege.PrivilegeId == privilegeId));
			}
			else
			{
				isAuthorized = userPrivilegeList.Any(privilege => (privilege.PrivilegeId == privilegeId) && ((privilege.PropertyId == cc.PropertyId) || privilege.IsSystemPrivilege.Value));
			}

			return isAuthorized;
		}

		private List<PermissionModel> CreatePermissionModel(CallContext cc)
		{
			const bool DEFAULT_IS_ENABLED_VALUE = true;
			const bool DEFAULT_IS_VISIBLE_VALUE = true;

			var permissions = new List<PermissionModel>();

			foreach (ActionEnum action in Enum.GetValues(typeof(ActionEnum)))
			{
				var permission = new PermissionModel
				{
					Action = action,
					IsEnabled = this.ApplyPropertyTypeRelatedPermissions(cc, 2, action,
						DEFAULT_IS_ENABLED_VALUE),
					IsVisible = this.ApplyPropertyTypeRelatedPermissions(cc, 1, action,
						DEFAULT_IS_VISIBLE_VALUE)
				};
				permissions.Add(permission);
			}

			return permissions;
		}
		
		private void ApplyPropertyStatusRelatedPermissions(int statusId, IEnumerable<PermissionModel> permissionModel)
		{

			switch (statusId)
			{
				case 1:
					// Status is "Initial".
					break;
				case 2:
					// Status is "Customers are waiting".
					break;
				case 3:
					// Status is "Viewing".
					break;
				case 4:
					// Status is "Bid is happening".
					break;
				case 5:
					// Status is "Bid Finished".

					// Context menu Make Offer Action
					permissionModel.First(t => t.Action == ActionEnum.MakeOffer).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.MakeOffer).IsEnabled = false;

					// Context menu Schedule Viewing Action
					permissionModel.First(t => t.Action == ActionEnum.MenuScheduleViewing).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.MenuScheduleViewing).IsEnabled = false;

					// Context menu Check Bidding Status Action
					permissionModel.First(t => t.Action == ActionEnum.CheckBiddingStatus).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.CheckBiddingStatus).IsEnabled = false;

					// Context menu Change Bidding Price Action
					permissionModel.First(t => t.Action == ActionEnum.ChangeBiddingPrice).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.ChangeBiddingPrice).IsEnabled = false;

					// Context menu Sending Documents Action
					permissionModel.First(t => t.Action == ActionEnum.SendingDocuments).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.SendingDocuments).IsEnabled = false;

					// Context menu Delete Property Action
					permissionModel.First(t => t.Action == ActionEnum.DeleteProperty).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.DeleteProperty).IsEnabled = false;

					// Context menu Add Photo Action
					permissionModel.First(t => t.Action == ActionEnum.AddPhoto).IsEnabled = false;
					permissionModel.First(t => t.Action == ActionEnum.AddPhoto).IsEnabled = false;
					break;
			}
		}

		private bool ApplyPropertyTypeRelatedPermissions(CallContext cc, int toApplayFor, ActionEnum actionEnum, bool currentValue = true)
		{
			//ToApplayFor = 1 : IsVisible
			//ToApplayFor = 2 : IsEnabled
			var result = true;
			switch (actionEnum)
			{
				case ActionEnum.MenuViewProperty:
					break;
				case ActionEnum.ShareProperty:
					break;
				case ActionEnum.DeleteProperty:
					result = !cc.IsPrivate;
					break;
				case ActionEnum.CheckBiddingStatus:
					result = !cc.IsPrivate;
					break;
				case ActionEnum.SendingDocuments:
					break;
				case ActionEnum.ChangeBiddingPrice:
					result = !cc.IsPrivate;
					break;
				case ActionEnum.MenuScheduleViewing:
					break;
				case ActionEnum.ContextMenuMoreInformation:
					break;
				case ActionEnum.MakeOffer:
					result = !cc.IsPrivate;
					break;
				case ActionEnum.AddPhoto:
					break;
				case ActionEnum.MenuAskQuestion:
					break;
				case ActionEnum.MenuReplyQuestion:
					break;
				case ActionEnum.MenuAddFavourites:
					break;
				case ActionEnum.MessageToAgent:
					break;
				case ActionEnum.ViewPhoneCallOfAgent:
					break;
				case ActionEnum.DownloadBrochure:
					break;
				case ActionEnum.ViewLocation:
					break;
				case ActionEnum.RequestEnergyLabel:
					break;
				case ActionEnum.Request360Video:
					break;
			}
			return result & currentValue;
		}

		private void ApplyBiddingRelatedPermission(Property property, List<PermissionModel> permissionModel)
		{
			if (property.IsBiddingLocked.GetValueOrDefault() == false)
			{
				return;
			}

			var biddingRelatedActions = permissionModel.Where(x => x.Action == ActionEnum.ChangeBiddingPrice
																		 || x.Action == ActionEnum.MakeOffer);

			if (biddingRelatedActions == null || biddingRelatedActions.Any() == false)
			{
				return;
			}

			foreach (var action in biddingRelatedActions)
			{
				action.IsEnabled = false;
				action.IsVisible = false;
			}
		}
	}
}