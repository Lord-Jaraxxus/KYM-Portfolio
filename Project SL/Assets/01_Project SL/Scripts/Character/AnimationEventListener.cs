using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class AnimationEventListener : MonoBehaviour
    {
        public System.Action<string> OnReceiveAnimationEvent;
        public void OnReceiveEvent(string eventName)
        {
            OnReceiveAnimationEvent?.Invoke(eventName);
        }
    }
}
