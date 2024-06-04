using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseManager<T> where T : class{
    private static T instance;
    protected static readonly object lockObj = new object();
    public static T Instance {
        get {
            if(instance == null) {
                lock(lockObj) {
                    if(instance == null) {
                        Type type = typeof(T);
                        ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,null,Type.EmptyTypes,null);
                        if(constructorInfo != null)
                            instance = constructorInfo.Invoke(null) as T;
                        else
                            Debug.LogError("Not Found Constructor");
                    }
                }
            }
            return instance;
        }
    }
}
