using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.System.CameraMovement
{
    public class Player_Camera : MonoBehaviour
    {
        #region Variables
        [Header("Camera Settings")]
        [SerializeField] float Sensibility = 200f;
        [SerializeField] float CameraRotationSpeed = 6f;
        [Space]
        [Range(-1f, 3f)] [SerializeField] float Height = 1.5f;
        [Range(-1f, 1f)] [SerializeField] float Right = 0.5f;
        [SerializeField] float CameraDistance = 3f;
        [Space]
        [SerializeField] float MaxRotation = 90f;
        [SerializeField] float MinRotation = -90f;
        [Space]
        [SerializeField] LayerMask CameraColliderLayer;

        public Transform Target { get; set;}

        [Space]
        [SerializeField] bool CanMove = true; public bool _CanMove { get{return CanMove;} set{CanMove = value;} }
        [SerializeField] bool HideMouse = true; public bool _HideMouse { get{return HideMouse;} set{HideMouse = value;} }

        float MouseY, MouseX;
        float RotationY, RotationX;
        float SmoothedXRotation, SmoothedYRotation;
        Quaternion CameraCurrentRotation;
        #endregion

        #region Core Methods
        void Awake() {}

        void Start() {}

        void Update()
        {
            if (Target != null) CameraInputs();
            SetStateMouse(HideMouse);
        }
        
        void FixedUpdate()
        {
            if (Target != null) SmoothingCameraRotation();
            if (Target != null) CameraCollision();
        }

        void LateUpdate()
        {
            if (Target != null) CameraMovement();
        }
        #endregion

        #region General Methods
        public void CameraInputs()
        {
            #region Input Axis
            switch (CanMove)
            {
                case true:
                    MouseX = Input.GetAxis("Mouse X");
                    MouseY = Input.GetAxis("Mouse Y");

                    MouseX = Mathf.Clamp(MouseX, -3f, 3f);
                    MouseY = Mathf.Clamp(MouseY, -3f, 3f);
                    break;

                case false:
                    MouseX = 0;
                    MouseY = 0;
                    break;
            }
            #endregion

            RotationY += MouseX * Sensibility * Time.deltaTime;
            RotationX -= MouseY * Sensibility * Time.deltaTime;
            RotationX = Mathf.Clamp(RotationX, MinRotation, MaxRotation);
        }

        void SmoothingCameraRotation()
        {
            SmoothedXRotation = Mathf.LerpAngle(SmoothedXRotation, RotationX, CameraRotationSpeed * Time.fixedDeltaTime);
            SmoothedYRotation = Mathf.LerpAngle(SmoothedYRotation, RotationY, CameraRotationSpeed * Time.fixedDeltaTime);
            CameraCurrentRotation = Quaternion.Euler(SmoothedXRotation, SmoothedYRotation, 0); 
        }

        void CameraMovement()
        {
            transform.rotation = CameraCurrentRotation;
            transform.position = Target.transform.position + Target.transform.up * Height + transform.right * Right - transform.forward * CameraDistance;

            if (CameraCollision())
                transform.position = hit.point + transform.forward * 0.13f;
        }

        RaycastHit hit;
        bool CameraCollision()
        {
            if(Physics.Linecast(Target.transform.position + Target.transform.up * Height + transform.right * Right, transform.position, out hit, CameraColliderLayer))
                return true;
            else
                return false;
        }

        public void SetCameraTarget(Transform target)
        {
            Target = target;
        }

        void SetStateMouse(bool MouseState)
        {
            switch(HideMouse)
            {
                case true:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;

                case false:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
        #endregion
    }
}

