using System;

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

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
    }
}
