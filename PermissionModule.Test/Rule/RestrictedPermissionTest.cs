using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PermissionModule.Permissions;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class RestrictedPermissionTest
    {
        private RestrictedPermission sut;
        private PermissionSettings permissionSettings;
        private IList<IPermissionRule> permissionRules;
        private IPermissionRule mockPermission; 

        [SetUp]
        public void SetUp()
        {
            this.permissionSettings = new PermissionSettings();
            this.mockPermission = Substitute.For<IPermissionRule>();
            this.permissionRules = new List<IPermissionRule>()
            {
	            this.mockPermission
            };
        }

        [Test]
		public void ApplyRule_WhenThereIsAOneRule_ShouldIsEnableAndIsVisibleCalledOnce()
		{
			// Arrange
			var restrictedPermission = new RestrictedPermission(this.permissionSettings, this.permissionRules);

			// Act
			restrictedPermission.ApplyRules();

			// Assert
			this.mockPermission.Received(1).IsEnabled(this.permissionSettings);
			this.mockPermission.Received(1).IsVisible(this.permissionSettings);
		}

		[Test]
		public void ApplyRule_WhenThereIsAOneRuleReturnTrue_ShouldIsEnableAndIsVisibleEqualToRule()
		{
			// Arrange
			this.mockPermission.IsEnabled(this.permissionSettings).Returns(true);
			this.mockPermission.IsVisible(this.permissionSettings).Returns(true);
			var restrictedPermission = new RestrictedPermission(this.permissionSettings, this.permissionRules);
		
			// Act
			restrictedPermission.ApplyRules();
		
			// Assert
			Assert.That(restrictedPermission.IsVisible, Is.EqualTo(true));
			Assert.That(restrictedPermission.IsEnabled, Is.EqualTo(true));
		}
		
		[Test]
		public void ApplyRule_WhenThereAreTwoRulesAndTheFirstOneIsEnabledIsFalse_ShouldSecondRuleIsEnabledCalledNever()
		{
			// Arrange
			this.mockPermission.IsEnabled(this.permissionSettings).Returns(false);
			var secondMockedRule =  Substitute.For<IPermissionRule>();
			this.permissionRules.Add(secondMockedRule);
			var restrictedPermission = new RestrictedPermission(this.permissionSettings, this.permissionRules);
		
			// Act
			restrictedPermission.ApplyRules();
		
			// Assert
			secondMockedRule.DidNotReceive().IsEnabled(this.permissionSettings);
		}
		
		[Test]
		public void ApplyRule_WhenThereAreTwoRulesAndTheFirstOneIsVisibleIsFalse_ShouldSecondRuleIsVisibleCalledNever()
		{
			// Arrange
			this.mockPermission.IsEnabled(this.permissionSettings).Returns(false);
			var secondMockedRule =  Substitute.For<IPermissionRule>();
			this.permissionRules.Add(secondMockedRule);
			var restrictedPermission = new RestrictedPermission(this.permissionSettings, this.permissionRules);
			var permissionSettings = new PermissionSettings();
			
			// Act
			restrictedPermission.ApplyRules();
		
			// Assert
			secondMockedRule.DidNotReceive().IsVisible(this.permissionSettings);
		}
    }
}