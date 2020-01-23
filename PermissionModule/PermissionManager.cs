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
			this.ApplyBiddingRelatedPermission(cc.PropertyId, permissionModel);
			return permissionModel;
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

		private void ApplyBiddingRelatedPermission(int propertyId, List<PermissionModel> permissionModel)
		{
			Property property = this.propertyService.GetPropertyById(propertyId);
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