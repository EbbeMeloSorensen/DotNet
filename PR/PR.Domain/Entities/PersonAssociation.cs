using System;

namespace PR.Domain.Entities
{
    public class PersonAssociation
    {
        private Guid _id;
        private Guid _subjectPersonId;
        private Guid _objectPersonId;
        private Person _subjectPerson;
        private Person _objectPerson;
        private string _description;
        private DateTime _created;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Guid SubjectPersonId
        {
            get { return _subjectPersonId; }
            set { _subjectPersonId = value; }
        }

        public Guid ObjectPersonId
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

        public string? Description
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
