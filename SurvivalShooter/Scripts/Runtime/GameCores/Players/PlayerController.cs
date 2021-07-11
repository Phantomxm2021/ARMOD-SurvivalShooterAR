using System;
using System.Collections.Generic;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooter
{
    public class PlayerController : AbstractGameState
    {
        [SerializeField] internal float Speed = 0.6f;
        [SerializeField] internal GunConfig gunConfig;
        [SerializeField] internal ParticleSystem gunParticles;
        [SerializeField] internal LineRenderer gunLine;
        [SerializeField] internal AudioSource gunAudio;
        [SerializeField] internal Light gunLight;
        [SerializeField] internal Light faceLight;
        [SerializeField] internal Transform gunMuzzle;
        [SerializeField] internal Transform trans;

        internal Rigidbody playerRigidbody;

        private List<IBehaviour> playerBehaviour;
        private IBehaviour updateBehaviour;
        internal InputSystem inputSystem;
        private Health health;
        private int aliveTime;

        private void FixedUpdate()
        {
            if (playerBehaviour == null) return;
            foreach (var tmp_Behaviour in playerBehaviour)
            {
                tmp_Behaviour.DoBehaviour();
            }
        }

        public override void GameInit(BaseNotificationData _data)
        {
            trans = this.transform;

            playerRigidbody = GetComponent<Rigidbody>();
            health = GetComponent<Health>();
            inputSystem = FindObjectOfType<InputSystem>();
            
            updateBehaviour = new Shooting(gunParticles, gunLine, gunAudio,
                gunLight, faceLight, inputSystem, gunMuzzle, gunConfig);
            playerBehaviour = new List<IBehaviour>
            {
                new Movement(this),
                new Turning(this),
                new Animating(this),
            };
        }

        public override void GameStart(BaseNotificationData _data)
        {
            InvokeRepeating("TimeCalculator", 1, 1);
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
            updateBehaviour?.DoBehaviour();
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
            CancelInvoke("TimeCalculator");
            ActionNotificationCenter.DefaultCenter.PostNotification(ConstKey.CONST_TIME_COUNTER,
                new TimeCounterNotificationData() {counter = aliveTime});
        }


        private void TimeCalculator()
        {
            aliveTime++;
          
        }
    }
}