using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.ViewModel
{
    public class FindStationsViewModel : ViewModelBase
    {
        private string _countryFilter = "Danmark";
        private string _countryFilterInUppercase = "DANMARK";

        private string _stationIdFilter = "";

        public string CountryFilter
        {
            get { return _countryFilter; }
            set
            {
                _countryFilter = value;

                _countryFilterInUppercase = _countryFilter == null ? "" : _countryFilter.ToUpper();
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

        public bool FilterInPlace
        {
            get
            {
                return
                    CountryFilterInPlace ||
                    StationIdFilterInPlace;
            }
        }

        private bool CountryFilterInPlace
        {
            get
            {
                return !string.IsNullOrEmpty(_countryFilter);
            }
        }

        private bool StationIdFilterInPlace
        {
            get
            {
                return !string.IsNullOrEmpty(_stationIdFilter);
            }
        }

        public IList<Expression<Func<Station, bool>>> FilterAsExpressionCollection()
        {
            if (!FilterInPlace)
            {
                throw new InvalidOperationException();
            }

            var result = new List<Expression<Func<Station, bool>>>();

            if (CountryFilterInPlace)
            {
                result.Add(s => s.Country != null && s.Country.ToUpper().Contains(_countryFilterInUppercase));
            }

            if (StationIdFilterInPlace)
            {
                result.Add(s => s.StatID.ToString().Contains(_stationIdFilter));
            }

            return result;
        }
    }
}
