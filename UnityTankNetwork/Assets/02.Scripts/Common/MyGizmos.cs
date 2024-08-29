using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color _color = Color.green;
    public float _radius = 1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _radius);
    }

}
