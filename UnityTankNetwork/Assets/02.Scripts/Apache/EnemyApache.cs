using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 적이 복수의 갯수 일때 네트워크에서 프레이어도 복수의 갯수 일때
// 적 개인은 플레이어 중 가장 가까운 거리를 탐색해서 공격 로직 ( 중요 )
public class EnemyApache : MonoBehaviourPun
{
    [Header("Patrol")]
    public List<Transform>patrolList = new List<Transform>();
    private Transform tr=null;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 15f;
    bool isSearch = true;
    int wayPointCount = 0;

    [Header("ApacheCtrl")]
    [SerializeField] Transform FirePos1;
    [SerializeField] Transform FirePos2;
    [SerializeField] GameObject A_bullet;
    [SerializeField] LeaserBeam[] leaserBeams;
    public GameObject expEffect;
    float curDelay = 0f;
    float maxDelay =1f;

    // 가까운 플래이어 공격
    GameObject[] playerTanks = null;
    private string tankTag = "Player";

    private void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this;
    }

    void Start()
    {
        leaserBeams[0] = GetComponentsInChildren<LeaserBeam>()[0];
        leaserBeams[1] = GetComponentsInChildren<LeaserBeam>()[1];
        var patrolPoint = GameObject.Find("PatrolPoint");
        if(patrolPoint != null )
            patrolPoint.GetComponentsInChildren<Transform>(patrolList);
        patrolList.RemoveAt(0);
        tr = transform;
        A_bullet = Resources.Load<GameObject>("A_Bullet");
        curDelay = maxDelay;
        expEffect = Resources.Load<GameObject>("Explosion");
    }
    
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (isSearch)
            {
                WayPointMove();
            }
            else
            {
                Attack();
            }
        }
    }
    
    void WayPointMove()
    {
        Vector3 PointDist = Vector3.zero;
        float dist = 0f;
       if (wayPointCount ==0)
        {
            PointDist = patrolList[0].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[0].position);
            if (dist <= 5.5f)
                wayPointCount = 1;
        }
        else if(wayPointCount ==1)
        {
            PointDist = patrolList[1].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[1].position);
            if (dist <= 5.5f)
                wayPointCount = 2;
        }
        else if (wayPointCount == 2)
        {
            PointDist = patrolList[2].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[2].position);
            if (dist <= 5.5f)
                wayPointCount = 3;
        }
        else if (wayPointCount == 3)
        {
            PointDist = patrolList[3].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[3].position);
            if (dist <= 5.5f)
                wayPointCount = 0;
        }
        Search();
    }
    
    void Search()
    {
        // 하이라키 안에 아파치 태그를 넣음
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);

        // 위의 배열 적 오브젝트의 범위 안에 들어온 플레이어 오브젝트중 가장 가까운 오브젝트를 찾음
        Transform target = playerTanks[0].transform;

        //float distance = (target.position - tr.position).sqrMagnitude;
        ////               (  플레이어 위치 - 자신 아파치 위치)의 전체 대략적인 거리

        // 위의 함수보다 아래 함수가 더 계산이 빠르다.
        float dist = Vector3.Distance(target.position, tr.position);

        float dist2D;
        foreach (var _tank in playerTanks)
        {
            // 자신 아파치 위치에서 플레이어 오브젝트 배열에 들어간 오브젝트들의 거리를 다 구한다.
            dist2D = (_tank.transform.position - tr.position).sqrMagnitude;
            
            if (dist2D < dist) // 전체거리 < 개인거리
            {
                target = _tank.transform;
                dist = (_tank.transform.position).sqrMagnitude;
            }
        }

        if (Vector3.Distance(target.transform.position, tr.position) < 80f)
        {
            isSearch = false;
        }
    }
    
    void Attack()
    {
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        Transform target = playerTanks[0].transform;
        float dist = (target.position - tr.position).sqrMagnitude;
        float dist2D;
        foreach (GameObject _tank in playerTanks)
        {
            dist2D = (_tank.transform.position - tr.position).sqrMagnitude;
            if (dist2D < dist)
            {
                target = _tank.transform;
                dist = (_tank.transform.position - tr.position).sqrMagnitude;
            }
        }

        Vector3 _normal = target.position - tr.position;
        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(_normal),
            Time.deltaTime * 3.0f);

        FireRay();

        if (Vector3.Distance(target.transform.position, tr.position) > 80f)
        {
            isSearch = true;
        }
    }

    [PunRPC]
    private void FireRay()
    {
        Ray ray = new Ray(FirePos1.position, FirePos1.forward * 100f);
        Ray ray1 = new Ray(FirePos2.position, FirePos2.forward * 100f);
        RaycastHit hit;
        //RaycastHit hit1;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) ||
              Physics.Raycast(ray1, out hit, Mathf.Infinity, 1 << 9))
        {
            curDelay -= 0.01f;
            if (curDelay <= 0)
            {
                curDelay = maxDelay;
                leaserBeams[0].FireRay();
                leaserBeams[1].FireRay();
                ShowEffect(hit);
                if (hit.collider.CompareTag("Player"))
                {
                    string tag = hit.collider.tag;
                    hit.collider.transform.parent.SendMessage("OnEnenyDamage", tag);
                }
            }
        }
        //else
        //{
        //    GameObject hiteff1 = Instantiate(expEffect, tr.InverseTransformPoint(ray.GetPoint(200f)),
        //        Quaternion.identity);
        //    Destroy(hiteff1, 2.0f);
        //}
    }

    void ShowEffect(RaycastHit hit)
    {
        Vector3 hitPos = hit.point;
        Vector3 _normal =(FirePos1.position - hitPos).normalized;
        Quaternion rot  = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject hitEff = Instantiate(expEffect, hitPos, rot);
        Destroy(hitEff, 1.0f);
    }

    //private void Fire()
    //{
    //    //Instantiate(A_bullet, FirePos1.position, FirePos1.rotation);
    //    //Instantiate(A_bullet, FirePos2.position, FirePos2.rotation);
    //}
}
