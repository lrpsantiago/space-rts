using Assets.Scripts.Projectiles;
using UnityEngine;

namespace Assets.Scripts
{
    public class LaserBullet : Projectile
    {
        [SerializeField]
        private float _bulletSpeed = 20f;

        protected override void Start()
        {
            base.Start();

            DamageDescriptor = new DamageDescriptor(1, DamageType.Thermal);
            transform.LookAt(Target.transform.position);
        }

        protected override void Move()
        {
            var speed = _bulletSpeed * Time.deltaTime;
            var distance = Vector3.Distance(transform.position, LastTargetPosition);

            Velocity = transform.forward * speed;
            Velocity = Vector3.ClampMagnitude(Velocity, distance);

            transform.position += Velocity;
        }
    }
}
