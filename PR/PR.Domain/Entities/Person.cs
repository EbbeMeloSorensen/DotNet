using System;
using System.Collections.Generic;

namespace PR.Domain.Entities
{
    public class Person
    {
        // Notice that we're using a guid here rather than an int.
        // Usually I have always used int, but then I came across the Udemy course "Complete guide to
        // building an app with .Net Core and React" by Neil Cummings, where he argues that there is
        // a benefit in using a guid as an id, since you can then create the id at the client side
        // and get on with business rather than having to wait for the server side to generate the id
        private Guid _id;
        private string _firstName;
        private string? _surname;
        private string? _nickname;
        private string? _address;
        private string? _zipCode;
        private string? _city;
        private DateTime? _birthday;
        private string? _category;
        private string? _description;
        private bool? _dead;
        private DateTime _created;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string? Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }

        public string? Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        public string? Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string? ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        public string? City
        {
            get { return _city; }
            set { _city = value; }
        }

        public DateTime? Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }

        public string? Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string? Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public bool? Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        public virtual ICollection<PersonAssociation>? ObjectPeople { get; set; }
        public virtual ICollection<PersonAssociation>? SubjectPeople { get; set; }
    }
}
