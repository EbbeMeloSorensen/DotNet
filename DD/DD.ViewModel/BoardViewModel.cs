using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Math;
using Craft.ViewModels.Geometry2D.Scrolling;
using DD.Domain;
using DD.Application;

namespace DD.ViewModel
{
    /// <summary>
    /// Owns the view model collections for the obstacles and creatures on the board
    /// </summary>
    public class BoardViewModel : ImageEditorViewModel
    {
        private static Dictionary<string, double> _weaponImageBaseRotationAngleMap = new Dictionary<string, double>
        {
            { "Images/Arrow.png", 162.5 },
            { "Images/Sword.png", -56 },
        };

        private static double _creatureDiameter;
        private static double _weaponDiameter;

        private int _rows;
        private int _columns;
        private bool _currentCreatureIsHighlighted;
        private double _squareForCurrentCreatureLeft;
        private double _squareForCurrentCreatureTop;
        private double _squareForCurrentCreatureWidth;
        private ObservableCollection<ObstacleViewModel> _obstacleViewModels;
        private ObservableCollection<CreatureViewModel> _creatureViewModels;
        private ObservableCollection<HighlightedSquareViewModel> _highlightedSquareViewModelsForMove;
        private ObservableCollection<HighlightedSquareViewModel> _highlightedSquareViewModelsForMeleeAttack;
        private ObservableCollection<HighlightedSquareViewModel> _highlightedSquareViewModelsForRangedAttack;
        private double _currentCreaturePositionX;
        private double _currentCreaturePositionY;
        private double _translationX;
        private double _translationY;
        private string _creaturePath;
        private string _durationForMoveCreatureAnimation;
        private string _durationForAttackAnimation;
        private bool _moveCreatureAnimationRunning;
        private bool _attackAnimationRunning;
        private RelayCommand _moveCreatureAnimationCompletedCommand;
        private RelayCommand _attackAnimationCompletedCommand;
        private List<Creature> _creatures;
        private Creature _currentCreature;
        private string _weaponImagePath;
        private bool _weaponAutoReverse;

        public CreatureViewModel CurrentCreatureViewModel { get; }
        public WeaponViewModel WeaponViewModel { get; }

        public static double SquareLength { get; set; }

        public bool CurrentCreatureIsHighlighted
        {
            get { return _currentCreatureIsHighlighted; }
            set
            {
                _currentCreatureIsHighlighted = value;
                RaisePropertyChanged();
            }
        }

        public double SquareForCurrentCreatureLeft
        {
            get { return _squareForCurrentCreatureLeft; }
            set
            {
                _squareForCurrentCreatureLeft = value;
                RaisePropertyChanged();
            }
        }

        public double SquareForCurrentCreatureTop
        {
            get { return _squareForCurrentCreatureTop; }
            set
            {
                _squareForCurrentCreatureTop = value;
                RaisePropertyChanged();
            }
        }

        public double SquareForCurrentCreatureWidth
        {
            get { return _squareForCurrentCreatureWidth; }
            set
            {
                _squareForCurrentCreatureWidth = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ObstacleViewModel> ObstacleViewModels
        {
            get
            {
                return _obstacleViewModels;
            }
            set
            {
                _obstacleViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CreatureViewModel> CreatureViewModels
        {
            get
            {
                return _creatureViewModels;
            }
            set
            {
                _creatureViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<HighlightedSquareViewModel> HighligtedSquareViewModelsForMove
        {
            get
            {
                return _highlightedSquareViewModelsForMove;
            }
            set
            {
                _highlightedSquareViewModelsForMove = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<HighlightedSquareViewModel> HighligtedSquareViewModelsForMeleeAttack
        {
            get
            {
                return _highlightedSquareViewModelsForMeleeAttack;
            }
            set
            {
                _highlightedSquareViewModelsForMeleeAttack = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<HighlightedSquareViewModel> HighligtedSquareViewModelsForRangedAttack
        {
            get
            {
                return _highlightedSquareViewModelsForRangedAttack;
            }
            set
            {
                _highlightedSquareViewModelsForRangedAttack = value;
                RaisePropertyChanged();
            }
        }

        public string WeaponImagePath
        {
            get { return _weaponImagePath; }
            set
            {
                _weaponImagePath = value;
                RaisePropertyChanged();
            }
        }

        public bool WeaponAutoReverse
        {
            get { return _weaponAutoReverse; }
            set
            {
                _weaponAutoReverse = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand MoveCreatureAnimationCompletedCommand
        {
            get { return _moveCreatureAnimationCompletedCommand ?? (_moveCreatureAnimationCompletedCommand = 
                new RelayCommand(MoveCreatureAnimationCompletedHandler)); }
        }

        public RelayCommand AttackAnimationCompletedCommand
        {
            get { return _attackAnimationCompletedCommand ?? (_attackAnimationCompletedCommand = 
                new RelayCommand(AttackAnimationCompletedHandler)); }
        }

        public void MoveCurrentCreature(
            Creature currentCreature,
            int[] path)
        {
            TranslationX = (currentCreature.PositionX - _currentCreaturePositionX) * SquareLength;
            TranslationY = (currentCreature.PositionY - _currentCreaturePositionY) * SquareLength;

            var stringBuilder = new StringBuilder("M0,0");

            stringBuilder.Append(path
                .Skip(1)
                .Select(i => new { X = i.ConvertToXCoordinate(_columns), Y = i.ConvertToYCoordinate(_columns) })
                .Select(p => $"L{(p.X - _currentCreaturePositionX) * SquareLength},{(p.Y - _currentCreaturePositionY) * SquareLength}")
                .Aggregate((c, n) => $"{c}{n}"));

            CreaturePath = stringBuilder.ToString();

            var ticksPrStep = 1000000;
            var timeSpan = new TimeSpan(ticksPrStep * path.Length);

            DurationForMoveCreatureAnimation = $"0:0:{timeSpan.Seconds}.{timeSpan.Milliseconds.ToString().PadLeft(3, '0')}";
            _currentCreaturePositionX = currentCreature.PositionX;
            _currentCreaturePositionY = currentCreature.PositionY;

            MoveCreatureAnimationRunning = true;
        }

        public void AnimateAttack(
            Creature currentCreature,
            Creature targetCreature,
            bool ranged)
        {
            if (ranged)
            {
                WeaponImagePath = "Images/Arrow.png";
                WeaponAutoReverse = false;
            }
            else
            {
                WeaponImagePath = "Images/Sword.png";
                WeaponAutoReverse = true;
            }

            WeaponViewModel.Weapon = new Weapon(
                currentCreature.PositionX, 
                currentCreature.PositionY);

            WeaponViewModel.BaseRotationAngle = _weaponImageBaseRotationAngleMap[WeaponImagePath];

            var translationVector = new Vector2D(
            targetCreature.PositionX - currentCreature.PositionX,
            targetCreature.PositionY - currentCreature.PositionY);

            TranslationX = (translationVector.X) * SquareLength;
            TranslationY = (translationVector.Y) * SquareLength;

            var polarVector = translationVector.AsPolarVector();

            WeaponViewModel.RotationAngle = polarVector.Angle * 180 / System.Math.PI;

            WeaponViewModel.IsVisible = true;
            AttackAnimationRunning = true;
        }

        public bool MoveCreatureAnimationRunning
        {
            get { return _moveCreatureAnimationRunning; }
            set
            {
                _moveCreatureAnimationRunning = value;
                RaisePropertyChanged();
            }
        }

        public bool AttackAnimationRunning
        {
            get { return _attackAnimationRunning; }
            set
            {
                _attackAnimationRunning = value;
                RaisePropertyChanged();
            }
        }

        public double TranslationX
        {
            get { return _translationX; }
            set
            {
                _translationX = value;
                RaisePropertyChanged();
            }
        }

        public double TranslationY
        {
            get { return _translationY; }
            set
            {
                _translationY = value;
                RaisePropertyChanged();
            }
        }

        public string CreaturePath
        {
            get { return _creaturePath; }
            set
            {
                _creaturePath = value;
                RaisePropertyChanged();
            }
        }

        public string DurationForMoveCreatureAnimation
        {
            get { return _durationForMoveCreatureAnimation; }
            set
            {
                _durationForMoveCreatureAnimation = value;
                RaisePropertyChanged();
            }
        }

        public string DurationForAttackAnimation
        {
            get { return _durationForAttackAnimation; }
            set
            {
                _durationForAttackAnimation = value;
                RaisePropertyChanged();
            }
        }

        public void PlayerClickedOnBoard()
        {
            // Todo: Determine the index of the square that the user clicked
            var indexX = (int) System.Math.Floor(MouseWorldPosition.X / SquareLength);
            var indexY = (int) System.Math.Floor(MouseWorldPosition.Y / SquareLength);
            var squareIndex = indexY * _columns + indexX;

            OnPlayerClickedSquare(squareIndex);
        }

        public event EventHandler<PlayerClickedSquareEventArgs> PlayerClickedSquare;

        public event EventHandler MoveCreatureAnimationCompleted;

        public event EventHandler AttackAnimationCompleted;

        /// <summary>
        /// Update the board with the remaining creatures and their current condition
        /// </summary>
        /// <param name="creatures"></param>
        /// <param name="currentCreature"></param>
        public void UpdateCreatureViewModels(
            IEnumerable<Creature> creatures,
            Creature currentCreature)
        {
            _currentCreature = currentCreature;
            _creatures = creatures?.ToList();

            if (_currentCreature != null)
            {
                _creatures?.Remove(_currentCreature);

                // Vi gemmer det, fordi vi skal bruge det i animationen
                _currentCreaturePositionX = _currentCreature.PositionX;
                _currentCreaturePositionY = _currentCreature.PositionY;
            }

            UpdateCreatureViewModels();
        }

        private void UpdateCreatureViewModels()
        {
            if (_creatures == null)
            {
                CreatureViewModels?.Clear();
            }
            else
            {
                CreatureViewModels = new ObservableCollection<CreatureViewModel>(
                    _creatures.Select(c => new CreatureViewModel(c, _creatureDiameter)));
            }

            CurrentCreatureViewModel.Creature = _currentCreature;
        }

        public BoardViewModel(
            IEngine engine,
            double squareLength,
            double obstacleDiameter,
            double creatureDiameter,
            double weaponDiameter,
            ObservableObject<Scene> selectedScene) : base(0, 0)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            SquareLength = squareLength;
            _creatureDiameter = creatureDiameter;
            _weaponDiameter = weaponDiameter;

            CurrentCreatureViewModel = new CreatureViewModel(_creatureDiameter);
            WeaponViewModel = new WeaponViewModel(new Weapon(0, 0), _weaponDiameter) { IsVisible = false };

            var timeSpanForAttackAnimation = new TimeSpan(2000000);
            DurationForAttackAnimation = $"0:0:{timeSpanForAttackAnimation.Seconds}.{timeSpanForAttackAnimation.Milliseconds.ToString().PadLeft(3, '0')}";

            selectedScene.PropertyChanged += (s, e) =>
            {
                var scene = (s as ObservableObject<Scene>)?.Object;

                if (scene == null)
                {
                    _rows = 0;
                    _columns = 0;
                    ImageWidth = 0;
                    ImageHeight = 0;
                    ScrollableOffset = new PointD(0, 0);
                    ScrollOffset = new PointD(0, 0);
                }
                else
                {
                    _rows = scene.Rows;
                    _columns = scene.Columns;
                    ImageWidth = _columns * SquareLength;
                    ImageHeight = _rows * SquareLength;
                }

                if (scene == null || scene.Obstacles.Count == 0)
                {
                    ObstacleViewModels = new ObservableCollection<ObstacleViewModel>();
                }
                else
                {
                    ObstacleViewModels = new ObservableCollection<ObstacleViewModel>(
                        scene.Obstacles.Select(o => new ObstacleViewModel(o, obstacleDiameter)));
                }
            };

            engine.SquareIndexForCurrentCreature.PropertyChanged += (s, e) =>
            {
                var squareIndex = (s as ObservableObject<int?>)?.Object;

                if (squareIndex.HasValue)
                {
                    var positionX = squareIndex.Value.ConvertToXCoordinate(_columns);
                    var positionY = squareIndex.Value.ConvertToYCoordinate(_columns);
                    SquareForCurrentCreatureLeft = positionX * SquareLength;
                    SquareForCurrentCreatureTop = positionY * SquareLength;
                    SquareForCurrentCreatureWidth = SquareLength - 3;
                    CurrentCreatureIsHighlighted = true;
                }
                else
                {
                    CurrentCreatureIsHighlighted = false;
                }
            };

            engine.SquareIndexesCurrentCreatureCanMoveTo.PropertyChanged += (s, e) =>
            {
                var squareIndexes = (s as ObservableObject<Dictionary<int, double>>)?.Object;

                if (squareIndexes != null)
                {
                    HighligtedSquareViewModelsForMove = new ObservableCollection<HighlightedSquareViewModel>(
                        squareIndexes.Select(i => new HighlightedSquareViewModel(i.Key % _columns, i.Key / _columns)));
                }
                else
                {
                    HighligtedSquareViewModelsForMove = new ObservableCollection<HighlightedSquareViewModel>();
                }
            };

            engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.PropertyChanged += (s, e) =>
            {
                var squareIndexes = (s as ObservableObject<HashSet<int>>)?.Object;

                if (squareIndexes != null)
                {
                    HighligtedSquareViewModelsForMeleeAttack = new ObservableCollection<HighlightedSquareViewModel>(
                        squareIndexes.Select(i => new HighlightedSquareViewModel(i % _columns, i / _columns)));
                }
                else
                {
                    HighligtedSquareViewModelsForMeleeAttack = new ObservableCollection<HighlightedSquareViewModel>();
                }
            };

            engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.PropertyChanged += (s, e) =>
            {
                var squareIndexes = (s as ObservableObject<HashSet<int>>)?.Object;

                if (squareIndexes != null)
                {
                    HighligtedSquareViewModelsForRangedAttack = new ObservableCollection<HighlightedSquareViewModel>(
                        squareIndexes.Select(i => new HighlightedSquareViewModel(i % _columns, i / _columns)));
                }
                else
                {
                    HighligtedSquareViewModelsForRangedAttack = new ObservableCollection<HighlightedSquareViewModel>();
                }
            };
        }

        private void OnPlayerClickedSquare(int squareIndex)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = PlayerClickedSquare;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new PlayerClickedSquareEventArgs(squareIndex));
            }
        }

        private void MoveCreatureAnimationCompletedHandler()
        {
            MoveCreatureAnimationRunning = false;

            UpdateCreatureViewModels();

            OnAnimationCompleted();
        }

        private void AttackAnimationCompletedHandler()
        {
            AttackAnimationRunning = false;
            WeaponViewModel.IsVisible = false;

            OnAttackAnimationCompleted();
        }

        private void OnAnimationCompleted()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = MoveCreatureAnimationCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnAttackAnimationCompleted()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = AttackAnimationCompleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
