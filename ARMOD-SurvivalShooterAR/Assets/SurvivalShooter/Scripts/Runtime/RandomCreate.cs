using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivalShooter
{
    public class RandomCreate : MonoBehaviour
    {
        public float Offset;
        public List<GameObject> Enemies;

        private void CreateEnemies()
        {
            var tmp_RandomPos = Random.insideUnitCircle * Offset;
            var tmp_Enemy = Instantiate(Enemies[Random.Range(0, Enemies.Count)]);
            var tmp_Trans = tmp_Enemy.transform;
            tmp_Trans.position = new Vector3(tmp_RandomPos.x, 0, tmp_RandomPos.y);
        }

        private void Start()
        {
            for (int tmp_Idx = 0; tmp_Idx < 5; tmp_Idx++)
            {
                CreateEnemies();
            }

            InvokeRepeating(nameof(CreateEnemies), 1, 0.5f);
        }
    }
}