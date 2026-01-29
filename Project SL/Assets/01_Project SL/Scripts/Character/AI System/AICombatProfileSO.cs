using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public enum CombatSelectionMode
    {
        HighestPriority, // 가장 priority 높은 것 실행
        WeightedRandom,  // 후보 중 weight로 랜덤
        Sequence         // 보스 패턴처럼 순서대로
    }

    [CreateAssetMenu(fileName = "AICombatProfile", menuName = "PROJECT KYM/AI/CombatProfile")]
    public class AICombatProfileSO : ScriptableObject
    {
        public CombatSelectionMode selectionMode = CombatSelectionMode.HighestPriority;

        [Tooltip("전투 액션 목록")]
        public List<AICombatAction> actions = new();

        [Header("Fallback Move")]
        [Tooltip("실행 가능한 액션이 없을 때, 이 거리까지는 추격을 계속함")]
        public float chaseStopDistance = 2.2f;
    }
}
