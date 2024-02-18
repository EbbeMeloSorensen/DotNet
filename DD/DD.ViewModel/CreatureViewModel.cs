using DD.Domain;

namespace DD.ViewModel
{
    public class CreatureViewModel : BoardItemViewModel
    {
        private bool _isHostile;
        private double _hitPointsLeftExtent;
        private bool _isInjured;

        public bool IsHostile
        {
            get { return _isHostile; }
            set
            {
                _isHostile = value;
                RaisePropertyChanged();
            }
        }

        public double HitPointsLeftExtent
        {
            get { return _hitPointsLeftExtent; }
            set
            {
                _hitPointsLeftExtent = value;
                RaisePropertyChanged();
            }
        }

        public bool IsInjured
        {
            get { return _isInjured; }
            set
            {
                _isInjured = value;
                RaisePropertyChanged();
            }
        }

        public Creature Creature
        {
            set
            {
                if (value == null)
                {
                    IsVisible = false;
                }
                else
                {
                    Left = (value.PositionX + 0.5) * BoardViewModel.TileCenterSpacing - Diameter / 2;
                    Top = (value.PositionY + 0.5) * BoardViewModel.TileCenterSpacing - Diameter / 2;

                    IsHostile = value.IsHostile;
                    IsInjured = value.HitPoints < value.CreatureType.MaxHitPoints;
                    HitPointsLeftExtent = value.HitPoints * Diameter / value.CreatureType.MaxHitPoints;
                    ImagePath = GetImagePath(value.CreatureType.Name);
                    IsVisible = true;
                }
            }
        }

        public CreatureViewModel(
            double diameter) : base(0, 0, diameter)
        {
            IsVisible = false;
        }

        public CreatureViewModel(
            Creature creature,
            double diameter) : base(creature.PositionX, creature.PositionY, diameter)
        {
            IsVisible = true;
            IsHostile = creature.IsHostile;
            IsInjured = creature.HitPoints < creature.CreatureType.MaxHitPoints;
            HitPointsLeftExtent = creature.HitPoints * Diameter / creature.CreatureType.MaxHitPoints;
            ImagePath = GetImagePath(creature.CreatureType.Name);
        }
    }
}
