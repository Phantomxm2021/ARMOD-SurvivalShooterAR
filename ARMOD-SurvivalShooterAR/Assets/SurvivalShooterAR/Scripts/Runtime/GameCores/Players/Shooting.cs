using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR
{
    public class Shooting : IBehaviour
    {
        private GunConfig gunConfig;
        private Ray shootRay;
        private RaycastHit shootHit;
        private ParticleSystem gunParticles;
        private LineRenderer gunLine;
        private AudioSource gunAudio;
        private Light gunLight;
        private Light faceLight;
        private InputSystem inputSystem;
        private Transform selfTrans;

        private float effectsDisplayTime;
        private float timer;
        private bool canShooting;
        private int grenadeStock;
        private bool isGameOver = false;

        public Shooting(ParticleSystem _gunParticles, LineRenderer _gunLine, AudioSource _gunAudio, Light _gunLight,
            Light _faceLight, InputSystem _inputSystem, Transform _selfTrans, GunConfig _gunConfig)
        {
            shootRay = new Ray();
            this.gunParticles = _gunParticles;
            this.gunLine = _gunLine;
            this.gunAudio = _gunAudio;
            this.gunLight = _gunLight;
            this.faceLight = _faceLight;
            this.inputSystem = _inputSystem;
            this.selfTrans = _selfTrans;
            gunConfig = _gunConfig;
            _inputSystem.Shooting += Shoot;
            effectsDisplayTime = .2f;
            grenadeStock = 99;
            ActionNotificationCenter.DefaultCenter.AddObserver(GameOver, NotifyKeys.GAME_OVER);
        }

        public void DoBehaviour()
        {
            if (isGameOver)
                return;
            timer += Time.deltaTime;
            canShooting = timer >= gunConfig.TimeBetweenBullets && Time.timeScale != 0;

            if (!(timer >= gunConfig.TimeBetweenBullets * effectsDisplayTime)) return;
            gunLight.enabled = false;
            faceLight.enabled = false;
            gunLine.enabled = false;
        }

        private void Shoot()
        {
            if (isGameOver)
                return;

            if (!canShooting) return;

            timer = 0f;
            gunAudio.Play();
            gunLight.enabled = true;
            faceLight.enabled = true;
            gunParticles.Stop();
            gunParticles.Play();
            gunLine.enabled = true;
            var tmp_Position = selfTrans.position;
            gunLine.SetPosition(0, tmp_Position);

            shootRay.origin = tmp_Position;
            shootRay.direction = selfTrans.forward;

            if (Physics.Raycast(shootRay, out shootHit, gunConfig.Range))
            {
                gunLine.SetPosition(1, shootHit.point);
                //挂载在同一物体上都会被识别为MonoBehaviour,故获取所有Components进行判断
                Health tmp_Health = Helper.GetComponent<Health>(typeof(Health), shootHit.collider.gameObject);
                if (tmp_Health)
                    tmp_Health.TakeDamage(gunConfig.DamagePerShot, shootHit.point);
            }
            else
            {
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * gunConfig.Range);
            }
        }

        private void GameOver(BaseNotificationData _base)
        {
            isGameOver = true;
            gunLight.enabled = false;
            faceLight.enabled = false;
            gunLine.enabled = false;
        }
    }
}