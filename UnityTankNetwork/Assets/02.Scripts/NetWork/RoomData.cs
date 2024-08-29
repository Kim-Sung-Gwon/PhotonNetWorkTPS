using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomData : MonoBehaviourPun
{
    [HideInInspector]  // 외부 접근을 위해 public 이라고 선언했지만 인스펙트에 노출 되지 않음
    public string roomName = string.Empty;
    [HideInInspector]
    public int connectPlayer = 0;
    [HideInInspector]
    public int maxPlayers = 0;
    public Text textRoomName;
    public Text textConnectInfo;

    public void DisPlayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
