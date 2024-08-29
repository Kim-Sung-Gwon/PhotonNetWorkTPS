using UnityEngine;
using Photon.Pun;

public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr;
    public float rotSpeed = 5000f;
    public float upperAngle = -30f; //���� ����
    public float downAngle = -10f; //���� ���� 
    public float currentRotate = 0f; //���� ȸ�� ����
    private Quaternion curRot = Quaternion.identity;

    void Start()
    {
        tr = transform;
        curRot = tr.localRotation;
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
        if (photonView.IsMine)
        {
            //���� ����
            float Wheel = -Input.GetAxisRaw("Mouse ScrollWheel");
            float angle = Time.deltaTime * rotSpeed * Wheel;
            if (Wheel <= -0.01f) //������ �ø��� 
            {
                currentRotate += angle;
                if (currentRotate > upperAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotate = upperAngle;
                }
            }
            else //������ ������
            {
                currentRotate += angle;
                if (currentRotate < downAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotate = downAngle;
                }
            }
        }
        else
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 3f);
        }
    }
}
