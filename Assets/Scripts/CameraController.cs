using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Mathematics;

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
        initPos = transform.position;
        
    }

    static float rumbleInt = 10;
    public bool rumblin = false;
    public int angleWeight = 0;


    private async void rumble()
    {
        if (rumblin) return;
    
        rumblin = true;

        while (rumblin)
        {
            for (int j = 0; j < 100; j++)
            {

                float stPs = rumbleInt;
                float enPs = -rumbleInt;
                int cycleTime = 10;
                for (int i = 0; i < cycleTime; i++)
                {
                    rumbleInt = Mathf.Lerp(stPs, enPs, i / cycleTime);
                    baselineOffset = utilityScript.Clamp(rumbleInt, 0.01f, -0.01f);
                    await Task.Delay(1);
                }
                rumbleInt = -rumbleInt;
            }
        }    
    }

    //

    

    public async void addRumble(float amount)
    {
        
        
        rumble();
      
    }

    static string boundTo = "None";

    public async void bindTo(string typeOfBind)
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

            transform.position = new Vector3(initPos.x + (player.transform.position.x / 3), initPos.y, initPos.z + (player.transform.position.z / 3));
            
            //-----------

            //camera tilt
            switch(boundTo)
            {
                case "Ball":
                    targVec3 = new Vector3(baselineOffset + initRot.x - player.transform.position.z / 3, baselineOffset + initRot.y - player.transform.position.x * 1.2f, baselineOffset + initRot.z + angleWeight - player.transform.position.x * 1);
                    break;
                case "Mouse":
                    targVec3 = new Vector3(baselineOffset + initRot.x - (( Input.mousePosition.y + 300) *0.005f) , baselineOffset + initRot.y + ((Input.mousePosition.x - utilityScript.screenSize.Item2)  * 0.005f), baselineOffset + initRot.z);
                    break;
                  
            }
            

            float guessedSpeed = 1;
            //Debug.Log(transform.rotation);
            transform.rotation = Quaternion.Euler(targVec3);

            //----------
        }

        
    }
}
