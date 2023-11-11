using System.Windows;
using Simulator.Domain;

namespace Game.Zelda.ViewModel
{
    public static class Helpers
    {
        public static Point InitialWorldWindowFocus(
            this Scene scene)
        {
            return new Point(
                (scene.InitialWorldWindowUpperLeft.X + scene.InitialWorldWindowLowerRight.X) / 2,
                (scene.InitialWorldWindowUpperLeft.Y + scene.InitialWorldWindowLowerRight.Y) / 2);
        }

        public static Size InitialWorldWindowSize(
            this Scene scene)
        {
            return new Size(
                scene.InitialWorldWindowLowerRight.X - scene.InitialWorldWindowUpperLeft.X,
                scene.InitialWorldWindowLowerRight.Y - scene.InitialWorldWindowUpperLeft.Y);
        }
    }
}
