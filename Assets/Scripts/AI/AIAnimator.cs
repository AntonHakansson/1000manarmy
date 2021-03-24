using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ManArmy.AI
{
    public class AIAnimator : AIBehavior
    {
        private Animator m_EnemyAnimator;

        private string[] m_OneHandedAttackAnimations = {
            "oh_attack_1",
            "oh_attack_2",
            "oh_attack_3",
            // "standing_melee_attack_360_low",
            // "standing_melee_attack_backhand",
            // "standing_melee_attack_horizontal"
        };
        private string[] m_DamageAnimations = { "DirectionalDamage", "damage_1", "damage_2" };

//          [ShowOnly]
        private DamageCollider[] m_DamageColliders;

        private void Start()
        {
            m_EnemyAnimator = GetComponent<Animator>();

            m_DamageColliders = GetComponentsInChildren<DamageCollider>();

            AI.ChangeHealth.AddListener(OnEnemyHealthChange);
        }

        private void OnEnemyHealthChange(HealthEvent evt)
        {
            // Don't play animation if we recently just did
            if (AI.isTakingDamage.Get())
            {
                return;
            }

            if (evt.Delta < 0)
            {
                // Play Damage Animation
                string damageAnimation = m_DamageAnimations[Random.Range(0, m_DamageAnimations.Length)];

                if (evt.HitDirection != Vector3.zero)
                {
                    // Transform global hit direction to local space
                    Vector3 localHitDirection = transform.InverseTransformDirection(evt.HitDirection);
                    m_EnemyAnimator.SetFloat("DamageHorizontalDirection", localHitDirection.x);
                }

                m_EnemyAnimator.Play(damageAnimation);
                AI.isTakingDamage.Set(true);
                StartCoroutine(ResetIsTakingDamage());
            }
        }

        private IEnumerator ResetIsTakingDamage()
        {
            yield return new WaitForSeconds(1f);

            string attackAnimation = m_OneHandedAttackAnimations[Random.Range(0, m_OneHandedAttackAnimations.Length)];
            m_EnemyAnimator.Play(attackAnimation);

            AI.isTakingDamage.Set(false);
        }

        // Animation hooks
        public void OpenDamageColliders()
        {
            print("Opening colliders");
            foreach (var damageCollider in m_DamageColliders)
            {
                damageCollider.GetComponent<Collider>().enabled = true;
            }
        }

        public void CloseDamageColliders()
        {
            foreach (var damageCollider in m_DamageColliders)
            {
                damageCollider.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
