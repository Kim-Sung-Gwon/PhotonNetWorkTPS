using UnityEngine;
using Photon.Pun;
// A: 왼쪽 회전 D 오른쪽으로 회전  W 전진  S 후진 
public class TankMove : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private float h = 0f, v = 0f;
    private Transform tr;
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    private Vector3 curPos = Vector3.zero;  // 벡터나 쿼터니언 변수로 리모트 움직임 수신받는 변수
    private Quaternion curRot = Quaternion.identity;  // 회전 위치

    void Awake()
    {
        // 데이터 전송 타입
        photonView.Synchronization = ViewSynchronization.Unreliable;

        // photonView.ObservedComponents(관찰) 속성ㅇ에 TankMove 스크립트 연결
        photonView.ObservedComponents[0] = this;

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        curPos = tr.position;
        curRot = tr.rotation;
    }
    void Update()
    {
        if (photonView.IsMine)  // 포톤뷰가 자신이면 로컬이라면
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        else  // 다른 네크워크 유저 즉 리모트는
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3f);
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3f);
        }
    }

    // 자신의 이동과 회전을 서버로 송신하고 다른네트워크 유저외 움직임을 수신 받는 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // 로컬(자신)의 움직임을 송신한다.
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else  // 다른 네트워크 유저(리모트)의 움직임을 수신 받는다.
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
