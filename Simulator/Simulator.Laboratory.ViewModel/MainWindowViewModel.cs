using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using GalaSoft.MvvmLight;
using Simulator.Domain;
using Simulator.Domain.Boundaries;
using Simulator.ViewModel;

namespace Simulator.Laboratory.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Dictionary<Scene, ShapeSelectorCallback> _shapeSelectorCallbacks;
        private Dictionary<Scene, ShapeUpdateCallback> _shapeUpdateCallbacks;

        private ILogger _logger;
        private SceneViewManager _sceneViewManager;
        private string _outcome;

        public Application.Application Application { get; }
        public SceneListViewModel SceneListViewModel { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        private RelayCommand _startOrResumeAnimationCommand;
        private RelayCommand _pauseAnimationCommand;
        private RelayCommand _resetAnimationCommand;

        public RelayCommand StartOrResumeAnimationCommand =>
            _startOrResumeAnimationCommand ?? (_startOrResumeAnimationCommand =
                new RelayCommand(StartOrResumeAnimation, CanStartOrResumeAnimation));

        public RelayCommand PauseAnimationCommand =>
            _pauseAnimationCommand ?? (_pauseAnimationCommand =
                new RelayCommand(PauseAnimation, CanPauseAnimation));

        public RelayCommand ResetAnimationCommand =>
            _resetAnimationCommand ?? (_resetAnimationCommand =
                new RelayCommand(ResetAnimation, CanResetAnimation));

        public string Outcome
        {
            get { return _outcome; }
            set
            {
                _outcome = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            ILogger logger)
        {
            _shapeSelectorCallbacks = new Dictionary<Scene, ShapeSelectorCallback>();
            _shapeUpdateCallbacks = new Dictionary<Scene, ShapeUpdateCallback>();

            _logger = logger;
            //_logger = null; // Disable logging (it should only be used for debugging purposes)
            _logger?.WriteLine(LogMessageCategory.Information, "Simulator - Starting up");

            Outcome = null;

            Application = new Application.Application(_logger);
            Application.AnimationCompleted += (s, e) =>
            {
                // If the outcome is the name of another scene then switch to that scene
                if (SceneListViewModel.ContainsScene(Application.Engine.Outcome))
                {
                    Application.Engine.PreviousScene = Application.Engine.Scene.Name;
                    SceneListViewModel.ActiveScene = SceneListViewModel.GetScene(Application.Engine.Outcome);
                    StartOrResumeAnimationCommand.Execute(null);
                }
                else
                {
                    Outcome = Application.Engine.Outcome;
                }

                RefreshButtons();
            };

            // Bemærk: Det er et ALMINDELIGT view og altså ikke et "matematisk"
            GeometryEditorViewModel = new GeometryEditorViewModel(1)
            {
                UpdateModelCallBack = Application.UpdateModel
            };

            GeometryEditorViewModel.MouseClickOccured += (s, e) =>
            {
                Application.HandleMouseClickEvent(new Point2D(
                    e.CursorWorldPosition.X, 
                    e.CursorWorldPosition.Y));
            };

            _sceneViewManager = new SceneViewManager(Application, GeometryEditorViewModel);

            SceneListViewModel = new SceneListViewModel();
            SceneListViewModel.SelectedScene.PropertyChanged += SelectedScene_PropertyChanged;

            // Denne bruges indtil videre kun for Shoot 'Em Up 7 og 8
            ShapeSelectorCallback shapeSelectorCallback1 = (bs) =>
            {
                if (!(bs.Body is CircularBody))
                {
                    throw new InvalidOperationException();
                }

                var circularBody = bs.Body as CircularBody;

                return new TaggedEllipseViewModel
                {
                    //Point = new Point2D(bs.Position.X, bs.Position.Y),
                    Width = 2 * circularBody.Radius,
                    Height = 2 * circularBody.Radius,
                    Tag = bs.Life.ToString()
                };
            };

            // Denne bruges indtil videre kun for Shoot 'Em Up 7 og 8
            ShapeUpdateCallback shapeUpdateCallback1 = (shapeViewModel, bs) =>
            {
                shapeViewModel.Point = new PointD(bs.Position.X, bs.Position.Y);

                if (shapeViewModel is TaggedEllipseViewModel taggedEllipseViewModel)
                {
                    taggedEllipseViewModel.Tag = bs.Life.ToString();
                }
            };

            // Denne bruges indtil videre kun for Rotation scenerne
            ShapeSelectorCallback shapeSelectorCallback2 = (bs) =>
            {
                if (!(bs.Body is CircularBody))
                {
                    throw new InvalidOperationException();
                }

                var circularBody = bs.Body as CircularBody;

                return new RotatableEllipseViewModel
                {
                    //Point = new Point2D(bs.Position.X, bs.Position.Y),
                    Width = 2 * circularBody.Radius,
                    Height = 2 * circularBody.Radius,
                    Orientation = bs.Orientation
                };
            };

            // Denne bruges indtil videre kun for Rotation og Rocket scenerne
            ShapeUpdateCallback shapeUpdateCallback2 = (shapeViewModel, bs) =>
            {
                shapeViewModel.Point = new PointD(bs.Position.X, bs.Position.Y);

                if (shapeViewModel is RotatableEllipseViewModel)
                {
                    RotatableEllipseViewModel rotatableEllipseViewModel = shapeViewModel as RotatableEllipseViewModel;
                    rotatableEllipseViewModel.Orientation = bs.Orientation;
                }
            };

            AddScene(GenerateSceneAddBodiesByClicking1());
            AddScene(GenerateSceneAddBodiesByClicking2());
            AddScene(GenerateSceneAddBodiesByClicking3());
            AddScene(GenerateSceneAddBodiesByClicking4(), shapeSelectorCallback1, shapeUpdateCallback1);
            AddScene(GenerateSceneShootEmUp7(), shapeSelectorCallback1, shapeUpdateCallback1);
            AddScene(GenerateSceneShootEmUp8(), shapeSelectorCallback1, shapeUpdateCallback1);
            AddScene(GenerateSceneFountain1());
            AddScene(GenerateSceneFountain2());
            AddScene(GenerateSceneFountain3());
            AddScene(GenerateSceneFountain4());
            AddScene(GenerateSceneFountain5());
            AddScene(GenerateSceneFountain6());
            AddScene(GenerateSceneRocket1(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRocket2(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRocket3(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRocket4(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneBallTrain1());
            AddScene(GenerateSceneBallTrain2());
            AddScene(GenerateSceneBallTrain3());
            AddScene(GenerateSceneBallTrain4());
            AddScene(GenerateSceneMovingBall1());
            AddScene(GenerateSceneMovingBall2());
            AddScene(GenerateSceneMovingBall3());
            AddScene(GenerateSceneMovingBall4());
            AddScene(GenerateSceneMovingBall5());
            AddScene(GenerateSceneMovingBrick1());
            AddScene(GenerateSceneMovingBrick2());
            AddScene(GenerateSceneMovingBrick3());
            AddScene(GenerateSceneMovingBrick4());
            AddScene(GenerateSceneMovingBrick5());
            AddScene(GenerateSceneMovingBrick6());
            AddScene(GenerateSceneRotation1(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRotation2(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRotation3(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRotation4(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneBallRestingInGravityField());
            AddScene(GenerateSceneBallInteraction1());
            AddScene(GenerateSceneRotationConstrained1(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRotationConstrained2(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneRotationConstrained3(), shapeSelectorCallback2, shapeUpdateCallback2);
            AddScene(GenerateSceneBallInteraction2());
            AddScene(GenerateSceneBallInteraction3());
            AddScene(GenerateSceneBallInteraction4());
            AddScene(GenerateSceneBallInteraction5());
            AddScene(GenerateSceneBallInteractionLargeScene1());
            AddScene(GenerateSceneBallInteractionLargeScene2());
            AddScene(GenerateSceneBrickInteraction1());
            AddScene(GenerateSceneBouncingBall1());
            AddScene(GenerateSceneBouncingBall2());
            AddScene(GenerateScenePoolTableWithOneBall());
            AddScene(GenerateScenePoolTableWithOneBallAndThreeBoundaryPoints());
            AddScene(GenerateScenePoolTableWithTwoBallsAnd1LineSegment());
            AddScene(GenerateScenePoolTableWith1BallAnd1LineSegment());
            AddScene(GenerateScenePoolTableWithTwoBalls());
            AddScene(GenerateSceneSimultaneousCollisionsWithBoundary());
            AddScene(GenerateSceneNewtonsCradle1());
            AddScene(GenerateSceneNewtonsCradle2());
            AddScene(GenerateScenePoolTableWithThreeBalls());
            AddScene(GenerateScenePoolTableWithThreeBallsAndFriction());
            AddScene(GenerateSceneOrbit1());
            AddScene(GenerateSceneOrbit2());
            AddScene(GenerateSceneMoonAndEarth());
            AddScene(GenerateSceneFlappyBird());
            AddScene(GenerateScenePlatformer1());
            AddScene(GenerateScenePlatformer2());
            AddScene(GenerateScenePlatformer3());
            AddScene(GenerateSceneCollisionRegistrationTest());
            AddScene(GenerateSceneShootEmUp1());
            AddScene(GenerateSceneShootEmUp2());
            AddScene(GenerateSceneShootEmUp3());
            AddScene(GenerateSceneShootEmUp4());
            AddScene(GenerateSceneShootEmUp5());
            AddScene(GenerateSceneShootEmUp6());
            AddScene(GenerateSceneDodgeball());
            AddScene(GenerateSceneRambo());
            AddScene(GenerateSceneMultipleOutcomes());
            AddScene(GenerateSceneMazeRoom1());
            AddScene(GenerateSceneMazeRoom2());
        }

        private void SelectedScene_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            var scene = (sender as ObservableObject<Scene>)?.Object;

            if (scene != null)
            {
                if (_shapeSelectorCallbacks.TryGetValue(scene, out ShapeSelectorCallback shapeSelectorCallback))
                {
                    _sceneViewManager.ShapeSelectorCallback = shapeSelectorCallback;
                }
                else
                {
                    _sceneViewManager.SetShapeSelectorCallbackToDefault();
                }

                if (_shapeUpdateCallbacks.TryGetValue(scene, out ShapeUpdateCallback shapeUpdateCallback))
                {
                    _sceneViewManager.ShapeUpdateCallback = shapeUpdateCallback;
                }
                else
                {
                    _sceneViewManager.SetShapeUpdateCallbackToDefault();
                }
            }

            _sceneViewManager.ActiveScene = scene;
            RefreshButtons();
            Outcome = null;
        }

        private void StartOrResumeAnimation()
        {
            Application.StartOrResumeAnimation();
            RefreshButtons();
        }

        private void PauseAnimation()
        {
            Application.PauseAnimation();
            RefreshButtons();
        }

        private void ResetAnimation()
        {
            _sceneViewManager.ResetScene();
            RefreshButtons();
            Outcome = null;
        }

        private bool CanStartOrResumeAnimation()
        {
            return Application.CanStartOrResumeAnimation;
        }

        private bool CanPauseAnimation()
        {
            return Application.CanPauseAnimation;
        }

        private bool CanResetAnimation()
        {
            return Application.CanResetAnimation;
        }

        private void RefreshButtons()
        {
            StartOrResumeAnimationCommand.RaiseCanExecuteChanged();
            PauseAnimationCommand.RaiseCanExecuteChanged();
            ResetAnimationCommand.RaiseCanExecuteChanged();
        }

        private void AddScene(
            Scene scene,
            ShapeSelectorCallback shapeSelectorCallback = null,
            ShapeUpdateCallback shapeUpdateCallback = null)
        {
            SceneListViewModel.AddScene(scene);

            if (shapeSelectorCallback != null)
            {
                _shapeSelectorCallbacks[scene] = shapeSelectorCallback;
            }

            if (shapeUpdateCallback != null)
            {
                _shapeUpdateCallbacks[scene] = shapeUpdateCallback;
            }
        }

        private static Scene GenerateSceneRocket1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI,
                CustomForce = new Vector2D(1, 0)
            });

            var scene = new Scene("Rocket, loose, in space (Interaction)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);
            scene.IncludeCustomForces = true;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            return scene;
        }

        private static Scene GenerateSceneRocket2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rocket, controlled, in space (Interaction)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);
            scene.IncludeCustomForces = true;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
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
                    newCustomForce = new Vector2D(1, 0);
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.CustomForce = newCustomForce;

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    (newCustomForce - currentCustomForce).Length < 0.01)
                {
                    return false;
                }

                return true;
            };

            return scene;
        }

        private static Scene GenerateSceneRocket3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rocket, controlled, lunar surface (Interaction)", 120.0, new Point2D(-1.4, -1.3), initialState, 1.62, 0, 0, 1, false, 0.005);
            scene.IncludeCustomForces = true;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
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
                    newCustomForce = new Vector2D(3, 0);
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.CustomForce = newCustomForce;

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    (newCustomForce - currentCustomForce).Length < 0.01)
                {
                    return false;
                }

                return true;
            };

            return scene;
        }

        private static Scene GenerateSceneRocket4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(-0.5, -0.5), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rocket, escape from lunar base", 120.0, new Point2D(-1.4, -1.3), initialState, 1.62, 0, 0, 1, false, 0.005);
            scene.IncludeCustomForces = true;

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                if (boundaryCollisionReports.Any())
                {
                    response.Outcome = boundaryCollisionReports
                        .Any(bcr => bcr.Boundary.Tag == "Exit")
                        ? "You Win"
                        : "Game Over";

                    response.IndexOfLastState = propagatedState.Index + 1;
                }

                return response;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
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
                    newCustomForce = new Vector2D(3, 0);
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.CustomForce = newCustomForce;

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    (newCustomForce - currentCustomForce).Length < 0.01)
                {
                    return false;
                }

                return true;
            };

            scene.AddRectangularBoundary(-1, 3, -1, 3);
            scene.AddRectangularBoundary(0, 0.5, 0, 3);
            scene.AddRectangularBoundary(1.5, 2, -1, 2);

            scene.AddBoundary(new LineSegment(new Vector2D(2, -0.95), new Vector2D(3, -0.95), "Exit") { Visible = false });

            return scene;
        }

        private static Scene GenerateSceneBallTrain1()
        {
            var initialState = new State();

            var scene = new Scene("Ball train I (no gravity)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddBoundary(new HorizontalLineSegment(-1, 0.5, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, -1), new Vector2D(3, -0.5)));
            scene.AddBoundary(new VerticalLineSegment(3, -0.5, 1.5));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 1.5), new Vector2D(2.5, 2)));
            scene.AddBoundary(new HorizontalLineSegment(2, 0.5, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, 2), new Vector2D(0, 1.5)));
            scene.AddBoundary(new VerticalLineSegment(0, -0.5, 1.5));
            scene.AddBoundary(new LineSegment(new Vector2D(0, -0.5), new Vector2D(0.5, -1)));

            var extraBodies = Enumerable.Range(1, 32)
                .Select(i => new
                {
                    StateIndex = i * 60,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(0.5, -0.75), new Vector2D(1, 0))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBallTrain2()
        {
            var initialState = new State();

            var scene = new Scene("Ball train II (no gravity)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddBoundary(new HorizontalLineSegment(-1, 0, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, -1), new Vector2D(3, -0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, -0.5), new Vector2D(2.5, 0)));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, 0), new Vector2D(3, 0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 0.5), new Vector2D(2.5, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(2.5, 1), new Vector2D(3, 1.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 1.5), new Vector2D(2.5, 2)));
            scene.AddBoundary(new HorizontalLineSegment(2, 0, 2.5));
            scene.AddBoundary(new LineSegment(new Vector2D(0, -1), new Vector2D(0.5, -0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, -0.5), new Vector2D(0, 0)));
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0), new Vector2D(0.5, 0.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, 0.5), new Vector2D(0, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(0, 1), new Vector2D(0.5, 1.5)));
            scene.AddBoundary(new LineSegment(new Vector2D(0.5, 1.5), new Vector2D(0, 2)));

            var extraBodies = Enumerable.Range(1, 60)
                .Select(i => new
                {
                    StateIndex = i * 60,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(0.5, -0.75), new Vector2D(1, 0))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBallTrain3()
        {
            var initialState = new State();

            var scene = new Scene("Ball train III (with gravity)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);

            var extraBodies = Enumerable.Range(1, 16)
                .Select(i => new
                {
                    StateIndex = i * 150,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(-0.8, 0), new Vector2D(0.3, 0))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBallTrain4()
        {
            var initialState = new State();

            var scene = new Scene("Ball train IV (with gravity)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddRectangularBoundary(0, 0.5, 0.2, 0.7);
            scene.AddRectangularBoundary(1.5, 2, 0.2, 0.7);

            var extraBodies = Enumerable.Range(1, 20)
                .Select(i => new
                {
                    StateIndex = i * 20,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(-0.8, 0.8), new Vector2D(2.7, -4))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.BodyStates.Add(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneMovingBall1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0)));

            //var scene = new Scene("Moving ball I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);
            var scene = new Scene("Moving ball I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HalfPlane(new Vector2D(3, -1), new Vector2D(-1, 0)));
            scene.AddBoundary(new HalfPlane(new Vector2D(-1, 1), new Vector2D(1, 0)));

            return scene;
        }

        private static Scene GenerateSceneMovingBall2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0.5)));

            var scene = new Scene("Moving ball II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new RightFacingHalfPlane(-1));
            scene.AddBoundary(new LeftFacingHalfPlane(3));
            scene.AddBoundary(new DownFacingHalfPlane(-1));
            scene.AddBoundary(new UpFacingHalfPlane(1));

            return scene;
        }

        private static Scene GenerateSceneMovingBall3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0.5)));

            var scene = new Scene("Moving ball III", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new VerticalLineSegment(-1, -1, 1));
            scene.AddBoundary(new VerticalLineSegment(3, -1, 1));
            scene.AddBoundary(new HorizontalLineSegment(-1, -1, 3));
            scene.AddBoundary(new HorizontalLineSegment(1, -1, 3));

            return scene;
        }

        private static Scene GenerateSceneMovingBall4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0)));

            var scene = new Scene("Moving ball IV", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HalfPlane(new Vector2D(0, 0), new Vector2D(1, 1)));
            scene.AddBoundary(new HalfPlane(new Vector2D(2, 0), new Vector2D(-1, 1)));
            scene.AddBoundary(new HalfPlane(new Vector2D(2, 2), new Vector2D(-1, -1)));
            scene.AddBoundary(new HalfPlane(new Vector2D(0, 2), new Vector2D(1, -1)));

            return scene;
        }

        private static Scene GenerateSceneMovingBall5()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0)));

            var scene = new Scene("Moving ball V", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new LineSegment(new Vector2D(1, -1), new Vector2D(3, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(3, 1), new Vector2D(1, 3)));
            scene.AddBoundary(new LineSegment(new Vector2D(1, 3), new Vector2D(-1, 1)));
            scene.AddBoundary(new LineSegment(new Vector2D(-1, 1), new Vector2D(1, -1)));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.4, 0.2, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0.5)));

            var scene = new Scene("Moving brick I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new RightFacingHalfPlane(-1));
            scene.AddBoundary(new LeftFacingHalfPlane(3));
            scene.AddBoundary(new DownFacingHalfPlane(-1));
            scene.AddBoundary(new UpFacingHalfPlane(1));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.5, 0.3, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 1)));

            var scene = new Scene("Moving brick II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new RightFacingHalfPlane(-1));
            scene.AddBoundary(new LeftFacingHalfPlane(3));
            scene.AddBoundary(new DownFacingHalfPlane(-1));
            scene.AddBoundary(new UpFacingHalfPlane(1));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(0, -0.5)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(0, 0.5)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(2, -0.5)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(2, 0.5)));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.4, 0.2, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, 0.5)));

            var scene = new Scene("Moving brick III", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new VerticalLineSegment(-1, -1, 1));
            scene.AddBoundary(new VerticalLineSegment(3, -1, 1));
            scene.AddBoundary(new HorizontalLineSegment(-1, -1, 3));
            scene.AddBoundary(new HorizontalLineSegment(1, -1, 3));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.4, 0.2, 1, true), new Vector2D(1, 1), new Vector2D(1, 0)));

            var scene = new Scene("Moving brick IV", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HorizontalLineSegment(1, -1, 0));
            scene.AddBoundary(new HorizontalLineSegment(1, 2, 3));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick5()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.4, 0.4, 1, true), new Vector2D(1, 0.25), new Vector2D(1, 1)));

            var scene = new Scene("Moving brick V", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HorizontalLineSegment(1, -1, 0));
            scene.AddBoundary(new HorizontalLineSegment(1, 2, 3));
            scene.AddBoundary(new VerticalLineSegment(1, -1, 0));
            scene.AddBoundary(new VerticalLineSegment(1, 2, 3));

            return scene;
        }

        private static Scene GenerateSceneMovingBrick6()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.4, 0.3, 1, true), new Vector2D(1, -0.125), new Vector2D(1.5, -1.4)));

            var scene = new Scene("Moving brick VI", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new VerticalLineSegment(-1, -1, 1));
            scene.AddBoundary(new VerticalLineSegment(3, -1, 1));
            scene.AddBoundary(new HorizontalLineSegment(-1, -1, 3));
            scene.AddBoundary(new HorizontalLineSegment(1, -1, 3));

            scene.AddBoundary(new HorizontalLineSegment(-0.5, -0.5, 0.5));
            scene.AddBoundary(new HorizontalLineSegment(0.5, 1.5, 2.5));
            scene.AddBoundary(new VerticalLineSegment(0, 0, 0.5));
            scene.AddBoundary(new VerticalLineSegment(2, -0.5, 0));

            return scene;
        }

        private static Scene GenerateSceneRotation1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                RotationalSpeed = Math.PI
            });

            var scene = new Scene("Rotation I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            return scene;
        }

        private static Scene GenerateSceneRotation2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.25 * Math.PI,
                RotationalSpeed = 0.5 * Math.PI,
                ArtificialVelocity = new Vector2D(1, 0)
            });

            var scene = new Scene("Rotation II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);
            return scene;
        }

        private static Scene GenerateSceneRotation3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rotation III (Interaction)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01)
                {
                    return false;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;

                return true;
            };

            return scene;
        }

        private static Scene GenerateSceneRotation4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rotation IV (Interaction)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            return scene;
        }

        private static Scene GenerateSceneBallRestingInGravityField()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Ball resting in gravity field", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.AddBoundary(new HalfPlane(new Vector2D(2, 0), new Vector2D(0, -1)));
            return scene;
        }

        private static Scene GenerateSceneBallInteraction1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(0, 2, -0.3, 1);
            return scene;
        }

        private static Scene GenerateSceneRotationConstrained1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rotation V (Interaction, constrained)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            //scene.AddEnclosureOfHalfPlanes(0, 2, -0.3, 2);
            scene.AddBoundary(new LeftFacingHalfPlane(2));
            //scene.AddBoundary(new DownFacingHalfPlane(-0.3));
            //scene.AddBoundary(new UpFacingHalfPlane(2));
            return scene;
        }

        private static Scene GenerateSceneRotationConstrained2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rotation VI (Interaction, constrained)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            scene.AddRectangularBoundary(0, 2, -0.3, 2);
            return scene;
        }

        private static Scene GenerateSceneRotationConstrained3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1.7), new Vector2D(0, 0))
            {
                Orientation = 0.5 * Math.PI
            });

            var scene = new Scene("Rotation VII (Interaction, constrained)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            scene.AddRectangularBoundary(-1, 3, -0.3, 2);
            scene.AddRectangularBoundary(0, 2, 0.6, 1.1);
            return scene;
        }

        private static Scene GenerateSceneBallInteraction2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1.03, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(0, 2, -0.3, 1);
            scene.AddBoundary(new BoundaryPoint(new Vector2D(1, 0.4)));
            return scene;
        }

        private static Scene GenerateSceneBallInteraction3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(1, 0.0), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction III", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0.4), new Vector2D(2, 0.4)));
            return scene;
        }

        private static Scene GenerateSceneBallInteraction4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction IV", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddRectangularBoundary(0, 0.5, 0.2, 0.7);
            scene.AddRectangularBoundary(1.5, 2, 0.2, 0.7);
            return scene;
        }

        private static Scene GenerateSceneBallInteraction5()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction V", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            var diamondCenter = new Vector2D(0, 0.5);
            var diamondRadius = 0.4;
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(0, -diamondRadius),
                diamondCenter + new Vector2D(diamondRadius, 0)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(diamondRadius, 0),
                diamondCenter + new Vector2D(0, diamondRadius)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(0, diamondRadius),
                diamondCenter + new Vector2D(-diamondRadius, 0)));
            scene.AddBoundary(new LineSegment(
                diamondCenter + new Vector2D(-diamondRadius, 0),
                diamondCenter + new Vector2D(0, -diamondRadius)));
            return scene;
        }

        private static Scene GenerateSceneBallInteractionLargeScene1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(3, 2.5), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(0, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.125, 1, true), new Vector2D(1, 0), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction, large scene I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005, SceneViewMode.FocusOnFirstBody, -0.5, -0.5, 9.5, 9.5);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(-0.3, 9.3, -0.3, 9.3);

            var boxWidth = 0.4;
            var boxHeight = 0.3;
            Enumerable.Range(0, 10).ToList().ForEach(x =>
            {
                Enumerable.Range(0, 10).ToList().ForEach(y =>
                {
                    scene.AddRectangularBoundary(x - boxWidth / 2, x + boxWidth / 2, y - boxHeight / 2, y + boxHeight / 2);
                });
            });

            return scene;
        }

        private static Scene GenerateSceneBallInteractionLargeScene2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 0.5), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(0, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.125, 1, true), new Vector2D(1, 0), new Vector2D(0, 0)));

            var scene = new Scene("Ball interaction, large scene II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005,
                SceneViewMode.MaintainFocusInVicinityOfPoint, -0.5, -0.5, 9.5, 9.5, 0.25, 0.25, 0.5, 0.5);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            scene.AddEnclosureOfHalfPlanes(-0.3, 9.3, -0.3, 9.3);

            var boxWidth = 0.4;
            var boxHeight = 0.3;
            Enumerable.Range(0, 10).ToList().ForEach(x =>
            {
                Enumerable.Range(0, 10).ToList().ForEach(y =>
                {
                    scene.AddRectangularBoundary(x - boxWidth / 2, x + boxWidth / 2, y - boxHeight / 2, y + boxHeight / 2);
                });
            });

            return scene;
        }

        private static Scene GenerateSceneBrickInteraction1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.3, 0.3, 1, true), new Vector2D(1.5, 0.5), new Vector2D(0, 0)));

            var scene = new Scene("Brick interaction I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;
            //scene.AddEnclosureOfHalfPlanes(-1, 3, -0.8, 2.5);
            scene.AddBoundary(new HorizontalLineSegment(1, 0, 2));
            scene.AddBoundary(new VerticalLineSegment(1, 0, 2));
            return scene;
        }

        private static Scene GenerateSceneFountain1()
        {
            var initialState = new State();

            var scene = new Scene("Fountain I (diposal)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.AddBoundary(new HalfPlane(new Vector2D(3, 3), new Vector2D(0, -1)));

            var random = new Random(0);

            var extraBodies = Enumerable.Range(1, 1000)
                .Select(i => new
                {
                    StateIndex = i * 50,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(1, 2.85), new Vector2D(2 * random.NextDouble() - 1, -8))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports.Select(bcr => bcr.Body.Id));
                }

                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneFountain2()
        {
            var initialState = new State();

            var scene = new Scene("Fountain II (blocking)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.AddBoundary(new HalfPlane(new Vector2D(3, 3), new Vector2D(0, -1)));

            var random = new Random(0);

            var extraBodies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * 200,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(1, 2.85), new Vector2D(2 * random.NextDouble() - 1, -8))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneFountain3()
        {
            var initialState = new State();

            var scene = new Scene("Fountain III (blocking)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            Enumerable.Range(1, 10).ToList().ForEach(i =>
            {
                initialState.AddBodyState(new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(i * 0.25, 2.9), new Vector2D(0, 0)));
            });

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.AddBoundary(new HalfPlane(new Vector2D(3, 3), new Vector2D(0, -1)));

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (propagatedState.Index == 10)
                {
                    propagatedState.AddBodyState(new BodyState(new CircularBody(11, 0.1, 1, true), new Vector2D(0, 2.85), new Vector2D(0, -8)));
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneFountain4()
        {
            var initialState = new State();

            var scene = new Scene("Fountain IV (bouncing)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            scene.AddBoundary(new HalfPlane(new Vector2D(3, 3), new Vector2D(0, -1)));

            var random = new Random(0);

            var extraBodies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * 200,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(1, 2.85), new Vector2D(2 * random.NextDouble() - 1, -8))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneFountain5()
        {
            var initialState = new State();

            var scene = new Scene("Fountain V (limited lifetime)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            var random = new Random(0);

            var extraBodies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * 200,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(1, 2.85), new Vector2D(2 * random.NextDouble() - 1, -8))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            var temp = new Dictionary<int, int>
            {
                { 700, 1 },
                { 900, 2 },
                { 1100, 3 },
                { 1300, 4 },
                { 1500, 5 },
                { 1700, 6 },
                { 1900, 7 },
                { 2100, 8 },
                { 2300, 9 },
                { 2500, 10 }
            };

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (temp.ContainsKey(propagatedState.Index))
                {
                    var bodyState = propagatedState.TryGetBodyState(temp[propagatedState.Index]);
                    propagatedState?.RemoveBodyState(bodyState);
                }

                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneFountain6()
        {
            var initialState = new State();

            var scene = new Scene("Fountain VI (fireworks)", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.002);

            Dictionary<int, IEnumerable<int>> split = null;
            Dictionary<int, IEnumerable<int>> die = null;
            var nextBodyId = 11;

            scene.InitializationCallback = (state, message) =>
            {
                split = new Dictionary<int, IEnumerable<int>>
                {
                    { 700, new int [1]{1} },
                    { 900, new int [1]{2} },
                    { 1100, new int [1]{3} },
                    { 1300, new int [1]{4} },
                    { 1500, new int [1]{5} },
                    { 1700, new int [1]{6} },
                    { 1900, new int [1]{7} },
                    { 2100, new int [1]{8} },
                    { 2300, new int [1]{9} },
                    { 2500, new int [1]{10} }
                };

                die = new Dictionary<int, IEnumerable<int>>();

                nextBodyId = 11;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;

            var random = new Random(0);

            var extraBodies = Enumerable.Range(1, 10)
                .Select(i => new
                {
                    StateIndex = i * 200,
                    BodyState = new BodyState(new CircularBody(i, 0.1, 1, true), new Vector2D(1, 2.85), new Vector2D(2 * random.NextDouble() - 1, -8))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            var nFragments = 16;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                if (split.ContainsKey(propagatedState.Index))
                {
                    split[propagatedState.Index].ToList().ForEach(id => 
                    {
                        var bodyState = propagatedState.TryGetBodyState(id);

                        if (bodyState != null)
                        {
                            propagatedState.RemoveBodyState(bodyState);

                            var newBodyIds = new List<int>();

                            Enumerable.Range(1, nFragments).ToList().ForEach(i =>
                            {
                                var angle = 2.0 * Math.PI * i / nFragments;
                                var velocity = 2 * new Vector2D(Math.Cos(angle), Math.Sin(angle));

                                propagatedState.AddBodyState(new BodyState(new CircularBody(nextBodyId, 0.05, 0.2, true), bodyState.Position, velocity));
                                newBodyIds.Add(nextBodyId);
                                nextBodyId++;
                            });

                            die[propagatedState.Index + 200] = newBodyIds;
                        }
                    });
                }

                if (die.ContainsKey(propagatedState.Index))
                {
                    die[propagatedState.Index].ToList().ForEach(id =>
                    {
                        var bodyState = propagatedState.TryGetBodyState(id);

                        if (bodyState != null)
                        {
                            propagatedState.RemoveBodyState(bodyState);
                        }
                    });
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneBouncingBall1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0), new Vector2D(0, -5)));

            var scene = new Scene("Bouncing ball I", 120.0, new Point2D(-2, -3), initialState, 9.82, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HalfPlane(new Vector2D(1, 0.1), new Vector2D(0, -1)));
            return scene;
        }

        private static Scene GenerateSceneBouncingBall2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(2, 0)));

            //var scene = new Scene("Bouncing ball II", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.0003);
            var scene = new Scene("Bouncing ball II", 120.0, new Point2D(-1.4, -1.3), initialState, 9.82, 0, 0, 1, false, 0.001);
            scene.FinalStateIndex = 700;

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddBoundary(new HalfPlane(new Vector2D(3, -0.3), new Vector2D(-1, 0)));
            scene.AddBoundary(new HalfPlane(new Vector2D(3, 1), new Vector2D(0, -1)));
            scene.AddBoundary(new HalfPlane(new Vector2D(-1, 1), new Vector2D(1, 0)));
            return scene;
        }

        private static Scene GenerateScenePoolTableWithOneBall()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(2, 1)));

            var scene = new Scene("Pool table, 1 ball", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateScenePoolTableWithOneBallAndThreeBoundaryPoints()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(2, 1)));

            var scene = new Scene("Pool table, 1 ball, 3 boundary points", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddBoundary(new BoundaryPoint(new Vector2D(1, 0.35)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(0, 0.35)));
            scene.AddBoundary(new BoundaryPoint(new Vector2D(2, 0.35)));
            return scene;
        }

        private static Scene GenerateScenePoolTableWithTwoBallsAnd1LineSegment()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(2, 1)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(1, 0.7), new Vector2D(2, 1)));

            var scene = new Scene("Pool table, 2 balls, 1 boundary line segment", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddBoundary(new LineSegment(new Vector2D(-0.95, 0.35), new Vector2D(2.95, 0.35)));
            return scene;
        }

        private static Scene GenerateScenePoolTableWith1BallAnd1LineSegment()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(2, 1)));

            var scene = new Scene("Pool table, 1 ball, 1 boundary line segment", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            scene.AddBoundary(new LineSegment(new Vector2D(0, 0.35), new Vector2D(2, 0.35)));
            return scene;
        }

        private static Scene GenerateScenePoolTableWithTwoBalls()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 0), new Vector2D(2, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(2, 0), new Vector2D(0, 0)));

            var scene = new Scene("Pool table, 2 balls", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateSceneSimultaneousCollisionsWithBoundary()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(-0.8, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.1, 1, true), new Vector2D(-0.5, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.1, 1, true), new Vector2D(-0.2, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(4, 0.1, 1, true), new Vector2D(0.1, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(5, 0.1, 1, true), new Vector2D(0.4, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(6, 0.1, 1, true), new Vector2D(0.7, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(7, 0.1, 1, true), new Vector2D(1.0, 0), new Vector2D(0.2, -1)));
            initialState.AddBodyState(new BodyState(new CircularBody(8, 0.1, 1, true), new Vector2D(1.3, 0), new Vector2D(0.2, -1)));

            var scene = new Scene("Simultaneous collisions with boundary", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateSceneNewtonsCradle1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0), new Vector2D(3, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.1, 1, true), new Vector2D(1, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.1, 1, true), new Vector2D(1.2, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(4, 0.1, 1, true), new Vector2D(1.4, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(5, 0.1, 1, true), new Vector2D(1.6, 0), new Vector2D(0, 0)));

            var scene = new Scene("Newtons cradle I", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateSceneNewtonsCradle2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0), new Vector2D(3, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.1, 1, true), new Vector2D(0.2, 0), new Vector2D(3, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.1, 1, true), new Vector2D(1.2, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(4, 0.1, 1, true), new Vector2D(1.4, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(5, 0.1, 1, true), new Vector2D(1.6, 0), new Vector2D(0, 0)));

            var scene = new Scene("Newtons cradle II", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateScenePoolTableWithThreeBalls()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 0), new Vector2D(2, 1)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(0, 0.3), new Vector2D(1, 2)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.125, 1, true), new Vector2D(2, 0.6), new Vector2D(-1, 1)));

            var scene = new Scene("Pool table, 3 balls", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateScenePoolTableWithThreeBallsAndFriction()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 0), new Vector2D(4, 1.5)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 0.125, 1, true), new Vector2D(0, 0.3), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 0.125, 1, true), new Vector2D(2, 0.6), new Vector2D(0, 0)));

            var scene = new Scene("Pool table, 3 balls, friction", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0.5, 1, true, 0.001);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -0.3, 1);
            return scene;
        }

        private static Scene GenerateSceneOrbit1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 30, 5000, true), new Vector2D(300, 300), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 10, 20, true), new Vector2D(200, 300), new Vector2D(0, -1000)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 10, 20, true), new Vector2D(400, 300), new Vector2D(0, 1000)));

            var random = new Random(0);

            //Enumerable.Range(4, 3).ToList().ForEach(id =>
            //{
            //    var posX = random.Next(200, 400);
            //    var posY = random.Next(200, 400);
            //    var velocityX = random.Next(-100, 100);
            //    var velocityY = random.Next(-100, 100);

            //    initialState.AddBodyState(
            //        new BodyState(new CircularBody(id, 10, 20, true), new Vector2D(posX, posY), new Vector2D(velocityX, velocityY)));
            //});

            var scene = new Scene("Orbit I", 1.0, new Point2D(20, 115), initialState, 0, 1.5E4, 0, 0.5, false, 0.005, SceneViewMode.FocusOnCenterOfMass);
            return scene;
        }

        private static Scene GenerateSceneOrbit2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 30, 5000, true), new Vector2D(300, 300), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 10, 20, true), new Vector2D(200, 300), new Vector2D(0, -1000)));
            initialState.AddBodyState(new BodyState(new CircularBody(3, 10, 20, true), new Vector2D(400, 300), new Vector2D(0, 1000)));

            var random = new Random(0);

            Enumerable.Range(4, 200).ToList().ForEach(id =>
            {
                var posX = random.Next(200, 400);
                var posY = random.Next(200, 400);
                var velocityX = random.Next(500, 800);
                var velocityY = random.Next(500, 800);

                initialState.AddBodyState(
                    new BodyState(new CircularBody(id, 10, 20, true), new Vector2D(posX, posY), new Vector2D(velocityX, velocityY)));
            });

            var scene = new Scene("Orbit II", 1.0, new Point2D(20, 115), initialState, 0, 1.5E4, 0, 0.5, false, 0.005, SceneViewMode.FocusOnCenterOfMass);
            return scene;
        }

        private static Scene GenerateSceneMoonAndEarth()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 6371000, 5.972E+24, true), new Vector2D(0, 0), new Vector2D(0, 0)));
            initialState.AddBodyState(new BodyState(new CircularBody(2, 1737100, 7.34767309E+22, true), new Vector2D(385000000, 0), new Vector2D(0, -1022)));

            return new Scene("Moon and Earth", 7E-07, new Point2D(-21E7, -22E7), initialState, 0, 6.674E-11, 0, 315360, false, 360, SceneViewMode.FocusOnCenterOfMass);
        }

        private static Scene GenerateSceneFlappyBird()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(1, -0.1), new Vector2D(1.5, -3)));

            var scene = new Scene("Flappy Bird", 120.0, new Point2D(-1.4, -1.3),
                initialState, 9.82, 0, 0, 1, false, 0.005, SceneViewMode.MaintainFocusInVicinityOfPoint,
                double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, 0, 1E200, 0.25);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                if (boundaryCollisionReports.Any())
                {
                    response.Outcome = boundaryCollisionReports
                        .Any(bcr => bcr.Boundary.Tag == "Exit")
                        ? "You Win"
                        : "Game Over";

                    response.IndexOfLastState = propagatedState.Index + 150;
                }

                return response;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                if (!keyboardState.UpArrowDown) return false;

                currentState.BodyStates.First().NaturalVelocity = new Vector2D(1.5, -3);
                return true;
            };

            var floorLevel = 3;
            var ceilingLevel = -1;

            // Local Function
            void AddObstacleFloor(double x, double width, double height)
            {
                scene.AddRectangularBoundary(x - width / 2, x + width / 2, floorLevel - height, floorLevel);
            }

            // Local Function
            void AddObstacleCeiling(double x, double width, double height)
            {
                scene.AddRectangularBoundary(x - width / 2, x + width / 2, ceilingLevel, ceilingLevel + height);
            }

            // Local Function
            void AddObstacleGate(double x, double width, double gateCenterHeight, double gateHeight)
            {
                AddObstacleFloor(x, width, gateCenterHeight - gateHeight / 2);
                AddObstacleCeiling(x, width, floorLevel - ceilingLevel - gateCenterHeight - gateHeight / 2);
            }

            scene.AddEnclosureOfHalfPlanes(-1, 100, ceilingLevel, floorLevel);

            var xPos = 1;
            AddObstacleFloor(xPos += 2, 1, 1);
            AddObstacleFloor(xPos += 2, 1, 1.5);
            AddObstacleFloor(xPos += 2, 1, 2);
            AddObstacleFloor(xPos += 2, 1, 2.5);
            AddObstacleCeiling(xPos += 2, 1, 1);
            AddObstacleCeiling(xPos += 2, 1, 1.5);
            AddObstacleCeiling(xPos += 2, 1, 2);
            AddObstacleCeiling(xPos += 2, 1, 2.5);
            AddObstacleGate(xPos += 2, 1, 2, 3);
            AddObstacleGate(xPos += 2, 1, 2, 2);
            AddObstacleGate(xPos += 2, 1, 2, 1.5);
            AddObstacleGate(xPos += 3, 1, 3, 1.5);
            AddObstacleGate(xPos += 3, 1, 1, 1.5);

            scene.AddBoundary(new LeftFacingHalfPlane(xPos + 1, "Exit") { Visible = false });
            //scene.AddBoundary(new LeftFacingHalfPlane(2, "Exit") { Visible = false });

            return scene;
        }

        private static Scene GenerateScenePlatformer1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.2, 0.4, 1, true), new Vector2D(3, 0), new Vector2D(0, 0)));

            var scene = new Scene("Platformer I (Ghost'n Goblins style)", 120.0, new Point2D(-1.4, -1.3), initialState, 1.0 * 9.82, 0, 0, 1, false, 0.002,
                SceneViewMode.MaintainFocusInVicinityOfPoint, double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, 0.25, 1E200);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var grounded = false;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                // Find ud af, om figuren står på jorden eller ej
                // Vi vil gerne undersøge, hvad der er sket af kollisioner siden sidst, specielt om player 1 har kollideret med "ground", 
                // hvor effektiv surface normal har været opad
                if (collisions.Count == 0)
                {
                    // No collisions with ground since last refresh - must be due to jump or walking off edge
                    grounded = false;
                }
                else if (
                    !grounded && collisions
                    //collisions.ContainsKey(1) &&
                    //collisions[1].Any(bcr => Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                    .SelectMany(x => x.Value)
                    .Any(bcr => bcr.Body.Id == 1 && Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                {
                    // Yes, collided with ground since last refresh
                    grounded = true;
                }

                // Player 1 er IKKE grounded men i luften. Derfor ændrer vi IKKE dens hastighed (Ghost'n Goblins style)
                if (!grounded) return false;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;
                var newArtificialVelocity = new Vector2D(0, 0);
                var horizontalSpeed = 1.5;

                if (keyboardState.RightArrowDown)
                {
                    newArtificialVelocity += new Vector2D(horizontalSpeed, 0);
                }
                else if (keyboardState.LeftArrowDown)
                {
                    newArtificialVelocity += new Vector2D(-horizontalSpeed, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    currentStateOfMainBody.NaturalVelocity = new Vector2D(0, -4);
                    grounded = false;
                    return true;
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            scene.AddBoundary(new UpFacingHalfPlane(1));
            scene.AddBoundary(new HorizontalLineSegment(0.5, 2, 4));
            scene.AddBoundary(new HorizontalLineSegment(0, 5, 6));
            scene.AddBoundary(new HorizontalLineSegment(-0.5, 7, 10));

            return scene;
        }

        private static Scene GenerateScenePlatformer2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.2, 0.4, 1, true), new Vector2D(3, 0), new Vector2D(0, 0)));

            var scene = new Scene("Platformer (Moving while jumping)", 120.0, new Point2D(-1.4, -1.3), initialState, 1.0 * 9.82, 0, 0, 1, false, 0.002,
                SceneViewMode.MaintainFocusInVicinityOfPoint, double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, 0.25, 1E200);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var grounded = false;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                // Find ud af, om figuren står på jorden eller ej
                // Vi vil gerne undersøge, hvad der er sket af kollisioner siden sidst, specielt om player 1 har kollideret med "ground", 
                // hvor effektiv surface normal har været opad
                if (collisions.Count == 0)
                {
                    // No collisions with ground since last refresh - must be due to jump or walking off edge
                    grounded = false;
                }
                else if (
                    !grounded && collisions
                    //collisions.ContainsKey(1) &&
                    //collisions[1].Any(bcr => Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                    .SelectMany(x => x.Value)
                    .Any(bcr => bcr.Body.Id == 1 && Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                {
                    // Yes, collided with ground since last refresh
                    grounded = true;
                }

                // Player 1 er IKKE grounded men i luften. Derfor ændrer vi IKKE dens hastighed (Ghost'n Goblins style)
                //if (!grounded) return false;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;
                var newArtificialVelocity = new Vector2D(0, 0);
                var horizontalSpeed = 1.5;

                if (keyboardState.RightArrowDown)
                {
                    newArtificialVelocity += new Vector2D(horizontalSpeed, 0);
                }
                else if (keyboardState.LeftArrowDown)
                {
                    newArtificialVelocity += new Vector2D(-horizontalSpeed, 0);
                }

                if (keyboardState.UpArrowDown && grounded)
                {
                    currentStateOfMainBody.NaturalVelocity = new Vector2D(0, -4);
                    grounded = false;
                    return true;
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            scene.AddBoundary(new UpFacingHalfPlane(1));
            scene.AddBoundary(new HorizontalLineSegment(0.5, 2, 4));
            scene.AddBoundary(new HorizontalLineSegment(0, 5, 6));
            scene.AddBoundary(new HorizontalLineSegment(-0.5, 7, 10));

            return scene;
        }

        private static Scene GenerateScenePlatformer3()
        {
            var basicArtificialVelocity = new Vector2D(-0.5, 0);

            var initialState = new State();
            var bodyState = new BodyState(new RectangularBody(1, 0.2, 0.4, 1, true), new Vector2D(3, 0), new Vector2D(0, 0));
            bodyState.ArtificialVelocity = basicArtificialVelocity;
            initialState.AddBodyState(bodyState);

            var scene = new Scene("Platformer (Storm)", 120.0, new Point2D(-1.4, -1.3), initialState, 1.0 * 9.82, 0, 0, 1, false, 0.002,
                SceneViewMode.MaintainFocusInVicinityOfPoint, double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, 0.25, 1E200);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var grounded = false;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                // Find ud af, om figuren står på jorden eller ej
                // Vi vil gerne undersøge, hvad der er sket af kollisioner siden sidst, specielt om player 1 har kollideret med "ground", 
                // hvor effektiv surface normal har været opad
                if (collisions.Count == 0)
                {
                    // No collisions with ground since last refresh - must be due to jump or walking off edge
                    grounded = false;
                }
                else if (
                    !grounded && collisions
                    //.ContainsKey(1) &&
                    //collisions[1].Any(bcr => Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                    .SelectMany(x => x.Value)
                    .Any(bcr => bcr.Body.Id == 1 && Math.Abs(bcr.EffectiveSurfaceNormal.Y + 1) < 0.000001))
                {
                    // Yes, collided with ground since last refresh
                    grounded = true;
                }

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;
                var newArtificialVelocity = basicArtificialVelocity;
                var horizontalSpeed = 1.5;

                if (keyboardState.RightArrowDown)
                {
                    newArtificialVelocity += new Vector2D(horizontalSpeed, 0);
                }
                else if (keyboardState.LeftArrowDown)
                {
                    newArtificialVelocity += new Vector2D(-horizontalSpeed, 0);
                }

                if (keyboardState.UpArrowDown && grounded)
                {
                    currentStateOfMainBody.NaturalVelocity = new Vector2D(0, -4);
                    grounded = false;
                    return true;
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            scene.AddBoundary(new UpFacingHalfPlane(1));
            scene.AddBoundary(new HorizontalLineSegment(0.5, 2, 4));
            scene.AddBoundary(new HorizontalLineSegment(0, 5, 6));
            scene.AddBoundary(new HorizontalLineSegment(-0.5, 7, 10));

            return scene;
        }

        private static Scene GenerateSceneCollisionRegistrationTest()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new RectangularBody(1, 0.3, 0.3, 1, false), new Vector2D(-0.000000001, 0.000000000), new Vector2D(1.001, 1.001)));

            var scene = new Scene("Collision Registration Test", 120.0, new Point2D(-1.4, -1.3), initialState, 0.1 * 9.82, 0, 0, 1, false, 0.005, SceneViewMode.Stationary,
                double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, 0, 1E200, 0.25);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body =>
            {
                return OutcomeOfCollisionBetweenBodyAndBoundary.Reflect;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 1, -1, 1);

            return scene;
        }

        private static Scene GenerateSceneShootEmUp1()
        {
            // Intentionen her er, at der skal fyres et shot af, hver gang man trykker space-tasten.
            // Der fyres imidlertid shots af, hver gang man ændrer keyboard-tilstand og der i øvrigt gælder,
            // at space er trykket ned.

            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up I (Semi-automatic fire, shots not collected)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            //var currentKeyboardState = new Keyboard();
            var spaceKeyWasPressed = false;

            // Denne callback funktion er som udgangspunkt identisk med DungeonCrawler8Directions,
            // men vi har alligevel brug for at customize den, for at gemme information om hvorvidt space blev trykket ned
            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyWasPressed = keyboardEvents.SpaceDown && keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 3;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !spaceKeyWasPressed)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            var nextBodyId = 2;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (spaceKeyWasPressed)
                {
                    spaceKeyWasPressed = false;

                    propagatedState.AddBodyState(new BodyState(
                        new CircularBody(nextBodyId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -5)));
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneShootEmUp2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 1, true), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up II (Semi-automatic fire, shots collected)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var spaceKeyWasPressed = false;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyWasPressed = keyboardEvents.SpaceDown && keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 3;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !spaceKeyWasPressed)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            var nextBodyId = 1000;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (boundaryCollisionReports.Any())
                {
                    var idsOfDisposableBodies = boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id)
                        .ToArray();

                    if (idsOfDisposableBodies.Any())
                    {
                        propagatedState.RemoveBodyStates(idsOfDisposableBodies);
                    }
                }

                if (spaceKeyWasPressed)
                {
                    spaceKeyWasPressed = false;

                    propagatedState.AddBodyState(new BodyState(
                        new Projectile(nextBodyId++, 0.05, 1, true), propagatedState.BodyStates.First().Position + new Vector2D(0, -0.126), new Vector2D(0, -5)));
                }

                return new PostPropagationResponse();
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -1, 3);

            return scene;
        }

        private static Scene GenerateSceneShootEmUp3()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 1, true), new Vector2D(0, 0), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up III (Jitter)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var left = false;
            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                currentState.BodyStates.First().ArtificialVelocity = new Vector2D(left ? -3 : 3, 0);
                left = !left;

                return true;
            };

            var nextBodyId = 1000;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                if (propagatedState.Index % 2 == 0)
                {
                    propagatedState.AddBodyState(new BodyState(
                        new Projectile(nextBodyId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -1)));
                }

                return new PostPropagationResponse();
            };

            scene.AddBoundary(new HalfPlane(new Vector2D(-10, -1), new Vector2D(0, 1)));

            return scene;
        }

        private static Scene GenerateSceneShootEmUp4()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 1, true), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up IV (continuous fire)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var spaceKeyIsDown = false;
            var rateOfFire = 30;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                // Hvis vi er her, kommer vi til at invalidere fremtidige states, 

                if (keyboardEvents.SpaceDown)
                {
                    if (keyboardState.SpaceDown)
                    {
                        // Space blev trykket ned - så skal vi muligvis påbegynde en ny salve

                        // Indtil videre så påbegynder vi bare en ny, men det indebærer, at man kan skyde hurtigere end rateOfFire,
                        // hvilket tilskynder spilleren til at tæske løs på space tasten

                        // Vi skal kun gøre dette, HVIS man er færdig med en given salve, og det er man vel, hvis man når frem til et state index,
                        // hvor der egentlig skulle løsnes et skud, men hvor space ikke er trykket ned..

                        //Console.WriteLine($"Space pressed at state index: {currentState.Index}");

                        if (currentState.Index > stateIndexOfLastShotInBurst + rateOfFire) // Off by one?
                        {
                            stateIndexOfFirstShotInBurst = currentState.Index + 1;
                            //Console.WriteLine($"First projectile of autofire inserted at {stateIndexOfFirstShotInBurst}");
                        }
                        else
                        {
                            //Console.WriteLine("(continuing ongoing burst)");
                        }

                        // Det er vel for helvede fint nok, hvis bare vi sætter den til noget "SMART", f.eks. i henhold til scenariet:
                        // 1) Space trykkes ned, når current index er 32. Derfor invalideres state 33 og opefter, og vi planter skud i state 33, 133, 233, 333 osv
                        // 2) Space slippes, når current index er 56. Derfor invalideres state 57 og opefter, og der plantes ingen skud
                        // 3) Space trykkes ned, når current index er 147. Derfor invalideres state 148 og opefter, MEN vi planter IKKE skud
                        //    fra state 148, FORDI der jo altså er plantet et skud i state 133, så vi skal prøve at identificere index for den seneste state,
                        //    hvor der er plantet et skud. Så vi tager udgangspunkt i stateIndexOfFirstShotInBurst (dvs 33), som vi så sammenholder med
                        //    index of current state (147), så vi lægger rateOfFire til så mange gange vi kan uden at komme over current state.
                        //    Det giver så state 133, og så ved vi, at vi skal plante første skud i state 233
                    }
                    else
                    {
                        // Space blev sluppet

                        // her skulle vi gerne kunne beregne index for det SIDSTE skud, der blev løsnet..
                        stateIndexOfLastShotInBurst = stateIndexOfFirstShotInBurst;
                        while (stateIndexOfLastShotInBurst < currentState.Index)  // Off by one?
                        {
                            stateIndexOfLastShotInBurst += rateOfFire;
                        }

                        stateIndexOfLastShotInBurst -= rateOfFire;

                        //Console.WriteLine($"Last projectile of autofire inserted at {stateIndexOfLastShotInBurst}");
                    }
                }

                return true;
            };

            // Husk, at denne kaldes af BEREGNEREN, der jo altså som regel er langt foran current state
            // .. giver det overhovedet mening at bruge variablen spaceKeyIsDown her? (ja)
            var nextProjectileId = 1000;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                // Remove projectile?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                // Add a projectile from main body?
                if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -4)));
                }

                return new PostPropagationResponse();
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -1, 10);

            return scene;
        }

        private static Scene GenerateSceneShootEmUp5()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 1, false), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up IV (with enemies)", 120.0, new Point2D(-1.4, -1.3), initialState, 1, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => body is Enemy
                ? OutcomeOfCollisionBetweenBodyAndBoundary.Reflect
                : OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }

                    if (body1 is Player1Circular || body2 is Player1Circular)
                    {
                        return true;
                    }
                }

                return false;
            };

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.Block;

            var spaceKeyIsDown = false;
            var rateOfFire = 30;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                // Hvis vi er her, kommer vi til at invalidere fremtidige states, 

                if (keyboardEvents.SpaceDown)
                {
                    if (keyboardState.SpaceDown)
                    {
                        if (currentState.Index > stateIndexOfLastShotInBurst + rateOfFire) // Off by one?
                        {
                            stateIndexOfFirstShotInBurst = currentState.Index + 1;
                        }
                    }
                    else
                    {
                        // Space blev sluppet

                        // her skulle vi gerne kunne beregne index for det SIDSTE skud, der blev løsnet..
                        stateIndexOfLastShotInBurst = stateIndexOfFirstShotInBurst;
                        while (stateIndexOfLastShotInBurst < currentState.Index)  // Off by one?
                        {
                            stateIndexOfLastShotInBurst += rateOfFire;
                        }

                        stateIndexOfLastShotInBurst -= rateOfFire;
                    }
                }

                return true;
            };

            var extraBodies = Enumerable.Range(2, 100)
                .Select(i => new
                {
                    StateIndex = i * 10,
                    BodyState = new BodyState(new Enemy(i, 0.15, 1, true), new Vector2D(-0.8, -0.8), new Vector2D(2.0, 0.4))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            var nextProjectileId = 1000;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                // Remove projectile?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Player1Circular || bcr.Body2 is Player1Circular)
                    {
                        // Player collided with enemy, so game over
                        response.IndexOfLastState = propagatedState.Index;
                        response.Outcome = "Game Over";
                    }
                    else if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with enemy, so remove both
                        propagatedState.RemoveBodyStates(new List<int> { bcr.Body1.Id, bcr.Body2.Id });

                        if (!propagatedState.BodyStates.Any(bs => bs.Body is Enemy))
                        {
                            // All enemies are dead, so player wins
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "You Win";
                        }
                    }
                });

                // Add a projectile from main body?
                if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -4)));
                }

                // Add an enemy?
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return response;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -1, 3);

            return scene;
        }

        private static Scene GenerateSceneShootEmUp6()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 1, true), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Shoot 'em up VI (continuous fire, limited projectile lifetime)", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005);

            var spaceKeyIsDown = false;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;
            var nextProjectileId = 1000;
            var temp = new Dictionary<int, int>();
            var rateOfFire = 30;

            scene.InitializationCallback = (state, message) =>
            {
                spaceKeyIsDown = false;
                stateIndexOfFirstShotInBurst = -1000;
                stateIndexOfLastShotInBurst = -1000;
                nextProjectileId = 1000;
                temp.Clear();
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

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

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                // Remove projectile?
                if (temp.ContainsKey(propagatedState.Index))
                {
                    var projectile = propagatedState.TryGetBodyState(temp[propagatedState.Index]);
                    propagatedState?.RemoveBodyState(projectile);
                }

                // Add a projectile from main body?
                if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    nextProjectileId++;
                    temp[propagatedState.Index + 100] = nextProjectileId;
                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -4)));
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneShootEmUp7()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 999999, false), new Vector2D(2, 2.5), new Vector2D(0, 0)) { Life = 10 });

            var scene = new Scene("Shoot 'em up VII (with enemies)", 120.0, new Point2D(-1.4, -1.3), initialState, 1, 0, 0, 1, true, 0.005);

            var rateOfFire = 20;
            var nFragments = 8;
            var spaceKeyIsDown = false;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;
            var nextEnemyId = 1000;
            var nextProjectileId = 5000;

            scene.InitializationCallback = (state, message) =>
            {
                spaceKeyIsDown = false;
                stateIndexOfFirstShotInBurst = -1000;
                stateIndexOfLastShotInBurst = -1000;
                nextEnemyId = 1000;
                nextProjectileId = 5000;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => body is Enemy
                ? OutcomeOfCollisionBetweenBodyAndBoundary.Reflect
                : OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }

                    if (body1 is Player1Circular || body2 is Player1Circular)
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

                    if (body1 is Player1Circular || body2 is Player1Circular)
                    {
                        return OutcomeOfCollisionBetweenTwoBodies.Ignore;
                    }
                }

                return OutcomeOfCollisionBetweenTwoBodies.Block;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                // Hvis vi er her, kommer vi til at invalidere fremtidige states, 

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
                        // Space blev sluppet

                        // her skulle vi gerne kunne beregne index for det SIDSTE skud, der blev løsnet..
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

            var extraBodies = Enumerable.Range(2, 15)
                .Select(i => new
                {
                    StateIndex = i * 50,
                    BodyState = new BodyState(new Enemy(i, 0.15, 1, true), new Vector2D(-0.8, -0.8), new Vector2D(2.0, 0.4)) { Life = 5 }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                // Remove projectile?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                var hitEnemies = new HashSet<BodyState>();

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Player1Circular || bcr.Body2 is Player1Circular)
                    {
                        // Player collided with enemy
                        var damage =
                            (bcr.Body1 is Player1Circular && bcr.Body2.Mass > 0.5 ||
                             bcr.Body2 is Player1Circular && bcr.Body1.Mass > 0.5)
                             ? 3.0
                             : 1.0;

                        var player = propagatedState.BodyStates.First();
                        player.Life -= damage;

                        if (player.Life < 0.5)
                        {
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "Game Over";
                        }
                    }
                    else if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with enemy
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
                            Enumerable.Range(1, nFragments).ToList().ForEach(i =>
                            {
                                var angle = 0.125 * Math.PI + 2.0 * Math.PI * i / nFragments;
                                var velocity = 3.0 * new Vector2D(Math.Cos(angle), Math.Sin(angle));

                                propagatedState.AddBodyState(new BodyState(new Enemy(nextEnemyId++, 0.1, 0.1, true), e.Position, velocity));
                            });
                        }

                        if (!propagatedState.BodyStates.Any(bs => bs.Body is Enemy))
                        {
                            // All enemies are dead, so player wins
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "You Win";
                        }
                    }
                });

                // Add a projectile from main body?
                if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -4)));
                }

                // Add an enemy?
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return response;
            };

            scene.AddRectangularBoundary(-1, 3, -1, 3);

            scene.AddRectangularBoundary(1, 2, 2, 2.1);

            return scene;
        }

        private static Scene GenerateSceneShootEmUp8()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.125, 999999, false), new Vector2D(1, 2), new Vector2D(0, 0)) { Life = 10 });

            var scene = new Scene("Shoot 'em up VIII (automatic)", 120.0, new Point2D(-1.4, -1.3), initialState, 1, 0, 0, 1, true, 0.005);

            var rateOfFire = 20;
            var nFragments = 8;
            var spaceKeyIsDown = false;
            var stateIndexOfFirstShotInBurst = -1000;
            var stateIndexOfLastShotInBurst = -1000;
            var nextEnemyId = 1000;
            var nextProjectileId = 5000;

            scene.InitializationCallback = (state, message) =>
            {
                spaceKeyIsDown = false;
                stateIndexOfFirstShotInBurst = -1000;
                stateIndexOfLastShotInBurst = -1000;
                nextEnemyId = 1000;
                nextProjectileId = 5000;
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => body is Enemy
                ? OutcomeOfCollisionBetweenBodyAndBoundary.Reflect
                : OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
            {
                if (body1 is Enemy || body2 is Enemy)
                {
                    if (body1 is Projectile || body2 is Projectile)
                    {
                        return true;
                    }

                    if (body1 is Player1Circular || body2 is Player1Circular)
                    {
                        return true;
                        //return false;
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

                    if (body1 is Player1Circular || body2 is Player1Circular)
                    {
                        return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
                    }
                }

                return OutcomeOfCollisionBetweenTwoBodies.Block;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyIsDown = keyboardState.SpaceDown;

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !keyboardEvents.SpaceDown)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                // Hvis vi er her, kommer vi til at invalidere fremtidige states, 

                if (keyboardEvents.SpaceDown)
                {
                    if (keyboardState.SpaceDown)
                    {
                        if (currentState.Index > stateIndexOfLastShotInBurst + rateOfFire) // Off by one?
                        {
                            stateIndexOfFirstShotInBurst = currentState.Index + 1;
                        }
                    }
                    else
                    {
                        // Space blev sluppet

                        // her skulle vi gerne kunne beregne index for det SIDSTE skud, der blev løsnet..
                        stateIndexOfLastShotInBurst = stateIndexOfFirstShotInBurst;
                        while (stateIndexOfLastShotInBurst < currentState.Index)  // Off by one?
                        {
                            stateIndexOfLastShotInBurst += rateOfFire;
                        }

                        stateIndexOfLastShotInBurst -= rateOfFire;
                    }
                }

                return true;
            };

            //var extraBodies = Enumerable.Range(2, 4)
            var extraBodies = Enumerable.Range(2, 10)
                .Select(i => new
                {
                    //StateIndex = i * 50,
                    StateIndex = i * 20,
                    BodyState = new BodyState(new Enemy(i, 0.15, 1, true), new Vector2D(-0.8, -0.8), new Vector2D(2.0, 0.4)) { Life = 5 }
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                // Remove projectile?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                var hitEnemies = new HashSet<BodyState>();

                bodyCollisionReports.ForEach(bcr =>
                {
                    if (bcr.Body1 is Player1Circular || bcr.Body2 is Player1Circular)
                    {
                        // Player collided with enemy, so game over
                        //response.IndexOfLastState = propagatedState.Index;
                        //response.Outcome = "Game Over";
                    }
                    else if (bcr.Body1 is Projectile || bcr.Body2 is Projectile)
                    {
                        // Projectile collided with enemy
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
                            Enumerable.Range(1, nFragments).ToList().ForEach(i =>
                            {
                                var angle = 0.125 * Math.PI + 2.0 * Math.PI * i / nFragments;
                                var velocity = 3.0 * new Vector2D(Math.Cos(angle), Math.Sin(angle));

                                propagatedState.AddBodyState(new BodyState(new Enemy(nextEnemyId++, 0.1, 0.1, true), e.Position, velocity));
                            });
                        }

                        if (!propagatedState.BodyStates.Any(bs => bs.Body is Enemy))
                        {
                            // All enemies are dead, so player wins
                            response.IndexOfLastState = propagatedState.Index;
                            response.Outcome = "You Win";
                        }
                    }
                });

                // Add a projectile from main body?
                //if (spaceKeyIsDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                if ((propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    propagatedState.AddBodyState(new BodyState(new Projectile(nextProjectileId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, new Vector2D(0, -4)));
                }

                // Add an enemy?
                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return response;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -1, 3);

            return scene;
        }

        private static Scene GenerateSceneDodgeball()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Circular(1, 0.075, 1, true), new Vector2D(1, 2), new Vector2D(0, 0)));

            var scene = new Scene("Dodgeball", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, true, 0.005);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => body is Enemy
                ? OutcomeOfCollisionBetweenBodyAndBoundary.Reflect
                : OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                if (body1 is Player1Circular || body2 is Player1Circular)
                {
                    return OutcomeOfCollisionBetweenTwoBodies.Block;
                }

                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    newArtificialVelocity = speed * newMovementDirection.Normalize();
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            scene.AddEnclosureOfHalfPlanes(-1, 3, -1, 3);

            var extraBodies = Enumerable.Range(2, 15)
                .Select(i => new
                {
                    StateIndex = i * 80,
                    BodyState = new BodyState(new Enemy(i, 0.15, 1, true), new Vector2D(-0.8, -0.8), new Vector2D(2.0, 0.4))
                })
                .ToDictionary(x => x.StateIndex, x => x.BodyState);

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                if (bodyCollisionReports.Any(bcr => bcr.Body1 is Player1Circular || bcr.Body2 is Player1Circular))
                {
                    response.IndexOfLastState = propagatedState.Index;
                    response.Outcome = "Game Over";
                }

                if (extraBodies.ContainsKey(propagatedState.Index))
                {
                    propagatedState.AddBodyState(extraBodies[propagatedState.Index]);
                }

                return response;
            };

            return scene;
        }

        private static Scene GenerateSceneRambo()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new Player1Rectangular(1, 0.25, 0.25, 1, true), new Vector2D(0, 1), new Vector2D(0, 0)));

            var scene = new Scene("Rambo", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.005,
                SceneViewMode.MaintainFocusInVicinityOfPoint, -0.6, -0.6, 9.6, 9.6);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            var currentKeyboardState = new KeyboardState();
            var spaceKeyWasPressed = false;
            var lastMovementDirection = new Vector2D(0, -1);

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                spaceKeyWasPressed = keyboardState.SpaceDown && !currentKeyboardState.SpaceDown;
                currentKeyboardState = keyboardState.Clone();

                var currentStateOfMainBody = currentState.BodyStates.First();
                var currentArtificialVelocity = currentStateOfMainBody.ArtificialVelocity;

                var newMovementDirection = new Vector2D(0, 0);

                if (keyboardState.LeftArrowDown)
                {
                    newMovementDirection += new Vector2D(-1, 0);
                }

                if (keyboardState.RightArrowDown)
                {
                    newMovementDirection += new Vector2D(1, 0);
                }

                if (keyboardState.UpArrowDown)
                {
                    newMovementDirection += new Vector2D(0, -1);
                }

                if (keyboardState.DownArrowDown)
                {
                    newMovementDirection += new Vector2D(0, 1);
                }

                var newArtificialVelocity = new Vector2D(0, 0);

                if (newMovementDirection.Length > 0.01)
                {
                    var speed = 2;
                    lastMovementDirection = newMovementDirection.Normalize();
                    newArtificialVelocity = speed * lastMovementDirection;
                }

                if ((newArtificialVelocity - currentArtificialVelocity).Length < 0.01 && !spaceKeyWasPressed)
                {
                    return false;
                }

                currentStateOfMainBody.ArtificialVelocity = newArtificialVelocity;

                return true;
            };

            var nextBodyId = 1000;
            var stateIndexOfFirstShotInBurst = 0;
            var projectileSpeed = 5;
            var rateOfFire = 25;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                // Remove projectile?
                if (boundaryCollisionReports.Any())
                {
                    propagatedState.RemoveBodyStates(boundaryCollisionReports
                        .Where(bcr => bcr.Body is Projectile)
                        .Select(bcr => bcr.Body.Id));
                }

                if (spaceKeyWasPressed)
                {
                    stateIndexOfFirstShotInBurst = propagatedState.Index;
                    spaceKeyWasPressed = false;
                }

                if (currentKeyboardState.SpaceDown && (propagatedState.Index - stateIndexOfFirstShotInBurst) % rateOfFire == 0)
                {
                    propagatedState.AddBodyState(new BodyState(
                        new Projectile(nextBodyId++, 0.05, 1, true), propagatedState.BodyStates.First().Position, projectileSpeed * lastMovementDirection));
                }

                return new PostPropagationResponse();
            };

            scene.AddEnclosureOfHalfPlanes(-0.5, 9.5, -0.5, 9.5);

            var boxWidth = 0.95;
            var boxHeight = 0.95;
            var index = 0;
            var random = new Random(0);
            Enumerable.Range(0, 10).ToList().ForEach(x =>
            {
                Enumerable.Range(0, 10).ToList().ForEach(y =>
                {
                    if (index == 0 || index == 11 || random.Next(0, 100) < 25)
                    {
                        scene.AddRectangularBoundary(x - boxWidth / 2, x + boxWidth / 2, y - boxHeight / 2, y + boxHeight / 2);
                    }

                    index++;
                });
            });

            return scene;
        }

        private static Scene GenerateSceneMultipleOutcomes()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Multiple outcomes", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                if (boundaryCollisionReports.Any())
                {
                    var boundary = boundaryCollisionReports.First().Boundary;

                    response.Outcome = boundary.Tag;
                    response.IndexOfLastState = propagatedState.Index;
                }

                return response;
            };

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddBoundary(new LineSegment(new Vector2D(0, -0.5), new Vector2D(0, 0.5), "A"));
            scene.AddBoundary(new LineSegment(new Vector2D(2, -0.5), new Vector2D(2, 0.5), "B"));

            return scene;
        }

        private static Scene GenerateSceneMazeRoom1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, 1), new Vector2D(0, 0)));

            var scene = new Scene("Maze, room 1", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.InitializationCallback = (state, message) =>
            {
                if (message == "Maze, room 2")
                {
                    state.BodyStates.First().Position = new Vector2D(2.8, -0.9);
                }
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                var reportOnCollisionWithTaggedBoundary =
                    boundaryCollisionReports.FirstOrDefault(bcr => !string.IsNullOrEmpty(bcr.Boundary.Tag));

                if (reportOnCollisionWithTaggedBoundary != null)
                {
                    response.Outcome = reportOnCollisionWithTaggedBoundary.Boundary.Tag;
                    response.IndexOfLastState = propagatedState.Index;
                }

                return response;
            };

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddRectangularBoundary(-1.2, 3.3, -1.1, 3.2);
            scene.AddBoundary(new LineSegment(new Vector2D(2.4, -1.05), new Vector2D(3.2, -1.05), "Maze, room 2"));

            return scene;
        }

        private static Scene GenerateSceneMazeRoom2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.125, 1, true), new Vector2D(1, -0.125), new Vector2D(0, 0)));

            var scene = new Scene("Maze, room 2", 120.0, new Point2D(-1.4, -1.3), initialState, 0, 0, 0, 1, false, 0.002);

            scene.InitializationCallback = (state, message) =>
            {
                if (message == "Maze, room 1")
                {
                    state.BodyStates.First().Position = new Vector2D(-0.7, 2.95);
                }
            };

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack = body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                var reportOnCollisionWithTaggedBoundary =
                    boundaryCollisionReports.FirstOrDefault(bcr => !string.IsNullOrEmpty(bcr.Boundary.Tag));

                if (reportOnCollisionWithTaggedBoundary != null)
                {
                    response.Outcome = reportOnCollisionWithTaggedBoundary.Boundary.Tag;
                    response.IndexOfLastState = propagatedState.Index;
                }

                return response;
            };

            scene.StandardInteractionCallback = StandardInteractionCallback.DungeonCrawler8Directions;

            scene.AddRectangularBoundary(-1.2, 3.3, -1.1, 3.2);
            scene.AddBoundary(new LineSegment(new Vector2D(-1.1, 3.15), new Vector2D(-0.3, 3.15), "Maze, room 1"));

            return scene;
        }

        private static Scene GenerateSceneAddBodiesByClicking1()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0), new Vector2D(0, 0)));

            var standardGravity = 9.82;
            var gravitationalConstant = 0.0;
            var handleBodyCollisions = false;
            var coefficientOfFriction = 0.0;

            var scene = new Scene(
                "Add bodies by clicking - falling balls", 
                120.0, 
                new Point2D(-2, -3), 
                initialState,
                standardGravity, 
                gravitationalConstant,
                coefficientOfFriction, 
                1, 
                handleBodyCollisions, 
                0.005);

            Point2D mousePos = null;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                if (mouseClickPosition == null) return false;

                mousePos = mouseClickPosition.Position;
                return true;
            };

            var nextBodyId = 2;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (mousePos != null)
                {
                    var vector = new Vector2D(mousePos.X, mousePos.Y);

                    mousePos = null;

                    propagatedState.AddBodyState(new BodyState(
                        new CircularBody(nextBodyId++, 0.1, 1, true), vector, new Vector2D(0, 0)));
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneAddBodiesByClicking2()
        {
            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, 0.1, 1, true), new Vector2D(0, 0), new Vector2D(0, 0)));

            var standardGravity = 0.0;
            var gravitationalConstant = 10.0;
            var handleBodyCollisions = true;
            var coefficientOfFriction = 0.0;

            var scene = new Scene(
                "Add bodies by clicking - bouncing planets",
                120.0,
                new Point2D(-2, -3),
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                1,
                handleBodyCollisions,
                0.005);

            Point2D mousePos = null;

            scene.CollisionBetweenTwoBodiesOccuredCallBack = (body1, body2) =>
            {
                return OutcomeOfCollisionBetweenTwoBodies.ElasticCollision;
            };

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                if (mouseClickPosition == null) return false;

                mousePos = mouseClickPosition.Position;
                return true;
            };

            var nextBodyId = 2;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (mousePos != null)
                {
                    var vector = new Vector2D(mousePos.X, mousePos.Y);

                    mousePos = null;

                    propagatedState.AddBodyState(new BodyState(
                        new CircularBody(nextBodyId++, 0.1, 1, true), vector, new Vector2D(0, 0)));
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneAddBodiesByClicking3()
        {
            var radiusOfBalls = 0.5;

            var initialState = new State();
            initialState.AddBodyState(new BodyState(new CircularBody(1, radiusOfBalls, 1, false), new Vector2D(0, 0), new Vector2D(0, 0)));

            var standardGravity = 0.0;
            var gravitationalConstant = 0.0;
            var handleBodyCollisions = false;
            var coefficientOfFriction = 0.0;

            var scene = new Scene(
                "Add bodies by clicking - no overlap",
                120.0,
                new Point2D(-2, -3),
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                1,
                handleBodyCollisions,
                0.005);

            Point2D mousePos = null;

            double DistanceToClosestBody(
                State state,
                Point2D point,
                double radius)
            {
                var minSqrDist = state.BodyStates.Min(_ =>
                {
                    return _.Position.AsPoint2D().SquaredDistanceTo(point);
                });

                var minDist = minSqrDist < double.Epsilon ? 0.0 : Math.Sqrt(minSqrDist);

                return Math.Max(0.0, minDist - radius);
            }

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                if (mouseClickPosition == null) return false;

                mousePos = mouseClickPosition.Position;
                return true;
            };

            var nextBodyId = 2;

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                if (mousePos != null)
                {
                    var vector = new Vector2D(mousePos.X, mousePos.Y);

                    var distanceToClosesetBody = DistanceToClosestBody(propagatedState, mousePos, radiusOfBalls);

                    if (distanceToClosesetBody > radiusOfBalls)
                    {
                        propagatedState.AddBodyState(new BodyState(
                            new CircularBody(nextBodyId++, radiusOfBalls, 1, true), vector, new Vector2D(0, 0)));
                    }

                    mousePos = null;
                }

                return new PostPropagationResponse();
            };

            return scene;
        }

        private static Scene GenerateSceneAddBodiesByClicking4()
        {
            const double radiusOfCannons = 0.2;
            const double radiusOfProjectiles = 0.05;
            const int rateOfFire = 50;

            var nextCannonId = 1000;
            var nextProjectileId = 10000;
            var stateIndexOfFirstShotInSalvo = -1000;
            var stateIndexOfNextPossibleShot = -1000;
            var cannonCount = 0;

            var initialState = new State();

            var standardGravity = 0.0;
            var gravitationalConstant = 0.0;
            var handleBodyCollisions = false;
            var coefficientOfFriction = 0.0;

            var scene = new Scene(
                "Add bodies that shoot in predefined direction",
                120.0,
                new Point2D(-2, -3),
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                1,
                handleBodyCollisions,
                0.005);

            scene.InitializationCallback = (state, message) =>
            {
                nextCannonId = 1000;
                nextProjectileId = 10000;
                stateIndexOfFirstShotInSalvo = -1000;
                stateIndexOfNextPossibleShot = -1000;
                cannonCount = 0;
            };

            Point2D mousePos = null;
            var stateIndexAtLastClick = 0;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                if (mouseClickPosition == null)
                {
                    mousePos = null;
                    //return false;
                    return true;
                }

                mousePos = mouseClickPosition.Position;
                stateIndexAtLastClick = currentState.Index;
                return true;
            };

            // Husk, at denne kaldes af BEREGNEREN, der jo altså som regel er langt foran current state
            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                // Determine if we should add a new cannon
                if (mousePos != null && cannonCount == 0)
                {
                    var vector = new Vector2D(mousePos.X, mousePos.Y);

                    propagatedState.AddBodyState(new BodyState(
                        new CircularBody(nextCannonId, radiusOfCannons, 1, false), vector, new Vector2D(0, 0)));

                    stateIndexOfFirstShotInSalvo = stateIndexAtLastClick + 1;
                    stateIndexOfNextPossibleShot = stateIndexAtLastClick + 1 + rateOfFire;

                    cannonCount++;
                    nextCannonId++;

                    mousePos = null;
                }

                // Determine if a placed cannon shoots
                //if (cannonCount > 0 && (propagatedState.Index - stateIndexOfFirstShotInSalvo) % rateOfFire == 0) 
                if (cannonCount > 0 && propagatedState.Index == stateIndexOfNextPossibleShot)
                {
                    propagatedState.AddBodyState(new BodyState(
                        new Projectile(
                            nextProjectileId++,
                            radiusOfProjectiles,
                            1,
                            false),
                            new Vector2D(0, 0),
                            new Vector2D(0, 5)));

                    // DEN HER LINIE gør, at efterfølgende klik munder ud i, at der IKKE skydes.
                    //stateIndexOfNextPossibleShot = propagatedState.Index + rateOfFire;

                    // Hvorfor virker det ikke?
                    // -> I første omgang, så får den lavet tilstanden med et projektil, men så sker der det, at 
                    //    consumeren efterspørger en tidligere tilstand, hvor projektilet ENDNU IKKE ER.
                    //    Det er godt og fint, men når så interaction callback funktionen kaldes og man i den forbindelse
                    //    discarder future states, så FLUSHES tilstanden med projektilet ud, OG - når så man kommer op på den
                    //    tilstand, hvor projektilet i den tidligere kørsel blev genereret, så laves den IKKE, FORDI den jo
                    //    tror, at den skal laves senere, når nu vi jo altså har bumpet stateIndexOfNextPossibleShot-variablen.
                    //    Hvis man som i Shoot'Em Up 4 afgør, om man skal lave en ny under anvendelse af et tal, som IKKE bumpes
                    //    i forbindelse med at der tilføjes et projektil til en beregnet tilstand, så virker det.
                    // SUMMA SUMMARUM:
                    //    Du bør IKKE ændre en variabel, der bruges til at afgøre, om en tilsstand skal ændre sig, i forbindelse
                    //    med at du ændrer tilstanden, FORDI den tilstand med stor sandsynlighed bliver discarded. Du er nødt til
                    //    at sørge for, at tilstanden beregnes på SAMME måde, når produceren påny kommer hen til den tilstand, der
                    //    gjorde sig gældende umiddelbart før pågældende ændring, dvs værdierne for de variable, der afgør, om
                    //    og hvordan tilstanden ændres, skal bibeholdes.
                    // LØSNING:
                    //    Du skal lave en løsning, hvor kriteriet for at kanonen kan afgive et skud, er hvor lang tid der er gået
                    //    siden den SIDST afgav et skud, og det ved du altså kun, hvis du "høster" en tilstand, hvor den faktisk
                    //    HAR afgivet det skud - ellers er det bare et potentielt event på lige fod med det, du også opererer med
                    //    i andet regi, såsom body collisions.
                    // MEN - hvorfor er det nu lige, at der tilsyneladende ikke er problemer i Shoot'em Up 4? For der manipulerer
                    //       du jo altså også en variabel, der bruges i Post propagation callback. Måske fordi du kun sætter den
                    //       til current state index plus 1, så den somehow ikke når at blive invalideret.
                    //       Generelt må princippet vel være, at man kun kan ændre en tilstand i post propagation callback med
                    //       udgangspunkt i noget, der ER sket (dvs høstet). Det betyder, at man godt må ændre en tilstand med
                    //       udgangspunkt i variable, der sættes i handleren for INTERACTION CALLBACK (som jo repræsenterer
                    //       current state, dvs seneste HØSTEDE tilstand). Man må derimod IKKE manipulere variable, der influerer
                    //       på, om en tilstand skal ændres, i selve handleren for post propagation callback, idet den beskæftiger
                    //       sig med FREMTIDIGE tilstande, som ofte discardes.
                }

                return new PostPropagationResponse();
            };

            return scene;
        }
    }
}
