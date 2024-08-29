using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isMute = false;  // ���Ұ�
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

    public void BackGroundSound(Vector3 pos, AudioClip bgm, bool isLoop)  // ��� ����
    {
        if (isMute) return;  // ���Ұ� ���̶�� ��������
        GameObject soundObj = new GameObject("BackGroundSound");
        soundObj.transform.position = pos;  // �Ҹ��� �︱ ��ġ
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // ������ҽ� ���۳�Ʈ�� ����
        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 10f;
        audioSource.minDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }

    public void OtherPlaySound(Vector3 pos, AudioClip sfx)  // ���� �Ҹ�
    {
        if (isMute) return;  // ���Ұ� ���̶�� ��������
        GameObject soundObj = new GameObject("SFX");
        soundObj.transform.position = pos;  // �Ҹ��� �︱ ��ġ
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // ������ҽ� ���۳�Ʈ�� ����
        audioSource.clip = sfx;
        audioSource.minDistance = 10f;
        audioSource.minDistance = 30f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
}
