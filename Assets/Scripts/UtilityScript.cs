
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using UnityEngine;


public class AnimateJobs
{
    public GameObject obj;
    public int jobIndex;
}

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

    public AudioLowPassFilter filter;


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

    public bool isMuffled = false;

    public IEnumerator MuffleMusic()
    {
        //Debug.Log("Muffeling");
        isMuffled = true;
        filter.cutoffFrequency = 7500;
        while (filter.cutoffFrequency > 300)
        {
            //Debug.Log("GOIN DOWN");
            filter.cutoffFrequency -= Time.deltaTime * 100000;
            yield return null;
        }
        filter.cutoffFrequency = 300;
    }

    public IEnumerator UnMuffleMusic()
    {
        isMuffled = false;
        //Debug.Log("UnMuffeling");
        while (filter.cutoffFrequency < 22000)
        {
            filter.cutoffFrequency += Time.deltaTime * 100000;
            yield return null;
        }
        filter.cutoffFrequency = 22000;
    }

    private List<AnimateJobs> alreadyTweening = new List<AnimateJobs>();
    private int jobCount = 0;
    public IEnumerator Tween(GameObject item, Vector3 endPos, Vector3 endEuler, Vector3 endScale, int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        for (int i = 0; i < alreadyTweening.Count; i++)
        {
            if(alreadyTweening[i].obj == item)
            {
                alreadyTweening.Remove(alreadyTweening[i]);
                break;
            }
        }
        
        AnimateJobs curJob = new AnimateJobs();
        curJob.obj = item;
        curJob.jobIndex = jobCount;
        jobCount ++;
        alreadyTweening.Add(curJob);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        while (time < milliseconds)
        {
            if(!alreadyTweening.Contains(curJob)) yield break;
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
        alreadyTweening.Remove(curJob);
    }
    
    public IEnumerator Tween(GameObject item, Vector3 endPos, Vector3 endEuler,  int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        for (int i = 0; i < alreadyTweening.Count; i++)
        {
            if(alreadyTweening[i].obj == item)
            {
                alreadyTweening.Remove(alreadyTweening[i]);
                break;
            }
        }
        AnimateJobs curJob = new AnimateJobs();
        curJob.obj = item;
        curJob.jobIndex = jobCount;
        jobCount ++;
        alreadyTweening.Add(curJob);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(endEuler);
        while(time < milliseconds)
        {
            if(!alreadyTweening.Contains(curJob)) yield break;
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
        alreadyTweening.Remove(curJob);
    }

    public IEnumerator Tween(GameObject item, Vector3 endPos, int milliseconds, easingStyle style, easingDirection direction, CancellationToken token)
    {
        for (int i = 0; i < alreadyTweening.Count; i++)
        {
            if(alreadyTweening[i].obj == item)
            {
                alreadyTweening.Remove(alreadyTweening[i]);
                break;
            }
        }
        AnimateJobs curJob = new AnimateJobs();
        curJob.obj = item;
        curJob.jobIndex = jobCount;
        jobCount ++;
        alreadyTweening.Add(curJob);
        float time = 0;
        Vector3 startScale = item.transform.localScale;
        Vector3 startPos = item.transform.localPosition;
        Quaternion startRot = item.transform.localRotation;
        
        while (time < milliseconds)
        {
            if(!alreadyTweening.Contains(curJob)) yield break;
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
      
        alreadyTweening.Remove(curJob);
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
