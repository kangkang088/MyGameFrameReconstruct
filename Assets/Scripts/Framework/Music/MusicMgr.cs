using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���ֹ�����
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //�������ֲ������
    private AudioSource bkMusic = null;

    //��������������С
    private float bkMusicValue = 0.1f;

    //���ڲ��ŵ���Ч������
    private List<AudioSource> soundList = new();

    //��Ч������С
    private float soundValue = 0.1f;

    //��Ч�Ƿ��ڲ��š�true������  false����ͣ
    private bool soundIsPlay = true;

    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// ��ͣ���������������Ч�Ƿ񲥷���ϡ�
    /// </summary>
    private void Update()
    {
        if(!soundIsPlay)
            return;

        for(int i = soundList.Count - 1;i >= 0;i--)
        {
            if(!soundList[i].isPlaying)
            {
                //��Ч������ϣ��ÿ�
                soundList[i].clip = null;
                //���뻺���
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="name"></param>
    public void PlayBKMusic(string name)
    {
        //��̬�������ű������ֵ���������������Ƴ�
        if(bkMusic == null)
        {
            GameObject obj = new("BKMusic");
            Object.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        //��������
        ABResMgr.Instance.LoadResAsync<AudioClip>("music",name,(audioClip) =>
        {
            bkMusic.clip = audioClip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopBKMusic()
    {
        if(bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// ��ͣ��������
    /// </summary>
    public void PauseBKMusic()
    {
        if(bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// �ı䱳������������С
    /// </summary>
    /// <param name="value"></param>
    public void ChangeBKMusicVaule(float value)
    {
        bkMusicValue = value;
        if(bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="name">��Ч��</param>
    /// <param name="isLoop">�Ƿ�ѭ��</param>
    /// <param name="isSync">�Ƿ�ͬ��</param>
    /// <param name="callbakc">���ؽ�����ִ�еĻص�</param>
    public void PlaySound(string name,bool isLoop = false,bool isSync = false,UnityAction<AudioSource> callbakc = null)
    {
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound",name,(audioClip) =>
        {
            //�ӻ������ȡ����Ч���󣬵õ���Ч���
            AudioSource source = PoolMgr.Instance.GetObj("Sound/SoundObj").GetComponent<AudioSource>();
            //��һȡ������Ч��֮ǰ����ʹ�õģ����������ˣ�����ֹͣ
            source.Stop();

            source.clip = audioClip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //��������������֮���ж��Ƿ�Ҫֹͣ����Ϊ�Ǵӻ������ȡ�ģ����п��ܴ�����գ�ȡ������֮ǰ�ģ�Ϊ�˱����ظ���ӣ�Ҫ�жϣ�
            if(!soundList.Contains(source))
                soundList.Add(source);
            //�����ⲿ
            callbakc?.Invoke(source);
        },isSync);
    }

    /// <summary>
    /// ֹͣ������Ч
    /// </summary>
    /// <param name="source">��Ч�������</param>
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
            source.clip = null;
            //���뻺���
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// �ı���Ч������С
    /// </summary>
    /// <param name="value">����ֵ</param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for(int i = 0;i < soundList.Count;i++)
        {
            soundList[i].volume = value;
        }
    }

    /// <summary>
    /// �������Ż���ͣ������Ч
    /// </summary>
    /// <param name="isPlay">�Ƿ��������</param>
    public void PlayOrPauese(bool isPlay)
    {
        if(isPlay)
        {
            soundIsPlay = true;
            for(int i = 0;i < soundList.Count;i++)
            {
                soundList[i].Play();
            }
        }
        else
        {
            //���
            soundIsPlay = false;
            for(int i = 0;i < soundList.Count;i++)
            {
                //���⣺��ͣ��Ҳ�ᱻ��Ϊ����Чû�����ڲ��š���Update�����л᲻ͣ�ļ�⣬�Ӷ�����ͣ��Ҳ���Ƴ�����ô�죿
                soundList[i].Pause();
            }
        }
    }

    /// <summary>
    /// �����Ч��ؼ�¼ �����Ч�����֮ǰȥ���������
    /// </summary>
    public void ClearSound()
    {
        for(int i = 0;i < soundList.Count;i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //�����Ч�б�
        soundList.Clear();
    }
}