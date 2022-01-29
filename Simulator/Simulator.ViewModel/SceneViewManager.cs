using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Craft.Math;
using Craft.ViewModels.Geometry2D;
using Simulator.Domain;
using Simulator.Domain.Boundaries;
using LineSegment = Simulator.Domain.Boundaries.LineSegment;

namespace Simulator.ViewModel
{
    public delegate ShapeViewModel ShapeSelectorCallback(
        BodyState bodyState);

    public delegate void ShapeUpdateCallback(
        ShapeViewModel shapeViewModel,
        BodyState bodyState);

    // Denne klasse observerer current scene udstillet af application laget og vedligeholder GeometryEditorViewModel
    public class SceneViewManager
    {
        private Application.Application _application;
        private Scene _activeScene;
        private GeometryEditorViewModel _geometryEditorViewModel;
        private int[] _propIds;

        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                _activeScene = value;
                _application.Engine.Scene = value;
                Reset();

                if (value == null) return;

                PrepareAnimation();
            }
        }

        public ShapeSelectorCallback ShapeSelectorCallback { get; set; }

        public ShapeUpdateCallback ShapeUpdateCallback { get; set; }

        public SceneViewManager(
            Application.Application application,
            GeometryEditorViewModel geometryEditorViewModel,
            ShapeSelectorCallback shapeSelectorCallback = null,
            ShapeUpdateCallback shapeUpdateCallback = null)
        {
            _application = application;
            _geometryEditorViewModel = geometryEditorViewModel;

            _application.CurrentStateChangedCallback = UpdateScene;

            ShapeSelectorCallback = shapeSelectorCallback;
            ShapeUpdateCallback = shapeUpdateCallback;

            if (ShapeSelectorCallback == null)
            {
                SetShapeSelectorCallbackToDefault();
            }

            if (ShapeUpdateCallback == null)
            {
                SetShapeUpdateCallbackToDefault();
            }
        }

        public void SetShapeSelectorCallbackToDefault()
        {
            ShapeSelectorCallback = (bs) =>
            {
                switch (bs.Body)
                {
                    case RectangularBody body:
                        {
                            return new RectangleViewModel
                            {
                                //Point = new Point2D(bs.Position.X, bs.Position.Y),
                                Width = body.Width,
                                Height = body.Height
                            };
                        }
                    case CircularBody body:
                        {
                            return new EllipseViewModel
                            {
                                //Point = new Point2D(bs.Position.X, bs.Position.Y),
                                Width = 2 * body.Radius,
                                Height = 2 * body.Radius
                            };
                        }
                    default:
                        throw new ArgumentException("Unknown Body");
                }
            };
        }

        public void SetShapeUpdateCallbackToDefault()
        {
            ShapeUpdateCallback = (shapeViewModel, bs) =>
            {
                shapeViewModel.Point = new Point2D(bs.Position.X, bs.Position.Y);
            };
        }

        public void ResetScene()
        {
            Reset();
            PrepareAnimation();
        }

        private void PrepareAnimation()
        {
            _geometryEditorViewModel.Magnification = _application.Engine.Scene.InitialMagnification;

            _geometryEditorViewModel.WorldWindowUpperLeftLimit = new Point(
                _application.Engine.Scene.WorldWindowUpperLeftLimit.X,
                _application.Engine.Scene.WorldWindowUpperLeftLimit.Y);

            _geometryEditorViewModel.WorldWindowBottomRightLimit = new Point(
                _application.Engine.Scene.WorldWindowBottomRightLimit.X,
                _application.Engine.Scene.WorldWindowBottomRightLimit.Y);

            _geometryEditorViewModel.WorldWindowUpperLeft = new Point(
                _application.Engine.Scene.InitialWorldWindowUpperLeft.X,
                _application.Engine.Scene.InitialWorldWindowUpperLeft.Y);

            var lineThickness = 0.01;

            _application.Engine.Scene.Boundaries.ForEach(b =>
            {
                if (!b.Visible) return;

                // Todo: prøv at kør det polymorfisk i stedet
                switch (b)
                {
                    case HalfPlane halfPlane:
                        var v = halfPlane.SurfaceNormal.Hat();
                        _geometryEditorViewModel.AddLine(
                            new Point2D(halfPlane.Point.X - 500 * v.X, halfPlane.Point.Y - 500 * v.Y),
                            new Point2D(halfPlane.Point.X + 500 * v.X, halfPlane.Point.Y + 500 * v.Y),
                            lineThickness);
                        break;
                    case LeftFacingHalfPlane halfPlane:
                        _geometryEditorViewModel.AddLine(
                            new Point2D(halfPlane.X, -500),
                            new Point2D(halfPlane.X, 500),
                            lineThickness);
                        break;
                    case RightFacingHalfPlane halfPlane:
                        _geometryEditorViewModel.AddLine(
                            new Point2D(halfPlane.X, -500),
                            new Point2D(halfPlane.X, 500),
                            lineThickness);
                        break;
                    case UpFacingHalfPlane halfPlane:
                        _geometryEditorViewModel.AddLine(
                            new Point2D(-500, halfPlane.Y),
                            new Point2D(500, halfPlane.Y),
                            lineThickness);
                        break;
                    case DownFacingHalfPlane halfPlane:
                        _geometryEditorViewModel.AddLine(
                            new Point2D(-500, halfPlane.Y),
                            new Point2D(500, halfPlane.Y),
                            lineThickness);
                        break;
                    case LineSegment lineSegment:
                        _geometryEditorViewModel.AddLine(
                            lineSegment.Point1.AsPoint2D(),
                            lineSegment.Point2.AsPoint2D(),
                            lineThickness);
                        break;
                    case VerticalLineSegment lineSegment:
                        _geometryEditorViewModel.AddLine(
                            lineSegment.Point1.AsPoint2D(),
                            lineSegment.Point2.AsPoint2D(),
                            lineThickness);
                        break;
                    case HorizontalLineSegment lineSegment:
                        _geometryEditorViewModel.AddLine(
                            lineSegment.Point1.AsPoint2D(),
                            lineSegment.Point2.AsPoint2D(),
                            lineThickness);
                        break;
                    case BoundaryPoint boundaryPoint:
                        _geometryEditorViewModel.AddPoint(boundaryPoint.Point.AsPoint2D(), 3, new SolidColorBrush(Colors.Black));
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            _application.Engine.Scene.Props.ForEach(p => 
            {
                _geometryEditorViewModel.AddShape(p.Id, new RectangleViewModel
                {
                    Width = p.Width,
                    Height = p.Height,
                    Point = p.Position.AsPoint2D()
                });
            });

            _propIds = _application.Engine.Scene.Props.Select(p => p.Id).ToArray();

            var initialState = _application.Engine.SpawnNewThread();
            RepositionWorldWindowIfRequired(initialState);
            UpdateCurrentState(initialState);
        }

        private void Reset()
        {
            _application.ResetEngine();
            ClearCurrentScene();
        }

        private void ClearCurrentScene()
        {
            _geometryEditorViewModel.ClearShapes();
            _geometryEditorViewModel.ClearPoints();
            _geometryEditorViewModel.ClearLines();
        }

        private void UpdateScene(
            State state)
        {
            RepositionWorldWindowIfRequired(state);
            UpdateCurrentState(state);
        }

        private void RepositionWorldWindowIfRequired(
            State state)
        {
            switch (_application.Engine.Scene.ViewMode)
            {
                case SceneViewMode.FocusOnCenterOfMass:
                    var centerOfMass = state.CenterOfMass();
                    if (centerOfMass != null)
                    {
                        _geometryEditorViewModel.SetFocusForWorldWindow(centerOfMass.AsPoint2D());
                    }
                    break;
                case SceneViewMode.FocusOnFirstBody:
                    var centerOfInitialBody = state.CenterOfInitialBody();
                    if (centerOfInitialBody != null)
                    {
                        _geometryEditorViewModel.SetFocusForWorldWindow(centerOfInitialBody.AsPoint2D());
                    }
                    break;
                case SceneViewMode.MaintainFocusInVicinityOfPoint:
                    var centerOfInitialBody2 = state.CenterOfInitialBody();
                    if (centerOfInitialBody2 != null)
                    {
                        _geometryEditorViewModel.AdjustWorldWindowSoPointLiesInCentralSquare(
                            centerOfInitialBody2.AsPoint2D(),
                            _application.Engine.Scene.MaxOffsetXFraction,
                            _application.Engine.Scene.MaxOffsetYFraction,
                            _application.Engine.Scene.CorrectionXFraction,
                            _application.Engine.Scene.CorrectionYFraction);
                    }
                    break;
                case SceneViewMode.Stationary:
                default:
                    break;
            }
        }

        private void UpdateCurrentState(
            State state)
        {
            // Identificer eventuelle bodies, der ikke længere er med
            _geometryEditorViewModel.AllShapeIds
                .Except(_propIds)
                .Except(state.BodyStates.Select(bs => bs.Body.Id))
                .ToList()
                .ForEach(id =>
                {
                    _geometryEditorViewModel.RemoveShape(id);
                });

            state.BodyStates.ForEach(bs =>
            {
                var shape = _geometryEditorViewModel.TryGetShape(bs.Body.Id);

                if (shape != null)
                {
                    ShapeUpdateCallback.Invoke(shape, bs);
                }
                else
                {
                    var shapeViewModel = ShapeSelectorCallback.Invoke(bs);
                    shapeViewModel.Point = bs.Position.AsPoint2D();

                    if (shapeViewModel is RotatableShapeViewModel)
                    {
                        (shapeViewModel as RotatableShapeViewModel).Orientation = bs.Orientation;
                    }

                    _geometryEditorViewModel.AddShape(bs.Body.Id, shapeViewModel);
                }
            });
        }
    }
}
