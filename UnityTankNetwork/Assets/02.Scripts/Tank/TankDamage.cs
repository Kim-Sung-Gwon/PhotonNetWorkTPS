using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

// �ڽ��� Hp 0�����϶� ��� �޽��������� ��Ȱ��ȭ�ؼ� �ٽ� 5�� �Ŀ� Ȱ��ȭ ��ų ����
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
    void OnDamageRPC(string tag)  // �÷��̾� ���� ü���� ���̴� ����
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

    public void OnDamage(string tag)  // �÷��̾�� ü���� ���̴� �Լ� ȣ��
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnDamageRPC", RpcTarget.All, tag);
        }
    }

    [PunRPC]
    void OnE_Damage(string tag)  // ���� ���ݽ� �÷��̾� ü�� ���̴� ����
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

    public void OnEnenyDamage(string tag)  // ���� ���ݽ� ü���� ���̴� �Լ� ȣ��
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

    [PunRPC]  // �ٸ� �÷��̾� óġ�� ųī��Ʈ ����
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

    public void Die(int killActorNumber)  // ųī��Ʈ ȣ��
    {
        photonView.RPC("OnKilled", RpcTarget.All, killActorNumber);
    }
}
