using NSubstitute;
using NUnit.Framework;
using PermissionModule.Permissions;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest
{
    [TestFixture(Category = "Permission")]
    public class PermissionFactoryTest
    {
        private PermissionFactory sut;
        private PermissionSettings permissionSettings;
        private IPermissionAuthorizer permissionAuthorizer;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.permissionAuthorizer = Substitute.For<IPermissionAuthorizer>();
            this.sut = new PermissionFactory(this.permissionAuthorizer);
        }

        [SetUp]
        public void SetUp()
        {
            this.permissionSettings = new PermissionSettings();
        }
        
        [TestCase(ActionEnum.MenuViewProperty)]
        [TestCase(ActionEnum.ShareProperty)]
        [TestCase(ActionEnum.MenuAskQuestion)]
        [TestCase(ActionEnum.MenuReplyQuestion)]
        [TestCase(ActionEnum.ContextMenuMoreInformation)]
        [TestCase(ActionEnum.MenuAddFavourites)]
        [TestCase(ActionEnum.MessageToAgent)]
        [TestCase(ActionEnum.ViewPhoneCallOfAgent)]
        [TestCase(ActionEnum.DownloadBrochure)]
        [TestCase(ActionEnum.ViewLocation)]
        [TestCase(ActionEnum.RequestEnergyLabel)]
        [TestCase(ActionEnum.Request360Video)]
        public void GetPermissionModel_WhenActionIsAlwaysVisibleAndEnable_ShouldPermissionTypeEqualToNoRestriction(ActionEnum actionType)
        {
            // Arrange
            this.permissionSettings.IsPrivate = false;
            this.permissionSettings.Action = actionType;
            
            // Act
            var permission = this.sut.GetPermission(this.permissionSettings);

            // Assert
            Assert.That(permission, Is.InstanceOf<NoRestriction>(), $"{actionType.ToString()} action type is not NoRestriction type");
        }
        
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.MakeOffer)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.AddPhoto)]
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        public void GetPermissionModel_WhenActionTypeIsRestricted_shouldPermissionTypeEqualToRestricted(ActionEnum actionType)
        {
            // Arrange
            this.permissionSettings.IsPrivate = true;
            this.permissionSettings.Action = actionType;
            
            // Act
            var permission = this.sut.GetPermission(this.permissionSettings);

            // Assert
            Assert.That(permission, Is.InstanceOf<RestrictedPermission>(), $"{actionType.ToString()} action type is not RestrictedPermission type");
        }
        
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MakeOffer)]
        public void GetPermissionModel_WhenActionTypeIsRelatedWithPrivateProperty_ShouldHasPrivatePermissionRuleType(ActionEnum actionType)
        {
            // Arrange
            this.permissionSettings.Action = actionType;
            
            // Act
            var permission = this.sut.GetPermission(this.permissionSettings);

            // Assert
            RestrictedPermission restrictedPermission = (RestrictedPermission)permission;
            Assert.That(restrictedPermission.Rules, Has.Some.TypeOf<PrivatePermissionRule>(), $"{actionType.ToString()} action type doesnt have PermissionRuleModel");
        }
        
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        [TestCase(ActionEnum.AddPhoto)]
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MakeOffer)]
        public void GetPermissionModel_WhenActionTypeIsRelatedPropertyStatusRule_ShouldPermissionHasPropertyStatusRule(ActionEnum action)
        {
            // Arrange
            this.permissionSettings.Action = action;
            
            // Act
            var permission = this.sut.GetPermission(this.permissionSettings);

            // Assert
            RestrictedPermission restrictedPermission = (RestrictedPermission)permission;
            Assert.That(restrictedPermission.Rules, Has.Some.TypeOf<PropertyStatusPermissionRule>(), $"{action.ToString()} action type doesnt have RoomStatusPermissionRule");
        }
        
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        [TestCase(ActionEnum.MakeOffer)]
        public void GetPermissionModel_WhenActionTypeHasCustomerPermissionRule_ShouldPermissionRuleHasGroupPermissionRule(ActionEnum action)
        {
            // Arrange
            this.permissionSettings.Action = action;
			
            // Act
            var permissionModel = this.sut.GetPermission(this.permissionSettings);

            // Assert
            RestrictedPermission restrictedPermission = (RestrictedPermission)permissionModel;
            Assert.That(restrictedPermission.Rules, Has.Some.TypeOf<CostumerPermissionRule>(), $"{action.ToString()} action type doesnt have GroupPermissionRuleTest");
        }
    }
}