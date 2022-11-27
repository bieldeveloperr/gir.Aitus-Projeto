using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.System.Movement;
using Player.System.CameraMovement;

public class EmotePlayer : MonoBehaviour
{   
    Player_Movement Player_Movement;
    Player_Camera Player_Camera;

    void Start()
    {
        Player_Movement = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Movement>();
        Player_Camera = Camera.main.GetComponent<Player_Camera>();
    }

    public void HI()
    {
        Player_Movement.mAnimator.SetTrigger("Emote_Hi");
        Player_Movement._CanMove = false;

        this.transform.GetChild(0).gameObject.SetActive(false);

        Player_Camera._CanMove = true;
        Player_Camera._HideMouse = true;

        StartCoroutine(CanMoveTrue());
    }

    IEnumerator CanMoveTrue()
    {
        yield return new WaitForSeconds(2.5f);
        Player_Movement._CanMove = true;
    }
}
