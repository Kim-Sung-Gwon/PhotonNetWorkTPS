using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

// 자신의 Hp 0이하일때 잠시 메쉬렌더러를 비활성화해서 다시 5초 후에 활성화 시킬 예정
public class TankDamage : MonoBehaviourPunCallbacks
{
    [SerializeField] private MeshRenderer[] m_Renderer;
    [SerializeField] private GameObject expEffect;

    private int initHp = 100;
    private int curHp = 0;

    public Canvas hudCanvas;
    public Image HpBar;
    public Text KillCount;
    public int PlayerKill = 0;

    void Start()
    {
        m_Renderer = GetComponentsInChildren<MeshRenderer>();
        expEffect = Resources.Load<GameObject>("Explosion");
        curHp = initHp;
        HpBar.color = Color.green;
        KillCount.text = "<color=#00ff00>Tank Kill : </color>" +
            "<color=#ff0000>" + PlayerKill.ToString() + "</color>";
    }

    [PunRPC]
    void OnDamageRPC(string tag)  // 플레이어 끼리 체력이 깎이는 로직
    {
        if (curHp > 0 && tag == "Player")
        {
            curHp -= 25;
            HpBar.fillAmount = (float)curHp/(float)initHp;

            if (HpBar.fillAmount <= 0.6)
                HpBar.color = Color.yellow;
            if (HpBar.fillAmount <= 0.4)
                HpBar.color = Color.red;

            if (curHp <= 0)
            {
                StartCoroutine(ExplosionTank());
                Die(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    public void OnDamage(string tag)  // 플레이어끼리 체력이 깎이는 함수 호출
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnDamageRPC", RpcTarget.All, tag);
        }
    }

    [PunRPC]
    void OnE_Damage(string tag)  // 적이 공격시 플레이어 체력 깎이는 로직
    {
        if (curHp > 0 && tag == "Player")
        {
            curHp -= 5;
            HpBar.fillAmount = (float)curHp / (float)initHp;

            if (HpBar.fillAmount <= 0.6)
                HpBar.color = Color.yellow;
            if (HpBar.fillAmount <= 0.4)
                HpBar.color = Color.red;
            if (HpBar.fillAmount <= 0)
            {
                StartCoroutine(ExplosionTank());
            }
        }
    }

    public void OnEnenyDamage(string tag)  // 적이 공격시 체력이 깎이는 함수 호출
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnE_Damage", RpcTarget.All,tag);
        }
    }

    IEnumerator ExplosionTank()
    {
        Object effect = GameObject.Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 3.0f);
        SetTankvisible(false);
        hudCanvas.enabled = false;
        yield return new WaitForSeconds(5.0f);
        curHp = initHp;
        SetTankvisible(true);
        HpBar.fillAmount = 1.0f;
        HpBar.color = Color.green;
        hudCanvas.enabled = true;
    }

    void SetTankvisible(bool isvisible)
    {
        foreach (var tank in m_Renderer)
        {
            tank.enabled = isvisible;
        }
    }

    [PunRPC]  // 다른 플레이어 처치시 킬카운트 증가
    public void OnKilled(int killActorNumber)
    {
        if (photonView.IsMine)
        {
            //PhotonNetwork.Destroy(gameObject);
        }
        if (killActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            killActorNumber++;
            KillCount.text = "<color=#00ff00>Tank : </color>" +
                "<color=#ff0000>" + killActorNumber.ToString() + "</color>";
        }
    }

    public void Die(int killActorNumber)  // 킬카운트 호출
    {
        photonView.RPC("OnKilled", RpcTarget.All, killActorNumber);
    }
}
