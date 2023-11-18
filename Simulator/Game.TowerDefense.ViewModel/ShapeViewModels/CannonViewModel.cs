using System;
using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Game.TowerDefense.ViewModel.ShapeViewModels
{
    public class CannonViewModel : RotatableEllipseViewModel
    {
        private static readonly System.Windows.Media.Matrix _correctionMatrix;

        private System.Windows.Media.Matrix CorrectionMatrix => _correctionMatrix;

        static CannonViewModel()
        {
            var scaling = 1.1;
            var rotation = Math.PI / 2;
            var cosAngle = Math.Cos(rotation);
            var sinAngle = Math.Sin(rotation);

            _correctionMatrix = new System.Windows.Media.Matrix(scaling, 0, 0, scaling, 0.2, 0);
        }

        protected override void UpdateRotationMatrix()
        {
            var orientationCorrection = Math.PI / 2;
            var cosAngle = Math.Cos(Orientation + orientationCorrection);
            var sinAngle = Math.Sin(Orientation + orientationCorrection);
            var x = Width / 2;
            var y = Height / 2;

            var T1 = new System.Windows.Media.Matrix(1, 0, 0, 1, -x, -y);
            var R = new System.Windows.Media.Matrix(cosAngle, -sinAngle, sinAngle, cosAngle, 0, 0);
            var T2 = new System.Windows.Media.Matrix(1, 0, 0, 1, x, y);

            RotationMatrix = T1 * CorrectionMatrix * R * T2;
        }
    }
}
