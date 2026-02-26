using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class StrollState : AIStateBase
    {
        public override AIStateType StateType => AIStateType.Stroll;

        [SerializeField] private Transform[] stopPoints; // 정지 지점 배열
        [SerializeField] private int currentStopIndex = 0; // 현재 정지 지점 인덱스
        [SerializeField] private float stopDuration = 3f; // 정지 시간

        private bool isStopped = false; // 정지 여부를 나타내는 변수
        private float stopTimer = 0f; // 정지 타이머

        private AIBrain ownerBrain;

        public override void OnEnterState(AIBrain brain)
        {
            ownerBrain = brain;
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 중복 등록 방지
            brain.AIController.OnDestinationReachedEvent += OnDestinationReached; // 목적지 도달 이벤트 등록

            currentStopIndex = 0;
            int index = currentStopIndex;
            Vector3 destination = stopPoints[index].position;
            brain.AIController.SetDestination(destination);
        }

        public override void OnExitState(AIBrain brain)
        {
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 해제

            Debug.Log("[PatrolState] OnExitState.");
        }

        public override void OnUpdateState(AIBrain brain)
        {
            if (isStopped) 
            {
                stopTimer += Time.deltaTime; // 정지 타이머 증가

                if(stopTimer >= stopDuration) // 정지 시간이 경과하면
                {
                    isStopped = false; // 정지 상태 해제
                    stopTimer = 0f; // 타이머 초기화

                    // 다음 목적지로 이동 시작
                    currentStopIndex++;
                    int index = currentStopIndex % stopPoints.Length; // 순환 인덱스 계산
                    Vector3 destination = stopPoints[index].position;
                    ownerBrain.AIController.SetDestination(destination);
                }
            }
        }


        private void OnDestinationReached()
        {
            isStopped = true; // 목적지에 도달하면 정지 상태로 전환

            GetComponentInParent<Animator>().SetFloat("Magnitude", 0f); // 목적지에 도달하면 애니메이션의 Magnitude 파라미터를 0으로 설정하여 대기 애니메이션이 재생되도록 함
        }
    }
}
