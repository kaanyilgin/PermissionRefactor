using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class CustomerPermissionRuleTest
    {
        private CostumerPermissionRule sut;
        private IPermissionAuthorizer permissionAuthorizer;
        private CallContext callContext;
        private List<PropertyUserPrivilege> roomUserPrivileges;
        private PermissionSettings permissionSettings;

        [SetUp]
        public void SetUp()
        {
            InitializeVariables();
        }

        [TestCase(ActionEnum.DeleteProperty, (int) CostumerPrivilege.DeleteProperty)]
        [TestCase(ActionEnum.SendingDocuments, (int) PropertyPrivilege.SendDocument)]
        [TestCase(ActionEnum.ChangeBiddingPrice, (int) AgentPrivilege.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MenuScheduleViewing, (int) CostumerPrivilege.ScheduleViewing)]
        [TestCase(ActionEnum.MakeOffer, (int) CostumerPrivilege.MakeOffer)]
        public void IsVisibleAndEnabled_ShouldPermissionAuthorizerIsAuthorizedCalledOnce(ActionEnum action,
            int privilegeId)
        {
            // Arrange
            this.permissionSettings.Action = action;

            // Act
            this.sut.IsVisibleAndEnabled(this.permissionSettings);

            // Assert
            this.permissionAuthorizer.Received(1).IsAuthorized(this.permissionSettings.CallContext,
                this.roomUserPrivileges, privilegeId);
        }

        [Test]
        public void
            IsVisibleAndEnabled_WhenActionIsContextMenuPasteToAndPrivilegeIsCopyItemsIsAuthorized_ShouldPermissionAuthorizerIsAuthorizedCalledOnceWithCopyItemsPrivilege()
        {
            // Arrange
            this.permissionSettings.Action = ActionEnum.CheckBiddingStatus;

            // Act
            this.sut.IsVisibleAndEnabled(this.permissionSettings);

            // Assert
            this.permissionAuthorizer.Received(1).IsAuthorized(this.permissionSettings.CallContext,
                this.roomUserPrivileges,
                (int) CostumerPrivilege.ScheduleViewing);
        }

        [Test]
        public void
            IsVisibleAndEnabled_WhenActionIsContextMenuPasteToAndCopyItemsPrivilegeIsNotAuthorized_ShouldPermissionAuthorizerIsAuthorizedCalledOnceWithMoveItemsPrivilege()
        {
            // Arrange
            this.permissionSettings.Action = ActionEnum.CheckBiddingStatus;
            this.permissionAuthorizer.IsAuthorized(this.permissionSettings.CallContext,
                Arg.Any<List<PropertyUserPrivilege>>(),
                (int) CostumerPrivilege.ScheduleViewing).Returns(false);

            // Act
            this.sut.IsVisibleAndEnabled(this.permissionSettings);

            // Assert
            this.permissionAuthorizer.Received(1).IsAuthorized(this.permissionSettings.CallContext,
                Arg.Any<List<PropertyUserPrivilege>>(),
                (int) CostumerPrivilege.MakeOffer);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsVisibleAndEnabled_WhenIsAuthorizedReturnFalseOrTrue_ShouldReturnValueFalseOrTrue(
            bool isAuthorized)
        {
            // Arrange
            this.permissionSettings.Action = ActionEnum.DeleteProperty;
            this.permissionAuthorizer
                .IsAuthorized(Arg.Any<CallContext>(), Arg.Any<List<PropertyUserPrivilege>>(), Arg.Any<int>())
                .Returns(isAuthorized);

            // Act
            var isEnabled = this.sut.IsVisibleAndEnabled(this.permissionSettings);

            // Assert
            Assert.That(isEnabled, Is.EqualTo(isAuthorized));
        }

        [Test]
        public void IsEnable_ShouldIsVisibleAndEnabledCalledOnce()
        {
            // Arrange
            var sut = new CustomerPermissionRuleStub();

            // Act
            sut.IsEnabled(this.permissionSettings);

            // Assert
            Assert.That(sut.IsVisibleAndEnabledCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void IsVisible_ShouldIsVisibleAndEnabledCalledOnce()
        {
            // Arrange
            var sut = new CustomerPermissionRuleStub();

            // Act
            sut.IsVisible(this.permissionSettings);

            // Assert
            Assert.That(sut.IsVisibleAndEnabledCalledTimes, Is.EqualTo(1));
        }

        [TestCase(ActionEnum.DeleteProperty, (int) CostumerPrivilege.DeleteProperty)]
        [TestCase(ActionEnum.SendingDocuments, (int) PropertyPrivilege.SendDocument)]
        [TestCase(ActionEnum.ChangeBiddingPrice, (int) AgentPrivilege.ChangeBiddingPrice)]
        [TestCase(ActionEnum.MenuScheduleViewing, (int) CostumerPrivilege.ScheduleViewing)]
        [TestCase(ActionEnum.MakeOffer, (int) CostumerPrivilege.MakeOffer)]
        public void
            GetActionPrivilegeIds_WhenActionEnumIsOnlyDependsOnePrivilege_ShouldListContains1MemberWhichExactlyTheGivenPrivilege(
                ActionEnum actionEnum, int privilegeId)
        {
            // Arrange
            var permissionSettings = new PermissionSettings()
            {
                Action = actionEnum
            };

            // Act
            var actionPrivilegeIds = CostumerPermissionRule.GetActionPrivilegeIds(permissionSettings);

            // Assert
            Assert.That(actionPrivilegeIds, Has.Exactly(1).EqualTo(privilegeId));
        }

        [Test]
        public void
            GetActionPrivilegeIds_WhenActionIsCheckBiddingStatus_ShouldPrivilegesIdContains2ItemsAndValuesAre1And2()
        {
            // Arrange
            var permissionSettings = new PermissionSettings()
            {
                Action = ActionEnum.CheckBiddingStatus
            };

            // Act
            var actionPrivilegeIds = CostumerPermissionRule.GetActionPrivilegeIds(permissionSettings);

            // Assert
            Assert.That(actionPrivilegeIds, Has.Exactly(1).EqualTo(1)
                .And.Exactly(1).EqualTo(2));
        }

        private void InitializeVariables()
        {
            this.callContext = new CallContext();
            this.roomUserPrivileges = new List<PropertyUserPrivilege>();
            this.permissionAuthorizer = Substitute.For<IPermissionAuthorizer>();
            this.permissionSettings = new PermissionSettings()
            {
                CallContext = callContext,
                PrivilegesByUserRoom = this.roomUserPrivileges
            };
            this.sut = new CostumerPermissionRule(this.permissionAuthorizer);
        }
    }

    public class CustomerPermissionRuleStub : CostumerPermissionRule
    {
        public int IsVisibleAndEnabledCalledTimes { get; private set; }

        internal override bool IsVisibleAndEnabled(PermissionSettings permissionSettings)
        {
            this.IsVisibleAndEnabledCalledTimes++;
            return true;
        }
    }
}