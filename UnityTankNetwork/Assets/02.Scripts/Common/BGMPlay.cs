using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    [SerializeField] private Transform tr;
    public AudioClip bgm;

    void Start()
    {
        tr = transform;
        SoundManager.s_instance.BackGroundSound(tr.position, bgm, true);
    }
}
