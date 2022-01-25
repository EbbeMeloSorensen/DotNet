using System;

namespace DMI.SMS.Domain.Entities
{
    public class LegalOwner : ChildEntity
    {
        private StationInformation _stationinformation;
        private string _name { get; set; }
        private string _address { get; set; }
        private string _phonenumber { get; set; }
        private string _email { get; set; }
        private DateTime? _date { get; set; }
        private string _description { get; set; }

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

        public string Address
        {
            get { return _address; }
            set { _address = value; }
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
