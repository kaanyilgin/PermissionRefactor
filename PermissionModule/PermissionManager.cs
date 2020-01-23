using System;
using System.Collections.Generic;
using System.Linq;
using PermissionModule.Permissions;

namespace PermissionModule
{
	public class PermissionManager
	{
		private readonly IPropertyService propertyService;
		private readonly IAuthorizationService authorizationService;
		private readonly IPermissionFactory permissionFactory;

		public PermissionManager(IPropertyService propertyService, IAuthorizationService authorizationService, IPermissionFactory permissionFactory)
		{
			this.propertyService = propertyService;
			this.authorizationService = authorizationService;
			this.permissionFactory = permissionFactory;
		}

		public List<PermissionModel> GetPermissions(CallContext cc)
		{
			var permissionModel = this.CreatePermissionModel(cc);
			var property = this.propertyService.GetPropertyById(cc.PropertyId);
			this.ApplyPropertyStatusRelatedPermissions(property.StatusId, permissionModel);
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
			var permissionModels = new List<PermissionModel>();

			foreach (ActionEnum action in Enum.GetValues(typeof(ActionEnum)))
			{
				var permissionModel = CreatePermissionModel(cc, action);
				permissionModels.Add(permissionModel);
			}

			return permissionModels;
		}

		private PermissionModel CreatePermissionModel(CallContext cc, ActionEnum action)
		{
			var permissionSettings = new PermissionSettings()
			{
				IsPrivate = cc.IsPrivate,
				Action = action
			};
			var permission = this.permissionFactory.GetPermission(permissionSettings);

			if (permission is RestrictedPermission)
			{
				var restrictedPermission = (RestrictedPermission)permission;
				restrictedPermission.ApplyRules();
			}
			
			var permissionModel = new PermissionModel
			{
				Action = action,
				IsEnabled = permission.IsEnabled,
				IsVisible = permission.IsVisible
			};
			return permissionModel;
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