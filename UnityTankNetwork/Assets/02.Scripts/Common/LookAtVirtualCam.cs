using Photon.Pun;
using Cinemachine;

public class LookAtVirtualCam : MonoBehaviourPun
{
    CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            virtualCamera.LookAt = transform;
        }
    }
}
