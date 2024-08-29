using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks  // ���� using Photon.Pun;, using Photon.Realtime; ���
{
    public string Version = "V1.0";
    public InputField userId;
    public InputField roomName;
    public GameObject roomItem;
    public GameObject scrollContents;

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)  // �������� �ʾҴٸ�
        {
            PhotonNetwork.GameVersion = Version;    // ���� ����
            PhotonNetwork.ConnectUsingSettings();   // ���� ������ ������ ������ ����(�κ�)
            roomName.text = "Room" + Random.Range(0, 999).ToString("000");
        }
    }

    public override void OnConnectedToMaster()  // ��Ʈ��ũ�� ������ ���ٸ� �ڵ����� �ݹ�Ǿ� �κ� ����
    {
        PhotonNetwork.JoinLobby();  // �κ� ����
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby�� ����");
        //PhotonNetwork.JoinRandomRoom();  // �ƹ� ���̳� ����(���� ��ġ ����ŷ)
        userId.text = GetUserId();  // �κ� ������ ���� ���� ���̵� �ؽ�Ʈ ��ȯ
    }

    string GetUserId()
    {
        string userId = PlayerPrefs.GetString("USER_ID");  // ���� ��
        // ���̵� ����ų� null�̶��
        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");// ���� ���ڸ����� ǥ��
        }
        return userId;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {// ���� �뿡 ���� ���� �� ��� �ڵ� ȣ��
        print("No Rooms");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()
    {// ���� ����� ���� �ڵ� ����
        Debug.Log("Enter Room");
        StartCoroutine(LoadTankMainScene());
    }

    IEnumerator LoadTankMainScene()
    {
        // ���� �̵��ϴ� ���� ���� Ŭ���� ������ ���� ��Ʈ��ũ �޽��� �����ߴ�.
        PhotonNetwork.IsMessageQueueRunning = false;

        // �񵿱������� ��׶���� ���ε�
        // �񵿱��� �ε� : ���� �ε� �ɶ����� ��ٸ����ʰ� �ٸ� ��ũ��Ʈ�� �̿�
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene");
        
        yield return ao;
    }

    public void OnClickJoinRandRoom()
    {
        PhotonNetwork.NickName = userId.text;  // ���� ��Ʈ��ũ�� ���� ���̵� ����
        PlayerPrefs.SetString("USER_ID", userId.text);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }
        PhotonNetwork.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;     // ���� ���������� �Ѵ�.
        roomOptions.IsVisible = true;  // ���� ����Ʈ�� ���� ���̵��� �Ѵ�.
        roomOptions.MaxPlayers = 20;   // �ִ� ���� ���� �÷��̾�

        //roomOptions.CustomRoomProperties = new Hashtable() { { "Map", 5 }, { "GameType", "TDM" } };
        // ex)                                                   ��, 5��° ,   ���� Ÿ��, �� ������ġ
        //roomOptions.CustomRoomPropertiesForLobby = new string[] { "Map", "GameType" }
        // ������ ���ǿ� �´� �� ���� �޼���
        // CustomRoomProperties�� �Ӽ� ��Ī�� �迭�� ����

        // ������ ���ǿ� �´� �� ���� �޼���
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
        // Photon2������ TypedLobby.Default�� �Ƚᵵ ����� ����.
    }

    // ������  �� ����� ���� �Ǿ��� �� ȣ�� �Ǵ� �ݹ� �޼���
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM"))
        {
            Destroy(obj);
        }
        foreach (RoomInfo roomInfo in roomList)
        {
            // ������ 5~10�� ������ �޽��� ���
            Debug.Log(roomInfo.Name);

            // RoomItem �������� ���������� ����
            GameObject room = (GameObject)Instantiate(roomItem);
            
            // ���� ����            ( ���� ����ִ� �θ� ������Ʈ, ������ǥ false)
            room.transform.SetParent(scrollContents.transform, false);

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.connectPlayer = roomInfo.PlayerCount;
            roomData.maxPlayers = roomInfo.MaxPlayers;
            // �ؽ�Ʈ ������ ǥ���ϴ� �Լ�
            roomData.DisPlayRoomData();

            // RoomItem�� ��ư ���۳�Ʈ�� Ŭ�� �̺�Ʈ�� �������� ����
            roomData.GetComponent<Button>().onClick.
                AddListener(delegate { OnClickRoomItem(roomData.roomName); });
        }
    }

    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);
        PhotonNetwork.JoinRoom(roomName);
    }

    private void OnGUI()
    {// ����Ƽ �ǽð� ȭ�鿡 ���̺��� �ݹ� �޼���� ������
        GUILayout.Label(PhotonNetwork.InRoom.ToString());
    }
}
