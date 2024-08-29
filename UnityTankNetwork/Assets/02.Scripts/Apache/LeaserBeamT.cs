using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaserBeamT : MonoBehaviour
{
    [SerializeField]private LineRenderer lineRenderer;
    [SerializeField] private Transform tr;
   
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = false;
        tr = GetComponent<Transform>();
        
    }
    public void FireRay()
    {
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);
        RaycastHit hit;

        lineRenderer.SetPosition(0, tr.InverseTransformPoint(ray.origin));
        if(Physics.Raycast(ray, out hit,100f,1<<8|1<<10))
        {
            lineRenderer.SetPosition(1,tr.InverseTransformPoint(hit.point));
        }
        else
        {
            lineRenderer.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));
        }
        StartCoroutine(ShowLaserBeam());
    }
    IEnumerator ShowLaserBeam()
    {
        lineRenderer.enabled= true;
        yield return new WaitForSeconds(Random.Range(0.1f,0.3f));
        lineRenderer.enabled= false;

    }
}
