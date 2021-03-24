using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class CameraEventHandler : MonoBehaviour
    {
        public Value<Vector3>    Position = new Value<Vector3>(Vector3.zero);
        public Value<Quaternion> Rotation = new Value<Quaternion>(Quaternion.identity);

        // Used for camera shake and similar other effects
        public Value<Vector3>    FXPosition = new Value<Vector3>(Vector3.zero);
        public Value<Quaternion> FXRotation = new Value<Quaternion>(Quaternion.identity);

        private Vector3 m_CameraAvoidancePos = new Vector3();

        void LateUpdate()
        {
            // TODO: make camera collide with environment
            UpdateAvoidance();

            transform.position = Position.Get() + FXPosition.Get() + m_CameraAvoidancePos;
            transform.rotation = Rotation.Get() * FXRotation.Get();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Position.Get(), 0.2f);
        }

        private void UpdateAvoidance()
        {
            Vector3 playerPos = GameController.LocalPlayer.transform.position;
            Vector3 cameraPos = Position.Get();
            Vector3 forward = Rotation.Get() * Vector3.forward;

            float rayCastLength = (playerPos - cameraPos).magnitude;
            Ray ray = new Ray(playerPos, -forward);

            RaycastHit hit;
            bool foundHit = Physics.Raycast(ray, out hit, rayCastLength);

            if (foundHit && hit.transform.root.GetComponent<EntityBehaviour>() == null)
            {
                Vector3 targetPoint = hit.point + forward * 1f + hit.normal * 0.1f;
                float closeAmount = Mathf.Min(2f, 1f / Vector3.Distance(targetPoint, playerPos));
                targetPoint += Vector3.up * closeAmount;
                
                Debug.DrawLine(ray.origin, targetPoint, Color.red);

                m_CameraAvoidancePos = Vector3.Lerp(m_CameraAvoidancePos, targetPoint - cameraPos, Time.deltaTime * 20f);
            }
            else
            {
                Debug.DrawLine(ray.origin, Position.Get(), Color.green);
                m_CameraAvoidancePos = Vector3.zero;
            }
        }
    }
}