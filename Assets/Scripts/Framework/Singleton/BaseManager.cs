using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 单例模式基类，避免代码冗余的同时实现单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T : class
{
    private static T instance;

    protected static readonly object lockObj = new();

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock(lockObj)
                {
                    if(instance == null)
                    {
                        ConstructorInfo constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,null,Type.EmptyTypes,null);
                        if(constructorInfo != null)
                            instance = constructorInfo.Invoke(null) as T;
                        else
                            Debug.LogError("No Private Constructor!");
                    }
                }
            }

            return instance;
        }
    }

    public static T GetInstance()
    {
        if(instance == null)
        {
            ConstructorInfo constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,null,Type.EmptyTypes,null);
            if(constructorInfo != null)
                instance = constructorInfo.Invoke(null) as T;
            else
                Debug.LogError("No Private Constructor!");
        }
        return instance;
    }
}