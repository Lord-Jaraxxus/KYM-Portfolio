using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        Cast,
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
        public event System.Action<bool /*isPlayerCharacter*/, string /*CharacterID*/, Transform /*CharacterTransform*/> OnCharacterDeath; // 사망 이벤트 (CallBack)
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
        [SerializeField] private float moveSpeed = 2.0f; // 이동 속도

        // 장비와 관련되는 스텟 관련 변수들
        public float Attack => attack;
        public float Defense => defense;
        [SerializeField] private float attack; // 공격력
        [SerializeField] private float defense; // 방어력


        // 캐릭터 상태 관련 변수들
        [field: SerializeField] public CharacterState CurrentState { get; private set; } = CharacterState.Idle;
        CharacterState[] moveBlockedStates = { CharacterState.Attack, CharacterState.Cast, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Move 동작 진입이 불가한 상태들
        CharacterState[] attackBlockedStates = { CharacterState.Attack, CharacterState.Cast, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead };  // Attack 동작 진입이 불가한 상태들
        CharacterState[] castBlockedStates = { CharacterState.Attack, CharacterState.Cast, CharacterState.Interact, CharacterState.Hit, CharacterState.Dead }; // 스킬 시전 동작 진입이 불가 상태들
        CharacterState[] interactBlockedState = { CharacterState.Interact, CharacterState.Attack, CharacterState.Cast, CharacterState.Hit, CharacterState.Dead }; // 상호작용 동작 진입이 불가 상태들
        CharacterState[] hitBlockedStates = { CharacterState.Dead }; // 피격 동작 진입이 불가한 상태들

        // 캐릭터 이동 + 카메라 관련 변수들
        public bool wantsSprint { get; set; } = false;
        public bool isSprinting { get; set; } = false;
        public bool isSprintLocked { get; private set; } = false;
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


        // 스킬(투사체?) 관련 변수들
        [SerializeField] public Transform projectileSpawnPoint; // 투사체 생성 위치
        private float qNextReadyTime = 0f;
        [SerializeField] private float burstInterval = 0.2f; // 연사 간격. 나중에 SkillDataSO에 넣고 싶으면 거기로 빼면 됨.
        private Coroutine launchRoutine;
        public string CurrentQSkillID => currentQSkillID;
        public string CurrentESkillID => currentESkillID;
        private string currentQSkillID = string.Empty; // 캐릭터의 현재 Q스킬 ID
        private string currentESkillID = string.Empty; // 캐릭터의 현재 E스킬 ID
        private int currentQSkillLevel = 0; // 캐릭터의 현재 Q스킬 레벨
        private int currentESkillLevel = 0; // 캐릭터의 현재 E스킬 레벨

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
            var castState = animator.GetBehaviour<CastStateMachineBehaviour>();
            castState?.setCharacter(this);
        }

        private void Start()
        {
            animationEventListener.OnReceiveAnimationEvent += OnCallbackReceiveAnimationEvent; // 애니메이션 이벤트 리스너 콜백 등록

            OnCharacterDeath += DeathEventHandler.OnReceiveDeathEvent; // 사망 이벤트 핸들러 콜백 등록

            SetActiveRagdoll(false);

            curHP = MaxHP; // 초기 체력 설정
            curSP = MaxSP; // 초기 스태미나 설정

            InitializeLockOnPoint();

            if (lockOnPointData != null) CameraSystem.Instance.RegisterCharacter(this);
        }

        private void OnDestroy()
        {
            OnCharacterDeath -= DeathEventHandler.OnReceiveDeathEvent; // 사망 이벤트 핸들러 콜백 해제
        }

        private void Update()
        {
            walkBlend = Mathf.Lerp(walkBlend, isSprinting ? 1f : 0f, Time.deltaTime * 2.0f);
            animator.SetFloat("Running", walkBlend);

            if(characterStat == null) { return; } // 캐릭터 스탯 데이터가 없으면 종료

            if (isSprinting) // 달리기 중일때 
            {
                ConsumeSp(characterStat.SpConsumeRate * Time.deltaTime);
            }
            else // 달리기 중이 아닐 때
            {
                RecoverySp(characterStat.SpRecoveryRate * Time.deltaTime);
            }
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

        private void SetActiveRagdoll(bool isActive)
        {
            animator.enabled = !isActive;
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
                    if (CurrentState != CharacterState.Attack) { return; } // 공격 상태가 아닐 때는 히트박스 활성화 안함, 중간에 피격 등으로 끊겼을 경우 등
                    weaponHitBox?.EnableHitbox(); // 무기가 없으면 ?로 일단 거름
                    // Debug.Log("Enable Hitbox");
                    break;
                case "DisableHitbox":
                    weaponHitBox?.DisableHitbox();
                    // Debug.Log("Disable Hitbox");
                    break;
                case "EndCombo":
                    break;
                case "QSkillCast":
                    if (CurrentState != CharacterState.Cast) { return; } // 시전 상태가 아닐 때는 스킬 시전 안함, 중간에 피격 등으로 끊겼을 경우 등
                    LaunchProjectile(currentQSkillID, currentQSkillLevel);
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
            isSprinting = wantsSprint && hasInput && !isSprintLocked && CurSP > 0f; // 달리기 상태 결정
            moveSpeed = isSprinting ? characterStat.SprintSpeed : characterStat.MoveSpeed; // 달리기 중인지에 따라 속도 결정
            Vector3 displacement = moveDir * moveSpeed * dt;
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
            CurrentState = CharacterState.Dead; // 사망 상태로 변경, 이건 굳이 애니메이션 스테이트머신비헤이비어 연동 안해도 괜찮을듯?

            SetActiveRagdoll(true);

            animator.SetTrigger("DeathTrigger");

            // 사망 이벤트 호출
            bool isPlayerCharacter = (characterStat.CharType == CharacterType.Player); // 플레이어 캐릭터인지 여부
            OnCharacterDeath(isPlayerCharacter, characterStat.ID, this.transform); // 사망 이벤트 호출
            characterController.enabled = false; // 사망 시 캐릭터 컨트롤러 비활성화 (충돌 판정 사라지게 하기 위해)

            Debug.Log($"{gameObject.name} is dead!");
        }

        public float TakeDamage(float damage)
        {
            float finalDamage = DamageCalculator.CalculateDamage(damage, defense); // 스태틱 클래스인 DamageCalculator에서 데미지 계산

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
            CurrentState = CharacterState.Hit; // 피격 상태로 변경, 사실 위에 Hit 애니메이션 재생하면서 StateMachineBehaviour에서 바꿔주긴 하는데, 살짝 딜레이 있으니까..

            weaponHitBox?.DisableHitbox(); // 피격 시 무기 히트박스 비활성화

            TakeDamage(damage);

            Debug.Log($"OnHit!{gameObject.name} took {damage} damage!");
        }

        private void ConsumeSp(float amount)
        {
            if (curSP <= 0 || isSprintLocked == true) { return; } // 현재 스태미너가 0 이하인 경우 소비하지 않음

            curSP -= amount;
            curSP = Mathf.Clamp(curSP, 0f, MaxSP); // 스태미나 0 ~ 최대 스태미나 사이로 제한

            if (curSP <= 0)
            {
                isSprintLocked = true; // 스태미너가 0 이하일 때 달리기 잠금 상태 설정
            }

            OnSpChanged?.Invoke(CurSP, MaxSP); // 스태미나 변경 이벤트 호출
        }
        private void RecoverySp(float amount)
        {
            if (curSP >= MaxSP) return; // 이미 풀일 땐 회복 X
            curSP += amount;
            curSP = Mathf.Clamp(curSP, 0, MaxSP); // 스태미너를 0과 최대 스태미너 사이로 제한

            float sprintUnlockSp = 20.0f; // 나중에 스텟으로 뺄 수도 있으니?
            if (curSP >= sprintUnlockSp)
            {
                isSprintLocked = false; // 스태미너가 일정 이상으로 올라가면 달리기 잠금 상태 해제, 지금은 20
            }

            OnSpChanged?.Invoke(curSP, MaxSP); // 스태미너 변경 이벤트 호출
        }

        // 장비템 관련 메소드들
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


        // 스킬 관련 메소드들
        public void SetQSkill(string qSkillID, int qSkillLevel)
        {
            if(qSkillID == null) return;

            currentQSkillID = qSkillID;
            currentQSkillLevel = qSkillLevel;
        }
        public void SetESkill(string eSkillID, int eSkillLevel)
        {
            if(eSkillID == null) return;

            currentESkillID = eSkillID;
            currentESkillLevel = eSkillLevel;
        }

        public bool TryUseQSkill()
        {
            if (castBlockedStates.Contains(CurrentState)) { return false; }  // 스킬 시전 불가 상태라면 false 반환

            // 쿨타임 체크, 사용 불가 상태라면 false 반환
            if (Time.time < qNextReadyTime)
            {
                Debug.Log("Q Skill is on Cooldown.");
                return false;
            }

            SkillDataSO skillDataSO = GameDataModel.Singleton.GetSkillDataSO(currentQSkillID);

            // SP 체크, 충분한 SP가 없다면 false 반환
            if (curSP < skillDataSO.SkillCost) 
            {
                Debug.Log("Not enough SP to use Q Skill.");
                return false;
            }

            curSP -= skillDataSO.SkillCost; // SP 소모
            qNextReadyTime = Time.time + skillDataSO.Cooldown;    // 다음 사용 가능 시간 갱신

            animator.SetTrigger(skillDataSO.AnimationTriggerName); // 스킬 시전 애니메이션 재생 트리거 설정 
            SetCharacterState(CharacterState.Cast); // 캐릭터 상태를 스킬 시전 상태로 변경 <- 이부분은 좀 그 뭐냐 하드코딩? 이라 나중에 바꿔줄 필요가 있을지도?

            return true;
        }

        public void LaunchProjectile(string skillID, int skillLevel)
        {
            // TODO : 투사체 발사 로직 구현 

            SkillDataSO skillDataSO = GameDataModel.Singleton.GetSkillDataSO(skillID); // 스킬 데이터 SO 가져오기

            // SkillDataSO가 null이거나, 스킬 타입이 투사체가 아니거나, 투사체 생성 위치가 null일 경우 경고 로그 출력 후 함수 종료
            if (skillDataSO == null || skillDataSO.SkillType != SkillType.Projectile || projectileSpawnPoint == null)
            {
                Debug.LogWarning($"[CharacterBase] Skill ID {skillID} is invalid or not a projectile skill.");
                return;
            }

            // int skillLevel = UserDataModel.Singleton.GetSkillLevel(skillID); // 스킬 레벨 가져오기 <- 아 여기도 UserDataModel니까 빼야하는데;

            // 같은 스킬을 연속으로 쓸 때 이전 연사 루틴을 끊고 싶으면 이렇게
            if (launchRoutine != null) StopCoroutine(launchRoutine);
            launchRoutine = StartCoroutine(LaunchProjectileRoutine(skillDataSO, skillLevel));
        }

        private IEnumerator LaunchProjectileRoutine(SkillDataSO skillDataSO, int skillLevel)
        {
            ProjectileDataSO projectilelData = skillDataSO.SkillData as ProjectileDataSO;
            if(projectilelData == null)
            {
                Debug.LogWarning($"[CharacterBase] Skill ID {skillDataSO.SkillID} does not have valid ProjectileDataSO.");
                yield break;
            }

            int count = projectilelData.baseProjectileCount + projectilelData.extraProjectilePerLevel * (skillLevel - 1); // 발사체 개수 계산

            float damage = projectilelData.baseDamage + projectilelData.extraDamagePerLevel * (skillLevel - 1); // 투사체 데미지 계산
            float speed = projectilelData.speed;
            float lifeTime = projectilelData.lifeTime;

            for (int i = 0; i < count; i++)
            {
                SpawnOneProjectile(projectilelData.projectilePrefab, speed, damage, lifeTime);

                // 마지막 발 뒤엔 대기할 필요 없으니
                if (i < count - 1)
                    yield return new WaitForSeconds(burstInterval);
            }
            launchRoutine = null;
        }
        private void SpawnOneProjectile(Projectile prefab, float speed, float damage, float lifeTime)
        {
            if (prefab == null) return;

            // Instantiate할 때 위치/회전 바로 넣는 게 깔끔함
            Projectile projectile = Instantiate(prefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            projectile.Initialize(this);
            projectile.speed = speed;
            projectile.damage = damage;
            projectile.lifeTime = lifeTime;
        }
    }
}
