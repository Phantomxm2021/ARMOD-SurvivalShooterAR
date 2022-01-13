using System;
using UnityEngine;

namespace SurvivalShooterAR
{
    public class Helper : MonoBehaviour
    {
        public static T GetComponent<T>(Type _type, GameObject _target)
        {
            var tmp_AllComponents = _target.GetComponents<T>();
            foreach (var tmp_MonoBehaviour in tmp_AllComponents)
            {
                if (tmp_MonoBehaviour.GetType() == typeof(T))
                {
                    return tmp_MonoBehaviour;
                }
            }

            return default;
        }


        public static T FindTypes<T>(Type _type) where T : UnityEngine.Object
        {
            var tmp_AllComponents = GameObject.FindObjectsOfType<T>();
            foreach (var tmp_Component in tmp_AllComponents)
            {
                if (tmp_Component.GetType() == _type)
                {
                    return tmp_Component;
                }
            }

            return default;
        }
    }
}