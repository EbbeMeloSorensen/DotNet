using DD.Domain;

namespace DD.Application
{
    public static class BoardItemExtensions
    {
        public static int IndexOfOccupiedSquare(
            this BoardItem boardItem,
            int columns)
        {
            return boardItem.PositionY * columns + boardItem.PositionX;
        }
    }
}
