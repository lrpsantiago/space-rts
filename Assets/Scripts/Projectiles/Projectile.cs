using SpaceRts;
using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float _lifetime = 10;

        public Ship Target { get; set; }

        public DamageDescriptor DamageDescriptor { get; set; }

        public Vector3 Velocity { get; protected set; }

        public Vector3 LastTargetPosition { get; protected set; }

        protected virtual void Start()
        {
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            if (Target != null)
            {
                LastTargetPosition = Target.transform.position;
            }

            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Unit"))
            {
                if (Target != null)
                {
                    if (other.gameObject == Target.gameObject)
                    {
                        OnUnitImpact();
                    }
                }
            }
        }

        protected virtual void OnUnitImpact()
        {
            Target.TakeDamage(DamageDescriptor.Damage);
            Destroy(gameObject);
        }

        protected abstract void Move();
    }
}
