using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isSeen : MonoBehaviour
{
    //　カメラ内にいるかどうか
    private bool isInsideCamera;
    private const string CAMERA_TAG_NAME = "PoliceCamera";

    //カメラに表示されているか
    public bool Rendered = false;

    private void Update()
    {

        //if (Rendered)
        //{
        //    Debug.Log("カメラに映ってるよ！");
        //}

        Rendered = false;
    }

    //カメラに映ってる間に呼ばれる
    private void OnWillRenderObject()
    {
        //メインカメラに映った時だけ_isRenderedを有効に
        if (Camera.current.tag == CAMERA_TAG_NAME)
        {
            Rendered = true;
        }
    }
}
