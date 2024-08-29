using Photon.Pun;
using Cinemachine;

public class CameraSetUp : MonoBehaviourPun  // ����䰡 ���� ���̸� ����
{
    void Start()
    {
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType(typeof(CinemachineVirtualCamera))
                as CinemachineVirtualCamera;
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }
}
