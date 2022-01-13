using System;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR
{
    public class Health : MonoBehaviour
    {
        public int currentHealth = 100;
        public AudioSource audioSource = null;
        public ParticleSystem hitParticleSystem = null;
        public Collider collider = null;
        public Action<int> healthChange = null;
        private AudioClip hurtClip = null;
        private AudioClip deathClip = null;

        public void SetDeathClip(AudioClip _audio)
        {
            deathClip = _audio;
        }

        public void SetHurtClip(AudioClip _audio)
        {
            hurtClip = _audio;
        }

        public int MaxHealth { get; private set; } = 0;

        private void Start()
        {
            collider = null;
            deathClip = null;
            hitParticleSystem = null;

            audioSource = GetComponent<AudioSource>();
            if (String.Compare(gameObject.name, ConstKey.CONST_PLAYER_TAG, StringComparison.Ordinal) == 1)
                hitParticleSystem = GetComponentInChildren<ParticleSystem>();
            collider = GetComponent<Collider>();
            MaxHealth = currentHealth;
            healthChange?.Invoke(currentHealth);
        }


        private void OnEnable()
        {
            if (collider)
                collider.enabled = true;
        }

        public void SetHealth(int _heath)
        {
            currentHealth = _heath;
        }


        public bool IsDead()
        {
            return CurrentHealth() == (0);
        }

        public int CurrentHealth()
        {
            return currentHealth;
        }


        public void TakeDamage(int _damage, Vector3 _hitPoint)
        {
            if (IsDead())
            {
                return;
            }

            currentHealth = currentHealth - _damage;
            healthChange?.Invoke(CurrentHealth());
            if (hurtClip)
            {
                audioSource.clip = hurtClip;
                audioSource.Play();
            }
            else if (audioSource.clip)
            {
                audioSource.Play();
            }

            if (hitParticleSystem != null)
            {
                hitParticleSystem.transform.position = _hitPoint;
                hitParticleSystem.Play(true);
            }


            if (IsDead())
                Dead();
        }


        private void Dead()
        {
            //玩家死亡游戏结束
            if (gameObject.name.Contains(ConstKey.CONST_PLAYER_TAG))
            {
                ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_OVER, null);
            }


            collider.enabled = false;
            if (!deathClip) return;
            audioSource.clip = deathClip;
            audioSource.Play();
        }
    }
}