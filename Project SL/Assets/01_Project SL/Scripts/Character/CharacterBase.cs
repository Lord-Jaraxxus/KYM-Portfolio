using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace KYM
{
    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        Interact,
        Hit,
        Dead
    }

    public class CharacterBase : MonoBehaviour, IHittable
    {
        // Third Party...? 아무튼 뭐 애니메이터, 캐릭터 컨트롤러, 애니메이션 이벤트 리스너, 무기 등등 그런것들
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;

        public AnimationEventListener AnimationEventListener => animationEventListener;
        private AnimationEventListener animationEventListener { get; set; }

        [SerializeField] private Weapon weapon; // 일단 인스펙터에서 연결, 나중에 자동으로 바꿔도?

        // 이벤트들
        public event System.Action<float, float> OnHpChanged; // 체력 변경 이벤트 (CallBack), (현재 체력, 최대 체력)
        public event System.Action<float, float> OnSpChanged; // 스태미나 변경 이벤트 (CallBack), (현재 스태미나, 최대 스태미나)
        public event System.Action OnCharacterDeath; // 사망 이벤트 (CallBack)

        // 캐릭터 상태 관련 변수들
        [SerializeField] public CharacterState CurrentState { get; set; } = CharacterState.Idle;
        CharacterState[] moveBlockedStates = { CharacterState.Attack, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Move 동작 진입이 불가한 상태들
        CharacterState[] attackBlockedStates = { CharacterState.Attack, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Attack 동작 진입이 불가한 상태들
        CharacterState[] interactBlockedState = { CharacterState.Interact, CharacterState.Attack, CharacterState.Hit, CharacterState.Dead }; // 상호작용 동작 진입이 불가 상태들
        CharacterState[] hitBlockedStates = { CharacterState.Dead }; // 피격 동작 진입이 불가한 상태들

        // 캐릭터 스텟 관련 변수들
        private CharacterStatDataSO characterStat; // 캐릭터 스탯 데이터 (ScriptableObject)
        public float MaxHP => maxHP;
        public float CurHP => curHP;
        public float MaxSP => maxSP;
        public float CurSP => curSP;
        public float MoveSpeed => moveSpeed;

        [SerializeField] private float maxHP; // 최대 체력
        [SerializeField] private float curHP; // 현재 체력
        [SerializeField] private float maxSP; // 최대 스태미나
        [SerializeField] private float curSP; // 현재 스태미나
        [SerializeField] private float moveSpeed; // 이동 속도

        // 캐릭터 이동 + 카메라 관련 변수들
        public bool IsWalk { get; set; } = false;
        private float walkBlend;

        private Vector3 movementForward;
        private float verticalVelocity;
        private float targetRotation;
        private float rotationVelocity;
        private float rotationSmoothTime = 0.15f;
        private float smoothHorizontal;
        private float smoothVertical;

        private bool isStrafe = false;
        [SerializeField] private LockOnPointSO lockOnPointData;
        private List<Transform> lockOnPointContainer = new();


        private void Awake()
        {
            // 컴포넌트들 가져오기
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            animationEventListener = GetComponent<AnimationEventListener>();

            // State Machine Behaviour에 이 캐릭터 인스턴스 연결
            var attackState = animator.GetBehaviour<AttackStateMachineBehaviour>();
            attackState?.setCharacter(this);
            var interactState = animator.GetBehaviour<InteractStateMachineBehaviour>();
            interactState?.setCharacter(this);
            var hitState = animator.GetBehaviour<HitStateMachineBehaviour>();
            hitState?.setCharacter(this);
        }


        private void Start()
        {
            animationEventListener.OnReceiveAnimationEvent += OnCallbackReceiveAnimationEvent; // 애니메이션 이벤트 리스너 콜백 등록

            SetActiveRagdoll(false);

            curHP = MaxHP; // 초기 체력 설정
            curSP = MaxSP; // 초기 스태미나 설정

            InitializeLockOnPoint();

            if (lockOnPointData != null) CameraSystem.Instance.RegisterCharacter(this);
        }

        private void InitializeLockOnPoint()
        {
            if (lockOnPointData != null)
            {
                lockOnPointContainer.Clear();
                for (int i = 0; i < lockOnPointData.TargetPoints.Count; i++)
                {
                    HumanBodyBones tartgetBoneType = lockOnPointData.TargetPoints[i];
                    Transform targetBoneTransform = animator.GetBoneTransform(tartgetBoneType);
                    lockOnPointContainer.Add(targetBoneTransform);
                }
            }
        }

        public Transform GetLockOnPoint(int index)
        {
            return lockOnPointContainer[index % lockOnPointContainer.Count];
        }

        private void Update()
        {
            walkBlend = Mathf.Lerp(walkBlend, IsWalk ? 1f : 0f, Time.deltaTime);
            animator.SetFloat("Running", walkBlend);
        }

        private void SetActiveRagdoll(bool isActive)
        {
            animator.enabled = !isActive; // 랙돌이 제대로 일을 안해서 일단 킵..
            Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rigid in ragdollRigidbodies)
            {
                rigid.isKinematic = !isActive;
            }
        }

        public void Initialize(CharacterStatDataSO statDataSo, bool isPlayer)
        {
            this.characterStat = statDataSo; // 캐릭터 스탯 데이터 초기화

            this.maxHP = characterStat.MaxHP;
            this.curHP = characterStat.MaxHP;
            this.maxSP = characterStat.MaxSP;
            this.curSP = characterStat.MaxSP;
            this.moveSpeed = characterStat.MoveSpeed;
        }

        void OnCallbackReceiveAnimationEvent(string eventName)
        {
            switch (eventName)
            {
                case "EnableHitbox":
                    weapon?.EnableHitbox(); // 무기가 없으면 ?로 일단 거름
                    // Debug.Log("Enable Hitbox");
                    break;
                case "DisableHitbox":
                    weapon?.DisableHitbox();
                    // Debug.Log("Disable Hitbox");
                    break;
                case "EndCombo":
                    break;
            }
        }

        public void SetMovementForward(Vector3 forward)
        {
            movementForward = forward;
        }

        public void SetStrafe(bool strafe)
        {
            isStrafe = strafe;
            animator.SetFloat("Strafe", strafe ? 1f : 0f);
        }

        public void Move(Vector2 input)
        {
            if (moveBlockedStates.Contains(CurrentState)) // 해당 상태일 경우 Move 함수 종료
            {
                characterController.Move(Vector3.zero);
                animator.SetFloat("Magnitude", 0f);
                return;
            }

            float dt = Time.deltaTime;
            bool hasInput = input.sqrMagnitude > 0.0001f;

            // 1) 기준 전/우 벡터 (카메라 전방 수평 투영 → forward, 그에 직교 → right)
            Vector3 refForward = movementForward.sqrMagnitude > 1e-4f ? movementForward : transform.forward;
            refForward = Vector3.ProjectOnPlane(refForward, Vector3.up).normalized;
            Vector3 refRight = Vector3.Cross(Vector3.up, refForward); // 좌/우

            // 2) 입력을 월드 이동방향으로 변환 (W/S는 refForward, A/D는 refRight)
            Vector3 desiredDir = refForward * input.y + refRight * input.x;
            Vector3 moveDir = desiredDir.sqrMagnitude > 1e-4f ? desiredDir.normalized : Vector3.zero;

            // 3) 회전 처리
            if (hasInput)
            {
                // Strafe: 몸은 카메라 기준 전방(refForward)을 계속 바라봄
                // 일반: 입력 방향(moveDir)으로 몸을 돌림
                float desiredYaw = isStrafe
                    ? Mathf.Atan2(refForward.x, refForward.z) * Mathf.Rad2Deg
                    : Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

                float yaw = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    desiredYaw,
                    ref rotationVelocity,
                    rotationSmoothTime
                );

                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }

            // 4) 이동 (중력 없음) — 입력 크기에 따라 속도 보간을 원하면 곱해도 됨
            float speed = moveSpeed; // 필요 시: moveSpeed * Mathf.Clamp01(input.magnitude);
            Vector3 displacement = moveDir * speed * dt;
            characterController.Move(displacement);

            // 5) 애니메이터 파라미터 (Strafe 블렌딩/스틱 감 보정)
            smoothHorizontal = Mathf.Lerp(smoothHorizontal, input.x, dt * 10f);
            smoothVertical = Mathf.Lerp(smoothVertical, input.y, dt * 10f);

            animator.SetFloat("Magnitude", input.magnitude);
            animator.SetFloat("Horizontal", smoothHorizontal);
            animator.SetFloat("Vertical", smoothVertical);
        }

        public void MoveAI(Vector3 worldDir)
        {
            if (moveBlockedStates.Contains(CurrentState)) // 해당 상태일 경우 MoveAI 종료, AI도 공격하면서 이동하는거 막기 위함
            {
                characterController.Move(Vector3.zero);
                animator.SetFloat("Magnitude", 0f);
                return;
            }

            // 1) 입력 방향 체크
            bool hasInput = worldDir.sqrMagnitude > 0.0001f;
            Vector3 moveDir = hasInput ? worldDir.normalized : Vector3.zero;

            float dt = Time.deltaTime;

            // 2) AI 회전 — 입력 방향을 바라보도록
            if (hasInput)
            {
                float desiredYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

                float yaw = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    desiredYaw,
                    ref rotationVelocity,
                    rotationSmoothTime
                );

                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }

            // 3) 이동
            float speed = moveSpeed;
            Vector3 displacement = moveDir * speed * dt;
            characterController.Move(displacement);

            // 4) 애니메이션 — AI는 Magnitude 하나로 충분
            animator.SetFloat("Magnitude", hasInput ? 1f : 0f);
        }

        public void Rotate(Vector3 targetAimPoint)
        {
            if (moveBlockedStates.Contains(CurrentState)) { return; }   // AI가 공격하면서 회전하는 것을 막기 위함

            Vector3 aimTarget = targetAimPoint;
            aimTarget.y = transform.position.y;
            Vector3 pos = transform.position;
            Vector3 aimDirection = (aimTarget - pos).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        public void SetCharacterState(CharacterState state) 
        {
            CurrentState = state;
        }
        public bool CanInteract()
        {
            return !interactBlockedState.Contains(CurrentState); // 상호작용 가능한 상태인지 반환
        }
        public bool CanAttack()
        {
            return !attackBlockedStates.Contains(CurrentState); // 공격 가능한 상태인지 반환
        }

        public void TryInteract(InteractableType type)
        {
            if (interactBlockedState.Contains(CurrentState)) { return; } // 상호작용 불가 상태일 경우 Interact 함수 종료
            CurrentState = CharacterState.Interact; // 상호작용 상태로 전환, 줍기 애니메이션 전에 해줘야 여러번 줍는 버그가 안남..

            // TODO : 이 밑에서 이제 상호작용 종류를 switch문 같은걸로 나눠서 
            switch (type)
            {
                case InteractableType.DropItem: // 드롭 아이템과 상호작용 했을 때
                    Root(); // 상호작용 애니메이션 재생, 애니메이션이 끝나면 Idle 상태로 돌아감
                    break;
                case InteractableType.NPC_Merchant: // 상인 NPC와 상호작용 했을 때
                    // 뭐 별로 할거 없긴 함..
                    break;
                default:
                    break;
            }
        }

        public void Attack1()
        {
            //if (moveBlockedStates.Contains(CurrentState)) // 해당 상태일 경우 Move 함수 종료, AI도 공격하면서 이동하는거 막기 위함
            //{
            //    characterController.Move(Vector3.zero);
            //    animator.SetFloat("Magnitude", 0f);
            //    return;
            //}

            if (attackBlockedStates.Contains(CurrentState)) { return; }  // 해당 상태일 경우 Attack 함수 종료

            // CurrentState = CharacterState.Attack; // 일단 여기서 안해도 되긴 하는데, 버그 없던가?
            animator.ResetTrigger("AttackTrigger");
            animator.SetTrigger("AttackTrigger");

            // animator.SetInteger("AttackIndex", 0);
            Debug.Log("Attack!");
        }
        public void Attack2()
        {
            animator.SetTrigger("AttackTrigger");
            animator.SetInteger("AttackIndex", 1);
            // Debug.Log("Attack!");
        }
        public void Attack3()
        {
            animator.SetTrigger("AttackTrigger");
            animator.SetInteger("AttackIndex", 2);
            // Debug.Log("Attack!");
        }

        public void Root()
        {
            animator.SetTrigger("RootTrigger");
        }

        public void Die()
        {
            if (CurrentState == CharacterState.Dead) return; // 이미 사망 상태이면 종료
            CurrentState = CharacterState.Dead; // 사망 상태로 변경, 이건 굳이 애니메이션 연동 안해도 괜찮을듯?

            SetActiveRagdoll(true);

            animator.SetTrigger("DeathTrigger");
            Debug.Log($"{gameObject.name} is dead!");
        }

        public float TakeDamage(float damage)
        {
            curHP -= damage;
            curHP = Mathf.Clamp(curHP, 0f, MaxHP); // 체력 0 ~ 최대 체력 사이로 제한
            OnHpChanged?.Invoke(CurHP, MaxHP); // 체력 변경 이벤트 호출

            if (curHP <= 0)
            {
                curHP = 0;
                Die();
            }

            return CurHP;
        }

        public void Heal(float amount)
        {
            curHP += amount;
            curHP = Mathf.Clamp(curHP, 0f, MaxHP); // 체력 0 ~ 최대 체력 사이로 제한
            OnHpChanged?.Invoke(CurHP, MaxHP); // 체력 변경 이벤트 호출
        }

        public void OnHit(float damage)
        {
            if (hitBlockedStates.Contains(CurrentState)) { return; }   // 죽었는데 피격되면 안되니까

            // 현재 실행중일 수도 있는 애니메이션 트리거 초기화
            animator.ResetTrigger("AttackTrigger");
            animator.ResetTrigger("RootTrigger");
            animator.ResetTrigger("HitTrigger");

            // TODO : 피격 시 애니메이션 재생 -> StateMachineBehaviour에서 캐릭터 상태 피격상태로 전환
            animator.SetTrigger("HitTrigger");

            TakeDamage(damage);
        }
    }
}
