using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaserBeam : MonoBehaviour
{
    [SerializeField]LineRenderer lineRenderer;
    [SerializeField] Transform tr;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = false;//로컬좌표로 하기 위해 
        tr = GetComponent<Transform>();
        
    }
    public void FireRay()
    {
        Ray ray  = new Ray(tr.position+(Vector3.up *0.02f) , tr.forward);
                            // 약간 올림 , 포워드 방향으로 
                                     //월드 혹은 절대 좌표를 로컬좌표로 변환
        lineRenderer.SetPosition(0,tr.InverseTransformPoint(ray.origin));
                      //라인랜더러의 첫번째 점을 발사위치로 잡는다. 0 이 첫번째 점
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit,200f))
        {   // 맞았다면   맞은 지점을 끝점으로 잡는다. 1
            lineRenderer.SetPosition(1,tr.InverseTransformPoint(hit.point));
        }
        else
        {    //맞지 않았다면  마지막 끝점을 ray.GetPoint(200f) 200지점을 끝점으로 잡는다.
            lineRenderer.SetPosition(1,tr.InverseTransformPoint(ray.GetPoint(200f)));
        }

        StartCoroutine(ShowLaserBeam() ); //라인렌더러가 보였다 안보였다 하기위해 
    }
    IEnumerator ShowLaserBeam()
    {
        lineRenderer.enabled =true;
        yield return new WaitForSeconds(Random.Range(0.1f,0.3f));
        lineRenderer.enabled = false;

    }
}
