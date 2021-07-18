using System.Collections;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalShooter
{
    public class UISystem : AbstractGameState
    {
        public GameObject GamingView;
        public GameObject StartView;
        public GameObject GameOverView;

        internal Button startGameButton;
        internal Button placedButton;

        internal Text healthText;
        internal Text aliveTimeText;
        internal Button closeButton;

        public override void GameInit(BaseNotificationData _data)
        {
            ActionNotificationCenter.DefaultCenter.AddObserver(OnTimeCount, ConstKey.CONST_TIME_COUNTER);
        }

        public override void GameStart(BaseNotificationData _data)
        {
            StartView.SetActive(false);
            GamingView.SetActive(true);
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
            StartView.SetActive(false);
            GamingView.SetActive(false);
            StartCoroutine(WaitToShowGameOverView());
        }

        public void OnHealthChange(int _currentHealth)
        {
            healthText.text = _currentHealth.ToString();
        }


        public void OnTimeCount(BaseNotificationData _data)
        {
            if (_data is TimeCounterNotificationData tmp_Data)
                aliveTimeText.text = $"{tmp_Data.counter} Seconds";
        }

        private void OnDisable()
        {
            ActionNotificationCenter.DefaultCenter.RemoveObserver(ConstKey.CONST_TIME_COUNTER);
        }

        private IEnumerator WaitToShowGameOverView()
        {
            yield return new WaitForSeconds(2);
            GameOverView.SetActive(true);
        }
    }

    public class TimeCounterNotificationData : BaseNotificationData
    {
        public int counter;
    }
}