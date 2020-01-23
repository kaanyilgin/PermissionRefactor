using NUnit.Framework;
using PermissionModule.Rule;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class PrivatePermissionRuleTest
    {
        private PrivatePermissionRule sut;

        [SetUp]
        public void SetUp()
        {
            this.sut = new PrivatePermissionRule();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsEnabled_ShouldReverseOfIsPrivate(bool isPrivate)
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsPrivate = isPrivate
            };

            // Act
            var isEnabled = this.sut.IsEnabled(permissionValue);

            // Assert
            Assert.That(isEnabled, Is.EqualTo(!isPrivate));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsVisible_ShouldReverseOfIsPrivate(bool isPrivate)
        {
            // Arrange
            var permissionValue = new PermissionSettings()
            {
                IsPrivate = isPrivate
            };

            // Act
            var isVisible = this.sut.IsVisible(permissionValue);

            // Assert
            Assert.That(isVisible, Is.EqualTo(!isPrivate));
        }
    }
}