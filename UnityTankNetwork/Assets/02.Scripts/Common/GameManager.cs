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
                Debug.Log("경고");
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

        // 방이 만들어지고 씬으로 넘어 왔다면 포톤 클라우드 서버로부터
        // 메시지를 받아서 네트워크에서 동기화 되어야 함.
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
        // Resources 폴더에 오브젝트가 없다면 동기화 자체가 되지 않는다.
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
    public void ApplyPlayerCountUpdate()  // 플레이어의 접속 수를 다른 클라우드와 공유
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
            // 밑의 함수와 같다.
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

    public void OnClickExitRoom()  // 룸 나가기 버튼 클릭 이벤트에 연결될 함수
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>";
        photonView.RPC("LogMsg", RpcTarget.All, msg);
        PhotonNetwork.LeaveRoom();
        // 현재 룸을 빠져 나가면 생성한 모든 네트워크를 삭제
    }

    public override void OnLeftRoom()  // 룸에서 접속이 종료 되었을 때 호출되는 콜백 함수
    {
        SceneManager.LoadScene("LobbyScene");
    }

    [PunRPC]
    void LogMsg(string msg)
    {
        textLogMsg.text = textLogMsg.text + msg;
    }
}
