using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRotation : MonoBehaviour
{
    private float _scrollSpeed = 1.0f;
    private MeshRenderer _renderer;
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        var offset = Time.time * _scrollSpeed * Input.GetAxisRaw("Vertical");
        //현재 시간 * 1.0f * Input.GetAxisRaw("Vertical");w,s
        //기본 텍스처의 Y오프셋 값 변경
        _renderer.material.SetTextureOffset("_MainTex", new Vector2(0f, offset));
                                           //Diffuse 종류
        _renderer.material.SetTextureOffset("_BumpMap",new Vector2(0f, offset));
        //노멀 텍스처의 Y오프셋 값 변경       //노멀맵 종류 

    }
}
