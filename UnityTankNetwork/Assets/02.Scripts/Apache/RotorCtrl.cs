using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorCtrl : MonoBehaviour
{
    public float rotSpeed = 4000f;
    private Transform tr;
    void Start()
    {
        tr = transform;
    }
    void Update()
    {
        tr.Rotate(Vector3.up * Time.deltaTime *rotSpeed);
    }
}
