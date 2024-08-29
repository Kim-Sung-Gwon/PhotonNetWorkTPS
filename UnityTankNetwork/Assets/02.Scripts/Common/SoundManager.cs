using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isMute = false;  // 음소거
    public static SoundManager s_instance;
    
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void BackGroundSound(Vector3 pos, AudioClip bgm, bool isLoop)  // 배경 음악
    {
        if (isMute) return;  // 음소거 중이라면 빠져나감
        GameObject soundObj = new GameObject("BackGroundSound");
        soundObj.transform.position = pos;  // 소리가 울릴 위치
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // 오디오소스 컴퍼넌트를 생성
        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 10f;
        audioSource.minDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }

    public void OtherPlaySound(Vector3 pos, AudioClip sfx)  // 폭파 소리
    {
        if (isMute) return;  // 음소거 중이라면 빠져나감
        GameObject soundObj = new GameObject("SFX");
        soundObj.transform.position = pos;  // 소리가 울릴 위치
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // 오디오소스 컴퍼넌트를 생성
        audioSource.clip = sfx;
        audioSource.minDistance = 10f;
        audioSource.minDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
}
