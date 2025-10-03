using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;

using UnityEngine.Timeline;
using UnityEngine.UIElements;
using TMPro;



public class UtilityScript : MonoBehaviour
{


     public enum easingStyle
    {
        None,
        Cube,
   
    }
    public enum easingDirection
    {
        In,
        Out,
        
    }
    public async void Tween(GameObject item, Vector3 endPos, Vector3 endEuler, int milliseconds, easingStyle style, easingDirection direction) //i have taken the time to learn about quaternions
    {
        if (item == null) return;

        Vector3 startPos = item.transform.position;
        Quaternion startRot = item.transform.rotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        for (int i = 0; i <= milliseconds; i++)
        {
            float lerpyPos = (float)i / milliseconds;
            switch (style)
            {
                case easingStyle.None:
                    break;
                case easingStyle.Cube:
                    if (direction == easingDirection.In) lerpyPos = lerpyPos * lerpyPos * lerpyPos; // no ^ in c#? :(
                    else if (direction == easingDirection.Out) lerpyPos = 1.0f - Mathf.Pow(1.0f - lerpyPos, 3.0f);
                    break;
            }
            item.transform.position = Vector3.Lerp(startPos, endPos, lerpyPos);
            item.transform.rotation = Quaternion.Slerp(startRot, endRot, lerpyPos);
            await Task.Delay(1);
        }
        item.transform.position = endPos;
        item.transform.rotation = endRot;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
