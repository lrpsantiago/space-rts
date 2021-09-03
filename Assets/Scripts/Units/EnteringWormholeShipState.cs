using SpaceRts;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Units
{
    internal class EnteringWormholeShipState : State<Ship>
    {
        private float _originalDistance;

        public EnteringWormholeShipState(Ship owner)
            : base(owner)
        {

        }

        public override void OnEnter()
        {
            Owner.GoingToEnterWormhole = false;

            var destinationPos = Owner.DestinationQueue[0];
            var pos = Owner.transform.position;

            _originalDistance = Vector3.Distance(pos, destinationPos);
        }

        float speed = 0;

        public override void Update()
        {
            var destinationPos = Owner.DestinationQueue[0];
            var pos = Owner.transform.position;
            var distance = Vector3.Distance(pos, destinationPos);

            var accel = 50 * Time.deltaTime;
            speed = Mathf.Clamp(speed + accel, 0, 100);

            var spd = speed * Time.deltaTime;

            pos = Vector3.MoveTowards(pos, destinationPos, spd);
            Owner.transform.position = pos;

            var newSize = Vector3.one * (distance / _originalDistance);
            Owner.transform.localScale = Vector3.Max(Vector3.zero, newSize);

            if (pos == destinationPos)
            {
                Owner.gameObject.SetActive(false);
                //Owner.CurrentState = Owner.StationaryState;
            }
        }

        public override void OnLeave()
        {

        }
    }
}
