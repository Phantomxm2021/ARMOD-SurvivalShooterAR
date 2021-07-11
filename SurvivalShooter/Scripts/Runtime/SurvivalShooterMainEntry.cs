using System;
using UnityEngine;
using System.Collections.Generic;
using com.Phantoms.ActionNotification.Runtime;
using com.Phantoms.ARMODAPI.Runtime;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SurvivalShooter
{
    public class SurvivalShooterMainEntry
    {
        private InputSystem inputSystem;
        internal static EnemyPool EnemyPool;
        internal static readonly API API = new API();

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

        private void LoadAssets()
        {
            //Load environment 
            API.LoadGameObject(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_PROJECT_NAME, _env =>
            {
                environmentGameObject = API.InstanceGameObject(_env, "", null);
                contentTrans = environmentGameObject.transform;
            });

            //load canvas group
            API.LoadGameObject(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_CANVAS_GROUP, _canvasPrefab =>
            {
                canvas = API.InstanceGameObject(_canvasPrefab, "", null);
                uiSystem = canvas.AddComponent<UISystem>();
                inputSystem = canvas.AddComponent<InputSystem>();

                //Make Virtual Joystick
                var tmp_ShootingJoystickSystem =
                    API.FindGameObjectByName(ConstKey.CONST_SHOOTING_JOYSTICK_HANDLER).AddComponent<JoystickSystem>();
                tmp_ShootingJoystickSystem.GetJoystickType = JoystickType.Turning;

                inputSystem.AddNewJoyStick(tmp_ShootingJoystickSystem);

                uiSystem.GamingView = API.FindGameObjectByName(ConstKey.CONST_GAME_VIEW);
                uiSystem.StartView = API.FindGameObjectByName(ConstKey.CONST_START_VIEW);
                uiSystem.GameOverView = API.FindGameObjectByName(ConstKey.CONST_END_VIEW);
                uiSystem.healthText = API.FindGameObjectByName(ConstKey.CONST_HEALTH_TEXT).GetComponent<Text>();
                var tmp_SlamView = API.FindGameObjectByName(ConstKey.CONST_SLAM_VIEW);
                uiSystem.startGameButton = API.FindGameObjectByName(ConstKey.CONST_START_GAME).GetComponent<Button>();
                uiSystem.placedButton = API.FindGameObjectByName(ConstKey.CONST_PLACED).GetComponent<Button>();
                uiSystem.aliveTimeText = API.FindGameObjectByName(ConstKey.CONST_ALIVE_TIME_TEXT).GetComponent<Text>();

                uiSystem.closeButton = API.FindGameObjectByName(ConstKey.CONST_EXIT_AR).GetComponent<Button>();
                uiSystem.closeButton.onClick.AddListener(() => { API.ExitAR(); });

                uiSystem.placedButton.onClick.AddListener(() =>
                {
                    canPlaced = false;
                    focusGameObject.SetActive(false);
                    API.FindGameObjectByName(ConstKey.CONST_GROUND).SetActive(true);
                    API.FindGameObjectByName(ConstKey.CONST_DESKTOP_SCALE).SetActive(true);
                    tmp_SlamView.SetActive(false);
                    contentTrans.position = focusGroupTrans.position;
                    contentTrans.rotation = Quaternion.identity;
                    environmentGameObject.SetActive(true);
                    CreateEnemySpawner();
                    CreateGameManagerSystem();
                    CreatePlayer();
                    //Game all asset is ready!
                    ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_INIT, null);
                    ActionNotificationCenter.DefaultCenter.PostNotification(NotifyKeys.GAME_START, null);
                    GAME_STARTED = true;
                });

                uiSystem.startGameButton.onClick.AddListener(() =>
                {
                    //Load Focus 
                    uiSystem.StartView.SetActive(false);
                    tmp_SlamView.SetActive(true);
                    canPlaced = true;
                });
            });

            //Load focus group
            API.LoadGameObject(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_FOCUS_GROUP, _focusGroup =>
            {
                focusGameObject = API.InstanceGameObject(_focusGroup, "", null);
                focusGameObject.SetActive(false);
                focusGroupTrans = focusGameObject.transform;
                focusGroupTrans.localPosition = Vector3.zero;
                focusGroupTrans.localRotation = Quaternion.identity;
                focusFoundState = API.FindGameObjectByName(ConstKey.CONST_FOUND);
                focusFindingState = API.FindGameObjectByName(ConstKey.CONST_FIND);
            });


            //Load audio clips
            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_ZOMBUNNY_HURT_AUDIO,
                _zomBunnyHurt => bunyHurt = _zomBunnyHurt);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_ZOMBUNNY_DEAD_AUDIO,
                _zomBunnyDead => bunyDead = _zomBunnyDead);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_ZOMBEAR_HURT_AUDIO,
                _zomBearHurt => bearHurt = _zomBearHurt);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_ZOMBEAR_DEAD_AUDIO,
                _zomBearDead => bearDead = _zomBearDead);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_HELLEPHANT_HURT_AUDIO,
                _hellephantHurt => hellephantHurt = _hellephantHurt);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_HELLEPHANT_DEAD_AUDIO,
                _zomHellephantDead => hellephantDead = _zomHellephantDead);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_PLAYER_DEAD_AUDIO,
                _playerDead => playerDeathClip = _playerDead);

            API.LoadAudioClip(ConstKey.CONST_PROJECT_NAME, ConstKey.CONST_PLAYER_HURT_AUDIO,
                _playerHurt => playerHurtClip = _playerHurt);

            //Load Enemies prefab
            foreach (string tmp_EnemyName in ConstKey.EnemyNames)
            {
                API.LoadGameObject(ConstKey.CONST_PROJECT_NAME, tmp_EnemyName,
                    _enemyPrefab => enemies.Add(_enemyPrefab));
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

            switch (tmp_Data.FocusState)
            {
                case FindingType.Finding:
                    SetFocusVisualizerRenderState(true, true, false);
                    break;
                case FindingType.Found:
                    SetFocusVisualizerRenderState(true, false, true);
                    break;
                case FindingType.Limit:
                    if (focusGameObject)
                        focusGameObject.SetActive(false);
                    break;
            }

            if (uiSystem && uiSystem.placedButton)
                uiSystem.placedButton.gameObject.SetActive(tmp_Data.FocusState != FindingType.Finding
                                                           && tmp_Data.FocusState != FindingType.Limit);

            if (!focusGroupTrans) return;
            focusGroupTrans.position = tmp_Data.FocusPos;
            focusGroupTrans.rotation = tmp_Data.FocusRot;
        }

        private void SetFocusVisualizerRenderState(bool _focusGroupRenderState, bool _focusFindingRenderState,
            bool _focusFoundRenderState)
        {
            if (focusGameObject)
                focusGameObject.SetActive(_focusGroupRenderState);
            if (focusFindingState)
                focusFindingState.SetActive(_focusFindingRenderState);
            if (focusFoundState)
                focusFoundState.SetActive(_focusFoundRenderState);
        }

        private void CreatePlayer()
        {
            player = API.FindGameObjectByName(ConstKey.CONST_PLAYER_TAG);
            var tmp_PlayerController = player.AddComponent<PlayerController>();
            //Init Player Controller Component
            var tmp_GunBarrel = API.FindGameObjectByName(ConstKey.CONST_GUN_BARREL_END);
            tmp_PlayerController.gunConfig = new GunConfig();
            tmp_PlayerController.Speed = 2f;
            tmp_PlayerController.faceLight = API.FindGameObjectByName(ConstKey.CONST_FACE_LIGHT).GetComponent<Light>();
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
            GameObject tmp_EnemySpawner = API.FindGameObjectByName(ConstKey.CONST_ENEMY_SPAWNER);
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