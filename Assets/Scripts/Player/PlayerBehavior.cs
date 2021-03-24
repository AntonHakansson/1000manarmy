using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class PlayerBehavior : EntityBehaviour
    {
        public PlayerEventHandler Player
        {
            get
            {
                if (!m_Player)
                    m_Player = GetComponent<PlayerEventHandler>();
                if (!m_Player)
                    m_Player = GetComponentInParent<PlayerEventHandler>();

                return m_Player;
            }
        }

        private PlayerEventHandler m_Player;
    }
}