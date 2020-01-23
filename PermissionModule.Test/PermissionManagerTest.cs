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
            Assert.That(actionPermission, Is.Not.Null, $"There is no permission model for action type: {actionEnum}" );
        }

        private void InitializeVariables()
        {
            this.callContext = new CallContext();
            this.property = new Property();
            this.propertyService = Substitute.For<IPropertyService>();
            this.authorisationManagerService = Substitute.For<IAuthorizationService>();
            this.sut = new PermissionManager(this.propertyService, this.authorisationManagerService);
        }
        
        private void SetupMocks()
        {
            this.propertyService.GetPropertyById(this.callContext.PropertyId).Returns(this.property);
        }
    }
}