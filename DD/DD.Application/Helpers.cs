using System;
using System.ComponentModel;
using Craft.Math;
using DD.Domain;

namespace DD.Application
{
    public static class Helpers
    {
        public static Point2D GetTileCenterCoordinates(
            this int tileIndex,
            int columns,
            BoardTileMode mode)
        {
            var rowIndex = tileIndex / columns;
            var columnIndex = tileIndex % columns;

            switch (mode)
            {
                case BoardTileMode.Square:
                {
                    return new Point2D(columnIndex, rowIndex);
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