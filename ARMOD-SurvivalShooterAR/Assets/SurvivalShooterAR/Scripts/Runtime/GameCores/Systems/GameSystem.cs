using com.Phantoms.ActionNotification.Runtime;
using UnityEngine.AI;

namespace SurvivalShooterAR
{
    public class GameSystem : AbstractGameState
    {
        private NavMeshSurface navMeshSurface;

        public override void GameInit(BaseNotificationData _data)
        {
        }

        public override void GameStart(BaseNotificationData _data)
        {
            navMeshSurface = FindObjectOfType<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
        }
    }
}