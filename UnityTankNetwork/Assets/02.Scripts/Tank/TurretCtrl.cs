using UnityEngine;
using Photon.Pun;
// ray�� ��Ƽ� ���콺 ������ ���� 
public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr;
    private float rotSpeed = 5f;
    RaycastHit hit;
    private Quaternion curRot = Quaternion.identity;

    void Start()
    {
        tr = transform;
        curRot = tr.localRotation;  // local�� ���� ���� �θ� ������Ʈ�� ���� ȸ���ϱ�(�����̱�)���ؼ�
        photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (photonView.IsMine)  // ����䰡 ����(�ڽ�)�̶��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //ī�޶󿡼� ���콺 ������ �������� ������ �߻� 
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

            if (Physics.Raycast(ray, out hit, 100f, 1 << 8))
            {  //���̰� �ͷ��ο� �¾Ҵٸ� 

                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //�¾Ҵ� ���� ��ġ�� ��ũ�� �´� ������ǥ�� �ٲ�
                
                // Mathf.Deg2Rad;�Ϲݰ����� ���� ������ �ٲ� 
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                //���𰢵��� �Ϲݰ����� �ٲ�
                //��ź��Ʈ �Լ��� Atan2��  ������ ������ ���

                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f);
            }
        }
        else  // ����Ʈ(Remote : ����)
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 3f);
        }
    }
}
