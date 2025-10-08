using System;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class GroundMover : MonoBehaviour
{

    public GameObject mainGround;
    public GameObject spire;
    public UtilityScript utilityScript;
    public bool runGroundAnimation = true;


    //internal variables---------------------------------------------------------------------------------------

    static GameObject[] activeSpires = new GameObject[36]; 
    static int[] statuses = new int[36];// 0 = inactive, and awaiting to be moved up, 1 is already moved up, 2 is moving down
    static float xPosRange = 0;
    static float zSpawnPos = 7;
    static float zRemovePos = 0;
    static float zOffset = 0;


    //---------------------------------------------------------------------------------------------------------

    void Start()
    {
        xPosRange = mainGround.transform.localScale.x;
        zSpawnPos = mainGround.transform.localScale.z / 2 + zSpawnPos;
        zRemovePos = 0 - zSpawnPos;
        LoadSpires();
        
        
    }

    private async void DestroyIn(int milliseconds, GameObject toRemove)
    {
        await Task.Delay(milliseconds);
        DestroyImmediate(toRemove);

    }

    private async void LoadSpires()
    {
        Vector3 targp = new Vector3(9999, -100, 999);
        for (int i = 0; i < activeSpires.Length; i++)
        {
            if (activeSpires[i] != null) activeSpires[i] = null;
            activeSpires[i] = Instantiate(spire, gameObject.transform);
            activeSpires[i].transform.position = targp;
            statuses[i] = 1;

        }
        generateSpire();
        for (int i = 0; i < activeSpires.Length; i++) //initial load up
        {
            await Task.Delay(500);
            //Debug.Log("loading");
            if ((i + 1) % 3 == 0)
            {
                activeSpires[i].transform.localPosition = new Vector3(xPosRange / 4, -75, zSpawnPos + zOffset);

            }
            else if ((i + 1) % 2 == 0)
            {
                activeSpires[i].transform.localPosition = new Vector3(0 - xPosRange / 4, -75, zSpawnPos + zOffset);

            }
            else
            {
                activeSpires[i].transform.localPosition = new Vector3(0, -75, zSpawnPos + zOffset);

            }

            Vector3 targPos = new Vector3(activeSpires[i].transform.localPosition.x, -activeSpires[i].transform.localScale.y / 2, zSpawnPos + zOffset);
            utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 500, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out);
        }
    }

    private async void generateSpire()
    {
        //
        while (runGroundAnimation == true)
        {
            for (int i = 0; i < activeSpires.Length; i++)
            {
                
                
                if (statuses[i] == 0)
                {
                    Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
                    statuses[i] = 1;

                    //activeSpires[i].transform.position = Vector3.Lerp(new Vector3(zSpawnPos / 2, -75, xPosRange / 2 + zOffset), new Vector3(0 - zSpawnPos / 2, -75, 0 - xPosRange / 2 + zOffset), (float)i / (float)activeSpires.Length);

                    //Debug.Log(zOffset + zSpawnPos);
                    if ((i + 1) % 3 == 0)
                    {
                        activeSpires[i].transform.localPosition = new Vector3(xPosRange/ 4, -75, zSpawnPos  + zOffset);
                        
                    }
                    else if ((i + 1) % 2 == 0)
                    {
                        activeSpires[i].transform.localPosition = new Vector3(0 - xPosRange/ 4, -75, zSpawnPos + zOffset);
                        
                    }
                    else
                    {
                        activeSpires[i].transform.localPosition = new Vector3(0, -75, zSpawnPos  + zOffset);
                       
                    }

                    Vector3 targPos = new Vector3(activeSpires[i].transform.localPosition.x, -activeSpires[i].transform.localScale.y / 2, zSpawnPos + zOffset);
                    //activeSpires[i].transform.localPosition = targPos;
                    utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 500, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out);
                }
                else if (activeSpires[i].transform.position.z < -12 & statuses[i] == 1)
                {
                    statuses[i] = 2;
                    //statuses[i] = 0;
                    Vector3 targPos = new Vector3(activeSpires[i].transform.position.x, -75, activeSpires[i].transform.position.z);
                    utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 500, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.In);
                    SetInMiliseconds(510, i, 0);
                    //activeSpires[i] = null;
                }

            }
            await Task.Delay(1);
        }
        
    }

    private async void SetInMiliseconds(int miliseconds, int index, int status)
    {
        await Task.Delay(miliseconds);
        statuses[index] = status;
    }

  


    // Update is called once per frame
    void Update()
    {
        //generateSpire();
        transform.position = new Vector3(transform.position.x, transform.position.y, -zOffset);
        zOffset += 0.01f;

    }
}
