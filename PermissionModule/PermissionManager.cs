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
		private readonly IPermissionAuthorizer permissionAuthorizer;
		
		public PermissionManager(IPropertyService propertyService, IAuthorizationService authorizationService,
			IPermissionFactory permissionFactory, IPermissionAuthorizer permissionAuthorizer)
		{
			this.propertyService = propertyService;
			this.authorizationService = authorizationService;
			this.permissionFactory = permissionFactory;
			this.permissionAuthorizer = permissionAuthorizer;
		}

		public List<PermissionModel> GetPermissions(CallContext cc)
		{
			var permissionModel = this.CreatePermissionModel(cc);
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
			return this.permissionAuthorizer.IsAuthorized(cc, userPrivilegeList, privilegeId);
		}

		private List<PermissionModel> CreatePermissionModel(CallContext cc)
		{
			var permissionModels = new List<PermissionModel>();
			var permissionSettings = GetPermissionSettings(cc);
			
			foreach (ActionEnum action in Enum.GetValues(typeof(ActionEnum)))
			{
				permissionSettings.Action = action;
				var permissionModel = CreatePermissionModel(permissionSettings);
				permissionModels.Add(permissionModel);
			}

			return permissionModels;
		}

		private PermissionModel CreatePermissionModel(PermissionSettings permissionSettings)
		{
			var permission = this.permissionFactory.GetPermission(permissionSettings);

			if (permission is RestrictedPermission)
			{
				var restrictedPermission = (RestrictedPermission)permission;
				restrictedPermission.ApplyRules();
			}
			
			var permissionModel = new PermissionModel
			{
				Action = permissionSettings.Action,
				IsEnabled = permission.IsEnabled,
				IsVisible = permission.IsVisible
			};
			return permissionModel;
		}

		internal PermissionSettings GetPermissionSettings(CallContext cc)
		{
			var property = this.propertyService.GetPropertyById(cc.PropertyId);
			var permissionSettings = new PermissionSettings()
			{
				IsPrivate = cc.IsPrivate,
				PropertyStatusTypeId = property.StatusId,
				CallContext = cc,
				PrivilegesByUserRoom = this.authorizationService.GetPrivilegesByUserProperty(cc.LoginName, cc.PropertyId,
					PrivilegeCategoryEnum.All)
			};
			return permissionSettings;
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