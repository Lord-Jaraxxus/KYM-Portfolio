using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class AIBrain : MonoBehaviour
    {
        public AIController AIController => controller;
        public AISensor AISensor => sensor;

        [Header("State Componenets")]
        [SerializeField] private AIStateBase[] states;
        [SerializeField] private AIStateBase defaultState;

        [Header("AI State")]
        [SerializeField] private AIStateBase currentState;

        [Header("Third Party")]
        [SerializeField] private AISensor sensor; // AI 센서 컴포넌트
        [SerializeField] private AIController controller; // AI 컨트롤러 컴포넌트

        private Dictionary<AIStateType, AIStateBase> stateMap = new();

        private void Awake()
        {
            sensor = GetComponentInChildren<AISensor>();
            controller = GetComponent<AIController>();

            foreach (var state in states)   // states 배열을 순회하면서 상태를 딕셔너리에 추가
            {
                if (false == stateMap.ContainsKey(state.StateType))
                {
                    stateMap.Add(state.StateType, state);
                }
            }
        }

        private void Start()
        {
            ChangeState(defaultState);
            sensor.OnDetectedCharacterEvent += OnCallbackDetectedCharacter;
            sensor.OnLostCharacterEvent += OnCallbackLostCharacter;
        }
        private void Update()
        {
            currentState?.OnUpdateState(this);
        }

        private void ChangeState(AIStateBase newState)
        {
            if(currentState == newState)
                return;
            
            currentState?.OnExitState(this);
            currentState = newState;
            currentState?.OnEnterState(this);
        }


        private void OnCallbackDetectedCharacter(CharacterBase character)
        {
            if (character.gameObject.CompareTag("Player")) // AI가 플레이어를 감지했을 때 
            {
                // TODO : ChangeState => To CombatState
                if(stateMap.TryGetValue(AIStateType.Combat, out AIStateBase combatState))
                {
                    ChangeState(combatState);
                }
            }
        }

        private void OnCallbackLostCharacter(CharacterBase character)
        {
            if (character.gameObject.CompareTag("Player")) // AI가 플레이어를 놓쳤을 때 
            {
                // TODO : ChangeState => To PatrolState
                if(stateMap.TryGetValue(AIStateType.Patrol, out AIStateBase patrolState))
                {
                    ChangeState(patrolState);
                }
            }
        }
    }
}
