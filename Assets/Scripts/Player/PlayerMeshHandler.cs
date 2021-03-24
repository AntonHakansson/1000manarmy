using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy {
    public class PlayerMeshHandler : PlayerBehavior {

        [SerializeField]
        [Range(0f, 1f)]
        private float m_SmoothSteps = 0.5f;

        // Use this for initialization
        void Start() {
            Player.Velocity.AddChangeListener(OnPlayerVelocityChange);
        }

        private void OnPlayerVelocityChange()
        {
            if (!Player.LockOn.Active)
            {
                Vector3 targetMovementDir = Player.Velocity.Get();
                targetMovementDir.y = 0;
                if (targetMovementDir.sqrMagnitude < 0.1f)
                {
                    return;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetMovementDir, transform.up);
                targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, m_SmoothSteps);

                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 targetMovementDir = GameController.WorldCamera.transform.forward;
                targetMovementDir.y = 0;
                if (targetMovementDir.sqrMagnitude < 0.1f)
                {
                    return;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetMovementDir, transform.up);
                targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, m_SmoothSteps);

                transform.rotation = targetRotation;
            }
        }
    }
}