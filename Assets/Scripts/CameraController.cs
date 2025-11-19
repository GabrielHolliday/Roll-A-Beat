using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using System.Collections;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public UtilityScript utilityScript;
    private float baselineOffset;
    private Vector3 initPos;
    private Vector3 initRot;
    private System.Random rand = new System.Random();
    public bool cameraMoveActive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initRot = transform.rotation.eulerAngles;
        //Debug.Log(initRot);
        StartCoroutine(rumble());
        initPos = transform.position;
        
    }

    static float rumbleInt = 10;
    public bool rumblin = false;
    public int angleWeight = 0;


    private float rumbleIntensity=0;
    private float addRumbleToPos = 0;
    private IEnumerator rumble()
    {
        while(true)
        {
            addRumbleToPos = Mathf.Sin(Time.time*50) * rumbleIntensity;
            rumbleIntensity -= Time.deltaTime * 2;
            rumbleIntensity = utilityScript.Clamp(rumbleIntensity,1,0);
            yield return null;
        }
    }


    

    public void addRumble(float amount)
    {
        rumbleIntensity+= amount;
        
    }

    static string boundTo = "None";

    public void bindTo(string typeOfBind)
    {
        boundTo = (string)typeOfBind;

    }



    // Update is called once per frame
    static Vector3 targVec3 = new Vector3(0,0,0);
    static Vector3 prevPos = new Vector3(0,0,0);
    void Update()
    {
        if (cameraMoveActive)
        {
            //camera position

            transform.position = Vector3.Lerp(transform.position,new Vector3(initPos.x + (player.transform.position.x / 3) + addRumbleToPos, initPos.y, initPos.z + (player.transform.position.z / 3)),0.05f);
            
            //-----------

            //camera tilt
            switch(boundTo)
            {
                case "Ball":
                    targVec3 = Vector3.Lerp(targVec3, new Vector3(baselineOffset + initRot.x - player.transform.position.z / 3, baselineOffset + initRot.y - player.transform.position.x * 1.2f, baselineOffset + initRot.z + angleWeight - player.transform.position.x * 1),0.1f);
                    break;
                case "Mouse":
                    targVec3 = Vector3.Lerp(targVec3, new Vector3(baselineOffset + initRot.x - (( Input.mousePosition.y + 300) *0.005f) , baselineOffset + initRot.y + ((Input.mousePosition.x - utilityScript.screenSize.Item2)  * 0.005f), baselineOffset + initRot.z),0.1f);
                    break;
                  
            }
            

            float guessedSpeed = 1;
            //Debug.Log(transform.rotation);
            transform.rotation = Quaternion.Euler(targVec3);

            //----------
        }

        
    }
}
