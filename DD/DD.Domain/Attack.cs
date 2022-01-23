namespace DD.Domain
{
    public abstract class Attack
    {
        public string Name { get; set; }
        public int MaxDamage { get; set; }

        protected Attack(
            string name,
            int maxDamage)
        {
            Name = name;
            MaxDamage = maxDamage;
        }
    }

    public class MeleeAttack : Attack
    {
        public MeleeAttack(
            string name, 
            int maxDamage) : base(name, maxDamage)
        {
        }
    }

    public class RangedAttack : Attack
    {
        public double Range { get; set; }

        public RangedAttack(
            string name, 
            int maxDamage,
            double range) : base(name, maxDamage)
        {
            Range = range;
        }
    }
}
