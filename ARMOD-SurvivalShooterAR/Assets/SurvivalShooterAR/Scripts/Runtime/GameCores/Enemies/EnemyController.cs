using System.Collections.Generic;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace SurvivalShooterAR.Runtime
{
    public class EnemyController : AbstractGameState
    {
        private List<IBehaviour> enemyBehaviours;
        private NavMeshAgent navMeshAgent;
        private Health enemyHealth;
        public EnemyDataConfig Config;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            //挂载在同一物体上都会被识别为MonoBehaviour,故获取所有Components进行判断
            var tmp_Player = Helper.FindTypes<PlayerTag>(typeof(PlayerTag));
            enemyHealth = Helper.GetComponent<Health>(typeof(Health), this.gameObject);

            var tmp_PlayerHealth = Helper.GetComponent<Health>(typeof(Health), tmp_Player.gameObject);

            var tmp_EnemyMovement = new EnemyMovement(this.transform, navMeshAgent,
                enemyHealth, tmp_Player.transform, tmp_PlayerHealth, Config);
            var tmp_EnemyAnimating =
                new EnemyAnimating(GetComponent<Animator>(), enemyHealth, tmp_PlayerHealth, Config.EnemyType);
            var tmp_EnemyAttack = new EnemyAttack(tmp_PlayerHealth, enemyHealth);
            enemyBehaviours = new List<IBehaviour> {tmp_EnemyMovement, tmp_EnemyAttack, tmp_EnemyAnimating};
        }

        private void OnEnable()
        {
            if (navMeshAgent)
                navMeshAgent.enabled = true;
        }


        public override void GameInit(BaseNotificationData _data)
        {
        }

        public override void GameStart(BaseNotificationData _data)
        {
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
            if (enemyBehaviours == null) return;
            foreach (IBehaviour tmp_Behaviour in enemyBehaviours)
            {
                tmp_Behaviour?.DoBehaviour();
            }
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
        }
    }
}