using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : PlayerBehavior
    {
        [Header("General")]

        [SerializeField]
        [Tooltip("How fast the player will change direction / accelerate.")]
        private float m_acceleration = 5f;

        [SerializeField]
        [Tooltip("How fast the player will stop if no input is given (applies only when grounded).")]
        private float m_damping = 8f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How well the player can control direction while in air.")]
        private float m_airControl = 0.15f;

        [SerializeField]
        private float m_forwardSpeed = 9f;

        [SerializeField]
        private float m_sidewaysSpeed = 8f;

        [SerializeField]
        private float m_backwardSpeed = 7f;

        [SerializeField]
        [Tooltip("Curve for multiplying speed based on slope.")]
        private AnimationCurve m_slopeMultiplier = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));


        [Header("Physics")]

        [SerializeField]
        private float m_pushForce = 60f;

        [SerializeField]
        [Tooltip("How fast we accelerate into falling.")]
        private float m_gravity = 20f;


        [Header("Jumping")]

        [SerializeField]
        [Tooltip("How high do we jump when pressing jump and letting go immediately.")]
        private float m_jumpHeight = 1f;


        [Header("Crouching")]

        [SerializeField]
        [Tooltip("The current movement speed will be multiplied by this value, when moving crouched.")]
        private float m_crouchSpeedMultiplier = 0.7f;

        [SerializeField]
        [Tooltip("The CharacterController's height when fully-crouched.")]
        private float m_crouchHeight = 1f;

        [SerializeField]
        [Tooltip("How much time it takes to go in and out of crouch-mode.")]
        private float m_crouchDuration = 0.3f;


        [Header("Vision")]

        [SerializeField]
        private float m_LockOnRadius = 30.0f;

        [SerializeField]
        private float m_LockOnAngle = 45.0f;

        public float Gravity
        {
            get { return m_gravity; }
        }

        public float ForwardSpeed
        {
            get { return m_forwardSpeed; }
        }
        public float SidewaysSpeed
        {
            get { return m_sidewaysSpeed; }
        }

        private CharacterController m_controller;
        private float m_desiredSpeed;
        private Vector3 m_currentVelocity;
        private Vector3 m_slideVelocity;
        private Vector3 m_desiredVelocity;
        private bool m_previouslyGrounded;

        private CollisionFlags m_lastCollisionFlags;

        private bool m_runningIsEnbled = true;

        // Update is called once per frame
        void Start()
        {
            m_controller = GetComponent<CharacterController>();

            // Player actions
            Player.Jump.AddStartTryer(TryStart_Jump);
            Player.Attack.AddStartTryer(TryStart_Attack);
            Player.Roll.AddStartTryer(TryStart_Roll);
            Player.LockOn.AddStartTryer(TryStart_LockOn);
        }

        void Update()
        {
            // Move the controller
            bool shouldMoveController = !Player.Attack.Active && !Player.Roll.Active;
            if (shouldMoveController)
            {
                m_lastCollisionFlags = m_controller.Move(m_currentVelocity * Time.deltaTime);
                if ((m_lastCollisionFlags & CollisionFlags.Below) == CollisionFlags.Below && !m_previouslyGrounded)
                {
                    if (Player.Jump.Active)
                        Player.Jump.ForceStop();

                    Player.Land.Send(Mathf.Abs(Player.Velocity.Get().y));
                }

                Player.Velocity.Set(m_controller.velocity);
            }

            Player.IsGrounded.Set(m_controller.isGrounded);

            if (!m_controller.isGrounded)
            {
                if (Player.Walk.Active)
                    Player.Walk.ForceStop();
                UpdateFalling();
            }
            else if (!Player.Jump.Active)
                UpdateMovement();

            if (Player.LockOn.Active && Vector3.Distance(transform.position, GameController.WorldCamera.transform.position) > m_LockOnRadius) {
                Player.LockOn.ForceStop();
                print("Force stopping lockon");
            }

            // Set values for next tick
            m_previouslyGrounded = m_controller.isGrounded;

            Debug.DrawRay(transform.position, transform.forward);
        }

        private void UpdateFalling()
        {
            // Modify the current velocity by taking into account how well we can change direction when not grounded (see "m_AirControl" tooltip).
            m_currentVelocity += m_desiredVelocity * m_acceleration * m_airControl * Time.deltaTime;

            // Apply gravity.
            m_currentVelocity.y -= m_gravity * Time.deltaTime;
        }

        private void UpdateMovement()
        {
            CalculateDesiredVelocity();

            Vector3 targetVelocity = m_desiredVelocity;

            // TODO: Make sure to lower the speed when moving on steep surfaces.
            // float surfaceAngle = Vector3.Angle(Vector3.up, m_LastSurfaceNormal);
            // targetVelocity *= m_SlopeMultiplier.Evaluate(surfaceAngle / m_Controller.slopeLimit);

            // Calculate the rate at which the current speed should increase / decrease. 
            // If the player doesn't press any movement button, use the "m_Damping" value, otherwise use "m_Acceleration".
            float targetAccel = targetVelocity.sqrMagnitude > 0f ? m_acceleration : m_damping;

            m_currentVelocity = Vector3.Lerp(m_currentVelocity, targetVelocity, targetAccel * Time.deltaTime);

            // If we're moving, start the "Walk" activity.
            if (!Player.Walk.Active && targetVelocity.sqrMagnitude > 0.1f)
                Player.Walk.ForceStart();
            // If not moving, stop the "Walk" activity.
            else if (Player.Walk.Active && targetVelocity.sqrMagnitude < 0.1f)
                Player.Walk.ForceStop();
        }


        private void CalculateDesiredVelocity()
        {
            // Clamp the input vector's magnitude to 1; If we don't do this, when moving diagonally, the speed will be ~1.4x higher.
            Vector2 movementInputClamped = Vector2.ClampMagnitude(Player.MovementInput.Get(), 1f);

            // Has the player pressed any movement button?
            bool wantsToMove = movementInputClamped.sqrMagnitude > 0f;

            // Calculate the direction (relative to the camera), in which the player wants to move.
            Vector3 targetDirection = (wantsToMove ? GameController.WorldCamera.transform.TransformDirection(new Vector3(movementInputClamped.x, 0f, movementInputClamped.y)) : m_controller.velocity.normalized);

            m_desiredSpeed = 0f;

            if (wantsToMove)
            {
                // Set the default speed
                m_desiredSpeed = m_forwardSpeed * Player.MovementSpeedFactor.Get();

                // If the player wants to move sideways...
                if (Mathf.Abs(movementInputClamped.x) > 0f)
                    m_desiredSpeed = m_sidewaysSpeed;

                // If the player wants to move backwards...
                if (movementInputClamped.y < 0f)
                    m_desiredSpeed = m_backwardSpeed;

                // If we're crouching...
                if (Player.Crouch.Active)
                    m_desiredSpeed *= m_crouchSpeedMultiplier;
            }

            m_desiredVelocity = targetDirection * m_desiredSpeed;
        }

        private bool TryStart_Attack()
        {
            return Player.IsGrounded.Get() && !Player.Jump.Active && !Player.Roll.Active;
        }

        private bool TryStart_Roll()
        {
            return Player.IsGrounded.Get() && !Player.Jump.Active && !Player.Attack.Active;
        }

        private bool TryStart_LockOn()
        {
            Collider[] candidates = Physics.OverlapSphere(transform.position, m_LockOnRadius);

            // Sort candidates by distance to 

            // Sets bias towards where we are looking
            Vector3 cameraContribution = GameController.WorldCamera.transform.forward * 3f;
            candidates = candidates.OrderBy(x => Vector3.Distance(transform.position + cameraContribution, x.transform.position)).ToArray();

            for (int i = 0; i < candidates.Length; i++)
            {
                AI.AIBehavior enemy = candidates[i].GetComponentInParent<AI.AIBehavior>();
                if (!enemy) continue;

                // Check if the enemy is within vision angle
                float angle = Vector3.Angle(GameController.WorldCamera.transform.forward,
                                            enemy.transform.position - GameController.WorldCamera.transform.position);
                if (angle < m_LockOnAngle)
                {
                    // TODO: make sure line of sight exists
                    Player.LockOnTarget.Set(enemy);
                    return true;
                }
            }

            return false;   
        }

        private void OnLockOnStop()
        {
            Player.LockOnTarget.Set(null);
        }

        private bool TryStart_Jump()
        {
            bool canJump = Player.IsGrounded.Get() && (!Player.Crouch.Active || Player.Crouch.TryStop()) && !Player.Attack.Active && !Player.Roll.Active;

            if (canJump)
            {
                Vector3 jumpDirection = Vector3.up;
                //float surfaceAngle = Vector3.Angle(Vector3.up, m_LastSurfaceNormal);

                // Jump more perpendicular to the surface, on steep surfaces.
                //else if(surfaceAngle > 30f)
                //	jumpDirection = Vector3.Lerp(Vector3.up, m_LastSurfaceNormal, surfaceAngle / 60f).normalized;

                m_currentVelocity.y = 0f;

                m_currentVelocity += jumpDirection * CalculateJumpSpeed(m_jumpHeight);
                // m_jumpedFrom = JumpedFrom.Ground;
                

                return true;
            }

            return false;
        }

        private float CalculateJumpSpeed(float heightToReach)
        {
            return Mathf.Sqrt(2f * m_gravity * heightToReach);
        }

    }

}
