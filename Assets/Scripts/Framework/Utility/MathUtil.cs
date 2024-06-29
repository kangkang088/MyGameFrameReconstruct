using System;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region �ǶȺͻ���ת��

    /// <summary>
    /// �Ƕ�ת����
    /// </summary>
    /// <param name="deg">�Ƕ�</param>
    /// <returns></returns>
    public static float Deg2Rad(float deg) => deg * Mathf.Deg2Rad;

    /// <summary>
    /// ����ת�Ƕ�
    /// </summary>
    /// <param name="rad">����</param>
    /// <returns></returns>
    public static float Rad2Deg(float rad) => rad * Mathf.Rad2Deg;

    #endregion �ǶȺͻ���ת��

    #region �������

    /// <summary>
    /// XZƽ��������ľ���
    /// </summary>
    /// <param name="sourcePos">��1</param>
    /// <param name="targetPos">��2</param>
    /// <returns>����</returns>
    public static float GetObjDistanceXZ(Vector3 sourcePos,Vector3 targetPos) => Vector3.Distance(new Vector3(sourcePos.x,0,sourcePos.z),new Vector3(targetPos.x,0,targetPos.z));

    /// <summary>
    /// �ж�XZ��Ļ����������Ƿ�С�ڵ���ָ��ֵ
    /// </summary>
    /// <param name="sourcePos">��1</param>
    /// <param name="targetPos">��2</param>
    /// <param name="distance">ָ��ֵ</param>
    /// <returns>�Ƿ�С��</returns>
    public static bool CheckObjDistanceXZ(Vector3 sourcePos,Vector3 targetPos,float distance) => GetObjDistanceXZ(sourcePos,targetPos) <= distance;

    /// <summary>
    /// XYƽ��������ľ���
    /// </summary>
    /// <param name="sourcePos">��1</param>
    /// <param name="targetPos">��2</param>
    /// <returns>����</returns>
    public static float GetObjDistanceXY(Vector3 sourcePos,Vector3 targetPos) => Vector3.Distance(new Vector3(sourcePos.x,sourcePos.y,0),new Vector3(targetPos.x,targetPos.y,0));

    /// <summary>
    /// �ж�XY��Ļ����������Ƿ�С�ڵ���ָ��ֵ
    /// </summary>
    /// <param name="sourcePos">��1</param>
    /// <param name="targetPos">��2</param>
    /// <param name="distance">ָ��ֵparam>
    /// <returns>�Ƿ�С��</returns>
    public static bool CheckObjDistanceXY(Vector3 sourcePos,Vector3 targetPos,float distance) => GetObjDistanceXY(sourcePos,targetPos) <= distance;

    #endregion �������

    #region λ���ж�

    /// <summary>
    /// �ж�����ռ��µ�ĳ�����Ƿ�����Ļ�ɼ���Χ��
    /// </summary>
    /// <param name="worldPos">����ռ��еĵ�</param>
    /// <returns>�Ƿ�����Ļ�ⲿ</returns>
    public static bool AnalysisWorldPosIsOutOfScreen(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        if(screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height)
            return false;
        return true;
    }

    /// <summary>
    /// �ж�XZƽ����ĳ�㣬�Ƿ������η�Χ�ڣ��������������ϵ������ͬ��
    /// </summary>
    /// <param name="pos">�������ĵ�</param>
    /// <param name="forward">�����泯��</param>
    /// <param name="pos">Ŀ���</param>
    /// <param name="radius">�뾶</param>
    /// <param name="sectorAngle">���νǶ�</param>
    /// <returns>�Ƿ������η�Χ��</returns>
    public static bool AnalysisPosIsInTheSectorRangeXZ(Vector3 centerPos,Vector3 forward,Vector3 pos,float radius,float sectorAngle)
    {
        centerPos.y = forward.y = pos.y = 0;

        return Vector3.Distance(centerPos,pos) <= radius && Vector3.Angle(forward,pos - centerPos) <= sectorAngle / 2f;
    }

    #endregion λ���ж�

    #region ���߼��

    /// <summary>
    /// ���߼�⣬��ȡһ�����������Ķ���
    /// </summary>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����Ϣ</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void Raycast(Ray ray,UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo);
        }
    }

    /// <summary>
    /// ���߼�⣬��ȡһ������������GameObject����
    /// </summary>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����Ϣ</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void Raycast(Ray ray,UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo.collider.gameObject);
        }
    }

    /// <summary>
    /// ���߼�⣬��ȡһ�����������Ķ���
    /// </summary>
    /// <typeparam name="T">�������ϵ�ָ���ű�����</typeparam>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����ص��ض��ű�</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void Raycast<T>(Ray ray,UnityAction<T> callback,float maxDistance,int layerMask) where T : MonoBehaviour
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,maxDistance,layerMask))
        {
            callback?.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// ���߼�⣬��ȡ������������Ķ���
    /// </summary>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����Ϣ</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void RaycastAll(Ray ray,UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i]);
    }

    /// <summary>
    /// ���߼�⣬��ȡ�������������GameObject����
    /// </summary>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����Ϣ</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void RaycastAll(Ray ray,UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i].collider.gameObject);
    }

    /// <summary>
    /// ���߼�⣬��ȡ������������Ķ���
    /// </summary>
    /// <typeparam name="T">�������ϵ�ָ���ű�����</typeparam>
    /// <param name="ray">����</param>
    /// <param name="callback">�ص�������������ײ���Ķ�����ص��ض��ű�</param>
    /// <param name="maxDistance">������</param>
    /// <param name="layerMask">���㼶</param>
    public static void RaycastAll<T>(Ray ray,UnityAction<T> callback,float maxDistance,int layerMask) where T : MonoBehaviour
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for(int i = 0;i < raycastHits.Length;i++)
            callback?.Invoke(raycastHits[i].collider.gameObject.GetComponent<T>());
    }

    #endregion ���߼��

    #region ��Χ���

    /// <summary>
    /// ��״��Χ���
    /// </summary>
    /// <typeparam name="T">��Ҫ��ȡ����Ϣ������</typeparam>
    /// <param name="center">��״���ĵ�</param>
    /// <param name="rotation">��״�Ƕ�</param>
    /// <param name="halfExtents">����ߵ�һ��</param>
    /// <param name="layerMask">�㼶</param>
    /// <param name="callback">�ص�����</param>
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
    /// ��״��Χ���
    /// </summary>
    /// <typeparam name="T">��Ҫ��ȡ����Ϣ������</typeparam>
    /// <param name="center">��״���ĵ�</param>
    /// <param name="rotation">��״�Ƕ�</param>
    /// <param name="halfExtents">����ߵ�һ��</param>
    /// <param name="layerMask">�㼶</param>
    /// <param name="callback">�ص�����</param>
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

    #endregion ��Χ���
}