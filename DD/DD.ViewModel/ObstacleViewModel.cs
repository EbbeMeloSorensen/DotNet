﻿using System;
using DD.Domain;

namespace DD.ViewModel
{
    public class ObstacleViewModel : BoardItemViewModel
    {
        public ObstacleViewModel(
            Obstacle obstacle,
            double left,
            double top,
            double diameter) : base(left, top, diameter)
        {
            switch (obstacle.ObstacleType)
            {
                case ObstacleType.Wall:
                    ImagePath = "Images/Wall.jpg";
                    break;
                case ObstacleType.Water:
                    ImagePath = "Images/Water.PNG";
                    break;
                case ObstacleType.IronFence:
                    ImagePath = "Images/Iron Fence.PNG";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obstacle.ObstacleType), obstacle.ObstacleType, null);
            }
        }
    }
}