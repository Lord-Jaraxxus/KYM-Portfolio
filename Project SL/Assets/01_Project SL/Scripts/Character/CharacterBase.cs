using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CharacterBase : MonoBehaviour, IHittable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;

        public AnimationEventListener AnimationEventListener => animationEventListener;
        private AnimationEventListener animationEventListener { get; set; }

        [SerializeField] private Weapon weapon; // 일단 인스펙터에서 연결

        public bool IsWalk { get; set; } = false;


        private float walkBlend;

        public float MaxHP; // => maxHP;
        public float CurHP; // => curHP;
        public float MaxSP; // => maxSP;
        public float CurSP; // => curSP;

        private Vector3 movementForward;
        private float verticalVelocity;
        private float targetRotation;
        private float rotationVelocity;
        private float rotationSmoothTime = 0.15f;
        private float smoothHorizontal;
        private float smoothVertical;

        private bool isStrafe = false;
        private float moveSpeed = 3f;


        private void Awake()
        {
            // 일단 뭐 테스트 해야하니까 여기서 초기화
            MaxHP = 100f;
            //CurHP = MaxHP;

            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            animationEventListener = GetComponent<AnimationEventListener>();

            SetActiveRagdoll(false);
        }

        private void SetActiveRagdoll(bool isActive)
        {
            animator.enabled = !isActive;
            Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rigid in ragdollRigidbodies)
            {
                rigid.isKinematic = isActive;
            }
        }

        private void Start()
        {
            animationEventListener.OnReceiveAnimationEvent += OnCallbackReceiveAnimationEvent; // 애니메이션 이벤트 리스너 콜백 등록
        }

        private void Update()
        {
            walkBlend = Mathf.Lerp(walkBlend, IsWalk ? 1f : 0f, Time.deltaTime);
            animator.SetFloat("Running", walkBlend);
        }

        void OnCallbackReceiveAnimationEvent(string eventName)
        {
            switch (eventName)
            {
                case "EnableHitbox":
                    weapon.EnableHitbox();
                    // Debug.Log("Enable Hitbox");
                    break;
                case "DisableHitbox":
                    weapon.DisableHitbox();
                    // Debug.Log("Disable Hitbox");
                    break;
                case "EndCombo":
                    animator.SetTrigger("TransTrigger");
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


        public void Rotate(Vector3 targetAimPoint)
        {
            Vector3 aimTarget = targetAimPoint;
            aimTarget.y = transform.position.y;
            Vector3 pos = transform.position;
            Vector3 aimDirection = (aimTarget - pos).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        public void Attack1()
        {
            animator.SetTrigger("TransTrigger");
            animator.SetTrigger("AttackTrigger");
            animator.SetInteger("AttackIndex", 0);
            // Debug.Log("Attack!");
        }
        public void Attack2()
        {
            animator.SetTrigger("TransTrigger");
            animator.SetTrigger("AttackTrigger");
            animator.SetInteger("AttackIndex", 1);
            // Debug.Log("Attack!");
        }
        public void Attack3()
        {
            animator.SetTrigger("TransTrigger");
            animator.SetTrigger("AttackTrigger");
            animator.SetInteger("AttackIndex", 2);
            // Debug.Log("Attack!");
        }


        public void Die()
        {
            animator.SetTrigger("TransTrigger");
            animator.SetTrigger("DeathTrigger");
            Debug.Log($"{gameObject.name} is dead!");
        }

        public float TakeDamage(float damage)
        {
            CurHP -= damage;

            if (CurHP <= 0)
            {
                CurHP = 0;
                Die();
            }

            return CurHP;
        }

        public void Heal(float amount)
        {
            CurHP += amount;

            if (CurHP > MaxHP)
            {
                CurHP = MaxHP;
            }
        }

        public void OnHit(float damage)
        {
            TakeDamage(damage);
        }
    }
}
