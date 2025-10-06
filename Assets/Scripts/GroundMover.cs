using System;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

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


    //---------------------------------------------------------------------------------------------------------

    void Start()
    {
        xPosRange = mainGround.transform.localScale.x;
        zSpawnPos = mainGround.transform.localScale.z / 2;
        
    }

    private async void generateSpire()
    {
        if (runGroundAnimation == false) return;
        //
        for (int i = 0; i < activeSpires.Length; i++)
        {
            if (activeSpires[i] == null) ;
            {
                activeSpires[i] = Instantiate(spire, gameObject.transform);
                activeSpires[i].transform.position = Vector3.Lerp(new Vector3(0 - xPosRange / 2, -20, zSpawnPos), new Vector3(0 - xPosRange / 2, -20, zSpawnPos),  (float)i / activeSpires.Length);






            }

        }
    }




    // Update is called once per frame
    void Update()
    {
        


    }
}
