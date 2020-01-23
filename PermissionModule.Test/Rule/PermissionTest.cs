using NUnit.Framework;

namespace PermissionModule.UnitTest.Rule
{
    [TestFixture(Category = "Permission")]
    [Category("Permission > Rule")]
    public class PermissionTest
    {
        private Permissions.Permission sut;

        [SetUp]
        public void SetUp()
        {
            this.sut = new PermissionStub();
        }

        [Test]
        public void IsEnabled_ShouldEqualToTrueDefault()
        {
            // Assert
            Assert.That(this.sut.IsEnabled, Is.True);
        }

        [Test]
        public void IsVisible_ShouldEqualToTrueDefault()
        {
            // Assert
            Assert.That(this.sut.IsVisible, Is.True);
        }
    }

    public class PermissionStub : Permissions.Permission
    {
        public PermissionStub() : base()
        {
        }
    }
}