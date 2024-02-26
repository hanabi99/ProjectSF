using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    /// <summary>
    /// 通过父物体和目标物体的名字寻找目标物体
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="gameobjectName"></param>
    /// <returns></returns>
    public static Transform FindMyChild(this Transform parentTF, string childName)
    {
        Transform childTF = parentTF.Find(childName);
        if (childTF != null) return childTF;
        int count = parentTF.childCount;
        for (int i = 0; i < count; i++)
        {
            childTF = FindMyChild(parentTF.GetChild(i), childName);
            if (childTF != null) return childTF;
        }
        return null;
    }

    /// <summary>
    /// 跟随一个物体
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="parentTF"></param>
    /// <param name="offset"></param>
    public static void FollowObject(this Transform transform ,Transform parentTF, Vector3 offset)
    {
        transform.position = parentTF.position + offset;
        Quaternion newRotation = Quaternion.Euler(-20, parentTF.eulerAngles.y +180 ,0);
        transform.rotation = newRotation;
    }

}
