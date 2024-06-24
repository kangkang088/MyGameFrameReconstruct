using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器资源加载模块管理器(开发阶段使用)
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    private readonly string rootPath = "Assets/Editor/ArtRes/";

    private EditorResMgr()
    {
    }

    /// <summary>
    /// 编辑器环境下加载单个资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns>资源</returns>
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
    /// 加载图集中的某个图片资源
    /// </summary>
    /// <param name="path">图集路径</param>
    /// <param name="spriteName">图片名</param>
    /// <returns>图片资源</returns>
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
    /// 加载图集中的所有图片，放入字典集合，返回给外部处理
    /// </summary>
    /// <param name="path">图集路径</param>
    /// <returns>图集字典</returns>
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