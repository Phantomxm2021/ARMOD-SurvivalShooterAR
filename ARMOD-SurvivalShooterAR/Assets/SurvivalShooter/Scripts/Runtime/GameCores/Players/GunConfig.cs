using UnityEngine;

namespace SurvivalShooter
{
    [System.Serializable]
    public class GunConfig
    {
        public int DamagePerShot;
        public float TimeBetweenBullets;
        public float Range;
        public float GrenadeSpeed;
        public float GrenadeFireDelay;
        public GameObject Grenade;

        public GunConfig(int _damagePerShot = 20, float _timeBetweenBullets = 0.15f, float _range = 100f,
            float _grenadeSpeed = 200f, float _grenadeFireDelay = 0.5f, GameObject _grenade = null)
        {
            DamagePerShot = _damagePerShot;
            TimeBetweenBullets = _timeBetweenBullets;
            Range = _range;
            GrenadeSpeed = _grenadeSpeed;
            GrenadeFireDelay = _grenadeFireDelay;
            Grenade = _grenade;
        }
    }
}