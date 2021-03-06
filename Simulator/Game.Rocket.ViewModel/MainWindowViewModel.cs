using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Geometry2D;
using Simulator.Domain;
using Simulator.Domain.Boundaries;
using Simulator.Application;
using Simulator.ViewModel;
using Game.Rocket.ViewModel.Bodies;
using Game.Rocket.ViewModel.ShapeViewModels;

namespace Game.Rocket.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const int _initialMagnification = 174;
        private int _nextWallId = 100000;

        private ILogger _logger;   
        private SceneViewManager _sceneViewManager;
        private bool _rocketIgnited;
        private bool _startButtonVisible;
        private bool _geometryEditorVisible = true;        

        public Application Application { get; }

        public ApplicationStateListViewModel ApplicationStateListViewModel { get; }
        public UnlockedLevelsViewModel UnlockedLevelsViewModel { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        private RelayCommand _startOrResumeAnimationCommand;

        public RelayCommand StartOrResumeAnimationCommand =>
            _startOrResumeAnimationCommand ?? (_startOrResumeAnimationCommand =
                new RelayCommand(StartOrResumeAnimation, CanStartOrResumeAnimation));

        public bool StartButtonVisible     
        {
            get { return _startButtonVisible; }
            set
            {
                _startButtonVisible = value;
                RaisePropertyChanged();
            }
        }

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

            // General purpose interaction callbacks that works for all scenes
            var spaceKeyIsDown = false;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;
            var nextProjectileId = 1000;
            var disposeProjectilesMap = new Dictionary<int, int>();
            var rateOfFire = 30;
            var nextFragmentId = 1000;
            var nextMeteorId = 10000;

            InitializationCallback initializationCallback = (state, message) =>
            {
                spaceKeyIsDown = false;
                stateIndexOfFirstShotInBurst = -1000;
                stateIndexOfLastShotInBurst = -1000;
                nextProjectileId = 1000;
                disposeProjectilesMap.Clear();
                nextFragmentId = 100;
                nextMeteorId = 10000;
            };

            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return body is Meteor 
                    ? OutcomeOfCollisionBetweenBodyAndBoundary.Reflect 
                    : OutcomeOfCollisionBetweenBodyAndBoundary.Block;
            };

            CheckForCollisionBetweenBodiesCallback checkForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Meteor || body2 is Meteor)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }

                    if (body1 is Bodies.Rocket || body2 is Bodies.Rocket)
                    {
                        return true;
                    }
                }

                return false;
            };

            CollisionBetweenTwoBodiesOccuredCallBack collisionBetweenTwoBodiesOccuredCallBack =
                (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.Ignore;

            InteractionCallBack interactionCallBack = (keyboardState, keyboardEvents, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.FirstOrDefault();

                if (currentStateOfMainBody == null || currentStateOfMainBody.Body.Id != 1)
                {
                    return false;
                }

                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentCustomForce = currentStateOfMainBody.CustomForce;

                var newRotationalSpeed = 0.0;
                var newCustomForce = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                if (keyboardState.UpArrowDown)
                {
                    _rocketIgnited = true;
                    newCustomForce = new Vector2D(3, 0);
                }
                else
                {
                    _rocketIgnited = false;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.CustomForce = newCustomForce;

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    (newCustomForce - currentCustomForce).Length < 0.01 &&
                    !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                if (keyboardEvents.SpaceDown)
                {
                    if (keyboardState.SpaceDown)
                    {
                        if (currentState.Index > stateIndexOfLastShotInBurst + rateOfFire)
                        {
                            stateIndexOfFirstShotInBurst = currentState.Index + 1;
                        }
                    }
                    else
                    {
                        stateIndexOfLastShotInBurst = stateIndexOfFirstShotInBurst;
                        while (stateIndexOfLastShotInBurst < currentState.Index)
                        {
                            stateIndexOfLastShotInBurst += rateOfFire;
                        }

                        stateIndexOfLastShotInBurst -= rateOfFire;
                    }
                }

                return true;
            };

            var extraBodies = Enumerable.Range(2, 1)
                .Select(i => new
                {
                    StateIndex = i * 50,
                    BodyState = new BodyState(new Meteor(i, 0.15, 1, true), new Vector2D(-0.8, -0.8), new Vector2D(0.8, 0.2)) { Life = 5 }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            var nRocketFragments = 16;
            var nMeteorFragments = 4;

            PostPropagationCallBack postPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                // Remove projectile due to expiration?
                if (disposeProjectilesMap.ContainsKey(propagatedState.Index))
                {
                    var projectile = propagatedState.TryGetBodyState(disposeProjectilesMap[propagatedState.Index]);
                    propagatedState?.RemoveBodyState(projectile);
                }

                var random = new Random();
                var response = new PostPropagationResponse();

                var rocket = propagatedState.TryGetBodyState(1);

                if (rocket == null) return response;

                // Remove projectile due to collision with boundary?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                // Local function
                void DetonateRocket()
                {
                    propagatedState.RemoveBodyState(rocket);

                    Enumerable.Range(1, nRocketFragments).ToList().ForEach(i =>
                    {
                        var angle = 0.125 * Math.PI + 2.0 * Math.PI * i / nRocketFragments +
                                    (2 * random.NextDouble() - 1) * 0.1;
                        var velocity1 = (0.6 + (2 * random.NextDouble() - 1) * 0.2) *
                                        new Vector2D(Math.Cos(angle), Math.Sin(angle));
                        var velocity2 = (0.8 + (2 * random.NextDouble() - 1) * 0.2) *
                                        new Vector2D(Math.Cos(angle), Math.Sin(angle));

                        propagatedState.AddBodyState(new BodyState(new Fragment(nextFragmentId++, 0.1, 0.1, true),
                            rocket.Position, velocity1));
                        propagatedState.AddBodyState(new BodyState(new Fragment(nextFragmentId++, 0.1, 0.1, true),
                            rocket.Position, 2 * velocity2));
                    });

                    response.Outcome = "Game Over";
                    response.IndexOfLastState = propagatedState.Index + 200;
                }

                var hitEnemies = new HashSet<BodyState>();

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Bodies.Rocket || bcr.Body2 is Bodies.Rocket)
                    {
                        // Rocket collided with meteor
                        DetonateRocket();
                    }
                    else if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with meteor
                        if (bcr.Body1 is Projectile)
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body1.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body2.Id);
                            hitEnemies.Add(bodyState);
                        }
                        else
                        {
                            propagatedState.RemoveBodyStates(new List<int> { bcr.Body2.Id });
                            var bodyState = propagatedState.TryGetBodyState(bcr.Body1.Id);
                            hitEnemies.Add(bodyState);
                        }
                    }
                });

                hitEnemies.ToList().ForEach(e =>
                {
                    e.Life -= 1;

                    if (e.Life <= 0.1)
                    {
                        propagatedState.RemoveBodyStates(new List<int> { e.Body.Id });

                        if (e.Body.Mass > 0.9)
                        {
                            Enumerable.Range(1, nMeteorFragments).ToList().ForEach(i =>
                            {
                                var angle = 0.125 * Math.PI + 2.0 * Math.PI * i / nMeteorFragments;
                                var velocity = 0.5 * new Vector2D(Math.Cos(angle), Math.Sin(angle));

                                propagatedState.AddBodyState(new BodyState(new Meteor(nextMeteorId++, 0.1, 0.1, true), e.Position, velocity));
                            });
                        }
                    }
                });

                // Add a projectile from rocket?
                if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    nextProjectileId++;
                    disposeProjectilesMap[propagatedState.Index + 100] = nextProjectileId;
                    var projectileSpeed = 4;

                    var projectileVelocity = new Vector2D(
                        projectileSpeed * Math.Cos(rocket.Orientation),
                        projectileSpeed * -Math.Sin(rocket.Orientation));

                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId, 0.025, 1, true), rocket.Position, projectileVelocity));
                }

                // Add an enemy?
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                if (!boundaryCollisionReports.Any()) return response;

                // Check if the rocket collided with anything
                var boundaryCollisionReport = boundaryCollisionReports
                    .FirstOrDefault(bcr => bcr.Body is Bodies.Rocket);

                if (boundaryCollisionReport != null)
                {
                    if (string.IsNullOrEmpty(boundaryCollisionReport.Boundary.Tag))
                    {
                        DetonateRocket();
                    }
                    else
                    {
                        response.Outcome = boundaryCollisionReport.Boundary.Tag;
                        response.IndexOfLastState = propagatedState.Index + 1;
                    }
                }

                return response;
            };

            Application = new Application(_logger);
            Application.AddApplicationState(new ApplicationState { Name = "Welcome Screen" });
            Application.AddApplicationState(new ApplicationState { Name = "Unlocked Levels" });
            Application.AddApplicationState(new Level
            {
                Name = "Level 1a",
                Scene = GenerateScene1(
                    initializationCallback, 
                    interactionCallBack, 
                    collisionBetweenBodyAndBoundaryOccuredCallBack, 
                    checkForCollisionBetweenBodiesCallback,
                    collisionBetweenTwoBodiesOccuredCallBack,
                    postPropagationCallBack) 
            });
            Application.AddApplicationState(new ApplicationStateWithScene
            {
                Name = "Level 1b",
                Scene = GenerateScene2(
                    initializationCallback,
                    interactionCallBack,
                    collisionBetweenBodyAndBoundaryOccuredCallBack,
                    checkForCollisionBetweenBodiesCallback,
                    collisionBetweenTwoBodiesOccuredCallBack,
                    postPropagationCallBack)
            });
            Application.AddApplicationState(new ApplicationState { Name = "Level 1 Cleared" });
            Application.AddApplicationState(new Level
            {
                Name = "Level 2",
                Scene = GenerateScene3(
                    initializationCallback,
                    interactionCallBack,
                    collisionBetweenBodyAndBoundaryOccuredCallBack,
                    checkForCollisionBetweenBodiesCallback,
                    collisionBetweenTwoBodiesOccuredCallBack,
                    postPropagationCallBack)
            });
            Application.AddApplicationState(new ApplicationState { Name = "Game Over" });
            Application.AddApplicationState(new ApplicationState { Name = "You Win" });

            Application.KeyEventOccured += (s, e) =>
            {
                if (e.KeyboardKey != KeyboardKey.Space ||
                    e.KeyEventType != KeyEventType.KeyPressed)
                {
                    return;
                }

                // If a key is pressed while the application is in state that is not a scene, then it may
                // result in transition to another application state
                switch (Application.CurrentApplicationState.Name)
                {
                    case "Welcome Screen":
                    {
                        ApplicationStateListViewModel.CurrentApplicationState = Application.GetApplicationState("Level 1a");
                        StartOrResumeAnimationCommand.Execute(null);
                        break;
                    }
                    case "Level 1 Cleared":
                    {
                        var level = Application.GetApplicationState("Level 2") as Level;
                        UnlockedLevelsViewModel.UnlockLevel(level);
                        ApplicationStateListViewModel.CurrentApplicationState = level;
                        StartOrResumeAnimationCommand.Execute(null);
                        break;
                    }
                    case "Game Over":
                    case "You Win":
                    {
                        ApplicationStateListViewModel.CurrentApplicationState = Application.GetApplicationState("Welcome Screen");
                        _sceneViewManager.ActiveScene = null;
                        break;
                    }
                }
            };

            Application.AnimationCompleted += (s, e) =>
            {
                // The outcome should be the name of another application state
                if (string.IsNullOrEmpty(Application.Engine.Outcome))
                {
                    throw new InvalidOperationException("Animation completed without an outcome, which is illegal in this context");
                }

                var applicationState = Application.GetApplicationState(Application.Engine.Outcome);
                ApplicationStateListViewModel.CurrentApplicationState = applicationState;

                if (applicationState is ApplicationStateWithScene applicationStateWithScene)
                {
                    _sceneViewManager.ActiveScene = applicationStateWithScene.Scene;
                    StartOrResumeAnimationCommand.Execute(null);
                }

                RefreshButtons();
            };

            ApplicationStateListViewModel = new ApplicationStateListViewModel(Application);
            ApplicationStateListViewModel.SelectedApplicationState.PropertyChanged += 
                SelectedApplicationState_PropertyChanged1;

            UnlockedLevelsViewModel = new UnlockedLevelsViewModel();
            UnlockedLevelsViewModel.ApplicationStateListViewModel.AddApplicationState(
                Application.GetApplicationState("Level 1a"));

            UnlockedLevelsViewModel.ApplicationStateListViewModel.SelectedApplicationState.PropertyChanged +=
                SelectedApplicationState_PropertyChanged2;

            GeometryEditorViewModel = new GeometryEditorViewModel
            {
                UpdateModelCallBack = Application.UpdateModel
            };

            _sceneViewManager = new SceneViewManager(
                Application, 
                GeometryEditorViewModel, 
                (bs) =>
                {
                    switch (bs.Body)
                    {
                        case Bodies.Rocket rocket:
                            {
                                return new RocketViewModel
                                {
                                    Width = 2 * rocket.Radius,
                                    Height = 2 * rocket.Radius,
                                };
                            }
                        case Projectile projectile:
                        {
                            return new ProjectileViewModel()
                            {
                                Width = 2 * projectile.Radius,
                                Height = 2 * projectile.Radius,
                            };
                        }
                        case Fragment fragment:
                            {
                                return new FragmentViewModel
                                {
                                    Width = 2 * fragment.Radius,
                                    Height = 2 * fragment.Radius
                                };
                            }
                        case Meteor meteor:
                        {
                            return new MeteorViewModel
                            {
                                Width = 2 * meteor.Radius,
                                Height = 2 * meteor.Radius
                            };
                        }
                    }

                    throw new InvalidOperationException("Unknown Body Type - cannot select ShapeViewModel");
                },
                (shapeViewModel, bs) =>
                {
                    switch (bs.Body)
                    {
                        case Bodies.Rocket _:
                            {
                                if (shapeViewModel is RocketViewModel rocketViewModel)
                                {
                                    rocketViewModel.Orientation = bs.Orientation;
                                    rocketViewModel.Ignited = _rocketIgnited;
                                }

                                break;
                            }
                    }

                    shapeViewModel.Point = new Point2D(bs.Position.X, bs.Position.Y);
                });
        }

        public void HandleLoaded()
        {
            ApplicationStateListViewModel.CurrentApplicationState = Application.ApplicationStates.First();
        }

        private void StartOrResumeAnimation()
        {
            Application.StartOrResumeAnimation();
            RefreshButtons();
        }

        private bool CanStartOrResumeAnimation()
        {
            return Application.CanStartOrResumeAnimation;
        }

        private void RefreshButtons()
        {
            StartOrResumeAnimationCommand.RaiseCanExecuteChanged();
        }

        private void SelectedApplicationState_PropertyChanged1(
            object sender,
            PropertyChangedEventArgs e)
        {
            var applicationState = (sender as ObservableObject<ApplicationState>)?.Object;

            if (applicationState == null)
            {
                return;
            }

            Application.CurrentApplicationState = applicationState;

            if (!(applicationState is ApplicationStateWithScene applicationStateWithScene)) return;

            _sceneViewManager.ActiveScene = applicationStateWithScene.Scene;

            RefreshButtons();
        }

        private void SelectedApplicationState_PropertyChanged2(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (!((sender as ObservableObject<ApplicationState>)?.Object is Level level))
            {
                throw new InvalidOperationException();
            }

            ApplicationStateListViewModel.CurrentApplicationState = level;

            _sceneViewManager.ActiveScene = level.Scene;
            StartOrResumeAnimationCommand.Execute(null);
        }

        private Scene GenerateScene1(
            InitializationCallback initializationCallback,
            InteractionCallBack interactionCallBack,
            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack,
            CheckForCollisionBetweenBodiesCallback checkForCollisionBetweenBodiesCallback,
            CollisionBetweenTwoBodiesOccuredCallBack collisionBetweenTwoBodiesOccuredCallBack,
            PostPropagationCallBack postPropagationCallBack)
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Bodies.Rocket(1, 0.125, 1, true), new Vector2D(-1.5, -0.5), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Scene 1", _initialMagnification,
                new Point2D(-1.9321428571428569, -1.0321428571428573), initialState, 0, 0, 0, 1, true, 0.005)
            {
                IncludeCustomForces = true,
                InitializationCallback = initializationCallback,
                CollisionBetweenBodyAndBoundaryOccuredCallBack = collisionBetweenBodyAndBoundaryOccuredCallBack,
                CheckForCollisionBetweenBodiesCallback = checkForCollisionBetweenBodiesCallback,
                CollisionBetweenTwoBodiesOccuredCallBack = collisionBetweenTwoBodiesOccuredCallBack,
                PostPropagationCallBack = postPropagationCallBack,
                InteractionCallBack = interactionCallBack
            };

            var margin = 0.3;
            scene.AddRectangularBoundary(-1.9, 5.25, -1, 3);
            AddWall(scene, -1.9 - margin, 5.25 + margin, -1 - margin, -1, false, false, false, false);
            AddWall(scene, -1.9 - margin, 5.25 + margin, 3, 3 + margin, false, false, false, false);
            AddWall(scene, -1.9 - margin, -1.9, -1, 3, false, false, false, false);
            AddWall(scene, 5.25, 5.25 + margin, -1, 3, false, false, false, false);
            //AddWall(scene, -1, -0.5, 2, 3);
            //AddWall(scene, 0.5, 1, -1, 0);

            // Add exits
            scene.AddBoundary(new LineSegment(new Vector2D(4, -0.95), new Vector2D(5.25, -0.95), "Level 1b") { Visible = true });

            return scene;
        }

        private Scene GenerateScene2(
            InitializationCallback initializationCallback,
            InteractionCallBack interactionCallBack,
            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack,
            CheckForCollisionBetweenBodiesCallback checkForCollisionBetweenBodiesCallback,
            CollisionBetweenTwoBodiesOccuredCallBack collisionBetweenTwoBodiesOccuredCallBack,
            PostPropagationCallBack postPropagationCallBack)
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Bodies.Rocket(1, 0.125, 1, true), new Vector2D(-1.5, -0.5), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Scene 1", _initialMagnification,
                new Point2D(-1.9321428571428569, -1.0321428571428573), initialState, 0.5, 0, 0, 1, true, 0.005)
            {
                IncludeCustomForces = true,
                InitializationCallback = initializationCallback,
                CollisionBetweenBodyAndBoundaryOccuredCallBack = collisionBetweenBodyAndBoundaryOccuredCallBack,
                CheckForCollisionBetweenBodiesCallback = checkForCollisionBetweenBodiesCallback,
                CollisionBetweenTwoBodiesOccuredCallBack = collisionBetweenTwoBodiesOccuredCallBack,
                PostPropagationCallBack = postPropagationCallBack,
                InteractionCallBack = interactionCallBack
            };

            var margin = 0.3;
            scene.AddRectangularBoundary(-1.9, 5.25, -1, 3);
            AddWall(scene, -1.9 - margin, 5.25 + margin, -1 - margin, -1, false, false, false, false);
            AddWall(scene, -1.9 - margin, 5.25 + margin, 3, 3 + margin, false, false, false, false);
            AddWall(scene, -1.9 - margin, -1.9, -1, 3, false, false, false, false);
            AddWall(scene, 5.25, 5.25 + margin, -1, 3, false, false, false, false);
            AddWall(scene, -1, -0.5, 2, 3);
            AddWall(scene, 0.5, 1, -1, 0);
            AddWall(scene, 2, 2.5, 2, 3);
            AddWall(scene, 3.5, 4, -1, 0);

            // Add exits
            scene.AddBoundary(new LineSegment(new Vector2D(4, -0.95), new Vector2D(5.25, -0.95), "Level 1 Cleared") { Visible = true });

            return scene;
        }

        private Scene GenerateScene3(
            InitializationCallback initializationCallback,
            InteractionCallBack interactionCallBack,
            CollisionBetweenBodyAndBoundaryOccuredCallBack collisionBetweenBodyAndBoundaryOccuredCallBack,
            CheckForCollisionBetweenBodiesCallback checkForCollisionBetweenBodiesCallback,
            CollisionBetweenTwoBodiesOccuredCallBack collisionBetweenTwoBodiesOccuredCallBack,
            PostPropagationCallBack postPropagationCallBack)
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Bodies.Rocket(1, 0.125, 1, true), new Vector2D(-1.5, -0.5), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Scene 1", _initialMagnification,
                new Point2D(-1.9321428571428569, -1.0321428571428573), initialState, 0.5, 0, 0, 1, true, 0.005)
            {
                IncludeCustomForces = true,
                InitializationCallback = initializationCallback,
                CollisionBetweenBodyAndBoundaryOccuredCallBack = collisionBetweenBodyAndBoundaryOccuredCallBack,
                CheckForCollisionBetweenBodiesCallback = checkForCollisionBetweenBodiesCallback,
                CollisionBetweenTwoBodiesOccuredCallBack = collisionBetweenTwoBodiesOccuredCallBack,
                PostPropagationCallBack = postPropagationCallBack,
                InteractionCallBack = interactionCallBack
            };

            var margin = 0.3;
            scene.AddRectangularBoundary(-1.9, 5.25, -1, 3);
            AddWall(scene, -1.9 - margin, 5.25 + margin, -1 - margin, -1, false, false, false, false);
            AddWall(scene, -1.9 - margin, 5.25 + margin, 3, 3 + margin, false, false, false, false);
            AddWall(scene, -1.9 - margin, -1.9, -1, 3, false, false, false, false);
            AddWall(scene, 5.25, 5.25 + margin, -1, 3, false, false, false, false);
            var margin2 = 1;
            AddWall(scene, -1.9 + margin2, 5.25 - margin2, -1 + margin2, 3 - margin2);

            // Add exits
            scene.AddBoundary(new LineSegment(new Vector2D(4, -0.95), new Vector2D(5.25, -0.95), "You Win") { Visible = true });

            return scene;
        }

        // Scene building helpers
        private void AddWall(
            Scene scene,
            double x0,
            double x1,
            double y0,
            double y1,
            bool boundaryLeft = true,
            bool boundaryRight = true,
            bool boundaryTop = true,
            bool boundaryBottom = true)
        {
            scene.Props.Add(new Prop(_nextWallId++, x1 - x0, y1 - y0, new Vector2D((x0 + x1) / 2, (y0 + y1) / 2)));

            if (boundaryLeft)
            {
                scene.AddBoundary(new VerticalLineSegment(x0, y0, y1));
            }

            if (boundaryRight)
            {
                scene.AddBoundary(new VerticalLineSegment(x1, y0, y1));
            }

            if (boundaryTop)
            {
                scene.AddBoundary(new HorizontalLineSegment(y0, x0, x1));
            }

            if (boundaryBottom)
            {
                scene.AddBoundary(new HorizontalLineSegment(y1, x0, x1));
            }
        }
    }
}
