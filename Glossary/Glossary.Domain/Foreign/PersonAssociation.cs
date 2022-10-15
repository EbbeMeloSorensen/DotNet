using System;

namespace Glossary.Domain.Foreign
{
    public class PersonAssociation
    {
        private int _id;
        private int _subjectPersonId;
        private int _objectPersonId;
        private Person _subjectPerson;
        private Person _objectPerson;
        private string _description;
        private DateTime _created;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int SubjectPersonId
        {
            get { return _subjectPersonId; }
            set { _subjectPersonId = value; }
        }

        public int ObjectPersonId
        {
            get { return _objectPersonId; }
            set { _objectPersonId = value; }
        }

        public Person SubjectPerson
        {
            get { return _subjectPerson; }
            set { _subjectPerson = value; }
        }

        public Person ObjectPerson
        {
            get { return _objectPerson; }
            set { _objectPerson = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
    }
}
