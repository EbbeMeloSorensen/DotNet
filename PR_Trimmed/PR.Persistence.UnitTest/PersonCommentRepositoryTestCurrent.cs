using FluentAssertions;
using StructureMap;
using Xunit;
using PR.Domain.Entities;
using PR.Domain.Entities.PR;

namespace PR.Persistence.UnitTest
{
    public class PersonCommentRepositoryTestCurrent
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonCommentRepositoryTestCurrent()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Initialize(false);
            _unitOfWorkFactory.Reseed();
        }

        [Fact]
        public async Task CreatePersonComment()
        {
            // Arrange
            var personComment = new PersonComment
            {
                PersonID = new Guid("12345678-0000-0000-0000-000000000006"),
                Text = "She is initially a scavenger"
            };

            // Act
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            await unitOfWork1.PersonComments.Add(personComment);
            unitOfWork1.Complete();

            //// Assert
            //using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            //var people = await unitOfWork2.People.GetAll();
            //people.Count().Should().Be(2);
            //people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
            //people.Count(p => p.FirstName == "Wicket").Should().Be(1);
        }
    }
}
