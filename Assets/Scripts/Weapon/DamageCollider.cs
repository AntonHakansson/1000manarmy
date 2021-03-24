using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    [RequireComponent(typeof(Rigidbody))]
    public class DamageCollider : MonoBehaviour
    {
        private void Awake()
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            gameObject.layer = LayerMask.NameToLayer("DamageCollider");
        }

        private void OnCollisionEnter(Collision collision)
        {
            var striked = collision.transform.GetComponentInParent<EntityBehaviour>();

            if (!striked)
                return;
            if (!striked.Entity)
                return;

            var striker = transform.GetComponentInParent<EntityBehaviour>();
            if (striker == striked) return;

            var hitPoint = collision.contacts[0].point;
            var hitDirection = striked.transform.position - hitPoint;

            striked.Entity.ChangeHealth.Try(new HealthEvent(-1f, striker.Entity, hitPoint, hitDirection));
        }
    }
}
