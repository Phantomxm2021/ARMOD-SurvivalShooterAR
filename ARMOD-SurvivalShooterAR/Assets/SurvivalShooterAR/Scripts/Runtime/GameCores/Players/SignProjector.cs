using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;

namespace SurvivalShooterAR.Runtime
{
    public class SignProjector : AbstractGameState
    {
        private GameObject signProjectorGO;
        private InputSystem inputSystem;
        private Transform signProjectorTrans;
        private Transform playerTrans;
        private Vector3 movementDirection;

        public override async void GameInit(BaseNotificationData _data)
        {
            var tmp_SignProjectorPrefab =
                await SurvivalShooterARMainEntry.API.LoadAssetAsync<GameObject>(ConstKey.CONST_SIGN_PROJECTOR);
            signProjectorGO = Instantiate(tmp_SignProjectorPrefab);
            signProjectorTrans = signProjectorGO.transform;
        }

        public override void GameStart(BaseNotificationData _data)
        {
            inputSystem = FindObjectOfType<InputSystem>();
            playerTrans = GameObject.FindWithTag(ConstKey.CONST_PLAYER_TAG).transform;
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
            if (signProjectorTrans == null || inputSystem == null) return;
            movementDirection = inputSystem.GetInputAxis;
            var tmp_LocalPosition = playerTrans.position;
            movementDirection.y = tmp_LocalPosition.y;
            tmp_LocalPosition = Vector3.Lerp(tmp_LocalPosition, movementDirection, Time.deltaTime * 15);
            signProjectorTrans.position = tmp_LocalPosition;
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
            signProjectorGO.SetActive(false);
            this.enabled = false;
        }
    }
}