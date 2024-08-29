using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks  // 사용시 using Photon.Pun;, using Photon.Realtime; 사용
{
    public string Version = "V1.0";
    public InputField userId;
    public InputField roomName;
    public GameObject roomItem;
    public GameObject scrollContents;

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)  // 접속하지 않았다면
        {
            PhotonNetwork.GameVersion = Version;    // 게임 버전
            PhotonNetwork.ConnectUsingSettings();   // 위의 버전의 마스터 서버로 접속(로비)
            roomName.text = "Room" + Random.Range(0, 999).ToString("000");
        }
    }

    public override void OnConnectedToMaster()  // 네트워크에 문제가 없다면 자동으로 콜백되어 로비에 들어옴
    {
        PhotonNetwork.JoinLobby();  // 로비에 연결
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby에 입장");
        //PhotonNetwork.JoinRandomRoom();  // 아무 방이나 접속(랜덤 매치 메이킹)
        userId.text = GetUserId();  // 로비에 들어오자 마자 유저 아이디 텍스트 반환
    }

    string GetUserId()
    {
        string userId = PlayerPrefs.GetString("USER_ID");  // 예약 값
        // 아이디가 비었거나 null이라면
        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");// 숫자 세자리수로 표시
        }
        return userId;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {// 랜덤 룸에 접속 실패 한 경우 자동 호출
        print("No Rooms");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()
    {// 방이 만들어 지면 자동 접속
        Debug.Log("Enter Room");
        StartCoroutine(LoadTankMainScene());
    }

    IEnumerator LoadTankMainScene()
    {
        // 씬이 이동하는 동안 포톤 클라우드 서버로 부터 네트워크 메시지 수신중단.
        PhotonNetwork.IsMessageQueueRunning = false;

        // 비동기적으로 백그라운드로 씬로딩
        // 비동기적 로딩 : 씬이 로딩 될때까지 기다리지않고 다른 스크립트를 이용
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene");
        
        yield return ao;
    }

    public void OnClickJoinRandRoom()
    {
        PhotonNetwork.NickName = userId.text;  // 포톤 네트워크에 유저 아이디 전달
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
        roomOptions.IsOpen = true;     // 방을 공개방으로 한다.
        roomOptions.IsVisible = true;  // 방목록 리스트에 룸이 보이도록 한다.
        roomOptions.MaxPlayers = 20;   // 최대 접속 가능 플래이어

        //roomOptions.CustomRoomProperties = new Hashtable() { { "Map", 5 }, { "GameType", "TDM" } };
        // ex)                                                   맵, 5번째 ,   게임 타입, 팀 데스매치
        //roomOptions.CustomRoomPropertiesForLobby = new string[] { "Map", "GameType" }
        // 지정한 조건에 맞는 룸 생성 메서드
        // CustomRoomProperties의 속성 명칭을 배열로 전달

        // 지정한 조건에 맞는 룸 생성 메서드
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
        // Photon2에서는 TypedLobby.Default를 안써도 상관이 없다.
    }

    // 생성된  룸 목록이 변경 되었을 때 호출 되는 콜백 메서드
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM"))
        {
            Destroy(obj);
        }
        foreach (RoomInfo roomInfo in roomList)
        {
            // 느려서 5~10초 정도에 메시지 출력
            Debug.Log(roomInfo.Name);

            // RoomItem 프리팹을 동기적으로 생성
            GameObject room = (GameObject)Instantiate(roomItem);
            
            // 룸을 생성            ( 룸이 들어있는 부모 오브젝트, 월드좌표 false)
            room.transform.SetParent(scrollContents.transform, false);

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = roomInfo.Name;
            roomData.connectPlayer = roomInfo.PlayerCount;
            roomData.maxPlayers = roomInfo.MaxPlayers;
            // 텍스트 정보를 표시하는 함수
            roomData.DisPlayRoomData();

            // RoomItem의 버튼 컴퍼넌트에 클릭 이벤트를 동적으로 연결
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
    {// 유니티 실시간 화면에 레이블을 콜백 메서드로 보여줌
        GUILayout.Label(PhotonNetwork.InRoom.ToString());
    }
}
