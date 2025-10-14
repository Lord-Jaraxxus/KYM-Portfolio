using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance { get; private set; }
        [field: SerializeField] public Vector3 AimingPoint { get; private set; }

        private Camera mainCamera;

        private void Awake()
        {
            Instance = this;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            Ray screenCenterRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            if (Physics.Raycast(screenCenterRay, out RaycastHit hitInfo, 1000f))
            {
                AimingPoint = hitInfo.point;
            }
            else
            {
                AimingPoint = screenCenterRay.GetPoint(1000f);
            }
        }
    }
}
