using UnityEngine;
using System.Collections.Generic;
using com.Phantoms.ARMODAPI.Runtime;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SurvivalShooterAR 
{
    public class SurvivalShooterARMainEntry
    {
        private InputSystem inputSystem;
        internal static EnemyPool EnemyPool;
        internal static readonly API API = new API(nameof(SurvivalShooterAR));

        private List<GameObject> enemies;
        private GameObject player;
        private GameObject canvas;
        private GameObject focusGameObject;
        private Transform focusGroupTrans;
        private GameObject focusFoundState;
        private GameObject focusFindingState;
        private GameObject environmentGameObject;
        private Transform contentTrans;
        private UISystem uiSystem;

        private AudioClip bunyHurt;
        private AudioClip bearHurt;
        private AudioClip hellephantHurt;
        private AudioClip bunyDead;
        private AudioClip bearDead;
        private AudioClip hellephantDead;
        private AudioClip playerHurtClip;
        private AudioClip playerDeathClip;

        private bool canPlaced;
        private static bool GAME_STARTED;

        public void OnLoad()
        {
            enemies = new List<GameObject>();
            canPlaced = true;
            LoadAssets();
        }

        private async void LoadAssets()
        {
            ActionNotificationCenter.DefaultCenter.AddObserver(PlaceVirtualObject,nameof(PlaceVirtualObject));

            
            //Load environment 
            var tmp_EnvironmentPrefab = await API.LoadAssetAsync<GameObject>(ConstKey.CONST_PROJECT_NAME);
            environmentGameObject = UnityEngine.Object.Instantiate(tmp_EnvironmentPrefab);
            contentTrans = environmentGameObject.transform;

            //load canvas group
            var tmp_CanvasPrefab = await API.LoadAssetAsync<GameObject>(ConstKey.CONST_CANVAS_GROUP);
            canvas = Object.Instantiate(tmp_CanvasPrefab);
            uiSystem = Object.FindObjectOfType<UISystem>();

            //Load focus group
            var tmp_FocusGroup = await API.LoadAssetAsync<GameObject>(ConstKey.CONST_FOCUS_GROUP);
            focusGameObject = Object.Instantiate(tmp_FocusGroup);
            focusGameObject.SetActive(false);
            focusGroupTrans = focusGameObject.transform;
            focusGroupTrans.localPosition = Vector3.zero;
            focusGroupTrans.localRotation = Quaternion.identity;
            focusFoundState = GameObject.Find(ConstKey.CONST_FOUND);
            focusFindingState = GameObject.Find(ConstKey.CONST_FIND);
            
            //Load audio clips
            bunyHurt = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_ZOMBUNNY_HURT_AUDIO);
            bunyDead = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_ZOMBUNNY_DEAD_AUDIO);
            
            bearHurt = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_ZOMBEAR_HURT_AUDIO);
            bearDead = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_ZOMBEAR_DEAD_AUDIO);
            
            hellephantHurt = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_HELLEPHANT_HURT_AUDIO);
            hellephantDead = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_HELLEPHANT_DEAD_AUDIO);
            
            playerDeathClip = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_PLAYER_DEAD_AUDIO);
            playerHurtClip = await API.LoadAssetAsync<AudioClip>(ConstKey.CONST_PLAYER_HURT_AUDIO);
            
            //Load Enemies prefab
            foreach (string tmp_EnemyName in ConstKey.EnemyNames)
            {
                enemies.Add(await API.LoadAssetAsync<GameObject>(tmp_EnemyName));
            }
        }


        public void OnUpdate()
        {
            //OnUpdate is called once pre frame
            if (GAME_STARTED)
                ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_UPDATE, null);
        }

        public void ReleaseMemory()
        {
            //Cleanup the memory when this script disabled
            ActionNotificationCenter.DefaultCenter.RemoveObserver(NotifyKeys.GAME_INIT);
            ActionNotificationCenter.DefaultCenter.RemoveObserver(NotifyKeys.GAME_START);
            ActionNotificationCenter.DefaultCenter.RemoveObserver(NotifyKeys.GAME_UPDATE);
            ActionNotificationCenter.DefaultCenter.RemoveObserver(NotifyKeys.GAME_PAUSE);
            ActionNotificationCenter.DefaultCenter.RemoveObserver(NotifyKeys.GAME_OVER);
        }


        public void OnEvent(BaseNotificationData _data)
        {
            //General event callback
            if (!canPlaced) return;
            if (!(_data is FocusResultNotificationData tmp_Data)) return;
            if (focusGameObject == null || focusGroupTrans == null) return;

            if (focusGameObject)
                focusGameObject.SetActive(tmp_Data.FocusState != FindingType.Limit);

            if (focusFoundState)
                focusFoundState.SetActive(tmp_Data.FocusState != FindingType.Finding);


            if (focusFindingState)
                focusFindingState.SetActive(tmp_Data.FocusState != FindingType.Found);

            if (uiSystem && uiSystem.placedButton)
                uiSystem.placedButton.gameObject.SetActive(tmp_Data.FocusState != FindingType.Finding &&
                                                           tmp_Data.FocusState != FindingType.Limit);

            if (!focusGroupTrans) return;
            focusGroupTrans.position = tmp_Data.FocusPos;
            focusGroupTrans.rotation = tmp_Data.FocusRot;
        }


        private void PlaceVirtualObject(BaseNotificationData _data)
        {
            canPlaced = false;
            focusGameObject.SetActive(false);
            contentTrans.localRotation = Quaternion.identity;
            contentTrans.gameObject.SetActive(true);
            
            contentTrans.GetChild(0).gameObject.SetActive(true);
            contentTrans.GetChild(1).gameObject.SetActive(true);
            contentTrans.position = focusGroupTrans.position;
            CreateEnemySpawner();
            CreateGameManagerSystem();
            CreatePlayer();
            //Game all asset is ready!
            ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_INIT, null);
            ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_START, null);
            GAME_STARTED = true;
        }
        
        
        private void CreatePlayer()
        {
            player =GameObject.Find(ConstKey.CONST_PLAYER_TAG);
            var tmp_PlayerController = player.AddComponent<PlayerController>();
            //Init Player Controller Component
            var tmp_GunBarrel = GameObject.Find(ConstKey.CONST_GUN_BARREL_END);
            tmp_PlayerController.gunConfig = new GunConfig();
            tmp_PlayerController.Speed = 2f;
            tmp_PlayerController.faceLight = GameObject.Find(ConstKey.CONST_FACE_LIGHT).GetComponent<Light>();
            tmp_PlayerController.gunLight = tmp_GunBarrel.GetComponent<Light>();
            tmp_PlayerController.gunAudio = tmp_GunBarrel.GetComponent<AudioSource>();
            tmp_PlayerController.gunMuzzle = tmp_GunBarrel.transform;
            tmp_PlayerController.gunParticles = tmp_GunBarrel.GetComponent<ParticleSystem>();
            tmp_PlayerController.gunLine = tmp_GunBarrel.GetComponent<LineRenderer>();


            player.AddComponent<PlayerTag>();
            player.AddComponent<SignProjector>();


            //Init Health Component
            var tmp_PlayerHealth = player.AddComponent<Health>();
            tmp_PlayerHealth.SetHurtClip(playerHurtClip);
            tmp_PlayerHealth.SetDeathClip(playerDeathClip);
            tmp_PlayerHealth.SetHealth(100);
            tmp_PlayerHealth.audioSource = player.GetComponent<AudioSource>();

            tmp_PlayerHealth.healthChange = uiSystem.OnHealthChange;


            player.SetActive(true);
        }


        private void CreateEnemySpawner()
        {
            GameObject tmp_EnemySpawner =GameObject.Find(ConstKey.CONST_ENEMY_SPAWNER);
            var tmp_SpawnerSystem = tmp_EnemySpawner.AddComponent<EnemySpawnSystem>();
            tmp_SpawnerSystem.SpawnTime = Random.Range(1.5f, 5f);

            List<PoolConfig> tmp_PoolConfigs = new List<PoolConfig>();


            foreach (GameObject tmp_EnemyPrefab in enemies)
            {
                if (tmp_EnemyPrefab.name.Contains(ConstKey.CONST_ENEMY_BUNNY_TAG))
                {
                    var tmp_Config = new EnemyDataConfig
                    {
                        visionRange = 8,
                        hearingRange = 20,
                        wanderDistance = 40,
                        idleTimeRange = new Vector2(.5f, 2),
                        EnemyType = EnemyDataConfig.EnemyCategory.Bunny
                    };

                    var tmp_PoolConfig = new PoolConfig(45, 10)
                        {EnemyCategory = EnemyDataConfig.EnemyCategory.Bunny};
                    tmp_PoolConfigs.Add(tmp_PoolConfig);

                    InstanceEnemies(tmp_EnemyPrefab, tmp_EnemySpawner,
                        tmp_Config, tmp_PoolConfig, 10, bunyHurt, bunyDead, 60);
                }
                else if (tmp_EnemyPrefab.name.Contains(ConstKey.CONST_ENEMY_BEAR_TAG))
                {
                    var tmp_Config = new EnemyDataConfig
                    {
                        visionRange = 10,
                        hearingRange = 16,
                        wanderDistance = 28,
                        idleTimeRange = new Vector2(1f, 4f),
                        EnemyType = EnemyDataConfig.EnemyCategory.Bear
                    };
                    var tmp_PoolConfig = new PoolConfig(40, 10)
                        {EnemyCategory = EnemyDataConfig.EnemyCategory.Bear};
                    tmp_PoolConfigs.Add(tmp_PoolConfig);
                    InstanceEnemies(tmp_EnemyPrefab, tmp_EnemySpawner,
                        tmp_Config, tmp_PoolConfig, 10, bearHurt, bearDead, 100);
                }
                else
                {
                    var tmp_Config = new EnemyDataConfig
                    {
                        visionRange = 6,
                        hearingRange = 32,
                        wanderDistance = 28,
                        idleTimeRange = new Vector2(2f, 6f),
                        EnemyType = EnemyDataConfig.EnemyCategory.Hellephant
                    };

                    var tmp_PoolConfig = new PoolConfig(5, 5)
                        {EnemyCategory = EnemyDataConfig.EnemyCategory.Hellephant};
                    tmp_PoolConfigs.Add(tmp_PoolConfig);

                    InstanceEnemies(tmp_EnemyPrefab, tmp_EnemySpawner,
                        tmp_Config, tmp_PoolConfig, 5, hellephantHurt, hellephantDead, 300);
                }
            }

            EnemyPool = new EnemyPool(tmp_PoolConfigs);
        }

        private static void InstanceEnemies(GameObject _enemyPrefab, GameObject _enemySpawner,
            EnemyDataConfig _config, PoolConfig _poolConfig, int _count, AudioClip _hurtClip, AudioClip _deaAudioClip,
            int _health)
        {
            for (int tmp_Idx = 0; tmp_Idx < _count; tmp_Idx++)
            {
                var tmp_Enemy = Object.Instantiate(_enemyPrefab, _enemySpawner.transform);
                tmp_Enemy.AddComponent<EnemyTag>();
                var tmp_EnemyController = tmp_Enemy.AddComponent<EnemyController>();
                var tmp_EnemyHealth = tmp_Enemy.AddComponent<Health>();
                tmp_EnemyController.Config = _config;
                tmp_EnemyHealth.SetHealth(_health);
                tmp_Enemy.transform.localScale = Vector3.one;
                tmp_Enemy.SetActive(false);
                tmp_EnemyHealth.SetHurtClip(_hurtClip);
                tmp_EnemyHealth.SetDeathClip(_deaAudioClip);

                _poolConfig.SetNewEnemy(tmp_Enemy);
            }
        }


        private void CreateGameManagerSystem()
        {
            GameObject tmp_GameSystem = new GameObject(ConstKey.CONST_GAME_SYSTEM_TAG);
            tmp_GameSystem.AddComponent<GameSystem>();
        }
    }
}