using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Player.System.Movement
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]

    public class Player_Movement : MonoBehaviour
    {
        #region Variable
        public Animator mAnimator { get; set; }
        public CharacterController mCharacterController { get; set; }
        
        PhotonView mPhotonView;
        Transform mCamera;
        Transform DesiredDirectionTransform;

        [Header("Movement Settings")]
        [SerializeField] float Speed = 3.5f;
        [SerializeField] float JumpForce = 1f;
        [Space]
        [SerializeField] Vector2 m_wallCheck = new Vector2(0.1f, 0.2f);
        [SerializeField] float MinVelocityDistance = 0.35f;
        [SerializeField] float WaitTimeForJump = 0.35f;
        [Space]
        [SerializeField] LayerMask GroundLayers;
        [Space]

        bool edg;
        private float SpeedMultiply = 0f;

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

        float InputX, InputZ;
        Vector3 CurrentPlayerMovement, DesiredDirection;
        Vector3 DesiredEulerAngles;
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
            mPhotonView = GetComponent<PhotonView>();
            mCamera = Camera.main.GetComponent<Camera>().transform;
            DesiredDirectionTransform = new GameObject("DesiredDirectionTransform").transform;
        }

        void Update()
        {
            PlayerInputs();
            SetAnimations();
        }

        void FixedUpdate() 
        {
            PlayerGeneralMovement();
            GroundCheck();
        }
        #endregion

        #region General Methods
        void PlayerInputs()
        {
            #region Input Axis
            switch(CanMove && mPhotonView.IsMine)
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
            IsWalking = Mathf.Abs(InputX) > MinVelocityDistance || Mathf.Abs(InputZ) > MinVelocityDistance;
            IsRunning = IsWalking && Input.GetKey(KeyCode.LeftShift);
            JumpPressed = IsGrounded && CanMove && ActivatedJumpTime >= WaitTimeForJump && Input.GetKey(KeyCode.Space);

            if (JumpPressed) PlayerJump();
            #endregion
        }

        void PlayerGeneralMovement()
        {
            #region Player Movement
            CurrentPlayerMovement = transform.forward * Speed * SpeedMultiply * Time.fixedDeltaTime;
            if (mPhotonView.IsMine) mCharacterController.Move(CurrentPlayerMovement);

            PlayerGravity.y += Gravity * Time.fixedDeltaTime;
            if (mPhotonView.IsMine) mCharacterController.Move(PlayerGravity * Time.fixedDeltaTime);

            if (IsGrounded && PlayerGravity.y < 0)
                PlayerGravity.y = -2f;

            if (!mCharacterController.isGrounded && PlayerGravity.y < 0 && !edg)
                PlayerGravity += Vector3.up * (-JumpForce * 0.25f);

            if (PlayerGravity.y > 0 && (mCharacterController.collisionFlags & CollisionFlags.Above) != 0)
                PlayerGravity = Vector3.zero;

            PlayerGravity.y = Mathf.Clamp(PlayerGravity.y, -10f, 10f);

            if (IsGrounded)
                edg = false;
            else
                if(mCharacterController.velocity.y < 0) edg = SlipCheckers(); else edg = false;
            

            if (IsWalking)
            {
                if (IsRunning)
                {
                    if (IsGrounded) SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1.75f, 3f * Time.deltaTime);
                    else SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1.25f, 3f * Time.deltaTime);
                }
                else
                {
                    SpeedMultiply = Mathf.Lerp(SpeedMultiply, 1f, 3f * Time.deltaTime);
                }
                
                StartCoroutine(SetPlayerRotaionLow());
            }
            else
            {
                PlayerRotationLow = false;
                SpeedMultiply = Mathf.Lerp(SpeedMultiply, 0f, 6f * Time.deltaTime);
            }

            EnableJump();
            #endregion
            
            #region Player Rotation
            DesiredEulerAngles = transform.eulerAngles;
            if (IsWalking)
            {
                DesiredDirectionTransform.rotation = Quaternion.LookRotation(DesiredDirection) * DesiredCameraRotation;
                DesiredDirectionTransform.rotation = Quaternion.FromToRotation(DesiredDirectionTransform.up, transform.up) * DesiredDirectionTransform.rotation;
                DesiredDirectionTransform.position = transform.position;
            }
            StartCoroutine(RotationPlayer());
            transform.eulerAngles = DesiredEulerAngles;
            #endregion
        }

        void PlayerJump()
        {
            IsGrounded = false;
            mAnimator.SetTrigger("IsJumping");
            PlayerGravity = Vector3.zero;
            PlayerGravity.y += Mathf.Sqrt(JumpForce * -2f * Gravity);
            ActivatedJumpTime = 0;
        }

        void GroundCheck()
        {
            RaycastHit hit;
            IsGrounded = Physics.SphereCast(transform.position + Vector3.up * 1f, 0.225f, -Vector3.up, out hit, 1f, GroundLayers);
        }

        private bool SlipCheckers()
        {
            RaycastHit hit;
            Vector3 ray_spwan_pos = transform.position + Vector3.up * m_wallCheck.y; //Y as starting point

            Vector3 forward = transform.forward * m_wallCheck.x; //X as length of rays
            Vector3 back = -transform.forward * m_wallCheck.x;
            Vector3 right = transform.right * m_wallCheck.x;
            Vector3 left = -transform.right * m_wallCheck.x;

            Ray front_ray = new Ray(ray_spwan_pos, forward);
            Ray back_ray = new Ray(ray_spwan_pos, back);
            Ray right_ray = new Ray(ray_spwan_pos, right);
            Ray left_ray = new Ray(ray_spwan_pos, left);

            float dis = m_wallCheck.x;

            if(Physics.Raycast (front_ray, out hit, dis, GroundLayers)){
                HitForSlip(transform.forward);
                return true;
            }

            if(Physics.Raycast (back_ray, out hit, dis, GroundLayers) || Physics.Raycast (right_ray, out hit, dis, GroundLayers) || Physics.Raycast (left_ray, out hit, dis, GroundLayers)){
                HitForSlip(hit.normal);
                return true;
            }
            return false;
        }

        void HitForSlip(Vector3 slip_direction)
        {
            mCharacterController.Move(((slip_direction * 1.25f) + Vector3.down) * Time.fixedDeltaTime);
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
            if (IsGrounded) ActivatedJumpTime += Time.deltaTime;
            else ActivatedJumpTime = 0;
            ActivatedJumpTime = Math.Clamp(ActivatedJumpTime, 0, WaitTimeForJump + 0.1f);
        }

        IEnumerator RotationPlayer()
        {
            if (!PlayerRotationLow) DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 500f * Time.fixedDeltaTime);
            else if (!IsGrounded) DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 60f * Time.fixedDeltaTime);
            DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 260f * Time.fixedDeltaTime);
            yield return null;
        }

        IEnumerator SetPlayerRotaionLow()
        {
            yield return new WaitForSeconds(0.25f);
            if (IsWalking) PlayerRotationLow = true;
        }
        #endregion

        private void OnDrawGizmos() 
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.red;
            Vector3 ray_spwan_pos = transform.position + Vector3.up * m_wallCheck.y;
            
            Vector3 forward = transform.forward * m_wallCheck.x;
            Vector3 back = -transform.forward * m_wallCheck.x;
            Vector3 right = transform.right * m_wallCheck.x;
            Vector3 left = -transform.right * m_wallCheck.x;
            
        
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray_spwan_pos, forward);
            Gizmos.DrawRay(ray_spwan_pos, back);
            Gizmos.DrawRay(ray_spwan_pos, right);
            Gizmos.DrawRay(ray_spwan_pos, left);
        }
    }
}

