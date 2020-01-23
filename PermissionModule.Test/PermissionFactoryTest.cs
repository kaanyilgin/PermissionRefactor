using NUnit.Framework;
using PermissionModule.Permissions;

namespace PermissionModule.UnitTest
{
    [TestFixture(Category = "Permission")]
    public class PermissionFactoryTest
    {
        private PermissionFactory sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.sut = new PermissionFactory();
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
            // Act
            var permission = this.sut.GetPermission(actionType, false);

            // Assert
            Assert.That(permission, Is.InstanceOf<NoRestriction>(), $"{actionType.ToString()} action type is not NoRestriction type");
        }
        
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.MakeOffer)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        public void GetPermissionModel_WhenActionTypeIsRestricted_shouldPermissionTypeEqualToRestricted(ActionEnum actionType)
        {
            // Act
            var permission = this.sut.GetPermission(actionType, true);

            // Assert
            Assert.That(permission, Is.InstanceOf<RestrictedPermission>(), $"{actionType.ToString()} action type is not RestrictedPermission type");
        }
    }
}