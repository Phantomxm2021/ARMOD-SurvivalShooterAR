using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace SurvivalShooterAR
{
    public class EnemyMovement : IBehaviour
    {
        private readonly EnemyDataConfig enemyDataConfig;
        private float timer;
        private float currentVision;
        private Transform playerTransform;
        private Transform selfTransform;
        private NavMeshAgent nav;
        private Health enemyHealth;
        private Health playerHealth;

        public EnemyMovement(Transform _selfTransform, NavMeshAgent _nav, Health _enemyHealth,
            Transform _playerTransform,
            Health _playerHealth, EnemyDataConfig _enemyDataConfig)
        {
            selfTransform = _selfTransform;
            nav = _nav;
            enemyHealth = _enemyHealth;
            playerTransform = _playerTransform;
            playerHealth = _playerHealth;
            enemyDataConfig = _enemyDataConfig;

            _nav.enabled = true;
            ClearPath();
            ScaleVision(1f);
            IsPsychic();
            timer = 0f;
        }

        public void DoBehaviour()
        {
            if (enemyHealth.CurrentHealth() > 0 && playerHealth.CurrentHealth() > 0)
            {
                LookForPlayer();
                WanderOrIdle();
            }
            else
            {
                nav.enabled = false;
            }
        }

        private void LookForPlayer()
        {
            TestSense(playerTransform.position, currentVision);
        }

        private void HearPoint(Vector3 _position)
        {
            TestSense(_position, enemyDataConfig.hearingRange);
        }

        private void TestSense(Vector3 _position, float _senseRange)
        {
            if (Vector3.Distance(this.selfTransform.position, _position) <= _senseRange)
            {
                GoToPosition(_position);
            }
        }

        public void GoToPlayer()
        {
            GoToPosition(playerTransform.position);
        }

        private void GoToPosition(Vector3 _position)
        {
            timer = -1f;
            if (!enemyHealth.IsDead())
            {
                SetDestination(_position);
            }
        }


        private void SetDestination(Vector3 _position)
        {
            if (nav.isOnNavMesh)
            {
                nav.SetDestination(_position);
            }
        }


        private void WanderOrIdle()
        {
            if (nav.hasPath) return;
            if (timer <= 0f)
            {
                SetDestination(GetRandomPoint(enemyDataConfig.wanderDistance, 5));
                if (nav.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    ClearPath();
                }

                timer = Random.Range(enemyDataConfig.idleTimeRange.x, enemyDataConfig.idleTimeRange.y);
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        public void ScaleVision(float _scale)
        {
            currentVision = enemyDataConfig.visionRange * _scale;
        }


        private Vector3 GetRandomPoint(float _distance, int _layermask)
        {
            Vector3 tmp_RandomPoint = UnityEngine.Random.insideUnitSphere * _distance + this.selfTransform.position;
            NavMesh.SamplePosition(tmp_RandomPoint, out var tmp_NavMeshHit, _distance, _layermask);
            return tmp_NavMeshHit.position;
        }

        private void ClearPath()
        {
            if (nav.hasPath)
                nav.ResetPath();
        }

        private void IsPsychic()
        {
            GoToPlayer();
        }

        public void Dispose()
        {
            nav.enabled = true;
            ClearPath();
            ScaleVision(1f);
            IsPsychic();
            timer = 0f;
        }
    }
}