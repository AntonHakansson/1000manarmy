using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

namespace ManArmy
{
    [RequireComponent(typeof(CameraEventHandler))]
    public class MouseLook : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The up & down rotation will be inverted, if checked.")]
        private bool m_Invert;

        [SerializeField]
        [Tooltip("If checked, a button will show up which can lock the cursor.")]
        private bool m_ShowLockButton = true;

        public bool m_LockOn = false;
        public Transform m_LockOnTarget;

        [Header("Motion")]

        [SerializeField]
        [Tooltip("The higher it is, the faster the camera will rotate.")]
        private float m_Distance = 6f;

        [SerializeField]
        [Tooltip("The higher it is, the faster the camera will rotate.")]
        private float m_Sensitivity = 5f;

        [SerializeField]
        [Range(0, 20)]
        private int m_SmoothSteps = 10;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_SmoothWeight = 0.4f;

        [SerializeField]
        private float m_RollAngle = 10f;

        [SerializeField]
        private float m_RollSpeed = 1f;


        [Header("Rotation Limits")]

        [SerializeField]
        private Vector2 m_DefaultLookLimits = new Vector2(-60f, 90f);

        private float m_CurrentRollAngle;

        private Vector2 m_LookAngles;

        private int m_LastLookFrame;
        private Vector2 m_CurrentMouseLook;
        private Vector2 m_SmoothMove;
        private List<Vector2> m_SmoothBuffer = new List<Vector2>();

        private PlayerEventHandler m_Player;
        private CameraEventHandler m_Camera;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            m_Player = GameController.LocalPlayer;
            m_Player.LockOn.AddStartListener(OnLockOnStart);
            m_Player.LockOn.AddStopListener(OnLockOnEnd);

            m_Camera = GetComponent<CameraEventHandler>();
        }

        private void OnGUI()
        {
            if (!m_ShowLockButton)
                return;

            Vector2 buttonSize = new Vector2(256f, 24f);

            // NOTE: While in Unity Editor, pressing Esc will always unlock the cursor.
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            if (Cursor.lockState == CursorLockMode.None)
            {
                if (GUI.Button(new Rect(Screen.width * 0.5f - buttonSize.x / 2f, 16f, buttonSize.x, buttonSize.y), "Lock Cursor (Hit 'Esc' to unlock)"))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        void LateUpdate()
        {
            if (m_Player.ViewLocked.Is(false) && Cursor.lockState == CursorLockMode.Locked)
                LookAround();

            m_Player.ViewLocked.Set(Cursor.lockState != CursorLockMode.Locked);
        }

        /// <summary>
		/// Rotates the camera and character and creates a sensation of looking around.
		/// </summary>
		private void LookAround()
        {
            if (!m_LockOn)
            {
                CalculateMouseInput(Time.deltaTime);

                m_LookAngles.x += m_CurrentMouseLook.x * m_Sensitivity * (m_Invert ? 1f : -1f);
                m_LookAngles.y += m_CurrentMouseLook.y * m_Sensitivity;

                m_LookAngles.x = ClampAngle(m_LookAngles.x, m_DefaultLookLimits.x, m_DefaultLookLimits.y);

                m_CurrentRollAngle = Mathf.Lerp(m_CurrentRollAngle, m_Player.LookInput.Get().x * m_RollAngle, Time.deltaTime * m_RollSpeed);

                // Apply the current up & down rotation to the look root.
                m_Camera.Position.Set(m_Player.transform.position + Quaternion.Euler(m_LookAngles.x, m_LookAngles.y, 0) * new Vector3(0.0f, 0.0f, -m_Distance));
                m_Camera.Rotation.Set(Quaternion.Euler(m_LookAngles.x, m_LookAngles.y, m_CurrentRollAngle));
            }
            else
            {
                m_Camera.Position.Set(m_Player.transform.position + m_Camera.Rotation.Get() * new Vector3(0.0f, 0.0f, -m_Distance) + 2f * Vector3.up);
                m_Camera.Rotation.Set(Quaternion.LookRotation(m_LockOnTarget.position - m_Camera.Position.Get(), Vector3.up));
            }
        }

        /// <summary>
        /// Clamps the given angle between min and max degrees.
        /// </summary>
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle > 360f)
                angle -= 360f;
            else if (angle < -360f)
                angle += 360f;

            return Mathf.Clamp(angle, min, max);
        }

        private void CalculateMouseInput(float deltaTime)
        {
            if (m_LastLookFrame == Time.frameCount)
                return;

            m_LastLookFrame = Time.frameCount;

            var InputDevice = InputManager.ActiveDevice;
            m_SmoothMove = new Vector2(InputDevice.RightStickY.Value, InputDevice.RightStickX.Value);

            m_SmoothSteps = Mathf.Clamp(m_SmoothSteps, 1, 20);
            m_SmoothWeight = Mathf.Clamp01(m_SmoothWeight);

            while (m_SmoothBuffer.Count > m_SmoothSteps)
                m_SmoothBuffer.RemoveAt(0);

            m_SmoothBuffer.Add(m_SmoothMove);

            float weight = 1f;
            Vector2 average = Vector2.zero;
            float averageTotal = 0f;

            for (int i = m_SmoothBuffer.Count - 1; i > 0; i--)
            {
                average += m_SmoothBuffer[i] * weight;
                averageTotal += weight;
                weight *= m_SmoothWeight / (deltaTime * 60f);
            }

            averageTotal = Mathf.Max(1f, averageTotal);
            m_CurrentMouseLook = average / averageTotal;
        }

        private void OnLockOnStart()
        {
            m_LockOn = true;
            m_LockOnTarget = m_Player.LockOnTarget.Get().transform;
        }

        private void OnLockOnEnd()
        {
            m_LockOn = false;
            // Set our look angle to look at the "current" target
            m_LookAngles = m_Camera.Rotation.Get().eulerAngles;
        }
    }
}