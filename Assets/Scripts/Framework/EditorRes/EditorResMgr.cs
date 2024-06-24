using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �༭����Դ����ģ�������(�����׶�ʹ��)
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    private readonly string rootPath = "Assets/Editor/ArtRes/";

    private EditorResMgr()
    {
    }

    /// <summary>
    /// �༭�������¼��ص�����Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <returns>��Դ</returns>
    public T LoadEditorRes<T>(string path) where T : Object
    {
#if UNITY_EDITOR

        string suffixName = "";

        if(typeof(T) == typeof(GameObject))
            suffixName = ".prefab";
        else if(typeof(T) == typeof(Material))
            suffixName = ".mat";
        else if(typeof(T) == typeof(Texture))
            suffixName = ".png";
        else if(typeof(T) == typeof(AudioClip))
            suffixName = ".mp3";

        T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
#else
        return null;
#endif
    }

    /// <summary>
    /// ����ͼ���е�ĳ��ͼƬ��Դ
    /// </summary>
    /// <param name="path">ͼ��·��</param>
    /// <param name="spriteName">ͼƬ��</param>
    /// <returns>ͼƬ��Դ</returns>
    public Sprite LoadSprite(string path,string spriteName)
    {
#if UNITY_EDITOR
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(path + spriteName);
        foreach(Object obj in sprites)
        {
            if(spriteName == obj.name)
                return obj as Sprite;
        }
        return null;
#else
        return null;
#endif
    }

    /// <summary>
    /// ����ͼ���е�����ͼƬ�������ֵ伯�ϣ����ظ��ⲿ����
    /// </summary>
    /// <param name="path">ͼ��·��</param>
    /// <returns>ͼ���ֵ�</returns>
    public Dictionary<string,Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string,Sprite> spriteDic = new();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach(var item in sprites)
        {
            spriteDic.Add(item.name,item as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }
}