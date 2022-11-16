using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PD_CameraController : MonoBehaviour
{
    [Header("Settings")]
    public bool a;
    public GameObject Target;
    [Space]
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
    [SerializeField] bool CanMove = true; public bool _CanMove { get{return CanMove;} set{CanMove = value;} }
    [SerializeField] bool HideMouse = true; public bool _HideMouse { get{return HideMouse;} set{HideMouse = value;} }
    [SerializeField] bool Controller;
    [Space]
    [Range(0, 0.5f)] [SerializeField] float CameraRegulatorCollider = 0.013f;
    [SerializeField] LayerMask CameraColliderLayer;

    bool FixedCam;
    float MouseY, MouseX;
    public Vector2 MouseInputs { get; private set; }
    float RotationY, RotationX;
    float SmoothedXRotation, SmoothedYRotation;
    Quaternion CameraCurrentRotation;

    // -----------------------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        
    }

    void Start()
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

        if (Target == null && a)
            Target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        

        Inputs();

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

    void FixedUpdate()
    {
        SmoothingCameraRotation();
        CameraCollision();
    }

    void LateUpdate()
    {
        CameraMovement();
    }

    // -----------------------------------------------------------------------------------------------------------------------------

    Vector2 CorrectionCameraVectorMove, CorrectionVelocity;
    void Inputs()
    {
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

        MouseInputs = new Vector2(MouseY, MouseX);
    
		if (FixedCam) RotationY = Target.transform.eulerAngles.y; else RotationY += MouseX * Sensibility * Time.deltaTime;
        if (FixedCam) RotationX = Target.transform.eulerAngles.x; else RotationX -= MouseY * Sensibility * Time.deltaTime;
        RotationX = Mathf.Clamp(RotationX, MinRotation, MaxRotation);
    }

    void SmoothingCameraRotation()
    {
		if (!FixedCam) SmoothedXRotation = Mathf.LerpAngle(SmoothedXRotation, RotationX, CameraRotationSpeed * Time.fixedDeltaTime);
        else SmoothedXRotation = Mathf.LerpAngle(SmoothedXRotation, RotationX, (CameraRotationSpeed * 0.25f) * Time.fixedDeltaTime);
		if (!FixedCam) SmoothedYRotation = Mathf.LerpAngle(SmoothedYRotation, RotationY, CameraRotationSpeed * Time.fixedDeltaTime);
        else SmoothedYRotation = Mathf.LerpAngle(SmoothedYRotation, RotationY, (CameraRotationSpeed * 0.25f) * Time.fixedDeltaTime);
		CameraCurrentRotation = Quaternion.Euler(SmoothedXRotation, SmoothedYRotation, 0); 
    }

    void CameraMovement()
    {
        transform.rotation = CameraCurrentRotation;
        transform.position = Target.transform.position + Target.transform.up * Height + transform.right * Right - transform.forward * CameraDistance;

        if (CameraCollision())
            transform.position = hit.point + transform.forward * CameraRegulatorCollider;
    }

    RaycastHit hit;
    bool CameraCollision()
    {
        if(Physics.Linecast(Target.transform.position + Target.transform.up * Height + transform.right * Right, transform.position, out hit, CameraColliderLayer))
            return true;
        else
            return false;
    }

    // -----------------------------------------------------------------------------------------------------------------------------

    void IsInvokeFixedCamera()
    {
        if (IsInvoking("FixedCamera"))
        {
            CancelInvoke("FixedCamera");
            FixedCam = false;
        }
    }
}
