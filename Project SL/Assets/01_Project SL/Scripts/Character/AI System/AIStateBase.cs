using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public enum AIStateType
    {
        None = 0,
        Patrol = 1,
        Combat = 2,
        Chase = 3,
    }

    public abstract class AIStateBase : MonoBehaviour
    {
        public abstract AIStateType StateType { get; }
        public abstract void OnEnterState(AIBrain brain);
        public abstract void OnExitState(AIBrain brain);
        public abstract void OnUpdateState(AIBrain brain);
    }
}
