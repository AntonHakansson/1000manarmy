using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    [RequireComponent(typeof(CameraEventHandler))]
    public class CameraShake : MonoBehaviour
    {
        [Header("General")]

        [SerializeField]
        [Range(0f, 1f)]
        private float m_Trauma;

        [SerializeField]
        [Range(0.1f, 2f)]
        private float m_TraumaMultiplier = 1f;


        [Header("Camera Shake")]

        [SerializeField]
        [Range(0.1f, 10f)]
        private float m_ShakeAmount = 5f;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float m_ShakeOffsetAmount = 0.2f;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float m_ShakeYawAmount = 1f;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float m_ShakePitchAmount = 1f;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float m_ShakeRollAmount = 5f;

        // Holds reference to our camera
        private CameraEventHandler m_Camera;

        void Start()
        {
            m_Camera = GetComponent<CameraEventHandler>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
                m_Trauma = 1f;

            if (m_Trauma <= 0)
                return;

            // Decrease trauma level
            m_Trauma -= Time.deltaTime * m_TraumaMultiplier;


            /* Camera Shake */
            float cameraShakeTrauma = m_ShakeAmount * m_Trauma * m_Trauma;

            // calc pos offsets
            Vector3 deltaPos = new Vector3
            {
                x = m_ShakeOffsetAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(5f),
                y = m_ShakeOffsetAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(15f),
                z = m_ShakeOffsetAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(25f)
            };

            // calc rot offsets
            float dyaw = m_ShakeYawAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(0f);
            float dpitch = m_ShakePitchAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(10f);
            float droll = m_ShakeRollAmount * cameraShakeTrauma * PerlinNoiseNegOneToOne(20f);

            Quaternion deltaRotation = Quaternion.Euler(dyaw, dpitch, droll);

            // Update FX transform
            m_Camera.FXPosition.Set(deltaPos);
            m_Camera.FXRotation.Set(deltaRotation);
        }

        public void AddTrauma(float amount)
        {
            m_Trauma += amount;
        }

        private float PerlinNoiseNegOneToOne(float seed)
        {
            return Mathf.PerlinNoise(Time.timeSinceLevelLoad * 2f, seed) * 2 - 1f;
        }
    }
}