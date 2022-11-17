using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.System.Movement;

namespace Player.System.Hud
{
    public class Player_Hud : MonoBehaviour
    {   
        
        #region Variables
        Player_Movement Player_Movement;

        public Slider LifeBar;
        public Slider HungerBar;

        [Header("Statics Hud")]
        [SerializeField] float MaxLife; public float _MaxLife { get {return MaxLife;} set{MaxLife = value;} }
        public float CurrentLife { get; private set; }
        [SerializeField] float MaxHunger; public float _MaxHunger { get {return MaxHunger;} set{MaxHunger = value;} }
        public float CurrentHunger { get; private set; }

        bool IsHunger;
        bool IsDeath;
        #endregion

        #region Core Methods
        void Awake()
        {
            Player_Movement = GetComponent<Player_Movement>();

            CurrentLife = MaxLife;
            CurrentHunger = MaxHunger;
        }

        void Start()
        {
            LifeBar = GameObject.FindGameObjectWithTag("LifeSlider").GetComponent<Slider>();
            HungerBar = GameObject.FindGameObjectWithTag("HungerSlider").GetComponent<Slider>();

            SetCanvasLimits();
        }

        void Update()
        {
            InputPlayer();

            SetCanvasStatics();
            CurrentStatics();
        }
        
        void FixedUpdate() {}
        #endregion

        #region General Methods
        void InputPlayer()
        {
            if (Input.GetKeyDown(KeyCode.R)) {CurrentHunger = MaxHunger; CurrentLife = MaxLife;}
        }

        public void SetCanvasLimits()
        {
            LifeBar.maxValue = MaxLife;
            HungerBar.maxValue = MaxHunger;
        }

        void SetCanvasStatics()
        {
            LifeBar.value = CurrentLife;
            HungerBar.value = CurrentHunger;
        }

        void CurrentStatics()
        {
            if (CurrentHunger <= 0)
                IsHunger = true;
            else
            {
                IsHunger = false;
                CurrentHunger -= Time.deltaTime;
            }

            if (IsHunger)
                CurrentLife -= Time.deltaTime;

            if (CurrentLife <= 0)
                IsDeath = true;
            else
                IsDeath = false;

            // if (IsDeath)
            //     Player_Movement._CanMove = false;
            // else
            //     Player_Movement._CanMove = true;
        }
        #endregion
    }
}