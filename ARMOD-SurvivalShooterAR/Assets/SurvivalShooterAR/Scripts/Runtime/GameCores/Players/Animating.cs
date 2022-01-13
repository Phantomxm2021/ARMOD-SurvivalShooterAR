using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR
{
    public class Animating : IBehaviour
    {
        private static readonly int IS_WALKING = Animator.StringToHash("IsWalking");
        private static readonly int IS_DEAD = Animator.StringToHash("IsDead");
        private bool isGameOver;
        private readonly Animator anim;
        private readonly InputSystem inputSystem;

        public Animating(PlayerController _playerController)
        {
            inputSystem = _playerController.inputSystem;
            anim = _playerController.GetComponent<Animator>();
            ActionNotificationCenter.DefaultCenter.AddObserver(GameOver, NotifyKeys.GAME_OVER);
        }

        public void DoBehaviour()
        {
            if (isGameOver)
            {
                anim.SetBool(IS_DEAD, true);
            }
            else
            {
                var tmp_IsWalking = inputSystem.GetInputAxis != Vector3.zero;
                anim.SetBool(IS_WALKING, tmp_IsWalking);
            }
        }

        private void GameOver(BaseNotificationData _base)
        {
            isGameOver = true;
        }
    }
}