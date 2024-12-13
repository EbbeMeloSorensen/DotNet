using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain.Entities;
using PR.Persistence.Versioned;

namespace PR.Persistence.UnitTest
{
    // This test should work for an ordinary as well as a wrapped repository 
    public class PersonRepositoryTest
    {
        private const bool _bitemporalDB = true;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryTest()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Initialize(_bitemporalDB);
            _unitOfWorkFactory.Reseed();

            if (!_bitemporalDB) return;

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = null;
        }

        [Fact]
        public async void GetPersonById()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("12345678-0000-0000-0000-000000000005");

            // Act
            var person = await unitOfWork.People.Get(id);

            // Assert
            person.FirstName.Should().Be("Chewbacca");
        }

        [Fact]
        public async void FindPersonById()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000005")
            };

            // Act
            var people = await unitOfWork.People.Find(p => ids.Contains(p.ID));

            // Assert
            people.Count().Should().Be(1);
            people.Single().FirstName.Should().Be("Chewbacca");
        }

        [Fact]
        public async Task CreatePerson()
        {
            // Arrange
            var person = new Person
            {
                FirstName = "Wicket"
            };

            // Act
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            await unitOfWork1.People.Add(person);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var people = await unitOfWork2.People.GetAll();
            people.Count().Should().Be(3);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
            people.Count(p => p.FirstName == "Wicket").Should().Be(1);
        }

        //[Fact]
        //public void GetPerson_AfterPersonWasDeleted_Throws()
        //{
        //    // Arrange
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var act = () => unitOfWork.People.Get(
        //        new Guid("12345678-0000-0000-0000-000000000005"));

        //    // Assert
        //    var exception = Assert.Throws<InvalidOperationException>(act);
        //    exception.Message.Should().Be("Person does not exist");
        //}

        [Fact]
        public async void UpdatePerson()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("12345678-0000-0000-0000-000000000005");
            var person = await unitOfWork1.People.Get(id);
            person.FirstName = "Monkey";

            // Act
            await unitOfWork1.People.Update(person);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var person2 = await unitOfWork2.People.Get(id);
            person2.FirstName.Should().Be("Monkey");
        }

        [Fact]
        public async void UpdatePeople()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000005"),
                new("12345678-0000-0000-0000-000000000006")
            };

            // Act
            var people = (await unitOfWork1.People.Find(p => ids.Contains(p.ID))).ToList();

            people.ForEach(_ => _.FirstName = "Bamse");
            await unitOfWork1.People.UpdateRange(people);
            unitOfWork1.Complete();

            // Du er sgu nok nødt til at checke hvordan fucking repoet ser ud her, for det nedenunder returnerer fucking 4
            // og det lader til at skyldes at den FUCKING IKKE får ændret på superseded!! HVorfor gør den ikke det?
            // Hmmm husk lige, at Find returnerer kloner....
            // SATANS OGSÅ FUCKING NOK, MAND!!! HVORFOR KAN DU IKKE CHECKE EN DATABASE FRA DIN SIA-MASKINE
            // Nå, men det har jo nok netop noget at gøre med at det er kloner, som den returnerer, og når du ændrer på
            // en klon og efterfølgende.
            // Det med at du overhovedet kloner.... det var jo fordi du oplevede, at når du henter et objekt og så
            // ændrer det, så går det i fuck
            //
            // Øøøøh og nu virker det så tilsyneladende... uden at du er skarp på hvorfor...

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            people = (await unitOfWork2.People.Find(p => ids.Contains(p.ID))).ToList();
            people.Count.Should().Be(2);
        }

        [Fact]
        public async void GetAllPeople()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
        }
    }
}
