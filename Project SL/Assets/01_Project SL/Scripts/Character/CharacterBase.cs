using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CharacterBase : MonoBehaviour, IHasHp, IHittable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        private AnimationEventListener animationEventListener;

        [SerializeField] private Weapon weapon; // �ϴ� �ν����Ϳ��� ����

        public bool IsWalk { get; set; } = false;

        float IHasHp.MaxHP => MaxHP;
        float IHasHp.CurHP => CurHP;


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

        private void Awake()
        {
            // �ϴ� �� �׽�Ʈ �ؾ��ϴϱ� ���⼭ �ʱ�ȭ
            MaxHP = 100f;
            //CurHP = MaxHP;

            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            animationEventListener = GetComponent<AnimationEventListener>();
        }

        private void Start()
        {
            animationEventListener.OnReceiveAnimationEvent += OnCallbackReceiveAnimationEvent; // �ִϸ��̼� �̺�Ʈ ������ �ݹ� ���
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
            }
        }

        public void SetMovementForward(Vector3 forward) 
        {
            movementForward = forward;
        }

        public void Move(Vector2 input) 
        {
            bool isInputSomething = input.magnitude > 0.1f;
            if (isInputSomething) 
            {
                Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
                Vector3 worldDirection = Quaternion.LookRotation(movementForward) * inputDir;
                targetRotation = Quaternion.LookRotation(worldDirection).eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            smoothHorizontal = Mathf.Lerp(smoothHorizontal, input.x, Time.deltaTime * 10f);
            smoothVertical = Mathf.Lerp(smoothVertical, input.y, Time.deltaTime * 10f);

            animator.SetFloat("Magnitude", input.magnitude);
            animator.SetFloat("Horizontal", smoothHorizontal);
            animator.SetFloat("Vertical", smoothVertical);

            // Debug.Log($"mag : {input.magnitude}, hor : {smoothHorizontal}, ver : {smoothVertical}");
        }

        public void Rotate(Vector3 targetAimPoint) 
        {
            Vector3 aimTarget = targetAimPoint;
            aimTarget.y = transform.position.y;
            Vector3 pos = transform.position;
            Vector3 aimDirection = (aimTarget - pos).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        public void Attack() 
        {
            animator.SetTrigger("TransTrigger");
            animator.SetTrigger("AttackTrigger");
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
                CurHP = MaxHP;            }
        }

        public void OnHit(float damage)
        {
            TakeDamage(damage);
        }
    }
}
