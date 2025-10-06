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

    static GameObject[] activeSpires = new GameObject[20];
    static float xPosRange = 0;
    static float zSpawnPos = 0;
    static float zRemovePos = 0;
    static float zOffset = 0;


    //---------------------------------------------------------------------------------------------------------

    void Start()
    {
        xPosRange = mainGround.transform.localScale.x;
        zSpawnPos = mainGround.transform.localScale.z / 2;
        zRemovePos = 0 - zSpawnPos;

    }

    private async void DestroyIn(int milliseconds, GameObject toRemove)
    {
        await Task.Delay(milliseconds);
        DestroyImmediate(toRemove);

    }

    private async void generateSpire()
    {
        if (runGroundAnimation == false) return;
        //
        for (int i = 0; i < activeSpires.Length; i++)
        {
            await Task.Delay(100);
            if (activeSpires[i] == null)
            {
                
                activeSpires[i] = Instantiate(spire, gameObject.transform);
                activeSpires[i].transform.position = Vector3.Lerp(new Vector3(0 - zSpawnPos / 2, -100, xPosRange / 2), new Vector3(0 - zSpawnPos / 2, -100, 0- xPosRange / 2 + zOffset),  (float)i / (float)activeSpires.Length);
                Vector3 targPos = new Vector3(activeSpires[i].transform.position.x, -activeSpires[i].transform.localScale.y / 2, activeSpires[i].transform.position.z);
                utilityScript.Tween(activeSpires[i], targPos, new Vector3(0, 0, 0), 500, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out);
            }
            else if (activeSpires[i].transform.position.z < zRemovePos)
            {
                DestroyIn(2000, activeSpires[i]);
                activeSpires[i] = null;
            }

        }
    }




    // Update is called once per frame
    void Update()
    {
        generateSpire();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.05f);
        zOffset += 0.05f;

    }
}
