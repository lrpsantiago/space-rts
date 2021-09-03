namespace Assets.Scripts.Projectiles
{
    public struct DamageDescriptor
    {
        public float Damage { get; private set; }
        public DamageType Type { get; private set; }

        public DamageDescriptor(float damage, DamageType type)
        {
            Damage = damage;
            Type = type;
        }
    }
}
