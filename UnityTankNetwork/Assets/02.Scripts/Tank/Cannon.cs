using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float Speed = 4000f;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    public GameObject ExpEffect = null;
    private AudioSource source=null;
    [SerializeField] private AudioClip expClip;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        source = GetComponent<AudioSource>();
        ExpEffect = Resources.Load<GameObject>("Explosion");
        expClip = Resources.Load<AudioClip>("grenade_exp2");
        rb.AddForce(transform.forward * Speed);

      
        StartCoroutine(ExplosionCannon(3.0f));
    }
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ExplosionCannon(0f));
    }
    IEnumerator ExplosionCannon(float time)
    {
        yield return new WaitForSeconds(time);
        source.clip = expClip;
        source.Play();
        rb.isKinematic = true;
        capsuleCollider.enabled = false;
        GameObject eff = Instantiate(ExpEffect,transform.position,Quaternion.identity);
        Destroy(eff,1.0f);
        Destroy(this.gameObject, 1.5f);
    }


}
