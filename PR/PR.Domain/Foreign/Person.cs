using System;

namespace PR.Domain.Foreign
{
    public class Person
    {
        private int _id;
        private string _firstName;
        private string _surname;
        private string _nickname;
        private string _address;
        private string _zipCode;
        private string _city;
        private DateTime? _birthday;
        private string _category;
        private string _comments;
        private DateTime _created;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }

        public string Nickname
        {
            get { return _nickname; }
            set { _nickname = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public DateTime? Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
    }
}
