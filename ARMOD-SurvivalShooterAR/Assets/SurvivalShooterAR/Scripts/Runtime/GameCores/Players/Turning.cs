using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR
{
    public class Turning : IBehaviour
    {
        private readonly PlayerController playerController;
        private readonly Transform selfTrans;
        private bool isGameOver;
        private InputSystem inputSystem;
        private Transform cameraTrans;

        public Turning(PlayerController _playerController)
        {
            this.playerController = _playerController;
            this.selfTrans = _playerController.trans.GetChild(0);
            ActionNotificationCenter.DefaultCenter.AddObserver(GameOver, NotifyKeys.GAME_OVER);
            inputSystem = _playerController.inputSystem;
            cameraTrans = Camera.main.transform;
        }

        public void DoBehaviour()
        {
            if (isGameOver) return;
            if (inputSystem.LookPosition == Vector3.zero) return;
            var tmp_CamYAxis = (inputSystem.LookPosition - selfTrans.position).normalized;
            selfTrans.rotation = Quaternion.LookRotation(tmp_CamYAxis);
        }

        private void GameOver(BaseNotificationData _base)
        {
            isGameOver = true;
        }
    }
}