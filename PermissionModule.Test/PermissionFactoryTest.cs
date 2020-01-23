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
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.sut = new PermissionFactory();
        }

        [SetUp]
        public void SetUp()
        {
            this.permissionSettings = new PermissionSettings();
        }
        
        [TestCase(ActionEnum.MenuViewProperty)]
        [TestCase(ActionEnum.ShareProperty)]
        [TestCase(ActionEnum.AddPhoto)]
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
    }
}