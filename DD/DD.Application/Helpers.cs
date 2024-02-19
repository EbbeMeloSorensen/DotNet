using System;
using System.ComponentModel;
using Craft.Math;
using DD.Domain;

namespace DD.Application
{
    public static class Helpers
    {
        public static Point2D GetTileCenterCoordinates(
            int positionX,
            int positionY,
            BoardTileMode mode)
        {
            switch (mode)
            {
                case BoardTileMode.Square:
                {
                    return new Point2D(positionX, positionY);
                }
                case BoardTileMode.Hexagonal:
                {
                    return new Point2D(positionX % 2 == 0 ? positionX : positionX + 0.5, positionY * Math.Sqrt(3) / 2);
                }
                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }
        }
    }
}