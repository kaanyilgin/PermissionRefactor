using NUnit.Framework;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class BiddingLockedPermissionRuleTest
    {
        private BiddingLockedPermissionRule sut;

        [SetUp]
        public void SetUp()
        {
            this.sut = new BiddingLockedPermissionRule();
        }
        
        [Test]
        public void IsEnabled_WhenBiddingIsLocked_ShouldIsEnabledEqualToFalse()
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsBiddingLocked = true
            };

            // Act
            var isEnabled = this.sut.IsEnabled(permissionValue);

            // Assert
            Assert.That(isEnabled, Is.False);
        }
        
        [Test]
        public void IsEnabled_WhenBiddingIsLockedIsFalse_ShouldIsEnabledEqualToTrue()
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsBiddingLocked = false
            };

            // Act
            var isEnabled = this.sut.IsEnabled(permissionValue);

            // Assert
            Assert.That(isEnabled, Is.True);
        }
        
        [Test]
        public void IsVisible_WhenBiddingIsLocked_ShouldIsEnabledEqualToFalse()
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsBiddingLocked = true
            };

            // Act
            var isVisible = this.sut.IsVisible(permissionValue);

            // Assert
            Assert.That(isVisible, Is.False);
        }
        
        [Test]
        public void IsVisible_WhenBiddingIsLockedIsFalse_ShouldIsEnabledEqualToTrue()
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsBiddingLocked = false
            };

            // Act
            var isVisible = this.sut.IsVisible(permissionValue);

            // Assert
            Assert.That(isVisible, Is.True);
        }
    }
}