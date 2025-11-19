using System;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

public class GroundMover : MonoBehaviour
{

    public GameObject mainGround;
    public GameObject spire;
    public UtilityScript utilityScript;
    public PlayerController playerController;
    public bool runGroundAnimation = true;


    //internal variables---------------------------------------------------------------------------------------

    static GameObject[] activeSpires = new GameObject[48]; 
    static int[] statuses = new int[48];// 0 = inactive, and awaiting to be moved up, 1 is already moved up, 2 is moving down
    static float xPosRange = 0;
    static float zSpawnPos = 7;
    static float zRemovePos = 0;
    static float zOffset = 0;


    //---------------------------------------------------------------------------------------------------------

    //async cancel

    static CancellationTokenSource source;
    //

    void Start()
    {
        source = new CancellationTokenSource();
        xPosRange = mainGround.transform.localScale.x;
        zSpawnPos = mainGround.transform.localScale.z / 2 + zSpawnPos;
        zRemovePos = 0 - zSpawnPos;
        StartCoroutine(LoadSpires());
        StartCoroutine(moveGround(source.Token));   
    }

    private IEnumerator DestroyIn(int milliseconds, GameObject toRemove)
    {
        yield return new WaitForSeconds((float)milliseconds / 1000f);
        Destroy(toRemove);
    }

    private IEnumerator LoadSpires()
    {
        Vector3 targp = new Vector3(9999, -100, 999);
        for (int i = 0; i < activeSpires.Length; i++)
        {
            if (activeSpires[i] != null) activeSpires[i] = null;
            activeSpires[i] = Instantiate(spire, gameObject.transform);
            activeSpires[i].transform.position = targp;
            statuses[i] = 1;

        }
        StartCoroutine(generateSpire(source.Token));
        for (int i = 0; i < activeSpires.Length; i++) //initial load up
        {
            yield return new WaitForSeconds(0.3f);
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

            Vector3 targPos = new Vector3(activeSpires[i].transform.localPosition.x, -activeSpires[i].transform.localScale.y / 2 -0.02f, zSpawnPos + zOffset);
            StartCoroutine(utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, source.Token));
        }
        
    }


    private IEnumerator generateSpire(CancellationToken tkn)
    {
        //
        while (runGroundAnimation == true)
        {
            for (int i = 0; i < activeSpires.Length; i++)
            {
                
                
                if (statuses[i] == 0)
                {
                    //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAA");
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

                    Vector3 targPos = new Vector3(activeSpires[i].transform.localPosition.x, -activeSpires[i].transform.localScale.y / 2 - 0.02f, zSpawnPos + zOffset);
                    //activeSpires[i].transform.localPosition = targPos;
                    StartCoroutine(utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, source.Token));
                }
                else if (activeSpires[i].transform.position.z < -12 & statuses[i] == 1)
                {
                    statuses[i] = 2;
                    //statuses[i] = 0;
                    Vector3 targPos = new Vector3(activeSpires[i].transform.localPosition.x, -75, activeSpires[i].transform.localPosition.z);
                    StartCoroutine(utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 180, 0), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.In, source.Token));
                    StartCoroutine(SetInMiliseconds(510, i, 0));
                    //activeSpires[i] = null;
                }

            }

            yield return null;
        }
        
    }

    private IEnumerator SetInMiliseconds(int miliseconds, int index, int status)
    {
        yield return new WaitForSeconds((float)miliseconds / 1000f);
        statuses[index] = status;
    }

    public void StopGround()
    {
        source.Cancel();
        runGroundAnimation = false;
        playerController.speed = 5;
    }

    public void Play(float speed, string level)
    {
        playerController.groundSpeed = speed;
        
    }

    private IEnumerator moveGround(CancellationToken tkn)
    {
        while(runGroundAnimation == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zOffset);
            zOffset += 3 * Time.deltaTime;
            yield return null;
            
        }
    }

  


    // Update is called once per frame
    void Update()
    {
        //generateSpire();
       
        

    }
}
