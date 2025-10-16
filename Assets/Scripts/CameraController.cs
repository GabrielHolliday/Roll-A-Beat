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
        Debug.Log(initRot);
        initPos = transform.position;
        
    }

    static float rumbleInt = 0;
    static bool rumblin = false;

    private async void rumble()
    {
    
        rumblin = true;

        //utilityScript.tweenNumber(ref rumbBaseMovment, utilityScript.Clamp(rumbleInt, 1,-1), 10, UtilityScript.easingDirection.Out, UtilityScript.easingStyle.Cube);
        for (int j = 0; j < 100; j++)
        {

            float stPs = rumbleInt;
            float enPs = -rumbleInt;
            int cycleTime = 10;
            for (int i = 0; i < cycleTime; i++)
            {
                rumbleInt = Mathf.Lerp(stPs, enPs, i / cycleTime);
                baselineOffset = utilityScript.Clamp(rumbleInt, 0.2f, -0.2f);
                await Task.Delay(1);
            }
            rumbleInt = -rumbleInt / 1.4f;
        }
        rumblin = false;
  
        
    }

    public async void addRumble(float amount)
    {
        
        rumbleInt += amount;
        rumble();
        Debug.Log(rumbleInt);
    }

    

    // Update is called once per frame
    void Update()
    {
        if (cameraMoveActive)
        {
            //camera position

            transform.position = new Vector3(initPos.x + (player.transform.position.x / 3), initPos.y, initPos.z + (player.transform.position.z / 3));

            //-----------

            //camera tilt

            transform.rotation = Quaternion.Euler(new Vector3(baselineOffset + initRot.x - player.transform.position.z / 3, baselineOffset + initRot.y - player.transform.position.x * 1.2f, baselineOffset + initRot.z - player.transform.position.x * 1)); 

            //----------
        }

        
    }
}
