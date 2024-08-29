using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomData : MonoBehaviourPun
{
    [HideInInspector]  // �ܺ� ������ ���� public �̶�� ���������� �ν���Ʈ�� ���� ���� ����
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
