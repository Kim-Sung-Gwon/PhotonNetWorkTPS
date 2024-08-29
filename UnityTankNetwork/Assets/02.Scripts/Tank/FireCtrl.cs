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
        //// UI�� ���콺 Ŀ���� �پҴٸ� ����������.

        if (HoverEvent.event_instance.isEnter) return;

        if (Input.GetMouseButtonDown(0) && photonView.IsMine)
        {// �ڽ��̶�� ���� �Լ��� ȣ���ؼ� �߻�
            Fire();
            photonView.RPC("Fire", RpcTarget.Others);
            // ���� ��Ʈ��ũ �÷���� ������Ʈ�� RPC�� �������� Fire �Լ� ȣ��
        }
    }

    [PunRPC]
    void Fire()
    {
        //Instantiate(bullet,firePos.position,firePos.rotation);
        RaycastHit hit;  // ����ĳ��Ʈ�� �浹 ������ ���̾� �Ǵ�
        Ray ray = new Ray(firePos.position, firePos.forward);
        if (Physics.Raycast(ray, out hit, 100f, 1 << 8 | 1 << 9 | 1 << 10))
        {
            BeamT.FireRay();
            ShowEffect(hit);
            if (hit.collider.CompareTag("Player"))  // �÷��̾� ���ݽ� ������ ȣ��
            {
                string Tag = hit.collider.tag;
                hit.collider.transform.parent.SendMessage("OnDamage", Tag);
            }
            if (hit.collider.CompareTag("APACHE"))  // �� ���ݽ� ������ ȣ��
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
