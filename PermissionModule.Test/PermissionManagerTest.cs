using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace PermissionModule.UnitTest
{
    [TestFixture(Category = "Permission")]
    public class PermissionManagerTest
    {
        private PermissionManager sut;
        private CallContext callContext;
        private IPropertyService propertyService;
        private Property property;
        private IAuthorizationService authorisationManagerService;
        private IPermissionFactory permissionFactory;

        [SetUp]
        public void SetUp()
        {
            this.InitializeVariables();
            this.SetupMocks();
        }

        [Test]
        public void CreatePermissionModel_ShouldPermissionModelHasSameCountAsActionEnumCount()
        {
            // Arrange
            var actionCount = Enum.GetValues(typeof(ActionEnum)).Length;

            // Act
            var permissionModels = this.sut.GetPermissions(this.callContext);

            // Assert
            Assert.That(permissionModels, Has.Count.EqualTo(actionCount));
        }

        [TestCase(ActionEnum.MenuViewProperty)]
        [TestCase(ActionEnum.ShareProperty)]
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        [TestCase(ActionEnum.ContextMenuMoreInformation)]
        [TestCase(ActionEnum.MakeOffer)]
        [TestCase(ActionEnum.AddPhoto)]
        [TestCase(ActionEnum.MenuAskQuestion)]
        [TestCase(ActionEnum.MenuReplyQuestion)]
        [TestCase(ActionEnum.MenuAddFavourites)]
        [TestCase(ActionEnum.MessageToAgent)]
        [TestCase(ActionEnum.ViewPhoneCallOfAgent)]
        [TestCase(ActionEnum.DownloadBrochure)]
        [TestCase(ActionEnum.ViewLocation)]
        [TestCase(ActionEnum.RequestEnergyLabel)]
        [TestCase(ActionEnum.Request360Video)]
        public void CreatePermissionModel_ShouldThereIsAOnePermissionForEveryActionEnum(ActionEnum actionEnum)
        {
            // Act
            var permissionModels = this.sut.GetPermissions(this.callContext);

            // Assert
            var actionPermission = permissionModels.FirstOrDefault(x => x.Action == actionEnum);
            Assert.That(actionPermission, Is.Not.Null, $"There is no permission model for action type: {actionEnum}");
        }

        [TestCase(ActionEnum.MenuViewProperty)]
        [TestCase(ActionEnum.ShareProperty)]
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        [TestCase(ActionEnum.ContextMenuMoreInformation)]
        [TestCase(ActionEnum.AddPhoto)]
        [TestCase(ActionEnum.MenuAskQuestion)]
        [TestCase(ActionEnum.MenuReplyQuestion)]
        [TestCase(ActionEnum.MenuAddFavourites)]
        [TestCase(ActionEnum.MessageToAgent)]
        [TestCase(ActionEnum.ViewPhoneCallOfAgent)]
        [TestCase(ActionEnum.DownloadBrochure)]
        [TestCase(ActionEnum.ViewLocation)]
        [TestCase(ActionEnum.RequestEnergyLabel)]
        [TestCase(ActionEnum.Request360Video)]
        public void CreatePermissionModel_WhenActionEnumHasNoRestriction_ShouldIsVisibleAndIsEnableAlwaysEqualToTrue(
            ActionEnum actionEnum)
        {
            //Arrange
            this.callContext.IsPrivate = true;

            // Act
            var permissionModels = this.sut.GetPermissions(this.callContext);

            // Assert
            var actionPermission = permissionModels.FirstOrDefault(x => x.Action == actionEnum);
            Assert.That(actionPermission.IsEnabled, Is.True, $"Is enabled is not true for action type: {actionEnum}");
            Assert.That(actionPermission.IsVisible, Is.True, $"Is enabled is not true for action type: {actionEnum}");
        }

        [TestCase(ActionEnum.DeleteProperty, true)]
        [TestCase(ActionEnum.DeleteProperty, false)]
        [TestCase(ActionEnum.MakeOffer, true)]
        [TestCase(ActionEnum.MakeOffer, false)]
        [TestCase(ActionEnum.ChangeBiddingPrice, true)]
        [TestCase(ActionEnum.ChangeBiddingPrice, false)]
        [TestCase(ActionEnum.CheckBiddingStatus, true)]
        [TestCase(ActionEnum.CheckBiddingStatus, false)]
        public void
            CreatePermissionModel_WhenActionTypeDependsOnIsPrivate_ShouldIsEnabledAndIsVisibleAreEqualToInverseOfIsPrivate(
                ActionEnum actionType, bool isPublished)
        {
            // Arrange
            this.callContext.IsPrivate = isPublished;

            // Act
            var permissionModels = this.sut.GetPermissions(this.callContext);

            // Assert
            var actionPermission = permissionModels.First(x => x.Action == actionType);
            Assert.That(actionPermission.IsEnabled, Is.EqualTo(!isPublished));
            Assert.That(actionPermission.IsVisible, Is.EqualTo(!isPublished));
        }


        private void InitializeVariables()
        {
            this.callContext = new CallContext();
            this.property = new Property();
            this.propertyService = Substitute.For<IPropertyService>();
            this.authorisationManagerService = Substitute.For<IAuthorizationService>();
            this.permissionFactory = new PermissionFactory();
            this.sut = new PermissionManager(this.propertyService, this.authorisationManagerService,
                this.permissionFactory);
        }

        private void SetupMocks()
        {
            this.propertyService.GetPropertyById(this.callContext.PropertyId).Returns(this.property);
        }
    }
}