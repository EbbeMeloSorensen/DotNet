﻿using FluentAssertions;
using PR.Domain.Entities;
using PR.Persistence.Versioned;
using StructureMap;
using Xunit;

namespace PR.Persistence.UnitTest
{
    public class RetrospectiveTests
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public RetrospectiveTests()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Reseed();

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade).DatabaseTime = null;
        }

        //[Fact]
        //public async void GetEarlierVersionOfPerson_1()
        //{
        //    // Arrange
        //    //_unitOfWorkFactory.DatabaseTime = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var person = await unitOfWork.People.Get(
        //        new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

        //    // Assert
        //    person.FirstName.Should().Be("Darth");
        //    person.Surname.Should().Be("Vader");
        //}

        //[Fact]
        //public async void GetEarlierVersionOfPerson_2()
        //{
        //    // Arrange
        //    //_unitOfWorkFactory.DatabaseTime = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var person = await unitOfWork.People.Get(
        //        new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

        //    // Assert
        //    person.FirstName.Should().Be("Anakin");
        //    person.Surname.Should().Be("Skywalker");
        //}

        ////[Fact]
        ////public void GetEarlierVersionOfPerson_BeforePersonWasCreated_Throws()
        ////{
        ////    // Arrange
        ////    //_unitOfWorkFactory.DatabaseTime = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        ////    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        ////    // Act
        ////    var act = () => unitOfWork.People.Get(
        ////        new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

        ////    // Assert
        ////    var exception = Assert.Throws<InvalidOperationException>(act);
        ////    exception.Message.Should().Be("Person does not exist");
        ////}

        //[Fact]
        //public async void GetEarlierVersionOfEntirePersonCollection_1()
        //{
        //    // Arrange
        //    //_unitOfWorkFactory.DatabaseTime = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var people = await unitOfWork.People.GetAll();

        //    // Assert
        //    people.Count().Should().Be(4);
        //    people.Count(p => p.FirstName == "Han").Should().Be(1);
        //    people.Count(p => p.FirstName == "Luke").Should().Be(1);
        //    people.Count(p => p.FirstName == "Leia").Should().Be(1);
        //    people.Count(p => p.FirstName == "Ben").Should().Be(1);
        //}

        //[Fact]
        //public async void GetEarlierVersionOfEntirePersonCollection_2()
        //{
        //    // Arrange
        //    //_unitOfWorkFactory.DatabaseTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var people = await unitOfWork.People.GetAll();

        //    // Assert
        //    people.Count().Should().Be(0);
        //}
    }
}
