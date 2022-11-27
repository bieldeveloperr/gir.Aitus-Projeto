using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.System.CameraMovement;

namespace Player.System.Movement
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]

    public class Player_Movement : MonoBehaviour
    {
        #region Variable
        public Animator mAnimator { get; set; }
        public CharacterController mCharacterController { get; set; }
        
        Transform mCamera;
        Transform DesiredDirectionTransform;
        RaycastHit SlopeHit;

        [Header("Movement Settings")]
        [SerializeField] float Speed = 3.5f;
        [SerializeField] float JumpForce = 2f;
        [Space]
        [SerializeField] float MinVelocityDistance = 0.35f;
        [SerializeField] float WaitTimeForJump = 0.35f;
        [SerializeField] Vector2 GroundCheckSlip = new Vector2(0.45f, -0.05f);
        [Space]
        [SerializeField] float MaxDistanceGroundCheckPosition;
        [Space]
        [SerializeField] LayerMask GroundLayers;
        [Space]

        bool edg;
        private float SpeedMultiply = 0f;

        float SlipSpeed;
        float Gravity = -9.81f; 
        float GravityForce;
        Vector3 PlayerGravity;

        [Header("States Settings")]
        [SerializeField] bool CanMove; 
        public bool _CanMove { get{return CanMove;} set{CanMove = value;} }
        [Space]
        [SerializeField] bool IsGrounded;
        [SerializeField] bool IsWalking;
        [SerializeField] bool IsRunning;

        private bool PlayerRotationLow;
        private bool JumpPressed;

        bool Stop;
        float InputX, InputZ;
        Vector3 CurrentPlayerMovement, DesiredDirection;
        Vector3 DesiredEulerAngles;
        Vector3 SlopeDirection;
        Quaternion DesiredCameraRotation;
        #endregion

        #region Core Methods
        void Awake() 
        {
            mAnimator = GetComponent<Animator>();
            mCharacterController = GetComponent<CharacterController>();
        }

        void Start() 
        {
            mCamera = Camera.main.GetComponent<Camera>().transform;
            DesiredDirectionTransform = new GameObject("DesiredDirectionTransform").transform;

            mCamera.GetComponent<Player_Camera>().SetCameraTarget(this.transform);
        }

        void Update()
        {
            PlayerInputs();
            SetAnimations();

            GroundCheck();
        }

        void FixedUpdate() 
        {
            PlayerGeneralMovement();
        }
        #endregion

        #region General Methods
        void PlayerInputs()
        {
            #region Input Axis
            switch(CanMove)
            {
                case true:
                    InputX = Input.GetAxisRaw("Horizontal");
                    InputZ = Input.GetAxisRaw("Vertical");
                    break;

                case false:
                    InputX = Mathf.Lerp(InputX, 0, 4f * Time.deltaTime);
                    InputZ = Mathf.Lerp(InputZ, 0, 4f * Time.deltaTime);
                    break;
            }

            DesiredDirection = new Vector3(InputX, 0, InputZ);
            #endregion

            #region Direction of the Camera
            DesiredCameraRotation = mCamera.transform.rotation;
            DesiredCameraRotation.z = 0;
            DesiredCameraRotation.x = 0;
            #endregion

            #region Inputs
            IsWalking = (Mathf.Abs(InputX) > MinVelocityDistance || Mathf.Abs(InputZ) > MinVelocityDistance) && !SlopeCheck();
            IsRunning = IsWalking && Input.GetKey(KeyCode.LeftShift);
            JumpPressed = IsGrounded && CanMove && ActivatedJumpTime >= WaitTimeForJump && Input.GetKey(KeyCode.Space);

            if (JumpPressed && !SlopeCheck()) PlayerJump();
            #endregion
        }

        void PlayerGeneralMovement()
        {
            #region Player Movement
            if (IsGrounded && PlayerGravity.y < 0) GravityForce = -3.5f;
            if (GravityForce > 0 && (mCharacterController.collisionFlags & CollisionFlags.Above) != 0) GravityForce = 0;
            if (IsGrounded) edg = false;
            else if(mCharacterController.velocity.y < 0) edg = SlipCheckers(); else edg = false;

            CurrentPlayerMovement = transform.forward * SpeedMultiply * Speed * Time.deltaTime + Vector3.up * GravityForce * Time.fixedDeltaTime;
            GravityForce += Gravity * Time.fixedDeltaTime;
            GravityForce = Mathf.Clamp(GravityForce, -10f, 10f);
            mCharacterController.Move(PlayerGravity);
            PlayerGravity = CurrentPlayerMovement; 

            EnableJump();

            if (IsWalking)
            {
                Stop = true;
                if (IsRunning)
                {
                    if (IsGrounded) SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1.75f, 3f * Time.fixedDeltaTime);
                    else SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1.25f, 3f * Time.fixedDeltaTime);
                }
                else
                {
                    SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1f, 3f * Time.fixedDeltaTime);
                }
                
                StartCoroutine(SetPlayerRotaionLow());
            }
            else
            {
                PlayerRotationLow = false;
                SpeedMultiply = Mathf.Lerp(SpeedMultiply, 0f, 6f * Time.fixedDeltaTime);

                if (Stop == true)
                {
                    LockMovementForSeconds(0.25f);
                    Stop = false;
                }
            }
            #endregion
            
            #region Player Rotation
            DesiredEulerAngles = transform.eulerAngles;
            if (IsWalking)
            if (IsWalking && !SlopeCheck())
            {
                DesiredDirectionTransform.rotation = Quaternion.LookRotation(DesiredDirection) * DesiredCameraRotation;
                DesiredDirectionTransform.rotation = Quaternion.FromToRotation(DesiredDirectionTransform.up, transform.up) * DesiredDirectionTransform.rotation;
                DesiredDirectionTransform.position = transform.position;
            }
            StartCoroutine(RotationPlayer());
            transform.eulerAngles = DesiredEulerAngles;
            #endregion

            #region Slope Slip
            SlopeDirection = Vector3.up - SlopeHit.normal * Vector3.Dot(Vector3.up, SlopeHit.normal);
            if (SlopeCheck())
            {
                HitForSlip(SlopeDirection, -SlipSpeed);
                DesiredDirectionTransform.rotation = Quaternion.Lerp(DesiredDirectionTransform.rotation, Quaternion.LookRotation(SlopeDirection)
                     * Quaternion.Euler(0, 180, 0), 5f * Time.deltaTime); 
            }

            if (SlopeCheck())
            {    
                SlipSpeed = Mathf.Lerp(SlipSpeed, Speed * 4.5f, 8f * Time.deltaTime);
                LockMovementForSeconds(0.1f);
            }
            else
                SlipSpeed = Mathf.Lerp(SlipSpeed, 0, 12f * Time.deltaTime);
            #endregion
        }

        void PlayerJump()
        {
            IsGrounded = false;
            mAnimator.SetTrigger("IsJumping");
            PlayerGravity = Vector3.zero;
            GravityForce += Mathf.Sqrt(JumpForce * -2.5f * Gravity);
            ActivatedJumpTime = 0;
        }

        void GroundCheck()
        {
            IsGrounded = Physics.SphereCast(transform.position + transform.up * 1f, 0.1f, -transform.up, out var hit, MaxDistanceGroundCheckPosition, GroundLayers);
        }

        bool SlipCheckers()
        {
            RaycastHit hit;
            Vector3 ray_spwan_pos = transform.position + Vector3.up * GroundCheckSlip.y; //Y as starting point

            Vector3 forward = transform.forward * GroundCheckSlip.x; //X as length of rays
            Vector3 back = -transform.forward * GroundCheckSlip.x;
            Vector3 right = transform.right * GroundCheckSlip.x;
            Vector3 left = -transform.right * GroundCheckSlip.x;

            Ray front_ray = new Ray(ray_spwan_pos, forward);
            Ray back_ray = new Ray(ray_spwan_pos, back);
            Ray right_ray = new Ray(ray_spwan_pos, right);
            Ray left_ray = new Ray(ray_spwan_pos, left);

            float dis = GroundCheckSlip.x;

            if(Physics.Raycast (front_ray, out hit, dis, GroundLayers)){
                HitForSlip(transform.forward, 1.75f);
                return true;
            }

            if(Physics.Raycast (back_ray, out hit, dis, GroundLayers) || Physics.Raycast (right_ray, out hit, dis, GroundLayers) || Physics.Raycast (left_ray, out hit, dis, GroundLayers)){
                HitForSlip(hit.normal, 1.75f);
                return true;
            }
            return false;
	    }

        bool SlopeCheck()
        {
            if (Physics.SphereCast(transform.position + transform.up * 1f, 0.05f, -transform.up, out SlopeHit, 1.25f, GroundLayers) && Physics.SphereCast(transform.position + transform.up * 1f, 0.1f, -transform.up, out var hit, MaxDistanceGroundCheckPosition, GroundLayers))
            {
                float SlopeAngle = Vector3.Angle(SlopeHit.normal, Vector3.up);
                if (SlopeAngle > mCharacterController.slopeLimit + 10f)
                    return true;
            }
            return false;
        }

        void HitForSlip(Vector3 slip_direction, float slip_speed)
        {
            mCharacterController.Move(((slip_direction * slip_speed) + Vector3.down) * Time.fixedDeltaTime);
        }

        void SetAnimations()
        {
            mAnimator.SetBool("IsGrounded", IsGrounded);
            mAnimator.SetFloat("Movement", SpeedMultiply);
        }
        #endregion

        #region Coroutines
        float ActivatedJumpTime;
        void EnableJump()
        {
            if (IsGrounded && !SlopeCheck()) ActivatedJumpTime += Time.deltaTime;
            else ActivatedJumpTime = 0;
            ActivatedJumpTime = Math.Clamp(ActivatedJumpTime, 0, WaitTimeForJump + 0.1f);
        }

        IEnumerator RotationPlayer()
        {
            if (!PlayerRotationLow) DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 500f * Time.deltaTime);
            else if (!IsGrounded) DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 90f * Time.deltaTime);
            if (!SlopeCheck()) DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 240f * Time.deltaTime);
            yield return null;
        }

        IEnumerator SetPlayerRotaionLow()
        {
            yield return new WaitForSeconds(0.25f);
            if (IsWalking) PlayerRotationLow = true;
        }

        public void LockMovementForSeconds(float Time)
        {
            CanMove = false;
            Invoke("EnableMovement", Time);
        }

        private void EnableMovement()
        {
            CanMove = true;
            Stop = false;
            if (IsInvoking("EnableMovement"))
            {
                CancelInvoke("EnableMovement");
            }
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            Vector3 ray_spwan_pos = transform.position + Vector3.up * GroundCheckSlip.y;
            
            Vector3 forward = transform.forward * GroundCheckSlip.x;
            Vector3 back = -transform.forward * GroundCheckSlip.x;
            Vector3 right = transform.right * GroundCheckSlip.x;
            Vector3 left = -transform.right * GroundCheckSlip.x;
            
        
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray_spwan_pos, forward);
            Gizmos.DrawRay(ray_spwan_pos, back);
            Gizmos.DrawRay(ray_spwan_pos, right);
            Gizmos.DrawRay(ray_spwan_pos, left);

            Gizmos.DrawRay(transform.position + transform.up * 1f, -transform.up * MaxDistanceGroundCheckPosition);
	    }
    }
}