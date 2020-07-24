using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using LiftLog.Data.Repositories;
using LiftLog.Data.Tests.Utility;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LiftLog.Data.Tests.UnitTests
{
    public class UserRepositoryTest
    {
        [Fact(DisplayName = "CreateAsync user Given Valid input Returns Id")]
        public async Task Test_CreateAsync_Returns_Id()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);

            using (repository)
            {
                var user = new User()
                {
                    Name = "Test"
                };
                //Act
                var id = await repository.CreateAsync(user);
                //Assert
                Assert.Equal(1, id);
            }
        }
        public class TestDbSet<T> : DbSet<T> where T : class
        {

        }

        [Fact(DisplayName = "CreateAsync user given null input throws ArgumentNullException")]
        public async Task Test_CreateAsync_Given_Null_Throws_ArgumentNullException()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);

            using (repository)
            {
                //Act + Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.CreateAsync(null));
            }
        }

        [Fact(DisplayName = "Delete user Given existing user Returns true")]
        public async Task Test_DeleteAsync_Returns_true()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);

            using (repository)
            {
                var user = new User()
                {
                    Name = "Test"
                };
                //Act
                var id = await repository.CreateAsync(user);
                var actual = await repository.DeleteAsync(id);
                //Assert
                Assert.True(actual);
            }
        }

        [Fact(DisplayName = "Delete non-existing user Returns false")]
        public async Task Test_DeleteAsync_Returns_false()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);

            using (repository)
            {
          
                //Act
                var actual = await repository.DeleteAsync(123);
                //Assert
                Assert.False(actual);
            }
        }

        [Fact(DisplayName = "Update Given user Returns true")]
        public async Task Test_UpdateAsync_Returns_true()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);
            var expected = "Done";

            using (repository)
            {
                var user = new User()
                {
                    Name = "Test"
                };
                //Act
                var id = await repository.CreateAsync(user);
                var toEdit = await repository.FindAsync(id);
                toEdit.Name = expected;
                var isUpdate = await repository.UpdateAsync(toEdit);
                var actual = await repository.FindAsync(id);

                //Assert
                Assert.True(isUpdate);
                Assert.True(actual.Name == expected);
            }
        }

        [Fact(DisplayName = "Update null user Returns false")]
        public async Task Test_UpdateAsync_Returns_false()
        {
            //Arrange
            var options = TestUtility.CreateNewContextOptions();
            var context = new Context(options);
            var repository = new UserRepository(context);

            using (repository)
            {
           
                //Act
                var isUpdate = await repository.UpdateAsync(null);

                //Assert
                Assert.False(isUpdate);
            }
        }

    }

}
