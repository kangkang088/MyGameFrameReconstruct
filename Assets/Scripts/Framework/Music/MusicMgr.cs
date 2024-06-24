using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音乐管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    //背景音乐播放组件
    private AudioSource bkMusic = null;

    //背景音乐音量大小
    private float bkMusicValue = 0.1f;

    //正在播放的音效的容器
    private List<AudioSource> soundList = new();

    //音效音量大小
    private float soundValue = 0.1f;

    //音效是否在播放。true：播放  false：暂停
    private bool soundIsPlay = true;

    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// 不停遍历容器，检测音效是否播放完毕。
    /// </summary>
    private void Update()
    {
        if(!soundIsPlay)
            return;

        for(int i = soundList.Count - 1;i >= 0;i--)
        {
            if(!soundList[i].isPlaying)
            {
                //音效播放完毕，置空
                soundList[i].clip = null;
                //放入缓存池
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBKMusic(string name)
    {
        //动态创建播放背景音乐的组件，过场景不移除
        if(bkMusic == null)
        {
            GameObject obj = new("BKMusic");
            Object.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        //播放音乐
        ABResMgr.Instance.LoadResAsync<AudioClip>("music",name,(audioClip) =>
        {
            bkMusic.clip = audioClip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if(bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if(bkMusic == null)
            return;
        bkMusic.Pause();
    }

    /// <summary>
    /// 改变背景音乐音量大小
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
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步</param>
    /// <param name="callbakc">加载结束后执行的回调</param>
    public void PlaySound(string name,bool isLoop = false,bool isSync = false,UnityAction<AudioSource> callbakc = null)
    {
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound",name,(audioClip) =>
        {
            //从缓存池中取出音效对象，得到音效组件
            AudioSource source = PoolMgr.Instance.GetObj("Sound/SoundObj").GetComponent<AudioSource>();
            //万一取出的音效是之前正在使用的（超过上限了），先停止
            source.Stop();

            source.clip = audioClip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            //存入容器，用于之后判断是否要停止（因为是从缓存池中取的，所有可能存在清空：取出的是之前的，为了避免重复添加，要判断）
            if(!soundList.Contains(source))
                soundList.Add(source);
            //传给外部
            callbakc?.Invoke(source);
        },isSync);
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
            source.clip = null;
            //放入缓存池
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效音量大小
    /// </summary>
    /// <param name="value">音量值</param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for(int i = 0;i < soundList.Count;i++)
        {
            soundList[i].volume = value;
        }
    }

    /// <summary>
    /// 继续播放或暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否继续播放</param>
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
            //解决
            soundIsPlay = false;
            for(int i = 0;i < soundList.Count;i++)
            {
                //问题：暂停，也会被认为是音效没有正在播放。而Update函数中会不停的检测，从而把暂停的也给移除，怎么办？
                soundList[i].Pause();
            }
        }
    }

    /// <summary>
    /// 清空音效相关记录 清空音效缓存池之前去调用最合适
    /// </summary>
    public void ClearSound()
    {
        for(int i = 0;i < soundList.Count;i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        //清空音效列表
        soundList.Clear();
    }
}