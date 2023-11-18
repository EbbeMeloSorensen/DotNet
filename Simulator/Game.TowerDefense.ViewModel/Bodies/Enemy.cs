using Simulator.Domain;

namespace Game.TowerDefense.ViewModel.Bodies;

public class Enemy : CircularBody
{
    public Enemy(
        int id,
        double radius,
        double mass,
        bool affectedByGravity) : base(id, radius, mass, affectedByGravity)
    {
    }
}