using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivalShooter
{
    public class EnemySpawnSystem : AbstractGameState
    {
        public float SpawnTime = 3f;
        public Transform[] SpawnPoints;
        private Health playerHealth;


        public override void GameInit(BaseNotificationData _data)
        {
            SpawnPoints = new Transform[6];
            SpawnPoints = GetComponentsInChildren<Transform>();
        }

        public override void GameStart(BaseNotificationData _obj)
        {
            playerHealth = FindObjectOfType<PlayerTag>().GetComponent<Health>();
            InvokeRepeating(nameof(Spawn), SpawnTime, SpawnTime);
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _obj)
        {
            CancelInvoke(nameof(Spawn));
        }

        private void Spawn()
        {
            if (playerHealth.CurrentHealth() <= 0f)
            {
                return;
            }

            int tmp_SpawnPointIndex = Random.Range(0, SpawnPoints.Length);
            GameObject tmp_GameObject = SurvivalShooterMainEntry.EnemyPool.GetNewEnemy();

            if (tmp_GameObject == null) return;

            var tmp_Trans = tmp_GameObject.transform;
            tmp_Trans.localPosition = SpawnPoints[tmp_SpawnPointIndex].localPosition;
            tmp_Trans.localRotation = SpawnPoints[tmp_SpawnPointIndex].localRotation;
        }
    }
}