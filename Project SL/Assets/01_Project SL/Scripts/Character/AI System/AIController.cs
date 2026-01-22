using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KYM
{
    public class AIController : MonoBehaviour
    {
        public CharacterBase LinkedCharacter => character;

        [field: SerializeField] public string EnemyID { get; private set; } // 적 캐릭터 ID
        public System.Action OnDestinationReachedEvent;

        private NavMeshAgent navAgent;
        private CharacterBase character;

        [Header("Arrival Settings")]
        [SerializeField] private float stoppingDistance = 0.1f; // 목표 위치에 도달했을 때의 최소 거리
        [SerializeField] private float stopEpsilon = 0.03f; // 목표 위치에 도달했을 때의 허용 오차
        [SerializeField] private float resumeEpsilon = 0.12f; // 다시 움직일 때의 여유 거리

        private bool isStopped; // 현재 멈춰있는 상태인지 여부
        private bool arrivbalInvokedOnce; // 도착 이벤트가 한 번만 호출되도록 하는 Flag 값

        private void Awake()
        {
            character = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            navAgent.updatePosition = false;
            navAgent.updateRotation = false;
            navAgent.stoppingDistance = stoppingDistance;

            EnemyDataSO enemyData = GameDataModel.Singleton.EnemyDataDto.EnemyDatas.Find(EnemyDataDto => EnemyDataDto.StatData.ID == EnemyID);
            character.Initialize(enemyData.StatData, false);
        }

        private void Update()
        {
            navAgent.nextPosition = transform.position; // NavMeshAgent의 위치를 캐릭터의 위치로 업데이트

            // ?? 이건 무슨 조건이징
            if (navAgent.pathPending ||
                navAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
                navAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                Debug.Log("케이스 0");

                StopMovement();
                return;
            }

            float remainDistance = navAgent.remainingDistance; // NavMeshAgent가 목적지까지 남은 거리

            // isStopped가 false라면
            if (isStopped == false)
            {
                if (remainDistance <= (stoppingDistance + stopEpsilon)) // 목적지까지 남은 거리가 정지 거리 + 허용 오차 이하라면
                {
                    StopMovement();
                    InvokeArrivalOnce();
                    return;

                    // Debug.Log("케이스 1");
                }
            }
            else // isStopped가 true라면
            {
                if (remainDistance > (stoppingDistance + resumeEpsilon)) // 목적지까지 남은 거리가 정지 거리 + 재개 오차 초과라면 (계속 움직여야 한다면)
                {
                    isStopped = false;
                    navAgent.isStopped = false;

                    //  Debug.Log("케이스 2");
                }
                else // (정지 거리 + 허용 오차)와 (정지 거리 + 재개 오차) 사이에 있다면
                {
                    StopMovement();
                    return;

                    //Debug.Log("케이스 3");
                }
            }


            navAgent.isStopped = false; // 위에서 return 되지 않은 상태이면 움직여야하는 상태라고 판단

            Vector3 toConer = navAgent.steeringTarget - transform.position;
            toConer.y = 0f;

            if (toConer.sqrMagnitude < 0.001f)
            {
                StopMovement(); // 목표 위치가 너무 가까우면 움직임을 멈춤
            }

            Vector3 dir = toConer.normalized;
            Vector3 input = new Vector3(dir.x, 0f, dir.z);
            character.MoveAI(input); // 캐릭터 이동 업데이트

            // navAgent.destination; : 진짜 목적지
            // navAgent.steeringTarget; : 현재 향하고있는 목표 지점 (이동 중에 변경될 수 있음)
            // navAgent.nextPosition : NavMeshAgent의 다음 프레임에서의 위치
        }

        internal void SetDestination(Vector3 destination)
        {
            arrivbalInvokedOnce = false;
            isStopped = false;

            navAgent.isStopped = false;
            navAgent.SetDestination(destination);
        }

        private void StopMovement()
        {
            isStopped = true;
            navAgent.isStopped = true;
            // character.MoveAI(Vector3.zero); // 캐릭터 이동 멈춤
        }
        private void InvokeArrivalOnce()
        {
            if (arrivbalInvokedOnce) return;
            arrivbalInvokedOnce = true;
            OnDestinationReachedEvent?.Invoke(); // 도착 이벤트 호출

            // Debug.Log("[AIController] Destination Reached");
        }
    }
}
