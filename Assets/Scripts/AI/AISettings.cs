using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy.AI
{
    public class AISettings : MonoBehaviour
    {
        public AIMovement Movement { get { return m_Movement; } }
        public AIDetection Detection { get { return m_Detection; } }
        public AIAnimation Animation { get { return m_Animator; } }

        [SerializeField]
        private AIMovement m_Movement;

        [SerializeField]
        private AIDetection m_Detection;

        [SerializeField]
        private AIAnimation m_Animator;

        private AIBrain m_Brain;
 

        [Header("Combat")]

        [SerializeField]
        [Range(0f, 500f)]
        private float m_HitDamage = 25f;

        [SerializeField]
        [Range(0f, 3f)]
        private float m_MaxAttackDistance = 2f;


        private void Start()
        {
            m_Brain = GetComponent<AIBrain>();

            m_Movement.Initialize(m_Brain);
            m_Detection.Initialize(transform);

            m_Animator = new AIAnimation();
            m_Animator.Initialize(m_Brain);
        }

        private void Update()
        {
            m_Movement.Update(transform);

            m_Detection.Update(m_Brain);
        }
    }
}