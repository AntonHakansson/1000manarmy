using System;
using UnityEngine;

namespace ManArmy
{
    /// <summary>
    /// 
    /// </summary>
    public class HealthEvent
    {
        /// <summary> </summary>
        public float Delta { get; set; }

        /// <summary> </summary>
        public EntityEventHandler Damager { get; private set; }

        /// <summary> </summary>
        public Vector3 HitPoint { get; private set; }

        /// <summary> </summary>
        public Vector3 HitDirection { get; private set; }

        /// <summary> </summary>
        public float HitImpulse { get; private set; }

        /// <summary> </summary>
        public Collider AffectedCollider { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public HealthEvent(float delta, EntityEventHandler damager = null, Vector3 hitPoint = default(Vector3), Vector3 hitDirection = default(Vector3), float hitImpulse = 0f)
        {
            Delta = delta;
            Damager = damager;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
            HitImpulse = hitImpulse;
        }
    }

    public interface IDamageable
    {
        void ReceiveDamage(HealthEvent damageData);
    }
}
