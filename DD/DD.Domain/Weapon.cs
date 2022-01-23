namespace DD.Domain
{
    public class Weapon : BoardItem
    {
        public Weapon(
            int positionX,
            int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}