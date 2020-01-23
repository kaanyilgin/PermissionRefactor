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
		
		public PermissionManager(IPropertyService propertyService, IAuthorizationService authorizationService,
			IPermissionFactory permissionFactory)
		{
			this.propertyService = propertyService;
			this.authorizationService = authorizationService;
			this.permissionFactory = permissionFactory;
		}

		public List<PermissionModel> GetPermissions(CallContext cc)
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
					PrivilegeCategoryEnum.All),
				IsBiddingLocked = property.IsBiddingLocked.GetValueOrDefault()
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
	}
}