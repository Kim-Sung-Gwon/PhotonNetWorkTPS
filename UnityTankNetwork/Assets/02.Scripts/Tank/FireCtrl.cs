using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class FireCtrl : MonoBehaviourPun
{
    public GameObject bullet;
    public Transform firePos;
    public LeaserBeamT BeamT;
    public GameObject expEffect;

    void Start()
    {
        bullet =  Resources.Load<GameObject>("Bullet");
        firePos = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(0).transform;
        BeamT = GetComponentInChildren<LeaserBeamT>();
        expEffect = Resources.Load<GameObject>("Explosion");
    }

    void Update()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;
        //// UI에 마우스 커서가 다았다면 빠져나간다.

        if (HoverEvent.event_instance.isEnter) return;

        if (Input.GetMouseButtonDown(0) && photonView.IsMine)
        {// 자신이라면 로컬 함수를 호출해서 발사
            Fire();
            photonView.RPC("Fire", RpcTarget.Others);
            // 원격 네트워크 플레어어 오브젝트에 RPC로 원격으로 Fire 함수 호출
        }
    }

    [PunRPC]
    void Fire()
    {
        //Instantiate(bullet,firePos.position,firePos.rotation);
        RaycastHit hit;  // 레이캐스트로 충돌 감지시 레이어 판단
        Ray ray = new Ray(firePos.position, firePos.forward);
        if (Physics.Raycast(ray, out hit, 100f, 1 << 8 | 1 << 9 | 1 << 10))
        {
            BeamT.FireRay();
            ShowEffect(hit);
            if (hit.collider.CompareTag("Player"))  // 플레이어 공격시 데미지 호출
            {
                string Tag = hit.collider.tag;
                hit.collider.transform.parent.SendMessage("OnDamage", Tag);
            }
            if (hit.collider.CompareTag("APACHE"))  // 적 공격시 데미지 호출
            {
                string Tag = hit.collider.tag;
                hit.collider.transform.SendMessage("OnDamage_E", Tag);
            }
        }
        else
        {
            BeamT.FireRay();
            Vector3 hitpos = ray.GetPoint(200f);
            Vector3 _normal = firePos.position - hitpos.normalized;
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
            GameObject eff = Instantiate(expEffect, hitpos, rot);
            Destroy(eff, 1.5f);
        }
    }

    void ShowEffect(RaycastHit hitTank)
    {
        Vector3 hitpos = hitTank.point;
        Vector3 _normal = firePos.position - hitpos.normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject eff = Instantiate(expEffect,hitpos,rot);
        Destroy(eff,1.5f );
    }
}
