using System.Collections.Generic;
using NUnit.Framework;

namespace PermissionModule.UnitTest
{
[TestFixture(Category = "Permission")]
	public class PermissionAuthorizerTest
	{
		private PermissionAuthorizer sut;
		private CallContext callContext;
		private IList<PropertyUserPrivilege> privilegeList;

		[SetUp]
		public void SetUp()
		{
			this.callContext = new CallContext();
			this.privilegeList = new List<PropertyUserPrivilege>();
			this.sut = new PermissionAuthorizer();
		}

		[Test]
		public void IsAuthorized_WhenPropertyIsPrivateAndThereIsAItemWithPrivilegeId_ShouldReturnTrue()
		{
			// Arrange
			var privilegeId = 1;
			this.privilegeList.Add(new PropertyUserPrivilege()
			{
				PrivilegeId = privilegeId
			});
			this.callContext.IsPrivate = true;

			// Act
			var isAuthorized = this.sut.IsAuthorized(this.callContext, this.privilegeList, privilegeId);

			// Assert
			Assert.That(isAuthorized, Is.True);
		}

		[Test]
		public void IsAuthorized_WhenPropertyIsPrivateAndThereIsNoItemWithPrivilegeId_ShouldReturnFalse()
		{
			// Arrange
			var privilegeId = 1;
			this.privilegeList.Add(new PropertyUserPrivilege()
			{
				PrivilegeId = 2
			});
			this.callContext.IsPrivate = true;

			// Act
			var isAuthorized = this.sut.IsAuthorized(this.callContext, this.privilegeList, privilegeId);

			// Assert
			Assert.That(isAuthorized, Is.False);
		}

		[Test]
		public void IsAuthorized_WhenPropertyIsNotPrivateAndPrivilegeIdAndPropertyIdMatched_ShouldReturnTrue()
		{
			// Arrange
			var propertyId = 1010;
			this.callContext.IsPrivate = false;
			this.callContext.PropertyId = propertyId;
			var privilegeId = 1;
			this.privilegeList.Add(new PropertyUserPrivilege()
			{
				PropertyId = propertyId,
				PrivilegeId = privilegeId
			});

			// Act
			var isAuthorized = this.sut.IsAuthorized(this.callContext, this.privilegeList, privilegeId);

			// Assert
			Assert.That(isAuthorized, Is.True);
		}

		[Test]
		public void IsAuthorized_WhenUserIsNotPrivateAndPrivilegeIdIsMatchedAnIsSystemPrivilegeIsTrue_ShouldReturnTrue()
		{
			// Arrange
			var propertyId = 1010;
			this.callContext.IsPrivate = false;
			this.callContext.PropertyId = propertyId;
			var privilegeId = 1;
			this.privilegeList.Add(new PropertyUserPrivilege()
			{
				PrivilegeId = privilegeId,
				PropertyId = 1011,
				IsSystemPrivilege = true
			});

			// Act
			var isAuthorized = this.sut.IsAuthorized(this.callContext, this.privilegeList, privilegeId);

			// Assert
			Assert.That(isAuthorized, Is.True);
		}

		[Test]
		public void IsAuthorized_WhenUserIsNotPrivateAndPrivilegeIdIsNotMatchedAnIsSystemPrivilegeIsFalse_ShouldReturnTrue()
		{
			// Arrange
			var propertyId = 1010;
			this.callContext.IsPrivate = false;
			this.callContext.PropertyId = propertyId;
			var privilegeId = 1;
			this.privilegeList.Add(new PropertyUserPrivilege()
			{
				PrivilegeId = privilegeId,
				PropertyId = 1011,
				IsSystemPrivilege = false
			});

			// Act
			var isAuthorized = this.sut.IsAuthorized(this.callContext, this.privilegeList, privilegeId);

			// Assert
			Assert.That(isAuthorized, Is.False);
		}
	}
}