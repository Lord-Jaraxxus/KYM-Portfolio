using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            Vector3 normal = (navAgent.steeringTarget - transform.position).normalized;
            Vector2 input = new Vector2(normal.x, normal.z);
            character.Move(input);
        }

    }
}
