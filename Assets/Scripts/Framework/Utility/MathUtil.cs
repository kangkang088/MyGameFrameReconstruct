using System;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region 角度和弧度转换

    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="deg">角度</param>
    /// <returns></returns>
    public static float Deg2Rad(float deg) => deg * Mathf.Deg2Rad;

    /// <summary>
    /// 弧度转角度
    /// </summary>
    /// <param name="rad">弧度</param>
    /// <returns></returns>
    public static float Rad2Deg(float rad) => rad * Mathf.Rad2Deg;

    #endregion 角度和弧度转换

    #region 距离计算

    /// <summary>
    /// XZ平面上两点的距离
    /// </summary>
    /// <param name="sourcePos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns>距离</returns>
    public static float GetObjDistanceXZ(Vector3 sourcePos,Vector3 targetPos) => Vector3.Distance(new Vector3(sourcePos.x,0,sourcePos.z),new Vector3(targetPos.x,0,targetPos.z));

    /// <summary>
    /// 判断XZ屏幕上两点距离是否小于等于指定值
    /// </summary>
    /// <param name="sourcePos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="distance">指定值</param>
    /// <returns>是否小于</returns>
    public static bool CheckObjDistanceXZ(Vector3 sourcePos,Vector3 targetPos,float distance) => GetObjDistanceXZ(sourcePos,targetPos) <= distance;

    /// <summary>
    /// XY平面上两点的距离
    /// </summary>
    /// <param name="sourcePos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns>距离</returns>
    public static float GetObjDistanceXY(Vector3 sourcePos,Vector3 targetPos) => Vector3.Distance(new Vector3(sourcePos.x,sourcePos.y,0),new Vector3(targetPos.x,targetPos.y,0));

    /// <summary>
    /// 判断XY屏幕上两点距离是否小于等于指定值
    /// </summary>
    /// <param name="sourcePos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="distance">指定值param>
    /// <returns>是否小于</returns>
    public static bool CheckObjDistanceXY(Vector3 sourcePos,Vector3 targetPos,float distance) => GetObjDistanceXY(sourcePos,targetPos) <= distance;

    #endregion 距离计算

    #region 位置判断

    /// <summary>
    /// 判断世界空间下的某个点是否在屏幕可见范围内
    /// </summary>
    /// <param name="worldPos">世界空间中的点</param>
    /// <returns>是否在屏幕外部</returns>
    public static bool AnalysisWorldPosIsOutOfScreen(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        if(screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            return false;
        return true;
    }

    /// <summary>
    /// 判断XZ平面中某点，是否在扇形范围内（所有坐标的坐标系必须相同）
    /// </summary>
    /// <param name="pos">扇形中心点</param>
    /// <param name="forward">扇形面朝向</param>
    /// <param name="pos">目标点</param>
    /// <param name="radius">半径</param>
    /// <param name="sectorAngle">扇形角度</param>
    /// <returns>是否在扇形范围内</returns>
    public static bool AnalysisPosIsInTheSectorRangeXZ(Vector3 centerPos,Vector3 forward,Vector3 pos,float radius,float sectorAngle)
    {
        centerPos.y = forward.y = pos.y = 0;

        return Vector3.Distance(centerPos,pos) <= radius && Vector3.Angle(forward,pos - centerPos) <= sectorAngle / 2f;
    }

    #endregion 位置判断

    #region 射线检测

    /// <summary>
    /// 射线检测，获取一个射线碰到的对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象信息</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void Raycast(Ray ray,UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo);
        }
    }

    /// <summary>
    /// 射线检测，获取一个射线碰到的GameObject对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象信息</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void Raycast(Ray ray,UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo.collider.gameObject);
        }
    }

    /// <summary>
    /// 射线检测，获取一个射线碰到的对象
    /// </summary>
    /// <typeparam name="T">对象身上的指定脚本类型</typeparam>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象挂载的特定脚本</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void Raycast<T>(Ray ray,UnityAction<T> callback,float maxDistance,int layerMask) where T : MonoBehaviour
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 射线检测，获取多个射线碰到的对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象信息</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void RaycastAll(Ray ray,UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i]);
    }

    /// <summary>
    /// 射线检测，获取多个射线碰到的GameObject对象
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象信息</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void RaycastAll(Ray ray,UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i].collider.gameObject);
    }

    /// <summary>
    /// 射线检测，获取多个射线碰到的对象
    /// </summary>
    /// <typeparam name="T">对象身上的指定脚本类型</typeparam>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调函数，传递碰撞到的对象挂载的特定脚本</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">检测层级</param>
    public static void RaycastAll<T>(Ray ray,UnityAction<T> callback,float maxDistance,int layerMask) where T : MonoBehaviour
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i].collider.gameObject.GetComponent<T>());
    }

    #endregion 射线检测

    #region 范围检测

    /// <summary>
    /// 盒状范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息的类型</typeparam>
    /// <param name="center">盒状中心点</param>
    /// <param name="rotation">盒状角度</param>
    /// <param name="halfExtents">长宽高的一半</param>
    /// <param name="layerMask">层级</param>
    /// <param name="callback">回调函数</param>
    public static void OverlapBox<T>(Vector3 center,Quaternion rotation,Vector3 halfExtents,int layerMask,UnityAction<T> callback) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapBox(center,halfExtents,rotation,layerMask,QueryTriggerInteraction.Collide);
        for(int i = 0;i < colliders.Length;i++)
        {
            if(type == typeof(Collider))
            {
                callback?.Invoke(colliders[i] as T);
            }
            else if(type == typeof(Collider))
            {
                callback?.Invoke(colliders[i].gameObject as T);
            }
            else
            {
                callback?.Invoke(colliders[i].GetComponent<T>());
            }
        }
    }

    /// <summary>
    /// 球状范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息的类型</typeparam>
    /// <param name="center">球状中心点</param>
    /// <param name="rotation">球状角度</param>
    /// <param name="halfExtents">长宽高的一半</param>
    /// <param name="layerMask">层级</param>
    /// <param name="callback">回调函数</param>
    public static void OverlapSphere<T>(Vector3 center,float radius,int layerMask,UnityAction<T> callback) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center,radius,layerMask,QueryTriggerInteraction.Collide);
        for(int i = 0;i < colliders.Length;i++)
        {
            if(type == typeof(Collider))
            {
                callback?.Invoke(colliders[i] as T);
            }
            else if(type == typeof(Collider))
            {
                callback?.Invoke(colliders[i].gameObject as T);
            }
            else
            {
                callback?.Invoke(colliders[i].GetComponent<T>());
            }
        }
    }

    #endregion 范围检测
}