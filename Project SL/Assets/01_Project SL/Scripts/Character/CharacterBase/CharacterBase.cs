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

    public partial class CharacterBase : MonoBehaviour, IHittable
    {
        // Third Party...? 아무튼 뭐 애니메이터, 캐릭터 컨트롤러, 애니메이션 이벤트 리스너 등등 그런것들
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        public AnimationEventListener AnimationEventListener => animationEventListener;
        private AnimationEventListener animationEventListener { get; set; }

        // 무기 & 장비들
        [SerializeField] private WeaponHitbox weaponHitBox; // Awake에서 GetComponentInChildren으로 가져옴
        public List<CharacterEquipment> CharacterEquipments => characterEquipments;
        [SerializeField] private List<CharacterEquipment> characterEquipments = new List<CharacterEquipment>();


        // 이벤트들
        public event System.Action<float, float> OnHpChanged; // 체력 변경 이벤트 (CallBack), (현재 체력, 최대 체력)
        public event System.Action<float, float> OnSpChanged; // 스태미나 변경 이벤트 (CallBack), (현재 스태미나, 최대 스태미나)
        public event System.Action OnCharacterDeath; // 사망 이벤트 (CallBack)
        public event System.Action<ItemDataSO /*Before*/, ItemDataSO /*After*/> OnEquipChanged; // 장비 변경 이벤트

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

        // 장비와 관련되는 스텟 관련 변수들
        public float Attack => attack;
        public float Defense => defense;
        [SerializeField] private float attack; // 공격력
        [SerializeField] private float defense; // 방어력


        // 캐릭터 상태 관련 변수들
        [field: SerializeField] public CharacterState CurrentState { get; private set; } = CharacterState.Idle;
        CharacterState[] moveBlockedStates = { CharacterState.Attack, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Move 동작 진입이 불가한 상태들
        CharacterState[] attackBlockedStates = { CharacterState.Attack, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Attack 동작 진입이 불가한 상태들
        CharacterState[] interactBlockedState = { CharacterState.Interact, CharacterState.Attack, CharacterState.Hit, CharacterState.Dead }; // 상호작용 동작 진입이 불가 상태들
        CharacterState[] hitBlockedStates = { CharacterState.Dead }; // 피격 동작 진입이 불가한 상태들

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
            weaponHitBox = GetComponentInChildren<WeaponHitbox>();

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
                    weaponHitBox?.EnableHitbox(); // 무기가 없으면 ?로 일단 거름
                    // Debug.Log("Enable Hitbox");
                    break;
                case "DisableHitbox":
                    weaponHitBox?.DisableHitbox();
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


        public void SetCharacterState(CharacterState state) // 캐릭터 상태를 외부에서 변경하기 위한 함수
        {
            CurrentState = state;
        }

        public bool CanAttack()
        {
            return !attackBlockedStates.Contains(CurrentState); // 공격 가능한 상태인지 반환
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
            // Debug.Log("Attack!");
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
            // 방어력 계산, 일단 방어력만큼 고정값으로 데미지 깎이도록, 나중에 퍼센트 계산으로 갈수도?
            float finalDamage = Mathf.Max(0f, damage - Defense); // 0보다 작아지면 안되니까 방어력 뺀 값이 음수면 0으로 처리

            curHP -= finalDamage;
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

            //// 현재 실행중일 수도 있는 애니메이션 트리거 초기화 <- 필요없을지도?
            //animator.ResetTrigger("AttackTrigger");
            //animator.ResetTrigger("RootTrigger");
            //animator.ResetTrigger("HitTrigger");

            // TODO : 피격 시 애니메이션 재생 -> StateMachineBehaviour에서 캐릭터 상태 피격상태로 전환
            animator.SetTrigger("HitTrigger");

            weaponHitBox?.DisableHitbox(); // 피격 시 무기 히트박스 비활성화

            TakeDamage(damage);
        }


        private void ApplyEquipStat(ItemDataSO beforeEqiupSO, ItemDataSO newEquipSO)
        {
            // TODO : 장비에 따른 스텟 변경 로직 구현
            if (beforeEqiupSO == null) // 해당 슬롯에 새로 장착하는 거라면
            {
                attack += newEquipSO.EquipmentStat.Attack;
                defense += newEquipSO.EquipmentStat.Defense;
            }
            else if (newEquipSO == null) // 해당 슬롯의 장비를 해제하는 거라면
            {
                attack -= beforeEqiupSO.EquipmentStat.Attack;
                defense -= beforeEqiupSO.EquipmentStat.Defense;
            }
            else // 해당 슬롯의 장비를 변경하는 거라면
            {
                attack += newEquipSO.EquipmentStat.Attack - beforeEqiupSO.EquipmentStat.Attack;
                defense += newEquipSO.EquipmentStat.Defense - beforeEqiupSO.EquipmentStat.Defense;
            }

            weaponHitBox.damage = attack; // 무기 데미지 갱신, 일단 여기서 이렇게 간단하게만 해놓고 나중에 콤보/스킬 확장되면... 그때 다시 생각!
        }

        public void EquipItem(ItemDataSO newEquipSO)
        {
            // TODO : 장비템 착용 후 필요한 동작들

            // TODO : 캐릭터의 저기 장비 리스트에 우겨넣어야함

            // 같은 슬롯의 장비를 이미 장착하고 있다면, 변수로 가져옴.
            CharacterEquipment beforeEquip = characterEquipments.Find(e => e.equipSlotType == newEquipSO.EquipSlotType);
            ItemDataSO beforeEquipSO = beforeEquip?.itemDataSO;

            if (beforeEquip == null) // 해당 슬롯에 새로 장착하는 거라면
            {
                CharacterEquipment newEquip = gameObject.AddComponent<CharacterEquipment>(); // 장비 클래스를 새로 만듬
                newEquip.equipSlotType = newEquipSO.EquipSlotType; // 슬롯 타입 설정
                newEquip.ChangeEquipment(null, newEquipSO); // 장비 변경 메서드 호출 (이전 장비는 없으니 null)
                characterEquipments.Add(newEquip); // 캐릭터 장비 리스트에 추가
            }
            else // 변경이라면
            {
                beforeEquip.ChangeEquipment(beforeEquip.itemDataSO, newEquipSO); // 장비 변경 메서드 호출
            }

            // TODO : 실제로 아이템이 장착된 효과를 구현해야함 (ex - 장비에 따른 외형 변경, 능력치 변경 등)
            ApplyEquipStat(beforeEquipSO, newEquipSO);

            // 장착 아이템 변경 이벤트를 보냄, 지금은 UI와 플레이어컨트롤러가 받고있음
            OnEquipChanged?.Invoke(beforeEquipSO, newEquipSO);
        }

        public void UneqipItem(ItemDataSO itemDataSO)
        {
            // TODO : 장비탬 해제 후 필요한 동작들

            // 선택된 슬롯의 장비를 가져옴.
            CharacterEquipment beforeEquip = characterEquipments.Find(e => e.equipSlotType == itemDataSO.EquipSlotType);
            ItemDataSO beforeEquipSO = beforeEquip?.itemDataSO;
            beforeEquip.ChangeEquipment(beforeEquip.itemDataSO, null); // 장비 변경 메서드 호출

            // 장착 아이템 변경 이벤트를 보냄, 일단 지금은 UI만 받고있음
            OnEquipChanged?.Invoke(beforeEquipSO, null);

            // TODO : 실제로 아이템이 해제됐을 때 효과를 구현해야함
            ApplyEquipStat(beforeEquipSO, null);
        }
    }
}
