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

    public IEnumerator Tween(GameObject item, Vector3 endPos, Vector3 endEuler, Vector3 endScale, int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        if (item == null | alreadyTweening.Contains(item)) yield break;
        alreadyTweening.Add(item);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        while (time < milliseconds)
        {
            float lerpyPos = time / milliseconds;
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
            time += Time.deltaTime * 1000;
            yield return null;
        }
        item.transform.localScale = endScale;
        item.transform.localPosition = endPos;
        item.transform.localRotation = endRot;
        alreadyTweening.Remove(item);
    }
    
    public IEnumerator Tween(GameObject item, Vector3 endPos, Vector3 endEuler,  int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        if (item == null | alreadyTweening.Contains(item)) yield break;
        alreadyTweening.Add(item);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        while(time < milliseconds)
        {
            float lerpyPos = time / milliseconds;
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
            time += Time.deltaTime * 1000;
            yield return null;
        }
        
        item.transform.localPosition = endPos;
        item.transform.localRotation = endRot;
        alreadyTweening.Remove(item);
    }

    public IEnumerator Tween(GameObject item, Vector3 endPos, int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        if (item == null | alreadyTweening.Contains(item)) yield break;
        alreadyTweening.Add(item);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        
        while (time < milliseconds)
        {
            float lerpyPos = time / milliseconds;
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
            
            time += Time.deltaTime * 1000;
            yield return null;
        }

        item.transform.localPosition = endPos;
      
        alreadyTweening.Remove(item);
    }




    /*
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
      
            await Task.Delay((int)(Time.deltaTime * 1000));
        }
        item.transform.localPosition = endPos;
        alreadyTweening.Remove(item);
    }
    */
    // Update is called once per frame
    void Update()
    {

    }
}
