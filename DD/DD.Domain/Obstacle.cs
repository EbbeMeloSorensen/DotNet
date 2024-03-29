﻿namespace DD.Domain
{
    public enum ObstacleType
    {
        Wall,
        Water,
        IronFence
    }

    public class Obstacle : BoardItem
    {
        private ObstacleType _obstacleType;

        public ObstacleType ObstacleType
        {
            get { return _obstacleType; }
            private set { _obstacleType = value; }
        }

        public Obstacle(
            ObstacleType obstacleType,
            int positionX, 
            int positionY)
        {
            ObstacleType = obstacleType;
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}