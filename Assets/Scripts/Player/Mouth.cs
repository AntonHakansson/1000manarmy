using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManArmy
{
    public class Mouth : MonoBehaviour
    {
        [SerializeField]
        private float m_FramesPerSecond = 10f;

        [SerializeField]
        private int m_Columns = 4;

        [SerializeField]
        private int m_Rows = 4;


        //the current frame to display
        private int m_Index = 0;
        private Renderer m_Renderer;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(updateTiling());
            Vector2 size = new Vector2(1f / m_Columns, 1f / m_Rows);

            m_Renderer = GetComponent<Renderer>();
            m_Renderer.material.SetTextureScale("_MainTex", size);
        }

        private IEnumerator updateTiling()
        {
            m_Renderer = GetComponent<Renderer>();
            while (true)
            {
                // Move to the next index
                // m_Index++;
                if (m_Index >= m_Rows * m_Columns)
                    m_Index = 0;

                // Split into x and y indexes
                Vector2 offset = new Vector2
                {
                    x = ((float)(m_Index % m_Columns)) / (float)m_Columns,
                    y = ((float)(m_Index / m_Columns)) / (float)m_Rows
                };

                m_Renderer.material.SetTextureOffset("_MainTex", offset);

                yield return new WaitForSeconds(1f / m_FramesPerSecond);
            }

        }
    }
}