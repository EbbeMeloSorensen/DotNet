using System;
using System.Collections.ObjectModel;
using Craft.ImageEditor.ViewModel;

namespace Craft.ViewModels.Chronology
{
    public class ChronologyViewModel : ImageEditorViewModel
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _isVisible;
        private ObservableCollection<VerticalLineViewModel> _verticalLineViewModels;
        private ObservableCollection<TimeIntervalBarViewModel> _timeIntervalBarViewModels;

        private double WidthOfLaneLabelColumn { get; }
        private double WidthOfBarLabelBufferColumn { get; }

        public ObservableCollection<VerticalLineViewModel> VerticalLineViewModels
        {
            get
            {
                return _verticalLineViewModels;
            }
            set
            {
                _verticalLineViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TimeIntervalBarViewModel> TimeIntervalBarViewModels
        {
            get
            {
                return _timeIntervalBarViewModels;
            }
            set
            {
                _timeIntervalBarViewModels = value;
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

        public ChronologyViewModel(
            DateTime startTime,
            DateTime endTime,
            double widthOfLaneLabelColumn,
            double widthOfBarLabelBufferColumn)
        {
            _startTime = startTime;
            _endTime = endTime;
            WidthOfLaneLabelColumn = widthOfLaneLabelColumn;
            WidthOfBarLabelBufferColumn = widthOfBarLabelBufferColumn;

            IsVisible = true;
            VerticalLineViewModels = new ObservableCollection<VerticalLineViewModel>();
            TimeIntervalBarViewModels = new ObservableCollection<TimeIntervalBarViewModel>();
        }

        public void SetTimeInterval(
            DateTime startTime,
            DateTime endTime)
        {
            _startTime = startTime;
            _endTime = endTime;
        }
    }
}
