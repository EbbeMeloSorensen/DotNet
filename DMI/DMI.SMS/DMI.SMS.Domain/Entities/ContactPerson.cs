using System;

namespace DMI.SMS.Domain.Entities
{
    public class ContactPerson : ChildEntity
    {
        private StationInformation _stationinformation;
        private string _name;
        private string _phonenumber;
        private string _email;
        private DateTime? _date;
        private string _description;

        public StationInformation StationInformation
        {
            get { return _stationinformation; }
            set { _stationinformation = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string PhoneNumber
        {
            get { return _phonenumber; }
            set { _phonenumber = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public DateTime? Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
