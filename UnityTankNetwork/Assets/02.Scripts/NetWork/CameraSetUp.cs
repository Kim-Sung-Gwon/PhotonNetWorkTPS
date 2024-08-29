using Photon.Pun;
using Cinemachine;

public class CameraSetUp : MonoBehaviourPun  // 포톤뷰가 나의 것이면 로컬
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
