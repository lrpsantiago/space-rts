using UnityEngine;

namespace SpaceRts
{
    internal class StationaryShipState : State<Ship>
    {
        public StationaryShipState(Ship owner)
            : base(owner)
        {

        }

        public override void OnEnter()
        {
            Owner.MovementSpeed = 0;
        }

        public override void Update()
        {
            if (Owner.DestinationQueue.Count <= 0)
            {
                if (Owner.DestinationFacingDirection.HasValue)
                {
                    var fromRotation = Owner.transform.rotation;
                    var toRotation = Quaternion.LookRotation(Owner.DestinationFacingDirection.Value, Vector3.up);
                    var rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                        Owner.StationaryAngularSpeed * Time.deltaTime);

                    Owner.transform.rotation = rotation;
                }

                return;
            }

            var destinationPos = Owner.DestinationQueue[0];
            var pos = Owner.transform.position;

            Owner.UpdateRotation(destinationPos, Owner.StationaryAngularSpeed);

            var distVector = destinationPos - pos;
            var angle = Vector3.Angle(Owner.transform.forward, distVector);

            if (angle == 0 || angle < Owner.StationaryAngularSpeed * Time.deltaTime)
            {
                Owner.CurrentState = Owner.MovingState;
            }
        }

        public override void OnLeave()
        {
        }
    }
}
