using UnityEngine;

namespace SpaceRts
{
    internal class MovingShipState : State<Ship>
    {
        public MovingShipState(Ship owner)
            : base(owner)
        {

        }

        public override void OnEnter()
        {
        }

        public override void Update()
        {
            if (Owner.DestinationQueue.Count <= 0)
            {
                Owner.CurrentState = Owner.StationaryState;
                return;
            }

            var destinationPos = Owner.DestinationQueue[0];
            var pos = Owner.transform.position;

            Owner.UpdateRotation(destinationPos, Owner.AngularSpeed);

            var distance = Vector3.Distance(pos, destinationPos);

            if (Owner.DestinationQueue.Count > 1)
            {
                var sum = distance;

                for (int i = 0; i < Owner.DestinationQueue.Count - 1; i++)
                {
                    var p1 = Owner.DestinationQueue[i];
                    var p2 = Owner.DestinationQueue[i + 1];

                    sum += Vector3.Distance(p1, p2);
                }

                distance = sum;
            }

            var decelDistance = CalculateDecelDistance();
            var variant = Owner.MovementSpeed + Owner.Acceleration * Time.deltaTime;

            if (distance <= decelDistance)
            {
                variant = Owner.MovementSpeed - Owner.Deceleration * Time.deltaTime;
            }

            Owner.MovementSpeed = Mathf.Clamp(variant, 0, Owner.MovementSpeedMax);

            var velocity = Owner.MovementSpeed * Time.deltaTime;
            pos = Vector3.MoveTowards(pos, destinationPos, velocity);

            Owner.transform.position = pos;
            distance = Vector3.Distance(pos, destinationPos);

            if (Owner.transform.position == destinationPos)
            {
                Owner.DestinationQueue.RemoveAt(0);
            }
        }

        public override void OnLeave()
        {
        }

        private double CalculateDecelDistance()
        {
            return Mathf.Pow(Owner.MovementSpeed, 2) / (2 * Owner.Deceleration);
        }
    }
}
