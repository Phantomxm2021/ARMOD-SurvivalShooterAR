using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SurvivalShooter
{
    public class PoolConfig : IDisposable
    {
        public int Weights;
        public EnemyDataConfig.EnemyCategory EnemyCategory;

        private readonly Queue<GameObject> enemies;
        private readonly List<GameObject> alwaysRef;

        public GameObject GetNewEnemy()
        {
            return enemies.Count > 0 ? enemies.Dequeue() : null;
        }


        public void SetNewEnemy(GameObject _enemy)
        {
            _enemy.SetActive(false);
            var tmp_Trans = _enemy.transform;
            tmp_Trans.localPosition = Vector3.zero;
            tmp_Trans.localRotation = Quaternion.identity;
            alwaysRef.Add(_enemy);
            enemies.Enqueue(_enemy);
        }


        public PoolConfig(int _weights, int _poolSize)
        {
            Weights = _weights;
            alwaysRef = new List<GameObject>();
            enemies = new Queue<GameObject>(_poolSize);
        }

        public void Dispose()
        {
            foreach (var tmp_EnemyObject in alwaysRef)
            {
                Object.Destroy(tmp_EnemyObject);
            }
        }
    }

    public class EnemyPool
    {
        // private readonly Queue<GameObject> enemies;

        private readonly List<PoolConfig> config;
        private readonly int[] allConfigRate;
        private readonly int totalRate;

        public EnemyPool(List<PoolConfig> _config)
        {
            config = _config;
            allConfigRate = new int[_config.Count];
            int tmp_Idx = 0;
            foreach (var tmp_Config in _config)
            {
                totalRate += tmp_Config.Weights;
                allConfigRate[tmp_Idx] = tmp_Config.Weights;
                tmp_Idx++;
            }
        }

        public GameObject GetNewEnemy()
        {
            int tmp_Idx = CalculateWeight(allConfigRate, totalRate);
            var tmp_Enemy = config[tmp_Idx].GetNewEnemy();

            if (tmp_Enemy)
                tmp_Enemy.SetActive(true);
            return tmp_Enemy;
        }

        public void SetNewEnemy(GameObject _enemy, EnemyDataConfig.EnemyCategory _category)
        {
            _enemy.SetActive(false);
            Health tmp_Health = _enemy.GetComponent<Health>();
            tmp_Health.SetHealth(tmp_Health.MaxHealth);
            var tmp_Trans = _enemy.transform;
            tmp_Trans.localPosition = Vector3.zero;
            tmp_Trans.localRotation = Quaternion.identity;

            var tmp_Config = config.Find(_config => _config.EnemyCategory.Equals(_category));
            tmp_Config.SetNewEnemy(_enemy);
        }


        private int CalculateWeight(int[] _rate, int _total)
        {
            int tmp_RandomValue = Random.Range(1, _total + 1);
            int tmp_Threashold = 0;
            for (int tmp_Idx = 0; tmp_Idx < _rate.Length; tmp_Idx++)
            {
                tmp_Threashold += _rate[tmp_Idx];
                if (tmp_Threashold > tmp_RandomValue)
                {
                    return tmp_Idx;
                }
            }

            return 0;
        }
    }
}