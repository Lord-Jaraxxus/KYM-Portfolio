using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class WaitState : AIStateBase
    {
        public override AIStateType StateType => AIStateType.Wait;
        private Vector3 waitPoint; // 원래 있던 대기 지점

        private void Start()
        {
            waitPoint = transform.position; // 대기 지점 위치를 현재 위치로 초기화하여 처음 대기 시 원래 있던 위치로 이동하도록 설정
        }

        public override void OnEnterState(AIBrain brain)
        {
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 중복 등록 방지
            brain.AIController.OnDestinationReachedEvent += OnDestinationReached; // 목적지 도달 이벤트 등록
            brain.AIController.SetDestination(waitPoint);  // 대기 지점으로 이동
        }

        public override void OnExitState(AIBrain brain)
        {
            brain.AIController.OnDestinationReachedEvent -= OnDestinationReached; // 이벤트 해제

            waitPoint = brain.transform.position; // 대기 지점 위치를 현재 위치로 업데이트하여 다음 대기 시 원래 있던 위치로 이동하도록 설정
        }

        public override void OnUpdateState(AIBrain brain)
        {

        }

        private void OnDestinationReached() 
        {
            GetComponentInParent<Animator>().SetFloat("Magnitude", 0f); // 목적지에 도달하면 애니메이션의 Magnitude 파라미터를 0으로 설정하여 대기 애니메이션이 재생되도록 함
        }
    }
}
