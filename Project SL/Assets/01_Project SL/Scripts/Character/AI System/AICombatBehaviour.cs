using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    /// <summary>
    /// 몬스터 프리팹에 붙이는 컴포넌트.
    /// CombatState는 여기 Tick만 호출하고,
    /// "무슨 공격/행동을 할지"는 여기서 결정한다.
    /// </summary>
    public class AICombatBehaviour : MonoBehaviour
    {
        [SerializeField] private AICombatProfileSO profile;
        [SerializeField] private float keepDistanceDeadZone = 0.2f; // KeepDistance 행동 시, desiredDistance 근처에서 멈추기 위한 여유 구간 (버벅거림 방지용)

        // 액션별 쿨타임 관리(마지막 사용 시각)
        private readonly Dictionary<int, float> lastUsedTimeByIndex = new();
        private int sequenceIndex = 0;

        public void Tick(AIBrain brain, CharacterBase target, float distance)
        {
            if (profile == null || target == null)
            {
                // 프로필 없으면 그냥 추격만이라도
                FallbackChase(brain, target, distance);
                return;
            }

            // 1) 실행 가능한 후보 뽑기
            List<int> candidates = GetCandidates(distance);

            // 2) 후보가 없으면 fallback 이동(추격)
            if (candidates.Count == 0)
            {
                FallbackChase(brain, target, distance);
                return;
            }

            // 3) 선택
            int chosenIndex = ChooseActionIndex(candidates);
            AICombatAction action = profile.actions[chosenIndex];

            // 4) 실행
            ExecuteAction(brain, target, distance, chosenIndex, action);
        }

        private List<int> GetCandidates(float distance)
        {
            var result = new List<int>();
            for (int i = 0; i < profile.actions.Count; i++)
            {
                var a = profile.actions[i];
                if (!a.IsInRange(distance)) continue;
                if (!IsCooldownReady(i, a.cooldown)) continue;
                result.Add(i);
            }
            return result;
        }

        private int ChooseActionIndex(List<int> candidates)
        {
            switch (profile.selectionMode)
            {
                case CombatSelectionMode.Sequence:
                    // 시퀀스는 "가능한 것" 중에서 sequenceIndex부터 앞으로 찾기
                    for (int step = 0; step < profile.actions.Count; step++)
                    {
                        int idx = (sequenceIndex + step) % profile.actions.Count;
                        if (candidates.Contains(idx))
                        {
                            sequenceIndex = (idx + 1) % profile.actions.Count;
                            return idx;
                        }
                    }
                    return candidates[0];

                case CombatSelectionMode.WeightedRandom:
                    {
                        float total = 0f;
                        foreach (int idx in candidates) total += Mathf.Max(0.01f, profile.actions[idx].weight);

                        float roll = Random.value * total;
                        foreach (int idx in candidates)
                        {
                            roll -= Mathf.Max(0.01f, profile.actions[idx].weight);
                            if (roll <= 0f) return idx;
                        }
                        return candidates[0];
                    }

                case CombatSelectionMode.HighestPriority:
                default:
                    {
                        // 우선순위가 가장 높은 것 선택
                        int best = candidates[0];
                        int bestPri = profile.actions[best].priority;

                        for (int i = 1; i < candidates.Count; i++)
                        {
                            int idx = candidates[i];
                            int pri = profile.actions[idx].priority;
                            if (pri > bestPri)
                            {
                                bestPri = pri;
                                best = idx;
                            }
                        }
                        return best;
                    }
            }
        }

        private void ExecuteAction(AIBrain brain, CharacterBase target, float distance, int actionIndex, AICombatAction action)
        {
            // 공통: 바라보기
            brain.AIController.LinkedCharacter.Rotate(target.transform.position);

            switch (action.type)
            {
                case CombatActionType.MeleeAttack:
                    Stop(brain);
                    TriggerMelee(brain, action.meleeAttackIndex);
                    MarkUsed(actionIndex);
                    break;

                case CombatActionType.ProjectileSkill:
                    Stop(brain);
                    TriggerProjectileSkill(brain, action.skillId);
                    MarkUsed(actionIndex);
                    break;

                case CombatActionType.KeepDistance:
                    KeepDistance(brain, target, distance, action.desiredDistance, action.retreatStep);
                    // KeepDistance는 “행동”이라기보다 이동이니까 쿨타임 걸지 않아도 되는데,
                    // 원하면 cooldown 걸고 싶을 때만 MarkUsed로 바꾸면 됨.
                    break;
            }
        }

        private void TriggerMelee(AIBrain brain, int attackIndex)
        {
            // CharacterBase에 Attack1/2/3가 있으니 간단하게
            switch (attackIndex)
            {
                case 1: brain.AIController.LinkedCharacter.Attack2(); break;
                case 2: brain.AIController.LinkedCharacter.Attack3(); break;
                default: brain.AIController.LinkedCharacter.Attack1(); break;
            }
        }

        private void TriggerProjectileSkill(AIBrain brain, string skillId)
        {
            if (string.IsNullOrEmpty(skillId))
            {
                Debug.LogWarning("[AICombatBehaviour] skillId is empty.");
                return;
            }

            // 일단 임시로 이렇게 하드코딩;
            brain.AIController.LinkedCharacter.SetQSkill("1", 1); // skillId, skillLevel
            brain.AIController.LinkedCharacter.TryUseQSkill();
        }

        private void KeepDistance(AIBrain brain, CharacterBase target, float distance, float desiredDistance, float retreatStep)
        {
            if (distance >= desiredDistance)
            {
                // 충분히 멀면 멈춤
                Stop(brain);
                return;
            }

            // 뒤로 빠지기: (나 - 타겟) 방향으로 retreatStep 만큼 이동
            Vector3 away = (brain.transform.position - target.transform.position);
            away.y = 0f;
            if (away.sqrMagnitude < 0.001f) away = -brain.transform.forward;

            Vector3 destination = brain.transform.position + away.normalized * retreatStep;
            brain.AIController.SetDestination(destination);

            Debug.Log($"KeepDistance 하는중!");
        }

        private void FallbackChase(AIBrain brain, CharacterBase target, float distance)
        {
            if (target == null) return;

            // 너무 멀면 추격, 가까우면 멈춤
            if (profile != null && distance <= profile.chaseStopDistance)
            {
                Stop(brain);
                return;
            }

            brain.AIController.SetDestination(target.transform.position);
        }

        private void Stop(AIBrain brain)
        {
            // 지금 네 AIController 구조에서는 "현재 위치로 SetDestination"이 사실상 정지용으로 쓰이고 있으니 그대로 활용
            brain.AIController.SetDestination(brain.transform.position);
        }

        private bool IsCooldownReady(int index, float cooldown)
        {
            if (cooldown <= 0f) return true;

            if (!lastUsedTimeByIndex.TryGetValue(index, out float lastTime))
                return true;

            return (Time.time - lastTime) >= cooldown;
        }

        private void MarkUsed(int index)
        {
            lastUsedTimeByIndex[index] = Time.time;
        }
    }
}
