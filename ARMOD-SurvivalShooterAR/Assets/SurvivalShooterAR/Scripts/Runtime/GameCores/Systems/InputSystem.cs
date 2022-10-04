using System;
using System.Collections.Generic;
using com.Phantoms.ActionNotification.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurvivalShooterAR.Runtime
{
    public class InputSystem : AbstractGameState
    {
        public Vector3 GetInputAxis = Vector3.zero;
        public Vector3 LookPosition = Vector3.zero;
        internal Action Shooting;

        private List<JoystickSystem> joystickSystems;

        private Camera mainCamera;
        private EventTrigger eventTrigger;
        private bool isLongPressed;
        private Ray walkRay;
        private Vector3 screenPos;


        public void AddNewJoyStick(JoystickSystem _joystick)
        {
            joystickSystems ??= new List<JoystickSystem>();
            joystickSystems.Add(_joystick);
        }

        private void StartLongPressed(BaseEventData _arg0)
        {
            isLongPressed = true;
        }

        private void EndLongPressed(BaseEventData _arg0)
        {
            isLongPressed = false;
        }


        public override void GameInit(BaseNotificationData _data)
        {
            mainCamera = Camera.main;
            screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 1);
        }

        public override void GameStart(BaseNotificationData _data)
        {
            eventTrigger = FindObjectOfType<EventTrigger>();
            EventTrigger.Entry tmp_PointerUp = new EventTrigger.Entry {eventID = EventTriggerType.PointerUp};
            EventTrigger.Entry tmp_PointerDown = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};
            tmp_PointerUp.callback.AddListener(EndLongPressed);
            tmp_PointerDown.callback.AddListener(StartLongPressed);
            eventTrigger.triggers.Add(tmp_PointerUp);
            eventTrigger.triggers.Add(tmp_PointerDown);
         
            walkRay = new Ray();
        }

        public override void GameUpdate(BaseNotificationData _data)
        {
            walkRay = mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(walkRay, out RaycastHit tmp_Hit))
            {
                GetInputAxis = tmp_Hit.point;
            }


            if (joystickSystems == null) return;
            foreach (JoystickSystem tmp_JoystickSystem in joystickSystems)
            {
                switch (tmp_JoystickSystem.GetJoystickType)
                {
                    case JoystickType.Movement:
                        break;
                    case JoystickType.Turning:
                        LookPosition = new Vector3(tmp_JoystickSystem.Direction.x, 0, tmp_JoystickSystem.Direction.y);
                        break;
                }
            }

            if (isLongPressed)
            {
                Shooting?.Invoke();
            }
        }

        public override void GamePaused(BaseNotificationData _data)
        {
        }

        public override void GameOver(BaseNotificationData _data)
        {
        }
    }
}