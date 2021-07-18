using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooter
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
            
            //Character should be follow joystick direction,
            //but when our camera position moved the character orientation will be wrong.
            //So we need calculate the direction between character and character.
            //Then superimpose two four elements to find the correct orientation.
            var tmp_Dir = cameraTrans.position - selfTrans.position;
            var tmp_NewQuaternion = Quaternion.LookRotation(tmp_Dir);
            tmp_NewQuaternion.x = tmp_NewQuaternion.z = 0;
            var tmp_FixQuaternion = tmp_NewQuaternion * inputSystem.LookPosition;
            selfTrans.rotation = Quaternion.LookRotation(tmp_FixQuaternion);
        }

        private void GameOver(BaseNotificationData _base)
        {
            isGameOver = true;
        }
    }
}