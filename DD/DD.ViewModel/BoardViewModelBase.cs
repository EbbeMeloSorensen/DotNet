using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Common;
using Craft.ViewModels.Geometry2D.Scrolling;
using DD.Application;
using DD.Domain;
using Newtonsoft.Json.Linq;

namespace DD.ViewModel;

public abstract class BoardViewModelBase : ImageEditorViewModel
{
    private static Dictionary<string, double> _weaponImageBaseRotationAngleMap = new Dictionary<string, double>
    {
        { "Images/Arrow.png", 162.5 },
        { "Images/Sword.png", -56 },
    };

    protected static double _creatureDiameter;
    protected static double _weaponDiameter;

    private int _rows;
    private int _columns;
    private double _squareForCurrentCreatureLeft;
    private double _squareForCurrentCreatureTop;
    private double _squareForCurrentCreatureWidth;
    private ObservableCollection<ObstacleViewModel> _obstacleViewModels;
    private ObservableCollection<CreatureViewModel> _creatureViewModels;
    private int _currentCreaturePositionX;
    private int _currentCreaturePositionY;
    private double _translationX;
    private double _translationY;
    private string _creaturePath;
    private string _durationForMoveCreatureAnimation;
    private string _durationForAttackAnimation;
    private bool _moveCreatureAnimationRunning;
    private bool _attackAnimationRunning;
    private List<Creature> _creatures;
    private Creature _currentCreature;
    private string _weaponImagePath;
    private bool _weaponAutoReverse;
    private double _boardWidth;
    private double _boardHeight;
    public CreatureViewModel CurrentCreatureViewModel { get; }
    public WeaponViewModel WeaponViewModel { get; }

    // Used for dimensioning the collection of pixels. It may differ from ImageWidth
    public double BoardWidth
    {
        get { return _boardWidth; }
        set
        {
            _boardWidth = value;
            RaisePropertyChanged();
        }
    }

    // Used for dimensioning the collection of pixels. It may differ from ImageHeight
    public double BoardHeight
    {
        get { return _boardHeight; }
        set
        {
            _boardHeight = value;
            RaisePropertyChanged();
        }
    }

    public static double TileCenterSpacing { get; set; }

    // Used by the view to draw the grid
    public int Rows
    {
        get => _rows;
        set
        {
            _rows = value;
            RaisePropertyChanged();
        }
    }

    // Used by the view to draw the grid
    public int Columns
    {
        get => _columns;
        set
        {
            _columns = value;
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

    // Used by the host (ActOutSceneViewModel) for triggering an animation
    public bool MoveCreatureAnimationRunning
    {
        get { return _moveCreatureAnimationRunning; }
        set
        {
            _moveCreatureAnimationRunning = value;
            RaisePropertyChanged();
        }
    }

    // Used by the host (ActOutSceneViewModel) for triggering an animation
    public bool AttackAnimationRunning
    {
        get { return _attackAnimationRunning; }
        set
        {
            _attackAnimationRunning = value;
            RaisePropertyChanged();
        }
    }

    // Used by a storyboard animation of the view
    public double TranslationX
    {
        get { return _translationX; }
        set
        {
            _translationX = value;
            RaisePropertyChanged();
        }
    }

    // Used by a storyboard animation of the view
    public double TranslationY
    {
        get { return _translationY; }
        set
        {
            _translationY = value;
            RaisePropertyChanged();
        }
    }

    // Used by a storyboard animation of the view
    public string CreaturePath
    {
        get { return _creaturePath; }
        set
        {
            _creaturePath = value;
            RaisePropertyChanged();
        }
    }

    // Used by a storyboard animation of the view
    public string DurationForMoveCreatureAnimation
    {
        get { return _durationForMoveCreatureAnimation; }
        set
        {
            _durationForMoveCreatureAnimation = value;
            RaisePropertyChanged();
        }
    }

    // Used by a storyboard animation of the view
    public string DurationForAttackAnimation
    {
        get { return _durationForAttackAnimation; }
        set
        {
            _durationForAttackAnimation = value;
            RaisePropertyChanged();
        }
    }

    // Constructor
    public BoardViewModelBase(
        IEngine engine,
        double tileCenterSpacing,
        double obstacleDiameter,
        double creatureDiameter,
        double weaponDiameter,
        ObservableObject<Scene> selectedScene) : base(0, 0)
    {
        ScrollableOffset = new PointD(0, 0);
        ScrollOffset = new PointD(0, 0);

        TileCenterSpacing = tileCenterSpacing;
        _creatureDiameter = creatureDiameter;
        _weaponDiameter = weaponDiameter;

        CurrentCreatureViewModel = new CreatureViewModel(_creatureDiameter);
        WeaponViewModel = new WeaponViewModel(new Weapon(0, 0), 0, 0, _weaponDiameter)
        {
            IsVisible = false
        };

        //var timeSpanForAttackAnimation = new TimeSpan(5000000);
        var timeSpanForAttackAnimation = new TimeSpan(2000000);
        //var timeSpanForAttackAnimation = new TimeSpan(20000);
        DurationForAttackAnimation = $"0:0:{timeSpanForAttackAnimation.Seconds}.{timeSpanForAttackAnimation.Milliseconds.ToString().PadLeft(3, '0')}";

        selectedScene.PropertyChanged += (s, e) =>
        {
            var scene = (s as ObservableObject<Scene>)?.Object;

            LayoutBoard(scene);

            if (scene == null || scene.Obstacles.Count == 0)
            {
                ObstacleViewModels = new ObservableCollection<ObstacleViewModel>();
            }
            else
            {
                ObstacleViewModels = new ObservableCollection<ObstacleViewModel>(
                    scene.Obstacles.Select(o =>
                    {
                        var left = (o.PositionX + 0.5) * TileCenterSpacing - obstacleDiameter / 2;
                        var top = (o.PositionY + 0.5) * TileCenterSpacing - obstacleDiameter / 2;
                        return new ObstacleViewModel(o, left, top, obstacleDiameter);
                    }));
            }
        };

        engine.SquareIndexForCurrentCreature.PropertyChanged += (s, e) =>
        {
            var squareIndex = (s as ObservableObject<int?>)?.Object;

            if (squareIndex.HasValue)
            {
                var positionX = squareIndex.Value.ConvertToXCoordinate(Columns);
                var positionY = squareIndex.Value.ConvertToYCoordinate(Columns);
                SquareForCurrentCreatureLeft = positionX * TileCenterSpacing;
                SquareForCurrentCreatureTop = positionY * TileCenterSpacing;
                SquareForCurrentCreatureWidth = TileCenterSpacing - 3;
            }
        };
    }

    public abstract void HighlightPlayerOptions(
        int squareIndexOfCurrentCreature,
        HashSet<int> squareIndexesCurrentCreatureCanMoveTo,
        HashSet<int> squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
        HashSet<int> squareIndexesCurrentCreatureCanAttackWithRangedWeapon);

    public abstract void ClearPlayerOptions();

    public void MoveCurrentCreature(
        Creature currentCreature,
        int[] path)
    {
        DetermineCanvasPosition(
            currentCreature.PositionX,
            currentCreature.PositionY,
            out var x2,
            out var y2);

        DetermineCanvasPosition(
            _currentCreaturePositionX,
            _currentCreaturePositionY,
            out var x1,
            out var y1);

        TranslationX = x2 - x1;
        TranslationY = y2 - y1;

        var stringBuilder = new StringBuilder("M0,0");

        stringBuilder.Append(path
            .Skip(1)
            .Select(i =>
            {
                DetermineCanvasPosition(
                    i.ConvertToXCoordinate(Columns),
                    i.ConvertToYCoordinate(Columns),
                    out var x,
                    out var y);

                return new
                {
                    X = x,
                    Y = y
                };
            })
            .Select(p => $"L{p.X - x1},{p.Y - y1}")
            .Aggregate((c, n) => $"{c}{n}"));

        CreaturePath = stringBuilder.ToString();

        //var ticksPrStep = 5000000;
        var ticksPrStep = 1000000;
        //var ticksPrStep = 10000;
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

        DetermineCanvasPosition(
            currentCreature.PositionX,
            currentCreature.PositionY,
            out var x1,
            out var y1);

        WeaponViewModel.Initialize(
            x1 - _weaponDiameter / 2,
            y1 - _weaponDiameter / 2);

        DetermineCanvasPosition(
            targetCreature.PositionX,
            targetCreature.PositionY,
            out var x2,
            out var y2);

        var translationVector = new Vector2D(
            x2 - x1,
            y2 - y1).Normalize();

        TranslationX = (translationVector.X) * TileCenterSpacing;
        TranslationY = (translationVector.Y) * TileCenterSpacing;

        var polarVector = translationVector.AsPolarVector();

        WeaponViewModel.BaseRotationAngle = _weaponImageBaseRotationAngleMap[WeaponImagePath];
        WeaponViewModel.RotationAngle = polarVector.Angle * 180 / Math.PI;
        WeaponViewModel.IsVisible = true;
        AttackAnimationRunning = true;
    }

    public void PlayerClickedOnBoard()
    {
        // Todo: Determine the index of the square that the user clicked
        var indexX = (int)System.Math.Floor(MouseWorldPosition.X / TileCenterSpacing);
        var indexY = (int)System.Math.Floor(MouseWorldPosition.Y / TileCenterSpacing);
        var squareIndex = indexY * Columns + indexX;

        OnPlayerClickedSquare(squareIndex);
    }

    public event EventHandler<PlayerClickedSquareEventArgs> PlayerClickedSquare;

    // Used to inform the host (ActOutSceneViewModel) that an animation is completed
    public event EventHandler MoveCreatureAnimationCompleted;

    // Used to inform the host (ActOutSceneViewModel) that an animation is completed
    public event EventHandler AttackAnimationCompleted;

    // Called by the  Update the board with the remaining creatures and their current condition
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

    // Called by the view, when a storyboard animation is complete
    public void CompleteMoveCreatureAnimation()
    {
        MoveCreatureAnimationRunning = false;

        UpdateCreatureViewModels();
        OnMoveAnimationCompleted();
    }

    // Called by the view, when a storyboard animation is complete
    public void CompleteAttackAnimation()
    {
        AttackAnimationRunning = false;
        WeaponViewModel.IsVisible = false;

        OnAttackAnimationCompleted();
    }

    public abstract void LayoutBoard(
        Scene scene);

    protected PixelViewModel GeneratePixel(
        int index,
        int squareIndexOfCurrentCreature,
        IReadOnlySet<int> squareIndexesCurrentCreatureCanMoveTo,
        IReadOnlySet<int> squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
        IReadOnlySet<int> squareIndexesCurrentCreatureCanAttackWithRangedWeapon)
    {
        if (index == squareIndexOfCurrentCreature)
        {
            return new PixelViewModel(index, new Pixel(200, 200, 100, 0));
        }

        if (squareIndexesCurrentCreatureCanMoveTo.Contains(index))
        {
            return new PixelViewModel(index, new Pixel(220, 220, 220, 0));
        }

        if (squareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Contains(index))
        {
            return new PixelViewModel(index, new Pixel(200, 100, 100, 0));
        }

        if (squareIndexesCurrentCreatureCanAttackWithRangedWeapon.Contains(index))
        {
            return new PixelViewModel(index, new Pixel(200, 100, 150, 0));
        }

        return new PixelViewModel(index, new Pixel(200, 200, 200, 0));
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
                _creatures.Select(c =>
                {
                    DetermineCanvasPosition(
                        c.PositionX, 
                        c.PositionY,
                        out var x, 
                        out var y);

                    return new CreatureViewModel(c, x - _creatureDiameter / 2, y - _creatureDiameter / 2, _creatureDiameter);
                }));
        }

        if (_currentCreature == null)
        {
            CurrentCreatureViewModel.IsVisible = false;
        }
        else
        {
            DetermineCanvasPosition(
                _currentCreature.PositionX,
                _currentCreature.PositionY,
                out var x,
                out var y);

            CurrentCreatureViewModel.Initialize(
                _currentCreature,
                x - _creatureDiameter / 2,
                y - _creatureDiameter / 2,
                _creatureDiameter);

            CurrentCreatureViewModel.IsVisible = true;
        }
    }

    public abstract void DetermineCanvasPosition(
        int positionX,
        int positionY,
        out double x,
        out double y);

    // Used to inform the host (ActOutSceneViewModel) that the user clicked the board
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

    // Used to inform the host (ActOutSceneViewModel) that an animation is completed
    private void OnMoveAnimationCompleted()
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

    // Used to inform the host (ActOutSceneViewModel) that an animation is completed
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