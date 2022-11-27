using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    Camera Cam;

    void Update()
    {
        if (Cam == null)
            Cam = Camera.main.GetComponent<Camera>();

        if (Cam == null)
            return;

        transform.LookAt(Cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
