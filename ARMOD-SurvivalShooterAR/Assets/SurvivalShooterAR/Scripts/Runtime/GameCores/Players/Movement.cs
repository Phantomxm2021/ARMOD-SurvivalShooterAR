using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR.Runtime
{
    public class Movement : IBehaviour
    {
        private readonly Rigidbody rigidbody;
        private readonly InputSystem inputSystem;
        private readonly float movementSpeed;
        private Vector3 movementDirection;
        private bool isGameOver;
        private Transform mainCamera;

        public Movement(PlayerController _playerController)
        {
            rigidbody = _playerController.playerRigidbody;
            inputSystem = _playerController.inputSystem;
            movementSpeed = _playerController.Speed;
            movementDirection = Vector3.zero;
            ActionNotificationCenter.DefaultCenter.AddObserver(GameOver, NotifyKeys.GAME_OVER);
            mainCamera = Camera.main.transform;
        }


        public void DoBehaviour()
        {
            if (isGameOver) return;
            if (!inputSystem) return;
            movementDirection = inputSystem.GetInputAxis;
            var tmp_LocalPosition = rigidbody.position;
            movementDirection.y = tmp_LocalPosition.y;
            tmp_LocalPosition = Vector3.Lerp(tmp_LocalPosition, movementDirection, Time.deltaTime * movementSpeed);
            rigidbody.MovePosition(tmp_LocalPosition);
        }


        private void GameOver(BaseNotificationData _base)
        {
            isGameOver = true;
        }


    }
}