using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

namespace KYM
{
    public class AIController : MonoBehaviour
    {
        [field: SerializeField] public string EnemyID { get; private set; } // 적 캐릭터 ID

        public Transform targetDestinationPoint;

        private NavMeshAgent navAgent;
        private CharacterBase character;

        private void Awake()
        {
            character = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            navAgent.updatePosition = false;
            navAgent.updateRotation = false;

            var statData = GameDataModel.Singleton.EnemyStatDto.GetEnemyStatData(EnemyID);
            character.Initialize(statData.EnemyStat, false);

            navAgent.SetDestination(targetDestinationPoint.position);
        }

        private void Update()
        {
            // navAgent.destination; : 진짜 목적지
            // navAgent.steeringTarget; : 현재 향하고있는 목표 지점 (이동 중에 변경될 수 있음)
            // navAgent.nextPosition : NavMeshAgent의 다음 프레임에서의 위치

            navAgent.nextPosition = transform.position; // NavMeshAgent의 위치를 캐릭터의 위치로 업데이트

            Vector3 normal = (navAgent.steeringTarget - transform.position).normalized;
            Vector3 input = new Vector3(normal.x, 0f, normal.z);

            character.MoveAI(input);
        }

    }
}
