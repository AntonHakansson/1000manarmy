using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class Weapon : MonoBehaviour
    {
        // [ShowOnly]
        private DamageCollider[] m_DamageColliders;

        // Use this for initialization
        void Start()
        {
            m_DamageColliders = GetComponentsInChildren<DamageCollider>();
            SetDamageColliders(false);
        }

        public void OpenDamageColliders()
        {
            SetDamageColliders(true);
        }

        public void CloseDamageColliders()
        {
            SetDamageColliders(false);
        }

        private void SetDamageColliders(bool enabled)
        {
            foreach (DamageCollider coll in m_DamageColliders)
            {
                Collider collider = coll.GetComponent<Collider>();
                if (collider == null)
                {
                    Debug.LogWarning("No Collider found for damage Collider: " + coll);
                    continue;
                }

                coll.GetComponent<Collider>().enabled = enabled;
            }
        }
    }
}
