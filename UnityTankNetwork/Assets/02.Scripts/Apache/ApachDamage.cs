using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ApachDamage : MonoBehaviourPun
{
    [SerializeField] private MeshRenderer[] m_Renderer;
    [SerializeField] private GameObject expEffect;

    void Start()
    {
        m_Renderer = GetComponentsInChildren<MeshRenderer>();
        expEffect = Resources.Load<GameObject>("Explosion");
    }

    [PunRPC]
    void OnApacheDamage(string tag)  // 레이캐스트 충돌 감지
    {
        Object effect = GameObject.Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3.0f);
        Destroy(gameObject);
        GameManager.Instance.KillCountText();
    }

    public void OnDamage_E(string tag)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnApacheDamage", RpcTarget.All, tag);
        }
    }
}
