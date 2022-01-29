using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Application;

namespace DMI.SMS.ViewModel
{
    public enum Option
    {
        Option1,
        Option2,
        Option3
    }

    public class FindStationInformationsViewModel : ViewModelBase
    {
        private Option _currentOption = Option.Option3;

        private string _nameFilter = "";
        private string _nameFilterInUppercase = "";

        private string _stationIdFilter = "";
        private string _objectIdFilter = "";

        // Gdb_date filter
        private bool _includeCurrent = true;
        private bool _includeOutdated = false;
        private bool _includeDeleted = false;

        // Status filter
        private bool _includeStatusActive = false;
        private bool _includeStatusDiscontinued = false;
        private bool _includeStatusUnspecified = false;

        // Stationstype filter
        private bool _includeStationTypeSynop = false;
        private bool _includeStationTypeStroemStation = false;
        private bool _includeStationTypeVandstand = false;
        private bool _includeStationTypeSVKGPRS = false;
        private bool _includeStationTypeGIWS = false;
        private bool _includeStationTypePluvio = false;
        private bool _includeStationTypeShipAWS = false;
        private bool _includeStationTypeTempShip = false;
        private bool _includeStationTypeLynPejleStation = false;
        private bool _includeStationTypeRadar = false;
        private bool _includeStationTypeRadiosonde = false;
        private bool _includeStationTypeHistoriskStationstype = false;
        private bool _includeStationTypeManuelNedbør = false;
        private bool _includeStationTypeBølgestation = false;
        private bool _includeStationTypeSnestation = false;
        private bool _includeStationTypeUnspecified = false;

        // Station owner filter
        private bool _includeStationOwnerDMI = false;
        private bool _includeStationOwnerSVK = false;
        private bool _includeStationOwnerHavne_Kommuner_mv = false;
        private bool _includeStationOwnerGC_net_Greenland_Climate_data = false;
        private bool _includeStationOwnerDanske_lufthavne = false;
        private bool _includeStationOwnerMITT_GRL_lufthavne = false;
        private bool _includeStationOwnerVejdirektoratet = false;
        private bool _includeStationOwnerSynop_Aarhus_Uni = false;
        private bool _includeStationOwnerAsiaq = false;
        private bool _includeStationOwnerKystdirektoratet = false;
        private bool _includeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland = false;
        private bool _includeStationOwnerForsvaret = false;
        private bool _includeStationOwnerUnspecified = false;

        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;

                _nameFilterInUppercase = _nameFilter == null ? "" : _nameFilter.ToUpper();
                RaisePropertyChanged();
            }
        }

        public string StationIdFilter
        {
            get { return _stationIdFilter; }
            set
            {
                _stationIdFilter = value;
                RaisePropertyChanged();
            }
        }

        public string ObjectIdFilter
        {
            get { return _objectIdFilter; }
            set
            {
                _objectIdFilter = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeCurrent
        {
            get { return _includeCurrent; }
            set
            {
                _includeCurrent = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeOutdated
        {
            get { return _includeOutdated; }
            set
            {
                _includeOutdated = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeDeleted
        {
            get { return _includeDeleted; }
            set
            {
                _includeDeleted = value;
                RaisePropertyChanged();
            }
        }

        public Option CurrentOption
        {
            get { return _currentOption; }
            set
            {
                _currentOption = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStatusActive
        {
            get { return _includeStatusActive; }
            set
            {
                _includeStatusActive = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStatusDiscontinued
        {
            get { return _includeStatusDiscontinued; }
            set
            {
                _includeStatusDiscontinued = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStatusUnspecified
        {
            get { return _includeStatusUnspecified; }
            set
            {
                _includeStatusUnspecified = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeSynop
        {
            get { return _includeStationTypeSynop; }
            set
            {
                _includeStationTypeSynop = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeStroemStation
        {
            get { return _includeStationTypeStroemStation; }
            set
            {
                _includeStationTypeStroemStation = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeVandstand
        {
            get { return _includeStationTypeVandstand; }
            set
            {
                _includeStationTypeVandstand = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeSVKGPRS
        {
            get { return _includeStationTypeSVKGPRS; }
            set
            {
                _includeStationTypeSVKGPRS = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeGIWS
        {
            get { return _includeStationTypeGIWS; }
            set
            {
                _includeStationTypeGIWS = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypePluvio
        {
            get { return _includeStationTypePluvio; }
            set
            {
                _includeStationTypePluvio = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeShipAWS
        {
            get { return _includeStationTypeShipAWS; }
            set
            {
                _includeStationTypeShipAWS = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeTempShip
        {
            get { return _includeStationTypeTempShip; }
            set
            {
                _includeStationTypeTempShip = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeLynPejleStation
        {
            get { return _includeStationTypeLynPejleStation; }
            set
            {
                _includeStationTypeLynPejleStation = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeRadar
        {
            get { return _includeStationTypeRadar; }
            set
            {
                _includeStationTypeRadar = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeRadiosonde
        {
            get { return _includeStationTypeRadiosonde; }
            set
            {
                _includeStationTypeRadiosonde = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeHistoriskStationstype
        {
            get { return _includeStationTypeHistoriskStationstype; }
            set
            {
                _includeStationTypeHistoriskStationstype = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeManuelNedbør
        {
            get { return _includeStationTypeManuelNedbør; }
            set
            {
                _includeStationTypeManuelNedbør = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeBølgestation
        {
            get { return _includeStationTypeBølgestation; }
            set
            {
                _includeStationTypeBølgestation = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeSnestation
        {
            get { return _includeStationTypeSnestation; }
            set
            {
                _includeStationTypeSnestation = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationTypeUnspecified
        {
            get { return _includeStationTypeUnspecified; }
            set
            {
                _includeStationTypeUnspecified = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerDMI
        {
            get { return _includeStationOwnerDMI; }
            set
            {
                _includeStationOwnerDMI = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerSVK
        {
            get { return _includeStationOwnerSVK; }
            set
            {
                _includeStationOwnerSVK = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerHavne_Kommuner_mv
        {
            get { return _includeStationOwnerHavne_Kommuner_mv; }
            set
            {
                _includeStationOwnerHavne_Kommuner_mv = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerGC_net_Greenland_Climate_data
        {
            get { return _includeStationOwnerGC_net_Greenland_Climate_data; }
            set
            {
                _includeStationOwnerGC_net_Greenland_Climate_data = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerDanske_lufthavne
        {
            get { return _includeStationOwnerDanske_lufthavne; }
            set
            {
                _includeStationOwnerDanske_lufthavne = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerMITT_GRL_lufthavne
        {
            get { return _includeStationOwnerMITT_GRL_lufthavne; }
            set
            {
                _includeStationOwnerMITT_GRL_lufthavne = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerVejdirektoratet
        {
            get { return _includeStationOwnerVejdirektoratet; }
            set
            {
                _includeStationOwnerVejdirektoratet = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerSynop_Aarhus_Uni
        {
            get { return _includeStationOwnerSynop_Aarhus_Uni; }
            set
            {
                _includeStationOwnerSynop_Aarhus_Uni = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerAsiaq
        {
            get { return _includeStationOwnerAsiaq; }
            set
            {
                _includeStationOwnerAsiaq = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerKystdirektoratet
        {
            get { return _includeStationOwnerKystdirektoratet; }
            set
            {
                _includeStationOwnerKystdirektoratet = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland
        {
            get { return _includeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland; }
            set
            {
                _includeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerForsvaret
        {
            get { return _includeStationOwnerForsvaret; }
            set
            {
                _includeStationOwnerForsvaret = value;
                RaisePropertyChanged();
            }
        }

        public bool IncludeStationOwnerUnspecified
        {
            get { return _includeStationOwnerUnspecified; }
            set
            {
                _includeStationOwnerUnspecified = value;
                RaisePropertyChanged();
            }
        }

        public bool FilterInPlace
        {
            get
            {
                return
                    GdbDateFilterInPlace ||
                    StationNameFilterInPlace ||
                    StationIdFilterInPlace ||
                    ObjectIdFilterInPlace ||
                    StatusFilterInPlace ||
                    StationTypeFilterInPlace ||
                    StationOwnerFilterInPlace;
            }
        }

        public bool GdbDateFilterInPlace
        {
            get
            {
                if (_includeCurrent &&
                    _includeOutdated &&
                    _includeDeleted)
                {
                    return false;
                }

                return
                    _includeCurrent ||
                    _includeOutdated ||
                    _includeDeleted;
            }
        }

        public void ClearFilters()
        {
            NameFilter = null;
            StationIdFilter = null;
            ObjectIdFilter = null;

            IncludeCurrent = false;
            IncludeDeleted = false;
            IncludeOutdated = false;

            IncludeStatusActive = false;
            IncludeStatusDiscontinued = false;
            IncludeStatusUnspecified = false;

            IncludeStationTypeSynop = false;
            IncludeStationTypeStroemStation = false;
            IncludeStationTypeVandstand = false;
            IncludeStationTypeSVKGPRS = false;
            IncludeStationTypeGIWS = false;
            IncludeStationTypePluvio = false;
            IncludeStationTypeShipAWS = false;
            IncludeStationTypeTempShip = false;
            IncludeStationTypeLynPejleStation = false;
            IncludeStationTypeRadar = false;
            IncludeStationTypeRadiosonde = false;
            IncludeStationTypeHistoriskStationstype = false;
            IncludeStationTypeManuelNedbør = false;
            IncludeStationTypeBølgestation = false;
            IncludeStationTypeSnestation = false;
            IncludeStationTypeUnspecified = false;

            IncludeStationOwnerDMI = false;
            IncludeStationOwnerSVK = false;
            IncludeStationOwnerHavne_Kommuner_mv = false;
            IncludeStationOwnerGC_net_Greenland_Climate_data = false;
            IncludeStationOwnerDanske_lufthavne = false;
            IncludeStationOwnerMITT_GRL_lufthavne = false;
            IncludeStationOwnerVejdirektoratet = false;
            IncludeStationOwnerSynop_Aarhus_Uni = false;
            IncludeStationOwnerAsiaq = false;
            IncludeStationOwnerKystdirektoratet = false;
            IncludeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland = false;
            IncludeStationOwnerForsvaret = false;
            IncludeStationOwnerUnspecified = false;
        }

        private bool StationNameFilterInPlace
        {
            get
            {
                return !string.IsNullOrEmpty(_nameFilter);
            }
        }

        private bool StationIdFilterInPlace
        {
            get
            {
                return !string.IsNullOrEmpty(_stationIdFilter);
            }
        }

        private bool ObjectIdFilterInPlace
        {
            get
            {
                return !string.IsNullOrEmpty(_objectIdFilter);
            }
        }

        private bool StatusFilterInPlace
        {
            get
            {
                if (IncludeStatusActive &&
                    IncludeStatusDiscontinued &&
                    IncludeStatusUnspecified)
                {
                    return false;
                }

                return
                    IncludeStatusActive ||
                    IncludeStatusDiscontinued ||
                    IncludeStatusUnspecified;
            }
        }

        private bool StationTypeFilterInPlace
        {
            get
            {
                if (IncludeStationTypeSynop &&
                    IncludeStationTypeStroemStation &&
                    IncludeStationTypeSVKGPRS &&
                    IncludeStationTypeVandstand &&
                    IncludeStationTypeGIWS &&
                    IncludeStationTypePluvio &&
                    IncludeStationTypeShipAWS &&
                    IncludeStationTypeTempShip &&
                    IncludeStationTypeLynPejleStation &&
                    IncludeStationTypeRadar &&
                    IncludeStationTypeRadiosonde &&
                    IncludeStationTypeHistoriskStationstype &&
                    IncludeStationTypeManuelNedbør &&
                    IncludeStationTypeBølgestation &&
                    IncludeStationTypeSnestation &&
                    IncludeStationTypeUnspecified)
                {
                    return false;
                }

                return
                    IncludeStationTypeSynop ||
                    IncludeStationTypeStroemStation ||
                    IncludeStationTypeSVKGPRS ||
                    IncludeStationTypeVandstand ||
                    IncludeStationTypeGIWS ||
                    IncludeStationTypePluvio ||
                    IncludeStationTypeShipAWS ||
                    IncludeStationTypeTempShip ||
                    IncludeStationTypeLynPejleStation ||
                    IncludeStationTypeRadar ||
                    IncludeStationTypeRadiosonde ||
                    IncludeStationTypeHistoriskStationstype ||
                    IncludeStationTypeManuelNedbør ||
                    IncludeStationTypeBølgestation ||
                    IncludeStationTypeSnestation ||
                    IncludeStationTypeUnspecified;
            }
        }

        private bool StationOwnerFilterInPlace
        {
            get
            {
                if (IncludeStationOwnerDMI &&
                    IncludeStationOwnerSVK &&
                    IncludeStationOwnerHavne_Kommuner_mv &&
                    IncludeStationOwnerGC_net_Greenland_Climate_data &&
                    IncludeStationOwnerDanske_lufthavne &&
                    IncludeStationOwnerMITT_GRL_lufthavne &&
                    IncludeStationOwnerVejdirektoratet &&
                    IncludeStationOwnerSynop_Aarhus_Uni &&
                    IncludeStationOwnerAsiaq &&
                    IncludeStationOwnerKystdirektoratet &&
                    IncludeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland &&
                    IncludeStationOwnerForsvaret &&
                    IncludeStationOwnerUnspecified)
                {
                    return false;
                }

                return
                    IncludeStationOwnerDMI ||
                    IncludeStationOwnerSVK ||
                    IncludeStationOwnerHavne_Kommuner_mv ||
                    IncludeStationOwnerGC_net_Greenland_Climate_data ||
                    IncludeStationOwnerDanske_lufthavne ||
                    IncludeStationOwnerMITT_GRL_lufthavne ||
                    IncludeStationOwnerVejdirektoratet ||
                    IncludeStationOwnerSynop_Aarhus_Uni ||
                    IncludeStationOwnerAsiaq ||
                    IncludeStationOwnerKystdirektoratet ||
                    IncludeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland ||
                    IncludeStationOwnerForsvaret ||
                    IncludeStationOwnerUnspecified;
            }
        }

        public IList<Expression<Func<StationInformation, bool>>> FilterAsExpressionCollection()
        {
            if (!FilterInPlace)
            {
                throw new InvalidOperationException();
            }

            var result = new List<Expression<Func<StationInformation, bool>>>();

            if (GdbDateFilterInPlace &&
                !ConditionFilteringInMemoryRequired)
            {
                var maxDate = new DateTime(9999, 12, 31, 23, 59, 59);
                result.Add(s => s.GdbToDate == maxDate);
            }

            if (StationNameFilterInPlace)
            {
                result.Add(s => s.StationName != null && s.StationName.ToUpper().Contains(_nameFilterInUppercase));
            }

            if (StationIdFilterInPlace)
            {
                result.Add(s => s.StationIDDMI != null && s.StationIDDMI.ToString().Contains(_stationIdFilter));
            }

            if (ObjectIdFilterInPlace)
            {
                var objectIdFilter = int.Parse(_objectIdFilter);
                result.Add(s => s.ObjectId == objectIdFilter);
            }

            if (StatusFilterInPlace)
            {
                var filter = new List<Status>();

                if (IncludeStatusActive)
                {
                    filter.Add(Status.Active);
                }

                if (IncludeStatusDiscontinued)
                {
                    filter.Add(Status.Inactive);
                }

                if (IncludeStatusUnspecified)
                {
                    if (filter.Count > 0)
                    {
                        result.Add(s => s.Status == null || filter.Contains(s.Status.Value));
                    }
                    else
                    {
                        result.Add(s => s.Status == null);
                    }
                }
                else
                {
                    result.Add(s => s.Status != null && filter.Contains(s.Status.Value));
                }
            }

            if (StationTypeFilterInPlace)
            {
                var filter = new List<StationType>();

                if (IncludeStationTypeSynop)
                {
                    filter.Add(StationType.Synop);
                }

                if (IncludeStationTypeStroemStation)
                {
                    filter.Add(StationType.Strømstation);
                }

                if (IncludeStationTypeSVKGPRS)
                {
                    filter.Add(StationType.SVK_gprs);
                }

                if (IncludeStationTypeVandstand)
                {
                    filter.Add(StationType.Vandstandsstation);
                }

                if (IncludeStationTypeGIWS)
                {
                    filter.Add(StationType.GIWS);
                }

                if (IncludeStationTypePluvio)
                {
                    filter.Add(StationType.Pluvio);
                }

                if (IncludeStationTypeShipAWS)
                {
                    filter.Add(StationType.SHIP_AWS);
                }

                if (IncludeStationTypeTempShip)
                {
                    filter.Add(StationType.Temp_ship);
                }

                if (IncludeStationTypeLynPejleStation)
                {
                    filter.Add(StationType.Lynpejlestation);
                }

                if (IncludeStationTypeRadar)
                {
                    filter.Add(StationType.Radar);
                }

                if (IncludeStationTypeRadiosonde)
                {
                    filter.Add(StationType.Radiosonde);
                }

                if (IncludeStationTypeHistoriskStationstype)
                {
                    filter.Add(StationType.Historisk_stationstype);
                }

                if (IncludeStationTypeManuelNedbør)
                {
                    filter.Add(StationType.Manuel_nedbør);
                }

                if (IncludeStationTypeBølgestation)
                {
                    filter.Add(StationType.Bølgestation);
                }

                if (IncludeStationTypeSnestation)
                {
                    filter.Add(StationType.Snestation);
                }

                if (IncludeStationTypeUnspecified)
                {
                    if (filter.Count > 0)
                    {
                        result.Add(s => s.Stationtype == null || filter.Contains(s.Stationtype.Value));
                    }
                    else
                    {
                        result.Add(s => s.Stationtype == null);
                    }
                }
                else
                {
                    result.Add(s => s.Stationtype != null && filter.Contains(s.Stationtype.Value));
                }
            }

            if (StationOwnerFilterInPlace)
            {
                var filter = new List<StationOwner>();

                if (IncludeStationOwnerDMI)
                {
                    filter.Add(StationOwner.DMI);
                }

                if (IncludeStationOwnerSVK)
                {
                    filter.Add(StationOwner.SVK);
                }

                if (IncludeStationOwnerHavne_Kommuner_mv)
                {
                    filter.Add(StationOwner.Havne_Kommuner_mv);
                }

                if (IncludeStationOwnerGC_net_Greenland_Climate_data)
                {
                    filter.Add(StationOwner.GC_net_Greenland_Climate_data);
                }

                if (IncludeStationOwnerDanske_lufthavne)
                {
                    filter.Add(StationOwner.Danske_lufthavne);
                }

                if (IncludeStationOwnerMITT_GRL_lufthavne)
                {
                    filter.Add(StationOwner.MITT_GRL_lufthavne);
                }

                if (IncludeStationOwnerVejdirektoratet)
                {
                    filter.Add(StationOwner.Vejdirektoratet);
                }

                if (IncludeStationOwnerSynop_Aarhus_Uni)
                {
                    filter.Add(StationOwner.Synop_Aarhus_Uni);
                }

                if (IncludeStationOwnerAsiaq)
                {
                    filter.Add(StationOwner.Asiaq);
                }

                if (IncludeStationOwnerKystdirektoratet)
                {
                    filter.Add(StationOwner.Kystdirektoratet);
                }

                if (IncludeStationOwnerPROMICE_GEUS_PROMICE_net_i_Grønland)
                {
                    filter.Add(StationOwner.PROMICE_GEUS_PROMICE_net_i_Grønland);
                }

                if (IncludeStationOwnerForsvaret)
                {
                    filter.Add(StationOwner.Forsvaret);
                }

                if (IncludeStationOwnerUnspecified)
                {
                    if (filter.Count > 0)
                    {
                        result.Add(s => s.StationOwner == null || filter.Contains(s.StationOwner.Value));
                    }
                    else
                    {
                        result.Add(s => s.StationOwner == null);
                    }
                }
                else
                {
                    result.Add(s => s.StationOwner != null &&  filter.Contains(s.StationOwner.Value));
                }
            }

            return result;
        }

        public HashSet<RowCondition> InMemoryConditionFilter
        {
            get
            {
                if (!IncludeCurrent && !IncludeOutdated && !IncludeDeleted)
                {
                    return new HashSet<RowCondition>
                    {
                        RowCondition.Current,
                        RowCondition.OutDated,
                        RowCondition.Deleted
                    };
                }

                var filter = new HashSet<RowCondition>();

                if (IncludeCurrent)
                {
                    filter.Add(RowCondition.Current);
                }

                if (IncludeOutdated)
                {
                    filter.Add(RowCondition.OutDated);
                }

                if (IncludeDeleted)
                {
                    filter.Add(RowCondition.Deleted);
                }

                return filter;
            }
        }

        public bool ConditionFilteringInMemoryRequired
        {
            get
            {
                // Sådan her var det før, men det lader ikke til at være korrekt (11-08-2021)
                //return !(IncludeCurrent && !IncludeOutdated && !IncludeDeleted);

                // Det burde egentlig bare være et spørgsmål om at sige, at hvis vi skal have
                // Outdated eller Deleted med og vel at mærke kun en af dem, så er der behov for
                // at hive alle rækker op og så filtrere
                return IncludeOutdated != IncludeDeleted;
            }
        }
    }
}
