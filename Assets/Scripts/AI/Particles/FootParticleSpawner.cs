using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    [RequireComponent(typeof(Collider))]
    public class FootParticleSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform m_DustParticleEffect;

        [SerializeField]
        private InAudioEvent m_FootstepEvents;

        private Collider m_Collider;

        private void Start()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root == transform.root) return;
            // TODO: spawn different effects based on ground
            ParticleSystem particle = Instantiate(m_DustParticleEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            StartCoroutine(KillParticle(particle));

            InAudio.PostEvent(gameObject, m_FootstepEvents);
        }

        IEnumerator KillParticle(ParticleSystem particle)
        {
            while (particle.IsAlive()) {
                yield return new WaitForSeconds(1);
            }

            // Code to execute after the particle is done playing
            Destroy(particle);
        }

    }
}