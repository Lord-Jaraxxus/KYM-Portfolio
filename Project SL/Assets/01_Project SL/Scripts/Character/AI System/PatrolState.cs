using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KYM
{
    public class PatrolState : AIStateBase
    {
        public override AIStateType StateType => AIStateType.Patrol;

        [SerializeField] private Transform[] patrolPoints; // 순찰 지점 배열
        [SerializeField] private int currentPatrolIndex = 0; // 현재 순찰 지점 인덱스

        private AIBrain ownerBrain;

        public override void OnEnterState(AIBrain brain)
        {
            ownerBrain = brain;
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 중복 등록 방지
            brain.AIController.OnDestinationReachedEvent += OnDestinationReached; // 목적지 도달 이벤트 등록

            currentPatrolIndex = 0;
            int index = currentPatrolIndex;
            Vector3 destination = patrolPoints[index].position;
            brain.AIController.SetDestination(destination);
        }

        public override void OnExitState(AIBrain brain)
        {
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 해제

            Debug.Log("[PatrolState] OnExitState.");
        }

        public override void OnUpdateState(AIBrain brain)
        {

        }

        private void OnDestinationReached()
        {
            currentPatrolIndex++;
            int index = currentPatrolIndex % patrolPoints.Length; // 순환 인덱스 계산
            Vector3 destination = patrolPoints[index].position;
            ownerBrain.AIController.SetDestination(destination);

            Debug.Log("[PatrolState] OnDestinationReached.");
        }
    }
}
