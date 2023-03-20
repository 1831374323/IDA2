using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoSingleton<AudioManager>
{
    //音效播放源
    private AudioSource sfxSource;

    //音乐播放源
    private AudioSource musicSource;

    [Header("音频文件")]
    [SerializeField] private AudioAsset sfxAssets;

    [SerializeField] private AudioAsset musicAssets;

    //获取/设置音效音量，使用属性，设置的同时更新PlayerPrefs,简单的存档功能
    public float SFXVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        }
        set
        {
            sfxSource.volume = value;
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
    }

    //获取/设置音乐音量，使用属性，设置的同时更新PlayerPrefs
    public float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        }
        set
        {
            musicSource.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    protected override void OnAwake()//重载初始化
    {
        base.OnAwake();
        musicSource = this.gameObject.AddComponent<AudioSource>();//初始化
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.volume=MusicVolume;

        sfxSource = this.gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume=SFXVolume;

        musicAssets.InitDictionary();
        sfxAssets.InitDictionary();
    }

    #region 播放音乐

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name">音乐名称</param>
    /// <param name="isLooping">是否循环播放</param>
    public void PlayMusic(string name, bool isLooping = false)
    {
        //如果玩家设置音量低于0.1f，则完全不播放，节省性能
        if (this.MusicVolume < 0.1f)
            return;
        //播放前检查键值是否存在
        if (musicAssets.audioClip.ContainsKey(name))
        {
            musicSource.clip = musicAssets.audioClip[name];
            //决定是否循环播放，默认不循环
            if (isLooping)
                musicSource.loop = true;
            musicSource.Play();
        }
    }

    /// <summary>
    /// 获取音乐播放进度
    /// </summary>
    /// <returns></returns>
    public float GetMusicPlayProgress()
    {
        return Mathf.Clamp01((musicSource.time) / musicSource.clip.length);
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    /// <summary>
    /// 继续播放音乐
    /// </summary>
    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    #endregion 播放音乐

    #region 播放音效

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名称</param>
    public void PlaySFX(string name)
    {
        //如果玩家设置音量低于0.1f，则完全不播放，节省性能
        if (this.SFXVolume < 0.1f)
            return;
        //播放前检查键值是否存在
        if (sfxAssets.audioClip.ContainsKey(name))
            sfxSource.PlayOneShot(sfxAssets.audioClip[name]);
    }

    #endregion 播放音效
}