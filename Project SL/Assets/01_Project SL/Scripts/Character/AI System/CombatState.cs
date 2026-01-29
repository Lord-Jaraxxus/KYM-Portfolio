using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CombatState : AIStateBase
    {
        public override AIStateType StateType => AIStateType.Combat;

        private CharacterBase target;
        private float targetDistance; // 타겟과의 거리
        // [SerializeField] private float AttackRange = 2f; // 공격 범위 설정 (예: 1미터)

        private AICombatBehaviour combatBehaviour;

        public override void OnEnterState(AIBrain brain)
        {
            brain.AISensor.OnDetectedCharacterEvent -= OnCallbackDetectedCharacter; // 이벤트 중복 등록 방지
            brain.AISensor.OnDetectedCharacterEvent += OnCallbackDetectedCharacter; // 캐릭터 감지 이벤트 등록

            target = brain.AISensor.DetectedTarget;
            combatBehaviour = brain.GetComponent<AICombatBehaviour>();
        }

        public override void OnExitState(AIBrain brain)
        {
            brain.AISensor.OnDetectedCharacterEvent -= OnCallbackDetectedCharacter; // 캐릭터 감지 이벤트 해제

            target = null;
        }

        public override void OnUpdateState(AIBrain brain)
        {
            // TODO : 전투 상태에서의 적 캐릭터 행동 로직 구현
            if (target != null) // 타겟(플레이어)이 존재할 때
            {
                targetDistance = Vector3.Distance(brain.transform.position, target.transform.position); // 타겟과의 거리 계산

                if (combatBehaviour != null)
                {
                    combatBehaviour.Tick(brain, target, targetDistance);
                }

                //if (targetDistance > AttackRange)
                //{
                //    Chase(brain); // 타겟이 공격 범위를 벗어나면 추격
                //    // Debug.Log("Chasing Target. Distance: " + targetDistance);
                //}
                //else
                //{
                //    Attack(brain); // 타겟이 공격 범위 내에 있으면 공격
                //    // Debug.Log("Attacking Target. Distance: " + targetDistance);
                //}
            }
        }

        private void OnCallbackDetectedCharacter(CharacterBase character)
        {
            // TODO : 캐릭터(플레이어) 감지 시 처리 로직 구현 
        }

        private void Chase(AIBrain brain)
        {
            brain.AIController.SetDestination(transform.position); // Chase에 진입하면 AI의 목적지를 현재 위치로 설정합니다. (이동을 멈춰서 목적지를 초기화하기 위함)
            Vector3 chaseDestination = target.transform.position; // 타겟의 현재 위치를 추격 목적지로 설정
            brain.AIController.SetDestination(chaseDestination); // AIController를 통해 NavMeshAgent의 목표 위치 설정
        }
        private void Attack(AIBrain brain)
        {
            brain.AIController.SetDestination(transform.position); // Attack에 진입하면 AI의 목적지를 현재 위치로 설정합니다. (이동을 멈추기 위함)
            brain.AIController.LinkedCharacter.Rotate(target.transform.position); // 타겟의 위치를 바라보도록 회전
            brain.AIController.LinkedCharacter.Attack1(); // 공격 동작 실행, 일단은 이거라도..
        }
    }
}
