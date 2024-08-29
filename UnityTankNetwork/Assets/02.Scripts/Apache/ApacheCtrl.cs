using UnityEngine;
// 서서히 누른 만큼 힘을 받아서 움직이게 하는 로직 
public class ApacheCtrl : MonoBehaviour
{
    public float moveSpeed = 0f;
    public float rotSpeed = 0f;
    [SerializeField] Transform tr;
    private float verticalSpeed = 0f;//위 아래 속도 
    void Start()
    {
        tr = transform;
    }
    void FixedUpdate()
    {
        #region 아파치 A,D 좌우회전
        if (Input.GetKey(KeyCode.A))
            rotSpeed += -0.02f;
        else if (Input.GetKey(KeyCode.D))
            rotSpeed += 0.02f;
        else //누르지 않았을 때 
        {
            if (rotSpeed > 0f) rotSpeed += -0.02f;
            else if (rotSpeed < 0f) rotSpeed += 0.02f;
        }
        tr.Rotate(Vector3.up *rotSpeed * Time.deltaTime);
        #endregion
        #region 아파치 앞뒤 이동 
        if (Input.GetKey(KeyCode.W))
            moveSpeed += 0.02f;
        else if (Input.GetKey(KeyCode.S))
            moveSpeed += -0.02f;
        else
        {
            if (moveSpeed > 0f) moveSpeed += -0.02f;
            else if (moveSpeed < 0f) moveSpeed += 0.02f;
        }
        
        tr.Translate(Vector3.forward *moveSpeed * Time.deltaTime,Space.Self);

        #endregion
        #region 아파치 위아래 이동 
        if (Input.GetKey(KeyCode.C))
            verticalSpeed += 0.04f;
        else if (Input.GetKey(KeyCode.Z))
            verticalSpeed += -0.04f;
        else
        {
            if (verticalSpeed > 0f) verticalSpeed += -0.04f;
            else if (verticalSpeed < 0f) verticalSpeed += 0.04f;
        }
        tr.Translate(Vector3.up * verticalSpeed * Time.deltaTime,Space.Self);
        #endregion

    }
}
