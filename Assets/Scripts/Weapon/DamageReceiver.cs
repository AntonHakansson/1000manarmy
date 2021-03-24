using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class DamageReceiver : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("DamageReceiver");

            var collider = GetComponent<Collider>();
            var entity = GetComponentInParent<EntityBehaviour>();
            foreach(var dmgColl in entity.transform.GetComponentsInChildren<DamageCollider>())
            {
                Physics.IgnoreCollision(collider, dmgColl.GetComponent<Collider>());
                Debug.LogWarning("Disabling collision between " + gameObject + " and " + dmgColl.gameObject);
            }
        }
    }
}