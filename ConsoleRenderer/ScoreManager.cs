using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class ScoreManager
    {
        public delegate void OnNextLevel(int level);

        static int c_SingleLineScore = 100;
        private int m_Score;
        private int m_CurrentLevelScore;
        private int m_Level;
        public int m_NextLevelTreshold;
        private static OnNextLevel m_OnNextLevel;

        private void InvokeOnNextLevel(int level)
        {
            if (m_OnNextLevel != null) m_OnNextLevel(level);
        }

        public int CurrentScore { get { return m_Score; } }
        public int Level { get { return m_Level; } }
        public float LevelProgress { get { return (float)(m_CurrentLevelScore) / (float)(m_NextLevelTreshold); } }
        public int LevelProgressPercent { get { return (int)((float)(m_CurrentLevelScore) / (float)(m_NextLevelTreshold)*100.0f); } }

        public ScoreManager()
        {
            if (m_OnNextLevel != null)
            {
                foreach (Delegate d in m_OnNextLevel.GetInvocationList())
                {
                    m_OnNextLevel -= (OnNextLevel)d;
                }
            }
            m_Score = 0;
            m_CurrentLevelScore = 0;
            m_Level = 1;
            m_NextLevelTreshold = 1000;
        }

        public int AddScore(int lines)
        {
            m_Score += (m_Level* lines * lines * c_SingleLineScore);
            m_CurrentLevelScore += (m_Level * lines * lines * c_SingleLineScore); 
            if(m_CurrentLevelScore >= m_NextLevelTreshold )
            {
                m_CurrentLevelScore = 0;
                m_Level++;
                m_NextLevelTreshold += 100 * m_Level;
                //if (m_CurrentLevelScore >= m_NextLevelTreshold) throw new Exception("jeb");
                InvokeOnNextLevel(m_Level);
            }
            return m_Score;
        }

        public void Subscribe_OnNextLevel(OnNextLevel listener)
        {
            m_OnNextLevel += listener;
        }

        public void Unsubscribe_OnNextLevel(OnNextLevel listener)
        {
            m_OnNextLevel -= listener;
        }


    }
}
