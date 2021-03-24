using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class PlayerAnimator : PlayerBehavior
    {

        [Range(0, 1)]
        private float m_Vertical;

        [Range(-1, 1)]
        private float m_Horizontal;

        Animator m_PlayerAnimator;
        CharacterController m_PlayerController;
        MovementController m_PlayerMovementController;
        Rigidbody m_PlayerRigidbody;

        private string[] m_OneHandedAttackAnimations = {
            "oh_attack_1",
            "oh_attack_2",
            "oh_attack_3",
            // "standing_melee_attack_360_low",
            // "standing_melee_attack_backhand",
            // "standing_melee_attack_horizontal"
        };
        private string[] m_DamageAnimations = {
            // "damage_1",
            // "damage_2",
            "damage_3"
        };

        private int m_NotGroundedConfidence = 0;

        // Use this for initialization
        void Start()
        {
            m_PlayerAnimator = GetComponent<Animator>();
            m_PlayerController = Player.GetComponent<CharacterController>();
            m_PlayerRigidbody = Player.GetComponent<Rigidbody>();
            m_PlayerMovementController = Player.GetComponent<MovementController>();

            Player.Attack.AddStartListener(OnPlayerAttackStart);
            Player.Roll.AddStartListener(OnPlayerRollStart);
            Player.Jump.AddStartListener(OnPlayerJumpStart);
            Player.Jump.AddStartListener(OnPlayerJumpEnd);
            Player.Velocity.AddChangeListener(OnPlayerVelocityChange);
            
            Player.LockOn.AddStartListener(OnPlayerLockOnStart);
            Player.LockOn.AddStopListener(OnPlayerLockOnStop);

            Player.ChangeHealth.AddListener(OnPlayerHealthChange);
        }

        private bool prevCanMoveStatus;
        private void Update()
        {
            if (!Player.LockOn.Active) {
                UpdateWithFreeLook();
            }
            else
            {
                UpdateWithLockOn();
            }

            // Stop Player Activities as override animation stopped
            if (!prevCanMoveStatus && m_PlayerAnimator.GetBool("CanMove"))
            {
                if (Player.Attack.Active)
                    Player.Attack.ForceStop();
                if (Player.Roll.Active)
                    Player.Roll.ForceStop();
            }

            const int notGroundedThreashold = 20;
            if (!m_PlayerController.isGrounded)
            {
                if (m_NotGroundedConfidence < notGroundedThreashold)
                    m_NotGroundedConfidence += 1;
            }
            else
                m_NotGroundedConfidence = 0;
            
            m_PlayerAnimator.SetBool("OnGround", !(m_NotGroundedConfidence >= notGroundedThreashold));

            prevCanMoveStatus = m_PlayerAnimator.GetBool("CanMove");
        }

        private void UpdateWithFreeLook()
        {
            m_PlayerAnimator.SetFloat("Vertical", m_Vertical);
            m_PlayerAnimator.SetFloat("Horizontal", 0f);
            
            if (Player.Roll.Active)
            {
                // Default to rolling forward
                m_PlayerAnimator.SetFloat("RollVertical", 1f);
                m_PlayerAnimator.SetFloat("RollHorizontal", 0f);
            }

        }

        private void UpdateWithLockOn()
        {
            m_PlayerAnimator.SetFloat("Vertical", m_Vertical);
            m_PlayerAnimator.SetFloat("Horizontal", m_Horizontal);
        }

        private void OnPlayerAttackStart()
        {
            m_PlayerAnimator.SetFloat("Vertical", 0f);
            string randomAnimation = m_OneHandedAttackAnimations[Random.Range(0, m_OneHandedAttackAnimations.Length)];
            m_PlayerAnimator.CrossFade(randomAnimation, 0.2f);

            // Stop character movement
            HaltPlayerMovement();
        }

        private void OnPlayerRollStart()
        {
            // We only set these values once as to not transition when rolling.
            m_PlayerAnimator.SetFloat("RollVertical", Input.GetAxisRaw("Vertical"));
            m_PlayerAnimator.SetFloat("RollHorizontal", Input.GetAxisRaw("Horizontal"));
            m_PlayerAnimator.Play("Rolls");

            // Stop character movement
            HaltPlayerMovement();
        }
        
        private void OnPlayerJumpStart()
        {
            m_PlayerAnimator.CrossFade("Jump", 0.1f);
        }

        private void OnPlayerJumpEnd()
        {

        }

        private void OnPlayerLockOnStart()
        {
            m_PlayerAnimator.SetBool("LockOn", true);
        }

        private void OnPlayerLockOnStop()
        {
            m_PlayerAnimator.SetBool("LockOn", false);
        }

        private void OnPlayerVelocityChange()
        {
            Vector3 playerDirection = Player.Velocity.Get();
            playerDirection.y = 0;

            Vector3 localDirection = Player.transform.InverseTransformDirection(playerDirection);

            m_Vertical = localDirection.z / m_PlayerMovementController.ForwardSpeed;
            m_Horizontal = localDirection.x / m_PlayerMovementController.SidewaysSpeed;
        }

        bool isTakingDamage = false;
        private void OnPlayerHealthChange(HealthEvent evt)
        {
            // Don't play animation if we recently just did
            if (isTakingDamage)
            {
                return;
            }

            if (evt.Delta < 0)
            {
                // Play Damage Animation
                string damageAnimation = m_DamageAnimations[Random.Range(0, m_DamageAnimations.Length)];
                m_PlayerAnimator.Play(damageAnimation);
                isTakingDamage = true;
                StartCoroutine(ResetIsTakingDamage());

                // Add some camera shake
                GameController.WorldCamera.GetComponent<CameraShake>().AddTrauma(1f);
            }
        }

        private IEnumerator ResetIsTakingDamage()
        {
            yield return new WaitForSeconds(1f);
            isTakingDamage = false;
        }

        void OnAnimatorMove()
        {
            if (m_PlayerAnimator.GetBool("CanMove"))
                return;

            if (Player.Jump.Active)
                return;
            
            Vector3 deltaPos = m_PlayerAnimator.deltaPosition;

            float multiplier = 1.1f;
            if (Player.Roll.Active)
                multiplier += 1.4f;

            deltaPos = deltaPos * multiplier;

            // TODO: This fucks shit up mah dude
            if (!Player.IsGrounded.Get())
                deltaPos.y = -1f * m_PlayerMovementController.Gravity * Time.deltaTime;
            else
                deltaPos.y = -0.1f;

            m_PlayerController.Move(deltaPos);
        }

        private void HaltPlayerMovement()
        {
            // Stop character movement
            m_PlayerRigidbody.velocity *= 0.2f; // Damp velocity to allow smooth transition into animation when moving
            m_PlayerRigidbody.angularVelocity = Vector3.zero;
            Player.Velocity.Set(Vector3.zero);
        }

        // Animation hooks
        public void OpenDamageColliders()
        {
            Player.Weapon.OpenDamageColliders();
        }

        public void CloseDamageColliders()
        {
            Player.Weapon.CloseDamageColliders();
        }
    }
}