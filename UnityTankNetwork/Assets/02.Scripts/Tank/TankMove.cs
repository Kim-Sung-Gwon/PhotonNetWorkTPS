using UnityEngine;
using Photon.Pun;
// A: ���� ȸ�� D ���������� ȸ��  W ����  S ���� 
public class TankMove : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private float h = 0f, v = 0f;
    private Transform tr;
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    private Vector3 curPos = Vector3.zero;  // ���ͳ� ���ʹϾ� ������ ����Ʈ ������ ���Ź޴� ����
    private Quaternion curRot = Quaternion.identity;  // ȸ�� ��ġ

    void Awake()
    {
        // ������ ���� Ÿ��
        photonView.Synchronization = ViewSynchronization.Unreliable;

        // photonView.ObservedComponents(����) �Ӽ����� TankMove ��ũ��Ʈ ����
        photonView.ObservedComponents[0] = this;

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        curPos = tr.position;
        curRot = tr.rotation;
    }
    void Update()
    {
        if (photonView.IsMine)  // ����䰡 �ڽ��̸� �����̶��
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        else  // �ٸ� ��ũ��ũ ���� �� ����Ʈ��
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3f);
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3f);
        }
    }

    // �ڽ��� �̵��� ȸ���� ������ �۽��ϰ� �ٸ���Ʈ��ũ ������ �������� ���� �޴� �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // ����(�ڽ�)�� �������� �۽��Ѵ�.
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else  // �ٸ� ��Ʈ��ũ ����(����Ʈ)�� �������� ���� �޴´�.
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
