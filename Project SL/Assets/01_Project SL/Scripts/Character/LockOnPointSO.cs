using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "LockOnPointSO", menuName = "PROJECT KYM/LockOnPointSO")]
    public class LockOnPointSO : ScriptableObject
    {
        [field:SerializeField] public List<HumanBodyBones> TargetPoints = new List<HumanBodyBones>();
    }
}
