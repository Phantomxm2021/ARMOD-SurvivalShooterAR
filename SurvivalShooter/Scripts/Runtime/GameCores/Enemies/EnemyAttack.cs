using UnityEngine;

namespace SurvivalShooter
{
    public class EnemyAttack : IBehaviour
    {
        float timer;

        readonly float timeBetweenAttacks;
        readonly int attackDamage;

        readonly Health playerHealth;
        readonly Health enemyHealth;


        private readonly Transform playerTrans;
        private readonly Transform enemyTrans;

        public EnemyAttack(Health _playerHealth, Health _enemyHealth, int _attackDamage = 10,
            float _timeBetweenAttacks = 0.5f)
        {
            timeBetweenAttacks = _timeBetweenAttacks;
            attackDamage = _attackDamage;
            playerHealth = _playerHealth;
            enemyHealth = _enemyHealth;
            playerTrans = playerHealth.transform;
            enemyTrans = enemyHealth.transform;
        }

        public void DoBehaviour()
        {
            if (!enemyTrans.gameObject.activeSelf) return;
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if (!(Vector3.Distance(enemyTrans.position, playerTrans.position) <= 0.2f)) return;
            if (timer >= timeBetweenAttacks && enemyHealth.CurrentHealth() > 0)
            {
                // ... attack.
                Attack();
            }
        }


        void Attack()
        {
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if (playerHealth.CurrentHealth() > 0)
            {
                // ... damage the player.
                playerHealth.TakeDamage(attackDamage, Vector3.zero);
            }
        }
    }
}