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
                    throw new NotImplementedException();
                }
                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }
        }
    }
}