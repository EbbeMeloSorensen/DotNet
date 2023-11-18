using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Utils.Linq;
using Craft.Logging;
using Craft.Math;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Simulator.Application;
using Simulator.Domain;
using Simulator.Domain.Props;
using Simulator.ViewModel;
using Simulator.ViewModel.ShapeViewModels;
using Game.TowerDefense.ViewModel.Bodies;
using Application = Simulator.Application.Application;
using ApplicationState = Craft.DataStructures.Graph.State;
using BodyStateCannon = Game.TowerDefense.ViewModel.BodyStates.BodyStateCannon;
using BodyStateEnemy = Game.TowerDefense.ViewModel.BodyStates.BodyStateEnemy;
using BodyStateProjectile = Game.TowerDefense.ViewModel.BodyStates.BodyStateProjectile;

namespace Game.TowerDefense.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static int _nextWallId = 100000;
        private static bool _boundariesVisible = false; // Set to true to inspect boundaries

        private ILogger _logger;
        private SceneViewManager _sceneViewManager;
        private bool _geometryEditorVisible = true;
        private Vector2D _worldWindowTranslation;
        private Stopwatch _stopwatch;

        private Dictionary<ApplicationState, List<Tuple<ApplicationState, ApplicationState>>> _transitionActivationMap;

        public Application Application { get; }

        public UnlockedLevelsViewModel UnlockedLevelsViewModel { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        private RelayCommand _startOrResumeAnimationCommand;

        public RelayCommand StartOrResumeAnimationCommand =>
            _startOrResumeAnimationCommand ?? (_startOrResumeAnimationCommand =
                new RelayCommand(StartOrResumeAnimation, CanStartOrResumeAnimation));

        public bool GeometryEditorVisible
        {
            get { return _geometryEditorVisible; }
            set
            {
                _geometryEditorVisible = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            ILogger logger)
        {
            _logger = logger;
            _logger = null; // Disable logging (it should only be used for debugging purposes)
            _stopwatch = new Stopwatch();

            UnlockedLevelsViewModel = new UnlockedLevelsViewModel();

            UnlockedLevelsViewModel.LevelSelected += (s, e) =>
            {
                Application.SwitchState(e.Level.Name);
            };

            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Block;
            };

            ShapeSelectorCallback shapeSelectorCallback = (bs) =>
            {
                if (!(bs.Body is CircularBody))
                {
                    throw new InvalidOperationException();
                }

                var circularBody = bs.Body as CircularBody;

                switch (bs)
                {
                    case BodyStateEnemy enemy:
                        {
                            return new TaggedEllipseViewModel
                            {
                                Width = 2 * circularBody.Radius,
                                Height = 2 * circularBody.Radius,
                                Tag = enemy.Life.ToString()
                            };
                        }
                    case BodyStateCannon cannon:
                    {
                        return new RotatableEllipseViewModel
                        {
                            Width = 2 * circularBody.Radius,
                            Height = 2 * circularBody.Radius,
                            Orientation = 0
                        };
                    }
                    case BodyStateProjectile:
                        {
                            return new EllipseViewModel
                            {
                                Width = 2 * circularBody.Radius,
                                Height = 2 * circularBody.Radius
                            };
                        }
                    default:
                        throw new ArgumentException();
                }
            };

            ShapeUpdateCallback shapeUpdateCallback = (shapeViewModel, bs) =>
            {
                shapeViewModel.Point = new PointD(bs.Position.X, bs.Position.Y);

                if (shapeViewModel is TaggedEllipseViewModel taggedEllipseViewModel &&
                    bs is BodyStateEnemy enemy)
                {
                    // Opdater position og life indikator for enemy
                    taggedEllipseViewModel.Tag = enemy.Life.ToString();
                }

                if (shapeViewModel is RotatableEllipseViewModel rotatableEllipseViewModel &&
                    bs is BodyStateCannon cannon)
                {
                    rotatableEllipseViewModel.Orientation = cannon.Orientation;
                }
            };

            var welcomeScreen = new ApplicationState("Welcome Screen");
            var unlockedLevelsScreen = new ApplicationState("Unlocked Levels Screen");

            var level1 = new Level("Level 1")
            {
                Scene = GenerateScene1(
                    collisionBetweenBodyAndBoundaryOccuredCallBack)
            };

            var level1Cleared = new ApplicationState("Level 1 Cleared");

            var level2 = new Level("Level 2")
            {
                Scene = GenerateScene2(
                    collisionBetweenBodyAndBoundaryOccuredCallBack)
            };

            var gameOver = new ApplicationState("Game Over");
            var youWin = new ApplicationState("You Win");

            Application = new Application(_logger, welcomeScreen);

            Application.AddApplicationState(unlockedLevelsScreen);
            Application.AddApplicationState(level1);
            Application.AddApplicationState(level1Cleared);
            Application.AddApplicationState(level2);
            Application.AddApplicationState(gameOver);
            Application.AddApplicationState(youWin);

            Application.AddApplicationStateTransition(welcomeScreen, level1);
            Application.AddApplicationStateTransition(level1, gameOver);
            Application.AddApplicationStateTransition(level1, level1Cleared);
            Application.AddApplicationStateTransition(level1Cleared, level2);
            Application.AddApplicationStateTransition(level2, gameOver);
            Application.AddApplicationStateTransition(level2, youWin);
            Application.AddApplicationStateTransition(gameOver, welcomeScreen);
            Application.AddApplicationStateTransition(youWin, welcomeScreen);

            _transitionActivationMap = new Dictionary<ApplicationState, List<Tuple<ApplicationState, ApplicationState>>>
            {
                {level1Cleared, new List<Tuple<ApplicationState, ApplicationState>>
                {
                    new (welcomeScreen, unlockedLevelsScreen),
                    new (unlockedLevelsScreen, level1),
                    new (unlockedLevelsScreen, level2)
                }}
            };

            // If the user hits the space key while the application is in a state that is not a level then switch application state
            Application.KeyEventOccured += (s, e) =>
            {
                if (e.KeyboardKey != KeyboardKey.Space ||
                    e.KeyEventType != KeyEventType.KeyPressed ||
                    Application.State.Object is Level ||
                    Application.State.Object == unlockedLevelsScreen)
                {
                    return;
                }

                if (Application.State.Object == welcomeScreen &&
                    Application.ExitsFromCurrentApplicationState().Contains("Unlocked Levels Screen"))
                {
                    Application.SwitchState("Unlocked Levels Screen");
                }
                else
                {
                    Application.SwitchState();
                }
            };

            Application.State.PropertyChanged += (s, e) =>
            {
                // Applikationen har skiftet tilstand (det, der vedligeholdes af state maskinen)
                if (Application.State.Object is Level level)
                {
                    if (Application.PreviousState is Level)
                    {
                        // Nu vil vi så gerne "slide" fra, hvor World Window pt er, og hen til det fokus, der gør sig gældende for næste levels scene
                        var previousWorldWindowFocus = GeometryEditorViewModel.WorldWindowFocus;
                        var nextWorldWindowFocus = level.Scene.InitialWorldWindowFocus();

                        _worldWindowTranslation = new Vector2D(
                            nextWorldWindowFocus.X - previousWorldWindowFocus.X,
                            nextWorldWindowFocus.Y - previousWorldWindowFocus.Y);

                        _stopwatch.Start();

                        // (I starten så lever vi med at den bare nuker nuværende scene,
                        // men senere vil vi gerne vente til den faktisk er landet på næste level
                    }
                    else
                    {
                        GeometryEditorViewModel.InitializeWorldWindow(
                            level.Scene.InitialWorldWindowFocus(),
                            level.Scene.InitialWorldWindowSize(),
                            false);
                    }

                    // Dette kald udvirker, at WorldWindow bliver sat
                    _sceneViewManager.ActiveScene = level.Scene;
                }
                else
                {
                    if (Application.State.Object == welcomeScreen)
                    {
                        _sceneViewManager.ActiveScene = null;
                    }
                }
            };

            Application.AnimationCompleted += (s, e) =>
            {
                Application.SwitchState(Application.Engine.Outcome);

                UnlockLevels(Application.State.Object);
            };

            // Aktiver nogle, så du ikke hele tiden skal gennemføre level 1
            //UnlockLevels(level1Cleared);
            //UnlockLevels(level2Cleared);

            GeometryEditorViewModel = new GeometryEditorViewModel()
            {
                UpdateModelCallBack = UpdateModel
            };

            _sceneViewManager = new SceneViewManager(
                Application,
                GeometryEditorViewModel,
                shapeSelectorCallback,  
                shapeUpdateCallback);
        }

        private void UpdateModel()
        {
            if (Application.AnimationRunning)
            {
                Application.UpdateModel();
            }
            else
            {
                if (_sceneViewManager.ActiveScene != null)
                {
                    if (_stopwatch.IsRunning)
                    {
                        var slideDuration = 0.5;
                        var fraction = Math.Max(0.0, 1.0 - _stopwatch.Elapsed.TotalSeconds / slideDuration);

                        var wwFocus = new Point(
                            _sceneViewManager.ActiveScene.InitialWorldWindowFocus().X - _worldWindowTranslation.X * fraction,
                            _sceneViewManager.ActiveScene.InitialWorldWindowFocus().Y - _worldWindowTranslation.Y * fraction);

                        GeometryEditorViewModel.InitializeWorldWindow(
                            wwFocus,
                            _sceneViewManager.ActiveScene.InitialWorldWindowSize(),
                            false);

                        if (fraction > 0.0) return;

                        _stopwatch.Stop();
                        _stopwatch.Reset();
                    }
                    else
                    {
                        StartOrResumeAnimationCommand.Execute(null);
                    }
                }
            }
        }

        private void StartOrResumeAnimation()
        {
            Application.StartOrResumeAnimation();
        }

        private bool CanStartOrResumeAnimation()
        {
            return Application.CanStartOrResumeAnimation;
        }

        private static Scene GenerateScene1(
            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack)
        {
            const double initialBalance = 300.0;
            const double initialHealth = 100.0;
            const double radiusOfCannons = 0.2;
            const double radiusOfProjectiles = 0.05;
            const double priceOfCannon = 50.0;
            const double priceForKilledEnemy = 20.0;
            const int cannonCoolDown = 300;
            const double rangeOfCannons = 1.0;
            const double projectileSpeed = 10.0;
            const int projectileLifespan = 50;
            const double enemyRadius = 0.15;
            const int enemySpacing = 200;
            const int enemyLife = 10;
            const double enemySpeed = 0.5;

            var path = new Path
            {
                WayPoints = new List<Vector2D>
                {
                    new Vector2D(-2, -1),
                    new Vector2D(1.5, -1),
                    new Vector2D(2, 0),
                    new Vector2D(-1, 0),
                    new Vector2D(-1.5, 1),
                    new Vector2D(5, 1)
                }
            };

            var nextCannonId = 1000;
            var nextProjectileId = 10000;
            var nextPropId = 100000;

            var initialState = new State();

            var standardGravity = 0.0;
            var gravitationalConstant = 0.0;
            var handleBodyCollisions = true;
            var coefficientOfFriction = 0.0;
            var balance = initialBalance;
            var health = initialHealth;

            var x0 = 0.0;
            var x1 = 16.0;
            var y0 = 0.0;
            var y1 = 8.0;

            var scene = new Scene("Scene 1",
                new Point2D(x0, y0),
                new Point2D(x1, y1),
                initialState, 0, 0, 0, 1, false)
            {
                CollisionBetweenBodyAndBoundaryOccuredCallBack = collisionBetweenBodyAndBoundaryOccuredCallBack
            };

            scene.InitializationCallback = (state, message) =>
            {
                balance = initialBalance;
                health = initialHealth;
                nextCannonId = 1000;
                nextProjectileId = 10000;
                nextPropId = 100000;
            };

            scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }
                }

                return false;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return OutcomeOfCollisionBetweenTwoBodies.Ignore;
                    }
                }

                return OutcomeOfCollisionBetweenTwoBodies.Block;
            };

            Point2D mousePos = null;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                //updateAuxFields($"Life: {health}", $"$: {balance}");

                if (mouseClickPosition == null)
                {
                    mousePos = null;
                    return false;
                }

                mousePos = mouseClickPosition.Position;
                return true;
            };

            var enemies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * enemySpacing,
                    BodyState = new BodyStateEnemy(new Enemy(i, enemyRadius, 1, true), new Vector2D(-3, -1))
                    {
                        Path = path,
                        Speed = enemySpeed,
                        NaturalVelocity = new Vector2D(0.2, 0),
                        Life = enemyLife
                    }
                })
                .ToDictionary(_ => _.StateIndex, _ => _.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                // Remove projectiles due to hitting enemies
                if (boundaryCollisionReports.Any())
                {
                    // Remove enemies, when they get to the exit
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.BodyState.Body is Enemy)
                        .Select(bcr => bcr.BodyState.Body.Id));

                    health -= boundaryCollisionReports.Count * 30.0;

                    if (health <= 0)
                    {
                        response.IndexOfLastState = propagatedState.Index;
                        response.Outcome = "Game Over";
                    }
                }

                var hitEnemies = new HashSet<BodyStateEnemy>();

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with enemy
                        if (bcr.Body1 is Projectile)
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body1.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body2.Id) as BodyStateEnemy;
                            hitEnemies.Add(bodyState);
                        }
                        else
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body2.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body1.Id) as BodyStateEnemy;
                            hitEnemies.Add(bodyState);
                        }
                    }
                });

                hitEnemies.ToList().ForEach(e =>
                {
                    e.Life -= 1;

                    if (e.Life <= 0.1)
                    {
                        balance += priceForKilledEnemy;
                        propagatedState.RemoveBodyStates(new List<int> { e.Body.Id });

                        if (!propagatedState.BodyStates.Any(bs => bs.Body is Enemy))
                        {
                            // All enemies are dead, so player wins
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "You Win";
                        }
                    }
                });

                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports.Select(bcr => bcr.BodyState.Body.Id));
                }

                // Add a new cannon?
                if (mousePos != null)
                {
                    var mousePosAsVector = mousePos.AsVector2D();

                    var cannonCenters = propagatedState.BodyStates
                        .Where(_ => _.Body is Cannon)
                        .Select(_ => _.Position);

                    if (balance >= priceOfCannon &&
                        cannonCenters.DistanceToClosestPoint(mousePosAsVector) > 2 * radiusOfCannons &&
                        scene.Props.Min(_ => _.DistanceToPoint(mousePosAsVector) - radiusOfCannons > 0.0))
                    {
                        propagatedState.AddBodyState(new BodyStateCannon(
                            new Cannon(nextCannonId++, radiusOfCannons), mousePosAsVector)
                        {
                            CoolDown = cannonCoolDown
                        });

                        balance -= priceOfCannon;
                    }

                    mousePos = null;
                }

                // Remove projectiles due to limited lifespan?
                var disposableProjectiles = propagatedState.BodyStates
                    .Where(_ =>
                        _.Body is Projectile &&
                        (_ as BodyStateProjectile).LifeSpan <= 0)
                    .Select(_ => _.Body.Id)
                    .ToList();

                propagatedState.RemoveBodyStates(disposableProjectiles);

                // Add projectiles?
                var bodyStatesOfCannonsThatMayShoot = propagatedState.BodyStates
                    .Where(_ =>
                        _.Body is Cannon &&
                        (_ as BodyStateCannon).CoolDown <= 0)
                    .ToList();

                bodyStatesOfCannonsThatMayShoot.ForEach(bodyState =>
                {
                    // This cannon can shoot

                    var rangeOfCannonsSquared = rangeOfCannons * rangeOfCannons;

                    var target = propagatedState.BodyStates
                        .Where(_ => _ is BodyStateEnemy)
                        .Select(_ => _ as BodyStateEnemy)
                        .Where(_ => _.Position.SquaredDistanceTo(bodyState.Position) < rangeOfCannonsSquared)
                        .Select(_ => new { BodyState = _, _.DistanceCovered })
                        .OrderByDescending(_ => _.DistanceCovered)
                        .FirstOrDefault();

                    if (target == null)
                    {
                        return;
                    }

                    var projectileVelocity = (target.BodyState.Position - bodyState.Position).Normalize() * projectileSpeed;

                    (bodyState as BodyStateCannon).Orientation = -projectileVelocity.AsPolarVector().Angle;

                    propagatedState.AddBodyState(new BodyStateProjectile(
                        new Projectile(
                            nextProjectileId++,
                            radiusOfProjectiles,
                            1,
                            false),
                        bodyState.Position)
                    {
                        NaturalVelocity = projectileVelocity,
                        LifeSpan = projectileLifespan
                    });

                    (bodyState as BodyStateCannon).CoolDown = cannonCoolDown;
                });

                // Add an enemy?
                if (enemies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(enemies[propagatedState.Index]);
                }

                return response;
            };

            AddPath(scene, path, enemyRadius * 2.5, nextPropId);

            return scene;
        }

        private static Scene GenerateScene2(
            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack)
        {
            const double initialBalance = 300.0;
            const double initialHealth = 100.0;
            const double radiusOfCannons = 0.2;
            const double radiusOfProjectiles = 0.05;
            const double priceOfCannon = 50.0;
            const double priceForKilledEnemy = 20.0;
            const int cannonCoolDown = 300;
            const double rangeOfCannons = 1.0;
            const double projectileSpeed = 10.0;
            const int projectileLifespan = 50;
            const double enemyRadius = 0.15;
            const int enemySpacing = 200;
            const int enemyLife = 10;
            const double enemySpeed = 0.5;

            var path = new Path
            {
                WayPoints = new List<Vector2D>
                {
                    new Vector2D(-2, -1),
                    new Vector2D(1.5, -1),
                    new Vector2D(2, 0),
                    new Vector2D(-1, 0),
                    new Vector2D(-1.5, 1),
                    new Vector2D(5, 1)
                }
            };

            var nextCannonId = 1000;
            var nextProjectileId = 10000;
            var nextPropId = 100000;

            var initialState = new State();

            var standardGravity = 0.0;
            var gravitationalConstant = 0.0;
            var handleBodyCollisions = true;
            var coefficientOfFriction = 0.0;
            var balance = initialBalance;
            var health = initialHealth;

            var x0 = 0.0;
            var x1 = 16.0;
            var y0 = 0.0;
            var y1 = 8.0;

            var scene = new Scene("Scene 2",
                new Point2D(x0, y0),
                new Point2D(x1, y1),
                initialState, 0, 0, 0, 1, false)
            {
                CollisionBetweenBodyAndBoundaryOccuredCallBack = collisionBetweenBodyAndBoundaryOccuredCallBack
            };

            scene.InitializationCallback = (state, message) =>
            {
                balance = initialBalance;
                health = initialHealth;
                nextCannonId = 1000;
                nextProjectileId = 10000;
                nextPropId = 100000;
            };

            scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }
                }

                return false;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return OutcomeOfCollisionBetweenTwoBodies.Ignore;
                    }
                }

                return OutcomeOfCollisionBetweenTwoBodies.Block;
            };

            Point2D mousePos = null;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                //updateAuxFields($"Life: {health}", $"$: {balance}");

                if (mouseClickPosition == null)
                {
                    mousePos = null;
                    return false;
                }

                mousePos = mouseClickPosition.Position;
                return true;
            };

            var enemies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * enemySpacing,
                    BodyState = new BodyStateEnemy(new Enemy(i, enemyRadius, 1, true), new Vector2D(-3, -1))
                    {
                        Path = path,
                        Speed = enemySpeed,
                        NaturalVelocity = new Vector2D(0.2, 0),
                        Life = enemyLife
                    }
                })
                .ToDictionary(_ => _.StateIndex, _ => _.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                // Remove projectiles due to hitting enemies
                if (boundaryCollisionReports.Any())
                {
                    // Remove enemies, when they get to the exit
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.BodyState.Body is Enemy)
                        .Select(bcr => bcr.BodyState.Body.Id));

                    health -= boundaryCollisionReports.Count * 30.0;

                    if (health <= 0)
                    {
                        response.IndexOfLastState = propagatedState.Index;
                        response.Outcome = "Game Over";
                    }
                }

                var hitEnemies = new HashSet<BodyStateEnemy>();

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with enemy
                        if (bcr.Body1 is Projectile)
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body1.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body2.Id) as BodyStateEnemy;
                            hitEnemies.Add(bodyState);
                        }
                        else
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body2.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body1.Id) as BodyStateEnemy;
                            hitEnemies.Add(bodyState);
                        }
                    }
                });

                hitEnemies.ToList().ForEach(e =>
                {
                    e.Life -= 1;

                    if (e.Life <= 0.1)
                    {
                        balance += priceForKilledEnemy;
                        propagatedState.RemoveBodyStates(new List<int> { e.Body.Id });

                        if (!propagatedState.BodyStates.Any(bs => bs.Body is Enemy))
                        {
                            // All enemies are dead, so player wins
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "You Win";
                        }
                    }
                });

                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports.Select(bcr => bcr.BodyState.Body.Id));
                }

                // Add a new cannon?
                if (mousePos != null)
                {
                    var mousePosAsVector = mousePos.AsVector2D();

                    var cannonCenters = propagatedState.BodyStates
                        .Where(_ => _.Body is Cannon)
                        .Select(_ => _.Position);

                    if (balance >= priceOfCannon &&
                        cannonCenters.DistanceToClosestPoint(mousePosAsVector) > 2 * radiusOfCannons &&
                        scene.Props.Min(_ => _.DistanceToPoint(mousePosAsVector) - radiusOfCannons > 0.0))
                    {
                        propagatedState.AddBodyState(new BodyStateCannon(
                            new Cannon(nextCannonId++, radiusOfCannons), mousePosAsVector)
                        {
                            CoolDown = cannonCoolDown
                        });

                        balance -= priceOfCannon;
                    }

                    mousePos = null;
                }

                // Remove projectiles due to limited lifespan?
                var disposableProjectiles = propagatedState.BodyStates
                    .Where(_ =>
                        _.Body is Projectile &&
                        (_ as BodyStateProjectile).LifeSpan <= 0)
                    .Select(_ => _.Body.Id)
                    .ToList();

                propagatedState.RemoveBodyStates(disposableProjectiles);

                // Add projectiles?
                var bodyStatesOfCannonsThatMayShoot = propagatedState.BodyStates
                    .Where(_ =>
                        _.Body is Cannon &&
                        (_ as BodyStateCannon).CoolDown <= 0)
                    .ToList();

                bodyStatesOfCannonsThatMayShoot.ForEach(bodyState =>
                {
                    // This cannon can shoot

                    var rangeOfCannonsSquared = rangeOfCannons * rangeOfCannons;

                    var target = propagatedState.BodyStates
                        .Where(_ => _ is BodyStateEnemy)
                        .Select(_ => _ as BodyStateEnemy)
                        .Where(_ => _.Position.SquaredDistanceTo(bodyState.Position) < rangeOfCannonsSquared)
                        .Select(_ => new { BodyState = _, _.DistanceCovered })
                        .OrderByDescending(_ => _.DistanceCovered)
                        .FirstOrDefault();

                    if (target == null)
                    {
                        return;
                    }

                    var projectileVelocity = (target.BodyState.Position - bodyState.Position).Normalize() * projectileSpeed;

                    (bodyState as BodyStateCannon).Orientation = -projectileVelocity.AsPolarVector().Angle;

                    propagatedState.AddBodyState(new BodyStateProjectile(
                        new Projectile(
                            nextProjectileId++,
                            radiusOfProjectiles,
                            1,
                            false),
                        bodyState.Position)
                    {
                        NaturalVelocity = projectileVelocity,
                        LifeSpan = projectileLifespan
                    });

                    (bodyState as BodyStateCannon).CoolDown = cannonCoolDown;
                });

                // Add an enemy?
                if (enemies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(enemies[propagatedState.Index]);
                }

                return response;
            };

            AddPath(scene, path, enemyRadius * 2.5, nextPropId);

            return scene;
        }

        private void UnlockLevels(
            ApplicationState applicationState)
        {
            if (!_transitionActivationMap.ContainsKey(applicationState)) return;

            _transitionActivationMap[applicationState].ForEach(_ =>
            {
                if (_.Item1.Name == "Unlocked Levels Screen")
                {
                    UnlockedLevelsViewModel.AddLevel(_.Item2 as Level);
                }

                Application.AddApplicationStateTransition(_.Item1, _.Item2);
            });

            _transitionActivationMap.Remove(applicationState);
        }

        // Scene building helpers
        private static void AddPath(
            Scene scene,
            Path path,
            double width,
            int firstPropId)
        {
            var propId = firstPropId;

            path.WayPoints.AdjacenPairs().ToList().ForEach(_ =>
            {
                AddPathSegment(scene, _.Item1, _.Item2, width, propId++);
            });

            path.WayPoints.ForEach(_ =>
            {
                scene.Props.Add(new PropCircle(propId, width, _));
            });
        }

        private static void AddPathSegment(
            Scene scene,
            Vector2D start,
            Vector2D end,
            double width,
            int propId)
        {
            if (start.X == end.X)
            {
                var x = start.X;
                var y0 = Math.Min(start.Y, end.Y);
                var y1 = Math.Max(start.Y, end.Y);

                scene.Props.Add(new PropRectangle(propId, width, y1 - y0, new Vector2D(x, (y0 + y1) / 2)));
            }
            else if (start.Y == end.Y)
            {
                var x0 = Math.Min(start.X, end.X);
                var x1 = Math.Max(start.X, end.X);
                var y = start.Y;
                scene.Props.Add(new PropRectangle(propId, x1 - x0, width, new Vector2D((x0 + x1) / 2, y)));
            }
            else
            {
                var v = end - start;
                var w = v.Length;
                var h = width;
                var center = (start + end) / 2;
                var orientation = -v.AsPolarVector().Angle;

                scene.Props.Add(new PropRotatableRectangle(propId, w, h, center, orientation));
            }
        }
    }
}
