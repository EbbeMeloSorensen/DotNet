using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace DD.ViewModel
{
    public abstract class BoardItemViewModel : ViewModelBase
    {
        private static Dictionary<string, string> _imagePathMap;

        private string _imagePath;
        private double _left;
        private double _top;
        private double _diameter;
        private bool _isVisible;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                RaisePropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
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

        /// <summary>
        /// Maps from creature type name to image path
        /// </summary>
        static BoardItemViewModel()
        {
            _imagePathMap = new Dictionary<string, string>
            {
                { "Boar", "Images/Boar.png" },
                { "Iron Golem", "Images/Iron Golem.png" },
                { "Goblin", "Images/Goblin.png" },
                { "Knight", "Images/Knight.png" },
                { "Archer", "Images/Archer.png" },
                { "Wizard", "Images/Wizard.png" },
                { "Goblin Archer", "Images/Goblin Archer.png" },
                { "Skeleton", "Images/Skeleton.png" },
                { "BlackPanther", "Images/Black Panther.png" },
                { "Behir", "Images/Behir.png" },
                { "Orange", "Images/Orange.png" },
                { "Red Dragon", "Images/Red dragon.png" },
                { "Panda", "Images/Panda.jpg" }
            };
        }

        public BoardItemViewModel(
            int positionX, 
            int positionY,
            double diameter)
        {
            Diameter = diameter;
            Left = (positionX + 0.5) * BoardViewModel.SquareLength - Diameter / 2;
            Top = (positionY + 0.5) * BoardViewModel.SquareLength - Diameter / 2;
        }

        public string GetImagePath(string creatureTypeName)
        {
            return _imagePathMap.ContainsKey(creatureTypeName) 
                ? _imagePathMap[creatureTypeName] 
                : "/Images/NoPreview.png";
        }
    }
}
