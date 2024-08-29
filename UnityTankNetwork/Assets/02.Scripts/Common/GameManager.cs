using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
                instance = value;
            else if (instance != value)
                Debug.Log("���");
        }
    }

    [SerializeField] private List<Transform> spawnList;
    [SerializeField] private GameObject apachePrefab;
    public bool isGameOver = false;

    public Text textConnect;
    public Text textLogMsg;
    public Text textCount;
    public int KillCount = 0;

    private void Awake()
    {

        Instance = this;
        DontDestroyOnLoad(gameObject);
        apachePrefab = Resources.Load<GameObject>("Apache");
        CreateTank();

        // ���� ��������� ������ �Ѿ� �Դٸ� ���� Ŭ���� �����κ���
        // �޽����� �޾Ƽ� ��Ʈ��ũ���� ����ȭ �Ǿ�� ��.
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    void Start()
    {
        var spawnPoint = GameObject.Find("SpawnPoints").gameObject;
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList);

        spawnList.RemoveAt(0);

        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "]Connected</color>";
        photonView.RPC("LogMsg", RpcTarget.AllBuffered, msg);

        //if (spawnList.Count > 0 && PhotonNetwork.IsMasterClient)
        //    InvokeRepeating("CreateApache", 0.01f, 3.0f);
        textCount.text = "<color=#00ff00>Kill Count : </color>" +
            "<color=#ff0000>" + KillCount.ToString("0000") + "</color>";
    }

    void CreateTank()
    {
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5f, pos), Quaternion.identity, 0);
        // Resources ������ ������Ʈ�� ���ٸ� ����ȭ ��ü�� ���� �ʴ´�.
    }

    void CreateApache()
    {
        if (isGameOver) return;
        int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length;
        if (count < 10)
        {
            int idx = Random.Range(0, spawnList.Count);
            PhotonNetwork.InstantiateRoomObject("Apache", spawnList[idx].position,
                spawnList[idx].rotation, 0, null);
        }
    }

    [PunRPC]
    public void KillCountText()
    {
        ++KillCount;
        textCount.text = "<color=#00ff00>Kill Count : </color>" +
            "<color=#ff0000>" + KillCount.ToString("0000") + "</color>";
    }

    [PunRPC]
    public void ApplyPlayerCountUpdate()  // �÷��̾��� ���� ���� �ٸ� Ŭ����� ����
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        textConnect.text = currentRoom.PlayerCount.ToString() +
            " / " + currentRoom.MaxPlayers.ToString();
    }

    [PunRPC]
    void GetConnectPlayerCount()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� �Լ��� ����.
            //Room currentRoom = PhotonNetwork.CurrentRoom;
            //textConnect.text = currentRoom.PlayerCount.ToString() +
            //    " / " + currentRoom.MaxPlayers.ToString();
            //photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.Others);
            //photonView.RPC("GetConnectPlayerCount", RpcTarget.Others);

            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All);
        }
    }

    public override void OnPlayerEnteredRoom(Player oterPlayer)
    {
        GetConnectPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();
    }

    public void OnClickExitRoom()  // �� ������ ��ư Ŭ�� �̺�Ʈ�� ����� �Լ�
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>";
        photonView.RPC("LogMsg", RpcTarget.All, msg);
        PhotonNetwork.LeaveRoom();
        // ���� ���� ���� ������ ������ ��� ��Ʈ��ũ�� ����
    }

    public override void OnLeftRoom()  // �뿡�� ������ ���� �Ǿ��� �� ȣ��Ǵ� �ݹ� �Լ�
    {
        SceneManager.LoadScene("LobbyScene");
    }

    [PunRPC]
    void LogMsg(string msg)
    {
        textLogMsg.text = textLogMsg.text + msg;
    }
}
