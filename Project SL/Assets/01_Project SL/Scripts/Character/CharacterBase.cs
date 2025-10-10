using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CharacterBase : MonoBehaviour
    {
        public float MaxHP; // => maxHP;
        public float CurHP; // => curHP;
        public float MaxSP; // => maxSP;
        public float CurSP; // => curSP;

        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;

        private Vector3 movementForward;
        private float verticalVelocity;
        private float targetRotation;
        private float rotationVelocity;
        private float rotationSmoothTime = 0.15f;
        private float smoothHorizontal;
        private float smoothVertical;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
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

            // animator.SetFloat("Magnitude", input.magnitude);
            // animator.SetFloat("Horizontal", smoothHorizontal);
            // animator.SetFloat("Vertical", smoothVertical);

            // 일단 움직여야 하니까 쓰는 코드 
            Vector3 movement = new Vector3(smoothHorizontal, 0, smoothVertical);
            transform.position += movement;
        }

        public void Rotate(Vector3 targetAimPoint) 
        {
            Vector3 aimTarget = targetAimPoint;
            aimTarget.y = transform.position.y;
            Vector3 pos = transform.position;
            Vector3 aimDirection = (aimTarget - pos).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

    }
}
