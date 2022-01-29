using DD.Domain;

namespace DD.ViewModel
{
    public class WeaponViewModel : BoardItemViewModel
    {
        private double _baseRotationAngle;
        private double _rotationAngle;

        public double BaseRotationAngle
        {
            get { return _baseRotationAngle; }
            set
            {
                _baseRotationAngle = value;
                RaisePropertyChanged();
            }
        }

        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = _baseRotationAngle + value;
                RaisePropertyChanged();
            }
        }

        public Weapon Weapon
        {
            set
            {
                if (value == null)
                {
                    IsVisible = false;
                }
                else
                {
                    Left = (value.PositionX + 0.5) * BoardViewModel.SquareLength - Diameter / 2;
                    Top = (value.PositionY + 0.5) * BoardViewModel.SquareLength - Diameter / 2;
                }
            }
        }

        public WeaponViewModel(
            Weapon weapon,
            double diameter) : base(weapon.PositionX, weapon.PositionY, diameter)
        {
            ImagePath = "Images/Arrow.png";
        }
    }
}