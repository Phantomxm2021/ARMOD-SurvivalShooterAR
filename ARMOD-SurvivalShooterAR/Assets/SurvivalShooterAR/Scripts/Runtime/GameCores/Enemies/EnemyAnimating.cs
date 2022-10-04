using UnityEngine;

namespace SurvivalShooterAR.Runtime
{
    public class EnemyAnimating : IBehaviour
    {
        private readonly Animator anim;
        private readonly Health enemyHealth;
        private readonly Health playerHealth;
        private static readonly int Dead = Animator.StringToHash(ConstKey.CONST_ENEMY_ANIMATOR_STATE_DEAD);
        private static readonly int PlayerDead = Animator.StringToHash(ConstKey.CONST_ENEMY_ANIMATOR_STATE_IDLE);
        private static readonly int DeathClip = Animator.StringToHash(ConstKey.CONST_ENEMY_ANIMATOR_STATE_DEADCLIP);
        private EnemyDataConfig.EnemyCategory category;

        public EnemyAnimating(Animator _anim, Health _enemyHealth, Health _playerHealth,
            EnemyDataConfig.EnemyCategory _category)
        {
            anim = _anim;
            enemyHealth = _enemyHealth;
            playerHealth = _playerHealth;
            category = _category;
        }

        public void DoBehaviour()
        {
            if (enemyHealth.IsDead())
            {
                anim.SetTrigger(Dead);
                if (AnimationEnd(DeathClip))
                {
                    SurvivalShooterARMainEntry.EnemyPool.SetNewEnemy(enemyHealth.gameObject, category);
                }
            }


            //设置为待机
            if (playerHealth.IsDead())
            {
                anim.SetTrigger(PlayerDead);
            }
        }

        private bool AnimationEnd(int _animClipHash)
        {
            AnimatorStateInfo tmp_StateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (tmp_StateInfo.shortNameHash.CompareTo(_animClipHash) == 0)
            {
                return tmp_StateInfo.normalizedTime >= 0.8f;
            }

            return false;
        }
    }
}