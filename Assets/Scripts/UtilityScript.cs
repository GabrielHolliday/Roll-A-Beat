using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.MessageBox;



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


    public (int, int) screenSize = (UnityEngine.Screen.width, UnityEngine.Screen.height);
    //cancelations


    public async Task WaitFor(int milliseconds)
    {
        await Task.Delay(milliseconds);
        return;
    }

    public float Clamp(float startNum, float maxNum, float minNum)
    {
        if (startNum > maxNum) return maxNum;
        else if (startNum < minNum) return minNum;
        return startNum;
    }

    private List<GameObject> alreadyTweening = new List<GameObject>();
    public async Task Tween(GameObject item, Vector3 endPos, Vector3 endEuler, Vector3 endScale,  int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        if (item == null | alreadyTweening.Contains(item)) return;
        alreadyTweening.Add(item);

        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        for (int i = 0; i <= milliseconds; i++)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
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
            item.transform.localScale = Vector3.Lerp(startScale, endScale, lerpyPos);
            item.transform.localPosition = Vector3.Lerp(startPos, endPos, lerpyPos);
            item.transform.localRotation = Quaternion.Slerp(startRot, endRot, lerpyPos);
            await Task.Delay((int)(Time.deltaTime * 1000));
        }
        item.transform.localScale = endScale;
        item.transform.localPosition = endPos;
        item.transform.localRotation = endRot;
        alreadyTweening.Remove(item);
    }
    public async Task Tween(GameObject item, Vector3 endPos, Vector3 endEuler, int milliseconds, easingStyle style, easingDirection direction, CancellationToken token) 
    {
        if (item == null | alreadyTweening.Contains(item)) return;
        alreadyTweening.Add(item);

        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        for (int i = 0; i <= milliseconds; i++)
        {
            if(token.IsCancellationRequested)
            {
                return;
            }
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
            item.transform.localPosition = Vector3.Lerp(startPos, endPos, lerpyPos);
            item.transform.localRotation = Quaternion.Slerp(startRot, endRot, lerpyPos);
            await Task.Delay(1);
        }
        item.transform.localPosition = endPos;
        item.transform.localRotation = endRot;
        alreadyTweening.Remove(item);
    }
    public async Task Tween(GameObject item, Vector3 endPos, int milliseconds, CancellationToken token) 
    {
        if (item == null | alreadyTweening.Contains(item)) return;
        alreadyTweening.Add(item);
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
       
        for (int i = 0; i <= milliseconds; i++)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            float lerpyPos = (float)i / milliseconds;
            item.transform.localPosition = Vector3.Lerp(startPos, endPos, lerpyPos);
      
            await Task.Delay(1);
        }
        item.transform.localPosition = endPos;
        alreadyTweening.Remove(item);
    }


    public void tweenNumber(ref float num, float endNum, int milliseconds, easingDirection easeingDir, easingStyle easingSty)
    {
        float startPos = num;
        for (int i = 0; i <= milliseconds; i++)
        {
            float lerpyPos = (float)i / milliseconds;
            switch (easingSty)
            {
                case easingStyle.None:
                    break;
                case easingStyle.Cube:
                    if (easeingDir == easingDirection.In) lerpyPos = lerpyPos * lerpyPos * lerpyPos; // no ^ in c#? :(
                    else if (easeingDir == easingDirection.Out) lerpyPos = 1.0f - Mathf.Pow(1.0f - lerpyPos, 3.0f);
                    break;
            }

            num = Mathf.Lerp(startPos, endNum, lerpyPos);
            Task task = WaitFor(100);//in milliseconds
            task.Wait();

        }
        num = endNum;
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
