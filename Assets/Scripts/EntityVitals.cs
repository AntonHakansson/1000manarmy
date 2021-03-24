using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ManArmy
{
    public class EntityVitals : EntityBehaviour
    {
        [SerializeField]
        private float m_MaxHealth = 100f;

        private void Start()
        {
            Entity.Health.Set(m_MaxHealth);
            Entity.ChangeHealth.SetTryer(Try_ChangeHealth);
        }

        protected virtual bool Try_ChangeHealth(HealthEvent evt)
        {
            if (Entity.Health.Get() == 0f)
                return false;
            if (evt.Delta > 0f && Entity.Health.Get() == 100f)
                return false;

            float newHealth = Mathf.Clamp(Entity.Health.Get() + evt.Delta, 0f, 100f);
            Entity.Health.Set(newHealth);

            if (newHealth == 0f)
            {
                print("Entity " + gameObject + " just died.");
            }

            return true;
        }
    }
}