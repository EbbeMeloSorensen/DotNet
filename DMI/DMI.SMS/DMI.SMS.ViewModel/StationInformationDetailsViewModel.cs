using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Dialogs;
using Craft.UI.Utils;
using DMI.SMS.Application;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.SMS.Persistence;

namespace DMI.SMS.ViewModel
{
    public class StationInformationDetailsViewModel : ViewModelBase, IDataErrorInfo
    {
        private StateOfView _state;
        private ObservableCollection<ValidationError> _validationMessages;
        private string _error = string.Empty;

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDialogService _applicationDialogService;
        private readonly ObjectCollection<StationInformation> _selectedStationInformations;
        private readonly ObservableObject<Dictionary<int, RowCharacteristics>> _rowCharacteristicsMap;

        private string _header;
        private bool _isVisible;
        private int _nRecordsSelected;
        private int _nCurrentRecordsSelected;

        private string _stationname;
        private string _stationid_dmi;
        private string _stationType;
        private string _accessAddress;
        private string _country;
        private string _status;
        private string _dateFrom;
        private string _dateTo;
        private string _stationOwner;
        private string _wgs_lat;
        private string _wgs_long;

        private int _gdb_archive_oid;
        private int _objectid;
        private string _globalid;
        private string _gdb_from_date;
        private string _gdb_to_date;
        private string _created_user;
        private string _created_date;
        private string _last_edited_user;
        private string _last_edited_date;

        private string _comment;
        private string _stationid_icao;
        private string _referencetomaintenanceagreement;
        private string _facilityid;
        private int? _si_utm;
        private double? _si_northing;
        private double? _si_easting;
        private double? _si_geo_lat;
        private double? _si_geo_long;
        private int? _serviceinterval;
        private string _lastservicedate;
        private string _nextservicedate;
        private string _addworkforcedate;
        private string _lastvisitdate;
        private string _altstationid;
        private string _wmostationid;
        private string _regionid;
        private string _wigosid;
        private string _wmocountrycode;
        private double? _hha;
        private double? _hhp;
        private int? _wmorbsn;
        private int? _wmorbcn;
        private int? _wmorbsnradio;
        private string _shape;

        private string _originalStationname;
        private string _originalAccessAddress;
        private string _original_wgs_lat;
        private string _original_wgs_long;
        private string _originalStationType;
        private string _originalStatus;
        private string _originalCountry;
        private string _originalStationOwner;
        private string _originalDateFrom;
        private string _originalDateTo;

        private RelayCommand _discardChangesCommand; // Den her clearer bare formen, hvis man fortryder de ændringer, man har indtastet
        private RelayCommand<object> _manipulateCommand; // Her manipulerer vi bare hårdt
        private RelayCommand<object> _eraseCommand; // Her bomber vi hårdt en række væk
        private RelayCommand _updateCommand; // Her superseder vi en eksisterende række, så man kan se, hvem der har ændret noget - svarende til, hvad man jo altså gør med det rigtige system. Den kan kun bruges for en current record
        private RelayCommand<object> _deleteCommand; // Her foretager vi en logisk delete
        private RelayCommand<object> _promoteCommand; // Den her kan bruges på rækker, der er supersedede, dvs outdated eller deleted - man fisker så at sige rækken op af papirkurven og gør den current
                                                      // Rækken skal tildeles et nyt objekt id
        private RelayCommand<object> _mergeCommand; // Denne er til for at facilitere, at man kan forskyde tidspunktet for skift mellem 2 records

        public event EventHandler RepositoryOperationPerformed;

        public ObservableCollection<string> StationTypeOptions { get; }
        public ObservableCollection<string> StatusOptions { get; }
        public ObservableCollection<string> CountryOptions { get; }
        public ObservableCollection<string> StationOwnerOptions { get; }

        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public string Stationname
        {
            get
            {
                return _stationname;
            }
            set
            {
                _stationname = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Stationid_dmi
        {
            get
            {
                return _stationid_dmi;
            }
            set
            {
                _stationid_dmi = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string StationType
        {
            get
            {
                return _stationType;
            }
            set
            {
                _stationType = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string AccessAddress
        {
            get
            {
                return _accessAddress;
            }
            set
            {
                _accessAddress = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string DateFrom
        {
            get
            {
                return _dateFrom;
            }
            set
            {
                _dateFrom = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string DateTo
        {
            get
            {
                return _dateTo;
            }
            set
            {
                _dateTo = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string StationOwner
        {
            get
            {
                return _stationOwner;
            }
            set
            {
                _stationOwner = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Wgs_lat
        {
            get
            {
                return _wgs_lat;
            }
            set
            {
                _wgs_lat = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Wgs_long
        {
            get
            {
                return _wgs_long;
            }
            set
            {
                _wgs_long = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int Gdb_archive_oid
        {
            get
            {
                return _gdb_archive_oid;
            }
            set
            {
                _gdb_archive_oid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int Objectid
        {
            get
            {
                return _objectid;
            }
            set
            {
                _objectid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Globalid
        {
            get
            {
                return _globalid;
            }
            set
            {
                _globalid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Gdb_from_date
        {
            get
            {
                return _gdb_from_date;
            }
            set
            {
                _gdb_from_date = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Gdb_to_date
        {
            get
            {
                return _gdb_to_date;
            }
            set
            {
                _gdb_to_date = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Created_user
        {
            get
            {
                return _created_user;
            }
            set
            {
                _created_user = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Created_date
        {
            get
            {
                return _created_date;
            }
            set
            {
                _created_date = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Last_edited_user
        {
            get
            {
                return _last_edited_user;
            }
            set
            {
                _last_edited_user = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Last_edited_date
        {
            get
            {
                return _last_edited_date;
            }
            set
            {
                _last_edited_date = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Stationid_icao
        {
            get
            {
                return _stationid_icao;
            }
            set
            {
                _stationid_icao = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Referencetomaintenanceagreement
        {
            get
            {
                return _referencetomaintenanceagreement;
            }
            set
            {
                _referencetomaintenanceagreement = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Facilityid
        {
            get
            {
                return _facilityid;
            }
            set
            {
                _facilityid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int? Si_utm
        {
            get
            {
                return _si_utm;
            }
            set
            {
                _si_utm = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Si_northing
        {
            get
            {
                return _si_northing;
            }
            set
            {
                _si_northing = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Si_easting
        {
            get
            {
                return _si_easting;
            }
            set
            {
                _si_easting = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Si_geo_lat
        {
            get
            {
                return _si_geo_lat;
            }
            set
            {
                _si_geo_lat = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Si_geo_long
        {
            get
            {
                return _si_geo_long;
            }
            set
            {
                _si_geo_long = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int? Serviceinterval
        {
            get
            {
                return _serviceinterval;
            }
            set
            {
                _serviceinterval = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Lastservicedate
        {
            get
            {
                return _lastservicedate;
            }
            set
            {
                _lastservicedate = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Nextservicedate
        {
            get
            {
                return _nextservicedate;
            }
            set
            {
                _nextservicedate = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Addworkforcedate
        {
            get
            {
                return _addworkforcedate;
            }
            set
            {
                _addworkforcedate = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Lastvisitdate
        {
            get
            {
                return _lastvisitdate;
            }
            set
            {
                _lastvisitdate = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Altstationid
        {
            get
            {
                return _altstationid;
            }
            set
            {
                _altstationid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Wmostationid
        {
            get
            {
                return _wmostationid;
            }
            set
            {
                _wmostationid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Regionid
        {
            get
            {
                return _regionid;
            }
            set
            {
                _regionid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Wigosid
        {
            get
            {
                return _wigosid;
            }
            set
            {
                _wigosid = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Wmocountrycode
        {
            get
            {
                return _wmocountrycode;
            }
            set
            {
                _wmocountrycode = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Hha
        {
            get
            {
                return _hha;
            }
            set
            {
                _hha = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public double? Hhp
        {
            get
            {
                return _hhp;
            }
            set
            {
                _hhp = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int? Wmorbsn
        {
            get
            {
                return _wmorbsn;
            }
            set
            {
                _wmorbsn = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int? Wmorbcn
        {
            get
            {
                return _wmorbcn;
            }
            set
            {
                _wmorbcn = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public int? Wmorbsnradio
        {
            get
            {
                return _wmorbsnradio;
            }
            set
            {
                _wmorbsnradio = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public string Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                _shape = value;
                RaisePropertyChanged();
                RefreshButtons();
            }
        }

        public BusinessRuleViolationListViewModel BusinessRuleViolationListViewModel { get; private set; }

        public RelayCommand DiscardChangesCommand
        {
            get { return _discardChangesCommand ?? (_discardChangesCommand = new RelayCommand(DiscardChanges, CanDiscardChanges)); }
        }

        public RelayCommand<object> ManipulateCommand
        {
            get { return _manipulateCommand ?? (_manipulateCommand = new RelayCommand<object>(Manipulate, CanManipulate)); }
        }

        public RelayCommand<object> EraseCommand
        {
            get { return _eraseCommand ?? (_eraseCommand = new RelayCommand<object>(Erase, CanErase)); }
        }

        public RelayCommand UpdateCommand
        {
            get { return _updateCommand ?? (_updateCommand = new RelayCommand(Update, CanUpdate)); }
        }

        public RelayCommand<object> DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new RelayCommand<object>(Delete, CanDelete)); }
        }

        public RelayCommand<object> PromoteCommand
        {
            get { return _promoteCommand ?? (_promoteCommand = new RelayCommand<object>(Promote, CanPromote)); }
        }

        public RelayCommand<object> MergeCommand
        {
            get { return _mergeCommand ?? (_mergeCommand = new RelayCommand<object>(Merge, CanMerge)); }
        }

        public StationInformationDetailsViewModel(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDialogService applicationDialogService,
            ObjectCollection<StationInformation> selectedStationInformations,
            ObservableObject<Dictionary<int, RowCharacteristics>> rowCharacteristicsMap)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _applicationDialogService = applicationDialogService;

            _selectedStationInformations = selectedStationInformations;
            _rowCharacteristicsMap = rowCharacteristicsMap;

            BusinessRuleViolationListViewModel = new BusinessRuleViolationListViewModel();

            StationTypeOptions = new ObservableCollection<string>(SharedData.StationTypeDisplayTextMap.Values);
            StatusOptions = new ObservableCollection<string>(SharedData.StatusDisplayTextMap.Values);
            CountryOptions = new ObservableCollection<string>(SharedData.CountryDisplayTextMap.Values);
            StationOwnerOptions = new ObservableCollection<string>(SharedData.StationOwnerDisplayTextMap.Values);

            Header = "Properties (No Records Selected)";
            _selectedStationInformations.PropertyChanged += Initialize;
        }

        private void Initialize(
            object sender, 
            PropertyChangedEventArgs e)
        {
            var stationInformations = sender as ObjectCollection<StationInformation>;

            if (stationInformations == null || stationInformations.Objects.Count() == 0)
            {
                BusinessRuleViolationListViewModel.Clear();

                _nRecordsSelected = 0;
                _nCurrentRecordsSelected = 0;
            }
            else if (stationInformations.Objects.Count() == 1)
            {
                Header = "Properties of 1 Selected Record";
                var stationInformation = stationInformations.Objects.Single();

                Stationname = stationInformation.StationName;
                Stationid_dmi = stationInformation.StationIDDMI.ToString();
                StationType = stationInformation.Stationtype.ToString();
                AccessAddress = stationInformation.AccessAddress;
                Country = stationInformation.Country.HasValue ? SharedData.CountryDisplayTextMap[stationInformation.Country.Value] : "";
                Status = stationInformation.Status.HasValue ? SharedData.StatusDisplayTextMap[stationInformation.Status.Value] : "";
                DateFrom = stationInformation.DateFrom.HasValue ? stationInformation.DateFrom.Value.AsDateTimeString(true, true) : "";
                DateTo = stationInformation.DateTo.HasValue ? stationInformation.DateTo.Value.AsDateTimeString(true, true) : "";
                StationOwner = stationInformation.StationOwner.HasValue ? SharedData.StationOwnerDisplayTextMap[stationInformation.StationOwner.Value] : "";
                Wgs_lat = stationInformation.Wgs_lat.HasValue ? stationInformation.Wgs_lat.Value.ToString(CultureInfo.InvariantCulture) : "";
                Wgs_long = stationInformation.Wgs_long.HasValue ? stationInformation.Wgs_long.Value.ToString(CultureInfo.InvariantCulture) : "";
                Gdb_archive_oid = stationInformation.GdbArchiveOid;
                Objectid = stationInformation.ObjectId;
                Globalid = stationInformation.GlobalId;
                Gdb_from_date = stationInformation.GdbFromDate.AsDateTimeString(true, true);
                Gdb_to_date = stationInformation.GdbToDate.AsDateTimeString(true, true);
                Created_user = stationInformation.CreatedUser;
                Created_date = stationInformation.CreatedDate.HasValue ? stationInformation.CreatedDate.Value.AsDateTimeString(true, true) : "";
                Last_edited_user = stationInformation.LastEditedUser;
                Last_edited_date = stationInformation.LastEditedDate.HasValue ? stationInformation.LastEditedDate.Value.AsDateTimeString(true, true) : "";
                Comment = stationInformation.Comment;
                Stationid_icao = stationInformation.Stationid_icao;
                Referencetomaintenanceagreement = stationInformation.Referencetomaintenanceagreement;
                Facilityid = stationInformation.Facilityid;
                Si_utm = stationInformation.Si_utm;
                Si_northing = stationInformation.Si_northing;
                Si_easting = stationInformation.Si_easting;
                Si_geo_lat = stationInformation.Si_geo_lat;
                Si_geo_long = stationInformation.Si_geo_long;
                Serviceinterval = stationInformation.Serviceinterval;
                Lastservicedate = stationInformation.Lastservicedate.HasValue ? stationInformation.Lastservicedate.Value.AsDateTimeString(true, true) : "";
                Nextservicedate = stationInformation.Nextservicedate.HasValue ? stationInformation.Nextservicedate.Value.AsDateTimeString(true, true) : "";
                Addworkforcedate = stationInformation.Addworkforcedate.HasValue ? stationInformation.Addworkforcedate.Value.AsDateTimeString(true, true) : "";
                Lastvisitdate = stationInformation.Lastvisitdate.HasValue ? stationInformation.Lastvisitdate.Value.AsDateTimeString(true, true) : "";
                Altstationid = stationInformation.Altstationid;
                Wmostationid = stationInformation.Wmostationid;
                Regionid = stationInformation.Regionid;
                Wigosid = stationInformation.Wigosid;
                Wmocountrycode = stationInformation.Wmocountrycode;
                Hha = stationInformation.Hha;
                Hhp = stationInformation.Hhp;
                Wmorbsn = stationInformation.Wmorbsn;
                Wmorbcn = stationInformation.Wmorbcn;
                Wmorbsnradio = stationInformation.Wmorbsnradio;
                Shape = stationInformation.Shape;

                if (_rowCharacteristicsMap.Object.ContainsKey(stationInformation.GdbArchiveOid))
                {
                    BusinessRuleViolationListViewModel.Populate(
                        _rowCharacteristicsMap.Object[stationInformation.GdbArchiveOid].ViolatedBusinessRules);
                }
                else
                {
                    BusinessRuleViolationListViewModel.Clear();
                }

                _nRecordsSelected = 1;
                _nCurrentRecordsSelected = stationInformation.GdbToDate.Year == 9999 ? 1 : 0;
            }
            else
            {
                Header = $"Properties of {stationInformations.Objects.Count()} Selected Records";

                Stationname = "";
                Stationid_dmi = "";
                StationType = "";
                AccessAddress = "";
                Country = "";
                Status = "";
                DateFrom = "";
                DateTo = "";
                StationOwner = "";
                Wgs_lat = "";
                Wgs_long = "";
                Gdb_archive_oid = -1;
                Objectid = -1;
                Globalid = "";
                Gdb_from_date = "";
                Gdb_to_date = "";
                Created_user = "";
                Created_date = "";
                Last_edited_user = "";
                Last_edited_date = "";
                Comment = "";
                Stationid_icao = "";
                Referencetomaintenanceagreement = "";
                Facilityid = "";
                Si_utm = new int?();
                Si_northing = new int?();
                Si_easting = new int?();
                Si_geo_lat = new int?();
                Si_geo_long = new int?();
                Serviceinterval = new int?();
                Lastservicedate = "";
                Nextservicedate = "";
                Addworkforcedate = "";
                Lastvisitdate = "";
                Altstationid = "";
                Wmostationid = "";
                Regionid = "";
                Wigosid = "";
                Wmocountrycode = "";
                Hha = new int?();
                Hhp = new int?();
                Wmorbsn = new int?();
                Wmorbcn = new int?();
                Wmorbsnradio = new int?();
                Shape = "";

                _nRecordsSelected = stationInformations.Objects.Count();
                _nCurrentRecordsSelected = stationInformations.Objects.Count(s => s.GdbToDate.Year == 9999);

                BusinessRuleViolationListViewModel.Clear();
            }

            IsVisible = _nRecordsSelected > 0;

            if (IsVisible)
            {
                UpdateState(StateOfView.Initial);

                _originalStationname = Stationname;
                _originalAccessAddress = AccessAddress;
                _original_wgs_lat = Wgs_lat;
                _original_wgs_long = Wgs_long;
                _originalStationType = StationType;
                _originalStatus = Status;
                _originalCountry = Country;
                _originalStationOwner = StationOwner;
                _originalDateFrom = DateFrom;
                _originalDateTo = DateTo;
            }

            RefreshButtons();
        }

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged("Stationname");
            RaisePropertyChanged("AccessAddress");
            RaisePropertyChanged("Wgs_lat");
            RaisePropertyChanged("Wgs_long");
            RaisePropertyChanged("StationType");
            RaisePropertyChanged("Status");
            RaisePropertyChanged("Country");
            RaisePropertyChanged("StationOwner");
            RaisePropertyChanged("DateFrom");
            RaisePropertyChanged("DateTo");
        }

        private void UpdateState(
            StateOfView state)
        {
            _state = state;
            RaisePropertyChanges();
        }

        public ObservableCollection<ValidationError> ValidationMessages
        {
            get
            {
                if (_validationMessages == null)
                {
                    _validationMessages = new ObservableCollection<ValidationError>
                    {
                        new ValidationError {PropertyName = "Stationname"},
                        new ValidationError {PropertyName = "AccessAddress"},
                        new ValidationError {PropertyName = "Wgs_lat"},
                        new ValidationError {PropertyName = "Wgs_long"},
                        new ValidationError {PropertyName = "DateFrom"},
                        new ValidationError {PropertyName = "DateTo"},
                    };
                }

                return _validationMessages;
            }
        }

        public string this[string columnName]
        {
            get
            {
                var errorMessage = string.Empty;

                if (_state == StateOfView.Updated)
                {
                    switch (columnName)
                    {
                        case "Stationname":
                            {
                                if (string.IsNullOrEmpty(Stationname))
                                {
                                    errorMessage = "Required";
                                }
                                else if (Stationname.Length > 500)
                                {
                                    errorMessage = "Must not exceed 500 characters";
                                }

                                break;
                            }
                        case "AccessAddress":
                        {
                            if (AccessAddress != null && AccessAddress.Length > 500)
                            {
                                errorMessage = "Must not exceed 500 characters";
                            }

                            break;
                        }
                        case "Wgs_lat":
                        {
                            double value;
                            if (!string.IsNullOrEmpty(Wgs_lat))
                            {
                                if (!double.TryParse(Wgs_lat, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                                {
                                    errorMessage = "Must be a valid number (use . as decimal separator)";
                                }
                                else if (value < -90.0 || value > 90.0)
                                {
                                    errorMessage = "Must be a number between -90 and 90";
                                }
                            }

                            break;
                        }
                        case "Wgs_long":
                        {
                            double value;
                            if (!string.IsNullOrEmpty(Wgs_long))
                            {
                                if (!double.TryParse(Wgs_long, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                                {
                                    errorMessage = "Must be a valid number (use . as decimal separator)";
                                }
                                else if (value < -180.0 || value > 180.0)
                                {
                                    errorMessage = "Must be a number between -180 and 180";
                                }
                            }

                            break;
                        }
                        case "DateFrom":
                        {
                            if (!string.IsNullOrEmpty(DateFrom))
                            {
                                if (!DateFrom.IsProperlyFormattedAsADateTime())
                                {
                                    errorMessage = "Format must be yyyy-mm-dd hh.mm.ss.fff";
                                }
                                else if (!DateFrom.TryParsingAsDateTime(out var dateTime))
                                {
                                    errorMessage = "Must be a valid date";
                                }
                            }

                            break;
                        }
                        case "DateTo":
                        {
                            if (!string.IsNullOrEmpty(DateTo))
                            {
                                if (!DateTo.IsProperlyFormattedAsADateTime())
                                {
                                    errorMessage = "Format must be yyyy-MM-dd or yyyy-MM-dd HH.mm.ss.fff";
                                }
                                else if (!DateTo.TryParsingAsDateTime(out var dateTime))
                                {
                                    errorMessage = "Must be a valid date";
                                }
                            }

                            break;
                        }
                    }
                }

                ValidationMessages
                    .First(e => e.PropertyName == columnName).ErrorMessage = errorMessage;

                return errorMessage;
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged();
            }
        }

        private void Manipulate(
            object owner)
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            var dialogViewModel = new MessageBoxDialogViewModel("Manipulate record?\n\n(this will update the existing record in the database without preserving information regarding when it was done and by whom, so it is generally not recommended)", true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var stationInformation = _selectedStationInformations.Objects.Single();

            CopyControlDataToStationInformation(stationInformation);

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.StationInformations.Update(stationInformation);
                unitOfWork.Complete();
            }

            _originalStationname = Stationname;
            _originalAccessAddress = AccessAddress;
            _original_wgs_lat = Wgs_lat;
            _original_wgs_long = Wgs_long;
            _originalStationType = StationType;
            _originalStatus = Status;
            _originalCountry = Country;
            _originalStationOwner = StationOwner;
            _originalDateFrom = DateFrom;
            _originalDateTo = DateTo;

            RefreshButtons();
            OnRepositoryOperationPerformed();
        }

        private bool CanManipulate(
            object owner)
        {
            return _nRecordsSelected == 1 && FormIsDirty();
        }

        // Indicating that the user changed some properties in the form
        private bool FormIsDirty()
        {
            return
                Stationname != _originalStationname ||
                AccessAddress != _originalAccessAddress ||
                Wgs_lat != _original_wgs_lat ||
                Wgs_long != _original_wgs_long ||
                StationType != _originalStationType ||
                Status != _originalStatus ||
                Country != _originalCountry ||
                StationOwner != _originalStationOwner ||
                DateFrom != _originalDateFrom ||
                DateTo != _originalDateTo;
        }

        private void Erase(
            object owner)
        {
            var message = _nRecordsSelected == 1
                ? "Erase record?\n\n(this will erase the record from the database without preserving information regarding when it was done and by whom, so it is generally not recommended)"
                : $"Erase {_nRecordsSelected} records?\n\n(this will erase the records from the database without preserving information regarding when it was done and by whom, so it is generally not recommended)";

            var dialogViewModel = new MessageBoxDialogViewModel(message, true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            var gdbArchiveOIDs = _selectedStationInformations.Objects.Select(s => s.GdbArchiveOid);

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsForDeletion = unitOfWork.StationInformations
                    .Find(s => gdbArchiveOIDs.Contains(s.GdbArchiveOid));

                unitOfWork.StationInformations.RemoveRange(stationInformationsForDeletion);
                unitOfWork.Complete();
            }

            RefreshButtons();
            OnRepositoryOperationPerformed();
        }

        private bool CanErase(
            object owner)
        {
            return _nRecordsSelected > 0 && !FormIsDirty();
        }

        private void Update()
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            // Vi skal opdatere den eksisterende række ved at sætte gdb_date_to til dags dato
            // Derudover skal vi lave en ny række med den nye information, og hvor gdb_from_date er lig med current time

            var stationInformation = _selectedStationInformations.Objects.Single();
            var newStationInformation = stationInformation.Clone();

            CopyControlDataToStationInformation(newStationInformation);

            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();
            stationInformation.GdbToDate = currentTime;
            newStationInformation.GdbArchiveOid = 0;
            newStationInformation.GdbFromDate = currentTime;
            newStationInformation.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            newStationInformation.LastEditedUser = SharedData.LoggedInUser;
            newStationInformation.LastEditedDate = currentTime;

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.StationInformations.Update(stationInformation);
                unitOfWork.StationInformations.Add(newStationInformation);
                unitOfWork.Complete();
            }

            _originalStationname = Stationname;
            _originalAccessAddress = AccessAddress;
            _original_wgs_lat = Wgs_lat;
            _original_wgs_long = Wgs_long;
            _originalStationType = StationType;
            _originalStatus = Status;
            _originalCountry = Country;
            _originalStationOwner = StationOwner;
            _originalDateFrom = DateFrom;
            _originalDateTo = DateTo;

            RefreshButtons();
            OnRepositoryOperationPerformed();
        }

        private bool CanUpdate()
        {
            return
                _nRecordsSelected == 1 &&
                _nCurrentRecordsSelected == 1 &&
                FormIsDirty();
        }

        private void Delete(
            object owner)
        {
            var message = _nRecordsSelected == 1
                ? "Delete record?\n\n(this will logically delete the record from the database while preserving information regarding when it was done and by whom, so the record may be undeleted or \"promoted\" later, if necessary)"
                : $"Delete {_nRecordsSelected} records?\n\n(this will logically delete the records from the database while preserving information regarding when it was done and by whom, so the records may be undeleted or \"promoted\" later, if necessary)";

            var dialogViewModel = new MessageBoxDialogViewModel(message, true);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            // Vi opdaterer de eksisterende rækker ved at sætte gdb_date_to til dags dato
            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

            foreach (var stationInformation in _selectedStationInformations.Objects)
            {
                stationInformation.GdbToDate = currentTime;
            }

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.StationInformations.UpdateRange(_selectedStationInformations.Objects.ToList());
                unitOfWork.Complete();
            }

            RefreshButtons();
            OnRepositoryOperationPerformed();
        }

        private bool CanDelete(
            object owner)
        {
            return 
                _nRecordsSelected > 0 &&
                _nRecordsSelected ==_nCurrentRecordsSelected && 
                !FormIsDirty();
        }

        private void Promote(
            object owner)
        {
            UpdateState(StateOfView.Updated);

            Error = string.Join("",
                ValidationMessages.Select(e => e.ErrorMessage).ToArray());

            if (!string.IsNullOrEmpty(Error))
            {
                return;
            }

            // Vi laver en ny række, som er current
            var newStationInformation = _selectedStationInformations.Objects.Single().Clone();

            var defaultDateFrom = _rowCharacteristicsMap.Object[newStationInformation.GdbArchiveOid].LatestTimeInPastWhenHistoricallyRelevantFieldsWereChanged;

            if (!defaultDateFrom.HasValue)
            {
                defaultDateFrom = newStationInformation.DateFrom;
            }

            var defaultDateTo = newStationInformation.GdbToDate;

            var dialogViewModel = new PromoteStationInformationRecordDialogViewModel(defaultDateFrom, defaultDateTo);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            // Vi skriver den nye række til databasen
            var currentTime = DateTime.UtcNow.TruncateToMilliseconds();
            newStationInformation.GlobalId = Guid.NewGuid().ToString();
            newStationInformation.GdbFromDate = currentTime;
            newStationInformation.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            newStationInformation.CreatedUser = SharedData.LoggedInUser;
            newStationInformation.CreatedDate = currentTime;
            newStationInformation.LastEditedUser = null;
            newStationInformation.LastEditedDate = null;

            dialogViewModel.DateFrom.TryParsingAsDateTime(out var dateFrom);
            newStationInformation.DateFrom = dateFrom;

            dialogViewModel.DateTo.TryParsingAsDateTime(out var dateTo);
            newStationInformation.DateTo = dateTo;

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                newStationInformation.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                newStationInformation.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                unitOfWork.StationInformations.Add(newStationInformation);
                unitOfWork.Complete();
            }

            RefreshButtons();
            OnRepositoryOperationPerformed();
        }

        private bool CanPromote(
            object owner)
        {
            return
                _nRecordsSelected == 1 &&
                _nCurrentRecordsSelected == 0 &&
                !CanDiscardChanges();
        }

        private void Merge(
            object owner)
        {
            var temp = _selectedStationInformations;

            var startTime = _selectedStationInformations.Objects.First().DateFrom.Value;
            var endTime = _selectedStationInformations.Objects.Last().DateTo;
            var transitionDate = _selectedStationInformations.Objects.Last().DateFrom.Value;

            var dialogViewModel = new MergeStationInformationRecordsDialogViewModel(startTime, endTime, transitionDate);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            if (dialogViewModel.TransitionTime == dialogViewModel.StartTime)
            {
                // Let the SECOND interval absorb the FIRST,
                // so.. delete both, but create a new one as a replacement for the SECOND

                var stationInformationBefore = _selectedStationInformations.Objects.First();
                var stationInformationAfter = _selectedStationInformations.Objects.Last();
                var stationInformationNew = stationInformationAfter.Clone();

                var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

                stationInformationBefore.GdbToDate = currentTime;
                stationInformationBefore.LastEditedUser = SharedData.LoggedInUser;
                stationInformationBefore.LastEditedDate = currentTime;

                stationInformationAfter.GdbToDate = currentTime;
                stationInformationAfter.LastEditedUser = SharedData.LoggedInUser;
                stationInformationAfter.LastEditedDate = currentTime;

                stationInformationNew.DateFrom = startTime;
                stationInformationNew.DateTo = endTime;
                stationInformationNew.CreatedUser = SharedData.LoggedInUser;
                stationInformationNew.CreatedDate = currentTime;
                stationInformationNew.LastEditedUser = null;
                stationInformationNew.LastEditedDate = null;

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.StationInformations.Update(stationInformationBefore);
                    unitOfWork.StationInformations.Update(stationInformationAfter);
                    stationInformationNew.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                    stationInformationNew.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                    unitOfWork.StationInformations.Add(stationInformationNew);
                    unitOfWork.Complete();
                }

                OnRepositoryOperationPerformed();
            }
            else if (dialogViewModel.TransitionTime == dialogViewModel.EndTime)
            {
                // Let the SECOND interval absorb the FIRST,
                // so.. delete both, but create a new one as a replacement for the SECOND

                var stationInformationBefore = _selectedStationInformations.Objects.First();
                var stationInformationAfter = _selectedStationInformations.Objects.Last();
                var stationInformationNew = stationInformationBefore.Clone();

                var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

                stationInformationBefore.GdbToDate = currentTime;
                stationInformationBefore.LastEditedUser = SharedData.LoggedInUser;
                stationInformationBefore.LastEditedDate = currentTime;

                stationInformationAfter.GdbToDate = currentTime;
                stationInformationAfter.LastEditedUser = SharedData.LoggedInUser;
                stationInformationAfter.LastEditedDate = currentTime;

                stationInformationNew.DateFrom = startTime;
                stationInformationNew.DateTo = endTime;
                stationInformationNew.CreatedUser = SharedData.LoggedInUser;
                stationInformationNew.CreatedDate = currentTime;
                stationInformationNew.LastEditedUser = null;
                stationInformationNew.LastEditedDate = null;

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.StationInformations.Update(stationInformationBefore);
                    unitOfWork.StationInformations.Update(stationInformationAfter);
                    stationInformationNew.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                    stationInformationNew.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                    unitOfWork.StationInformations.Add(stationInformationNew);
                    unitOfWork.Complete();
                }

                OnRepositoryOperationPerformed();
            }
            else
            {
                // Shift transition time, i.e. delete the one that expands, and update the one that shrinks
                dialogViewModel.TransitionTime.TryParsingAsDateTime(out var newTransitionTime);

                if (newTransitionTime < transitionDate)
                {
                    // Update the FIRST, delete the SECOND, and make a clone of the second
                    var stationInformationBefore = _selectedStationInformations.Objects.First();
                    var stationInformationAfter = _selectedStationInformations.Objects.Last();
                    var stationInformationNewBefore = stationInformationBefore.Clone();
                    var stationInformationNewAfter = stationInformationAfter.Clone();

                    var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

                    stationInformationBefore.GdbToDate = currentTime;
                    stationInformationBefore.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationBefore.LastEditedDate = currentTime;

                    stationInformationAfter.GdbToDate = currentTime;
                    stationInformationAfter.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationAfter.LastEditedDate = currentTime;

                    stationInformationNewBefore.DateFrom = startTime;
                    stationInformationNewBefore.DateTo = newTransitionTime;
                    stationInformationNewBefore.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationNewBefore.LastEditedDate = currentTime;

                    stationInformationNewAfter.DateFrom = newTransitionTime;
                    stationInformationNewAfter.DateTo = endTime;
                    stationInformationNewAfter.CreatedUser = SharedData.LoggedInUser;
                    stationInformationNewAfter.CreatedDate = currentTime;
                    stationInformationNewAfter.LastEditedUser = null;
                    stationInformationNewAfter.LastEditedDate = null;

                    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                    {
                        unitOfWork.StationInformations.Update(stationInformationBefore);
                        unitOfWork.StationInformations.Update(stationInformationAfter);
                        unitOfWork.StationInformations.Add(stationInformationNewBefore);
                        stationInformationNewAfter.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                        stationInformationNewAfter.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                        unitOfWork.StationInformations.Add(stationInformationNewAfter);
                        unitOfWork.Complete();
                    }
                }
                else
                {
                    // Update the SECOND, delete the FIRST, and make a clone of the first
                    var stationInformationBefore = _selectedStationInformations.Objects.First();
                    var stationInformationAfter = _selectedStationInformations.Objects.Last();
                    var stationInformationNewBefore = stationInformationBefore.Clone();
                    var stationInformationNewAfter = stationInformationAfter.Clone();

                    var currentTime = DateTime.UtcNow.TruncateToMilliseconds();

                    stationInformationBefore.GdbToDate = currentTime;
                    stationInformationBefore.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationBefore.LastEditedDate = currentTime;

                    stationInformationAfter.GdbToDate = currentTime;
                    stationInformationAfter.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationAfter.LastEditedDate = currentTime;

                    stationInformationNewBefore.DateFrom = startTime;
                    stationInformationNewBefore.DateTo = newTransitionTime;
                    stationInformationNewBefore.LastEditedUser = SharedData.LoggedInUser;
                    stationInformationNewBefore.LastEditedDate = currentTime;
                    stationInformationNewBefore.LastEditedUser = null;
                    stationInformationNewBefore.LastEditedDate = null;

                    stationInformationNewAfter.DateFrom = newTransitionTime;
                    stationInformationNewAfter.DateTo = endTime;
                    stationInformationNewAfter.CreatedUser = SharedData.LoggedInUser;
                    stationInformationNewAfter.CreatedDate = currentTime;

                    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                    {
                        unitOfWork.StationInformations.Update(stationInformationBefore);
                        unitOfWork.StationInformations.Update(stationInformationAfter);
                        stationInformationNewBefore.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                        stationInformationNewBefore.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                        unitOfWork.StationInformations.Add(stationInformationNewBefore);
                        unitOfWork.StationInformations.Add(stationInformationNewAfter);
                        unitOfWork.Complete();
                    }
                }

                OnRepositoryOperationPerformed();
            }
        }

        private bool CanMerge(
            object owner)
        {
            return 
                _nRecordsSelected == 2 &&
                _nCurrentRecordsSelected == 2;
        }

        private void DiscardChanges()
        {
            UpdateState(StateOfView.Initial);

            Error = null;

            Stationname = _originalStationname;
            AccessAddress = _originalAccessAddress;
            Wgs_lat = _original_wgs_lat;
            Wgs_long = _original_wgs_long;
            StationType = _originalStationType;
            Status = _originalStatus;
            Country = _originalCountry;
            StationOwner = _originalStationOwner;
            DateFrom = _originalDateFrom;
            DateTo = _originalDateTo;
        }

        private bool CanDiscardChanges()
        {
            return FormIsDirty();
        }

        private void RefreshButtons()
        {
            DiscardChangesCommand.RaiseCanExecuteChanged();
            ManipulateCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();
            PromoteCommand.RaiseCanExecuteChanged();
            EraseCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            MergeCommand.RaiseCanExecuteChanged();
        }

        private void CopyControlDataToStationInformation(
            StationInformation stationInformation)
        {
            stationInformation.StationName = Stationname;
            stationInformation.AccessAddress = AccessAddress;
            stationInformation.Wgs_lat = string.IsNullOrEmpty(Wgs_lat) ? new double?() : double.Parse(Wgs_lat, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            stationInformation.Wgs_long = string.IsNullOrEmpty(Wgs_long) ? new double?() : double.Parse(Wgs_long, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            stationInformation.Stationtype = string.IsNullOrEmpty(StationType) ? new StationType?() : SharedData.StationTypeDisplayTextMap.Single(kvp => kvp.Value == StationType).Key;
            stationInformation.Status = string.IsNullOrEmpty(Status) ? new Status?() : SharedData.StatusDisplayTextMap.Single(kvp => kvp.Value == Status).Key;
            stationInformation.Country = string.IsNullOrEmpty(Country) ? new Country?() : SharedData.CountryDisplayTextMap.Single(kvp => kvp.Value == Country).Key;
            stationInformation.StationOwner = string.IsNullOrEmpty(StationOwner) ? new StationOwner?() : SharedData.StationOwnerDisplayTextMap.Single(kvp => kvp.Value == StationOwner).Key;

            if (string.IsNullOrEmpty(DateFrom))
            {
                stationInformation.DateFrom = new DateTime?();
            }
            else
            {
                DateFrom.TryParsingAsDateTime(out var dateFrom);
                stationInformation.DateFrom = dateFrom;
            }

            if (string.IsNullOrEmpty(DateTo))
            {
                stationInformation.DateTo = new DateTime?();
            }
            else
            {
                DateTo.TryParsingAsDateTime(out var dateTo);
                stationInformation.DateTo = dateTo;
            }
        }

        protected virtual void OnRepositoryOperationPerformed()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = RepositoryOperationPerformed;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
