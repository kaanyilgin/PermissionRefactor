using NUnit.Framework;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class PropertyStatusPermissionRuleTest
    {
        private PropertyStatusPermissionRule sut;

        [SetUp]
        public void SetUp()
        {
            this.sut = new PropertyStatusPermissionRule();
        }

        [Test]
        public void IsEnabled_WhenPropertyStatusIs5_ShouldIsEnabledEqualToFalse()
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                PropertyStatusTypeId = 5
            };

            // Act
            var isEnabled = this.sut.IsEnabled(permissionValue);

            // Assert
            Assert.That(isEnabled, Is.False);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void IsEnabled_WhenPropertyStatusIsNot5_ShouldIsEnabledEqualToTrue(int propertyStatusTypeId)
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                PropertyStatusTypeId = propertyStatusTypeId
            };

            // Act
            var isEnabled = this.sut.IsEnabled(permissionValue);

            // Assert
            Assert.That(isEnabled, Is.True);
        }

        [Test]
        public void IsVisible_ShouldAlwaysEqualToTrue()
        {
            // Arrange
            var permissionValue = new PermissionSettings();

            // Act
            var isVisible = this.sut.IsVisible(permissionValue);

            // Assert
            Assert.That(isVisible, Is.True);
        }
    }
}