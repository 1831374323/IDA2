using System;
using System.Collections.Generic;
using UnityEngine;

// 创建资源创建菜单，文件名为fileName，菜单项名为menuName
[CreateAssetMenu(fileName = "NewAudioAsset", menuName = "Audio Asset")]
public class AudioAsset : ScriptableObject //定义一个音效列表资源，方便后期扩展以及加载
{
    //音效资源列表
    public Dictionary<string, AudioClip> audioClip = new Dictionary<string, AudioClip>();

    [Header("音效资源列表")]
    [SerializeField] private List<SerializableKeyValuePair> audioList = new List<SerializableKeyValuePair>();

    [Serializable]
    private class SerializableKeyValuePair
    {
        public string Key;
        public AudioClip Value;

        public SerializableKeyValuePair(string key, AudioClip value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// 初始化字典，使用前必须执行
    /// </summary>
    public void InitDictionary()
    {
        foreach (var item in audioList)
        {
            audioClip.Add(item.Key, item.Value);
        }
    }
}