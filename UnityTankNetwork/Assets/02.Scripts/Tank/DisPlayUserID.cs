using Photon.Pun;
using UnityEngine.UI;

public class DisPlayUserID : MonoBehaviourPun
{
    public Text UserID;

    void Start()
    {
        UserID.text = photonView.Owner.NickName;
        // 아이디를 오브젝트에 표시
    }
}
