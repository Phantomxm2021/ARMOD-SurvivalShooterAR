using UnityEngine;
using UnityEngine.EventSystems;

namespace SurvivalShooter
{
    public enum JoystickType : int
    {
        None = 0,
        Movement,
        Turning
    }
    public class JoystickSystem : MonoBehaviour
    {
        public Vector2 Direction;
        private RectTransform joystickHandlerRectTransform;
        private Vector2 joystickHandlerOriginPosition;
        public float MaxRadius = 100;
        public JoystickType GetJoystickType;
        private Transform player;
        private Transform camTrans;

        private void Awake()
        {
            MaxRadius = 100;
            joystickHandlerRectTransform = GetComponent<RectTransform>();
            joystickHandlerOriginPosition = joystickHandlerRectTransform.position;
            player = SurvivalShooterMainEntry.API.FindGameObjectByName("Player").transform;
            camTrans = Camera.main.transform;
        }

        public void OnPointerDown(PointerEventData _eventData)
        {
            OnDrag(_eventData);
        }

        public void OnPointerUp(PointerEventData _eventData)
        {
            joystickHandlerRectTransform.position = joystickHandlerOriginPosition;
            Direction = Vector2.zero;
        }

        public void OnDrag(PointerEventData _eventData)
        {
            Direction = _eventData.position - joystickHandlerOriginPosition;
            var tmp_LengthOfRadius = Direction.magnitude;
            var tmp_Radius = Mathf.Clamp(tmp_LengthOfRadius, 0, MaxRadius);
            var tmp_DragTargetPosition = joystickHandlerOriginPosition + Direction.normalized * tmp_Radius;
            joystickHandlerRectTransform.position = tmp_DragTargetPosition;
        }


        //avoid Incorrect orientation in first time
        public void OnBeginDrag(PointerEventData _eventData)
        {
            var tmp_Position = player.position - camTrans.position;
            player.LookAt(tmp_Position);
        }
    }
}