using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ManArmy
{
    public class PlayerEventHandler : EntityEventHandler
    {
		public Value<Vector2> MovementInput = new Value<Vector2>(Vector2.zero);

        public Value<Vector2> LookInput = new Value<Vector2>(Vector2.zero);

        public Value<Vector3> LookDirection = new Value<Vector3>(Vector3.zero);

        public Value<bool> ViewLocked = new Value<bool>(false);

        public Value<float> MovementSpeedFactor = new Value<float>(1f);

        public Value<AI.AIBehavior> LockOnTarget = new Value<AI.AIBehavior>(null);

        public Attempt InteractOnce = new Attempt();


        public Activity Walk = new Activity();

        public Activity Crouch = new Activity();

        public Activity Jump = new Activity();

        public Activity Attack = new Activity();

        public Activity Roll = new Activity();

        public Activity LockOn = new Activity();

        public Weapon Weapon;
    }
}