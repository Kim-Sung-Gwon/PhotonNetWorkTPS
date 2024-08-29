using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ���� ������ ���� �϶� ��Ʈ��ũ���� �����̾ ������ ���� �϶�
// �� ������ �÷��̾� �� ���� ����� �Ÿ��� Ž���ؼ� ���� ���� ( �߿� )
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

    // ����� �÷��̾� ����
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
        // ���̶�Ű �ȿ� ����ġ �±׸� ����
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);

        // ���� �迭 �� ������Ʈ�� ���� �ȿ� ���� �÷��̾� ������Ʈ�� ���� ����� ������Ʈ�� ã��
        Transform target = playerTanks[0].transform;

        //float distance = (target.position - tr.position).sqrMagnitude;
        ////               (  �÷��̾� ��ġ - �ڽ� ����ġ ��ġ)�� ��ü �뷫���� �Ÿ�

        // ���� �Լ����� �Ʒ� �Լ��� �� ����� ������.
        float dist = Vector3.Distance(target.position, tr.position);

        float dist2D;
        foreach (var _tank in playerTanks)
        {
            // �ڽ� ����ġ ��ġ���� �÷��̾� ������Ʈ �迭�� �� ������Ʈ���� �Ÿ��� �� ���Ѵ�.
            dist2D = (_tank.transform.position - tr.position).sqrMagnitude;
            
            if (dist2D < dist) // ��ü�Ÿ� < ���ΰŸ�
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
