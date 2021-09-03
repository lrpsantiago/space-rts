using PushingBoxStudios;
using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public class Missile : Projectile
    {
        [SerializeField]
        private float _speed = 1;

        [SerializeField]
        private float _angularSpeed = 180f;

        protected override void Move()
        {
            if (Target != null)
            {
                var rot = transform.rotation;
                var toTarget = Target.transform.position - transform.position;
                var lookRot = Quaternion.LookRotation(toTarget);

                rot = Quaternion.RotateTowards(rot, lookRot, _angularSpeed * Time.deltaTime);

                transform.rotation = rot;
            }

            var velocity = transform.forward.normalized * _speed * Time.deltaTime;
            transform.position += velocity;
        }
    }
}
