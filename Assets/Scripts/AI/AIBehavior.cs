using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy.AI
{
    public class AIBehavior : EntityBehaviour
    {
        public AIEventHandler AI
        {
            get
            {
                if (!m_AI)
                    m_AI = GetComponent<AIEventHandler>();
                if (!m_AI)
                    m_AI = GetComponentInParent<AIEventHandler>();

                return m_AI;
            }
        }

        private AIEventHandler m_AI;
    }
}