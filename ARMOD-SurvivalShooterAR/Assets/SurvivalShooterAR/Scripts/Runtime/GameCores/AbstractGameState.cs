using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR.Runtime
{
    public abstract class AbstractGameState : MonoBehaviour
    {
        public abstract void GameInit(BaseNotificationData _data);
        public abstract void GameStart(BaseNotificationData _data);
        public abstract void GameUpdate(BaseNotificationData _data);
        public abstract void GamePaused(BaseNotificationData _data);
        public abstract void GameOver(BaseNotificationData _data);

        protected virtual void Awake()
        {
            ActionNotificationCenter.DefaultCenter.AddObserver(GameInit, NotifyKeys.GAME_INIT);
            ActionNotificationCenter.DefaultCenter.AddObserver(GameStart, NotifyKeys.GAME_START);
            ActionNotificationCenter.DefaultCenter.AddObserver(GameUpdate, NotifyKeys.GAME_UPDATE);
            ActionNotificationCenter.DefaultCenter.AddObserver(GamePaused, NotifyKeys.GAME_PAUSE);
            ActionNotificationCenter.DefaultCenter.AddObserver(GameOver, NotifyKeys.GAME_OVER);
        }
    }
}