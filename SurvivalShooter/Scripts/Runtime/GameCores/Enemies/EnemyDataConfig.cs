using UnityEngine;

namespace SurvivalShooter
{
    [System.Serializable]
    public class EnemyDataConfig
    {
        public float visionRange = 8;
        public float hearingRange = 20;
        public float wanderDistance = 40;
        public Vector2 idleTimeRange = new Vector2(.5f, 2);
        public EnemyCategory EnemyType;
        
        
        
        public enum EnemyCategory
        {
            Bunny,
            Bear,
            Hellephant
        }
    }
}