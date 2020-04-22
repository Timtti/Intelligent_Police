using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isSeen : MonoBehaviour
{
    //　is it in camera view or not
    private bool isInsideCamera;
    private const string CAMERA_TAG_NAME = "PoliceCamera";

    //is it shown on camera or not
    public bool Rendered = false;

    private void Update()
    {
        Rendered = false;
    }

    //when show on camera
    private void OnWillRenderObject()
    {
        //only activate isrendered when rendered by camera with tag name
        if (Camera.current.tag == CAMERA_TAG_NAME)
        {
            Rendered = true;
        }
    }
}
