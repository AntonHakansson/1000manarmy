using System.Collections;
using UnityEngine;

namespace ManArmy
{
    /// <summary>
    /// The game's central control point.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        /// <summary> </summary>
        public static PlayerEventHandler LocalPlayer
        {
            get
            {
                if (m_Player == null)
                    m_Player = FindObjectOfType<PlayerEventHandler>();
                return m_Player;
            }
        }

        /// <summary></summary>
        public static float NormalizedTime { get; set; }

        /// <summary></summary>
        public static Camera WorldCamera { get; private set; }

        private static PlayerEventHandler m_Player;

        private void Awake()
        {
            WorldCamera = GameObject.Find("World Camera").GetComponent<Camera>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
