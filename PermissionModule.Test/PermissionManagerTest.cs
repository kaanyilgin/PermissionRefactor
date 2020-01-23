using System;
using System.Collections.Generic;
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
        private IAuthorizationService authorizationService;
        private IPermissionFactory permissionFactory;
        private IPermissionAuthorizer permissionAuthorizer;

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
        [TestCase(ActionEnum.ContextMenuMoreInformation)]
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
            // Arrange
            this.callContext.IsPrivate = true;
            this.property.StatusId = 5;

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
                ActionEnum actionType, bool isPrivate)
        {
            // Arrange
            this.callContext.IsPrivate = isPrivate;

            // Act
            var permissionModels = this.sut.GetPermissions(this.callContext);

            // Assert
            var actionPermission = permissionModels.First(x => x.Action == actionType);
            Assert.That(actionPermission.IsEnabled, Is.EqualTo(!isPrivate));
            Assert.That(actionPermission.IsVisible, Is.EqualTo(!isPrivate));
        }

        [TestCase(ActionEnum.MakeOffer)]
        [TestCase(ActionEnum.MenuScheduleViewing)]
        [TestCase(ActionEnum.CheckBiddingStatus)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        [TestCase(ActionEnum.SendingDocuments)]
        [TestCase(ActionEnum.DeleteProperty)]
        [TestCase(ActionEnum.AddPhoto)]
        public void
            GetPermission_WhenActionTypeDependsOnPropertyStatusIs5_ShouldIsEnabledEqualToFalseAndIsVisibleIsTrue(
                ActionEnum action)
        {
            // Arrange
            this.property.StatusId = 5;
            this.callContext.IsPrivate = false;

            // Act
            var smartSearchPermissions = this.sut.GetPermissions(this.callContext);

            // Assert
            var permission = smartSearchPermissions.FirstOrDefault(x => x.Action == action);
            Assert.That(permission.IsEnabled, Is.False, $"{action.ToString()} IsEnable is not false");
            Assert.That(permission.IsVisible, Is.True, $"{action.ToString()} IsVisible is not true");
        }

        [Test]
        public void GetPermissionSettings_ShouldPropertyServiceGetPropertyByIdCalledOnce()
        {
            // Act
            var smartSearchPermissions = this.sut.GetPermissionSettings(this.callContext);

            // Assert
            this.propertyService.Received(1).GetPropertyById(this.callContext.PropertyId);
        }

        [Test]
        public void GetPermissionSettings_ShouldReturnValueNotNull()
        {
            // Act
            var permissionSettings = this.sut.GetPermissionSettings(this.callContext);

            // Assert
            Assert.That(permissionSettings, Is.Not.Null);
        }

        [Test]
        public void GetPermissionSettings_WhenCallContextIsPrivateIsTrue_ShouldPermissionSettingsIsPrivateEqualToTrue()
        {
            // Arrange
            this.callContext.IsPrivate = true;

            // Act
            var permissionSettings = this.sut.GetPermissionSettings(this.callContext);

            // Assert
            Assert.That(permissionSettings.IsPrivate, Is.True);
        }

        [Test]
        public void GetPermissionSettings_WhenPropertyStatusIs3_ShouldPermissionSettingsPropertyStatusTypeIdEqaulTo3()
        {
            // Arrange
            this.property.StatusId = 3;

            // Act
            var permissionSettings = this.sut.GetPermissionSettings(this.callContext);

            // Assert
            Assert.That(permissionSettings.PropertyStatusTypeId, Is.EqualTo(this.property.StatusId));
        }

        [Test]
        public void
            GetPermissions_WhenActionTypeDependsOnCustomer_ShouldAuthorisationManagerServiceGetPrivilegesByUserCallecOnce()
        {
            // Arrange
            this.callContext.IsPrivate = false;

            // Act
            this.sut.GetPermissions(this.callContext);

            // Assert        
            this.authorizationService.Received(1).GetPrivilegesByUserProperty(this.callContext.LoginName,
                this.callContext.PropertyId,
                PrivilegeCategoryEnum.All);
        }

        [TestCase(ActionEnum.DeleteProperty, true)]
        [TestCase(ActionEnum.DeleteProperty, false)]
        [TestCase(ActionEnum.CheckBiddingStatus, true)]
        [TestCase(ActionEnum.CheckBiddingStatus, false)]
        [TestCase(ActionEnum.SendingDocuments, true)]
        [TestCase(ActionEnum.SendingDocuments, false)]
        [TestCase(ActionEnum.ChangeBiddingPrice, true)]
        [TestCase(ActionEnum.ChangeBiddingPrice, false)]
        [TestCase(ActionEnum.MenuScheduleViewing, true)]
        [TestCase(ActionEnum.MenuScheduleViewing, false)]
        [TestCase(ActionEnum.MakeOffer, true)]
        [TestCase(ActionEnum.MakeOffer, false)]
        public void GetPermissions_WhenActionTypeDependsOnCustomer_ShouldIsEnabledAndIsVisibleIsEqualToIsAuthorized(
            ActionEnum action, bool isAuthorized)
        {
            // Arrange
            this.permissionAuthorizer
                .IsAuthorized(Arg.Any<CallContext>(), Arg.Any<IList<PropertyUserPrivilege>>(), Arg.Any<int>())
                .Returns(isAuthorized);

            // Act
            var smartSearchPermissions = this.sut.GetPermissions(this.callContext);

            // Assert
            var smartSearchPermission = smartSearchPermissions.First(x => x.Action == action);
            Assert.That(smartSearchPermission.IsVisible, Is.EqualTo(isAuthorized),
                $"{action.ToString()} IsVisible is not correct");
            Assert.That(smartSearchPermission.IsEnabled, Is.EqualTo(isAuthorized),
                $"{action.ToString()} IsEnabled is not correct ");
        }

        [TestCase(ActionEnum.MakeOffer)]
        [TestCase(ActionEnum.ChangeBiddingPrice)]
        public void GetPermissions_WhenActionTypeDependsOnIsBiddingLockedAndIsBiddingLockedIsTrue_ShouldIsEnabledAndIsVisibleIsEqualToFalse(
            ActionEnum action)
        {
            // Arrange
            this.property.IsBiddingLocked = true;

            // Act
            var smartSearchPermissions = this.sut.GetPermissions(this.callContext);

            // Assert
            var smartSearchPermission = smartSearchPermissions.First(x => x.Action == action);
            Assert.That(smartSearchPermission.IsVisible, Is.False, $"{action.ToString()} IsVisible is not correct");
            Assert.That(smartSearchPermission.IsEnabled, Is.False, $"{action.ToString()} IsEnabled is not correct ");
        }
        
        private void InitializeVariables()
        {
            this.callContext = new CallContext()
            {
                LoginName = "login-name",
                IsPrivate = false,
                PropertyId = 3
            };
            this.property = new Property();
            this.propertyService = Substitute.For<IPropertyService>();
            this.authorizationService = Substitute.For<IAuthorizationService>();
            this.permissionAuthorizer = Substitute.For<IPermissionAuthorizer>();
            this.permissionFactory = new PermissionFactory(this.permissionAuthorizer);
            this.sut = new PermissionManager(this.propertyService,
                this.authorizationService,
                this.permissionFactory,
                this.permissionAuthorizer);
        }

        private void SetupMocks()
        {
            this.propertyService.GetPropertyById(this.callContext.PropertyId).Returns(this.property);
            this.permissionAuthorizer
                .IsAuthorized(Arg.Any<CallContext>(), Arg.Any<IList<PropertyUserPrivilege>>(), Arg.Any<int>())
                .Returns(true);
        }
    }
}