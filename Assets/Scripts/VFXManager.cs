using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{

    //externals
    public AudioSource audioSource;
    public UtilityScript utilityScript;
    public GameManage gameManage;

    public GameObject dungeonWallPrefab;
    public GameObject forrestWallPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector3 CurrentColorTarg = new Vector3(255, 61, 61);
    private Canvas backGround;
    private Image spirePref;
    private GameObject wallParent;

    private GameObject currentSetWall;
    private float zSize;

    private AudioClip assumedClip;
    private GameObject[] cureSpires = new GameObject[64];

    private Vector3 wallSpawnPointL = new Vector3(-30, 0, 0);
    private Vector3 wallSpawnPointR = new Vector3(30, 0, 0);

    //wall stuff
    private float spawnAt;
    private float curWallLeng;
    //


    
    void Start()
    {
        backGround = transform.Find("BackgroundEffect").GetComponent<Canvas>();
        spirePref = backGround.transform.Find("Spire").GetComponent<Image>();
        BuildSpires(CurrentColorTarg);
        totalMag = 0f;
        wallParent = transform.Find("Walls").gameObject;  
    }

    public void BuildSpires(Vector3 color)
    {
        CurrentColorTarg = color;
        Debug.Log("a");
        for (int i = 0; i < 64; i++)
        {
            GameObject curImg = Instantiate(spirePref.gameObject, backGround.transform);
            curImg.transform.localPosition = new Vector3(((float)i - 25) * 2, -80  + Mathf.Abs((( i -25)*(i-25)) * 0.01f), 0);
            cureSpires[i] = curImg;
        }
    }

    public void HideWalls()
    {
        StartCoroutine(utilityScript.Tween(gameObject, new Vector3(wallParent.transform.position.x, -500, wallParent.transform.position.z), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.In, gameManage.source.Token));
    }

    public void ShowWalls()
    {
        StartCoroutine(utilityScript.Tween(gameObject, new Vector3(wallParent.transform.position.x, 0, wallParent.transform.position.z), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
    }

    private void SpawnWall(float heightModifier)
    {
        Debug.Log("r");
        if (currentSetWall == null) return;
        Debug.Log("a");
        GameObject lWall = Instantiate(currentSetWall, wallSpawnPointL + (Vector3.forward *spawnAt) + Vector3.up * heightModifier, Quaternion.Euler(new Vector3(0, 90, 0)), wallParent.transform);
        GameObject rWall =Instantiate(currentSetWall, wallSpawnPointR + (Vector3.forward * spawnAt) + Vector3.up * heightModifier, Quaternion.Euler(new Vector3(0, -90, 0)), wallParent.transform);
        
        spawnAt += curWallLeng;
    }

    


    public IEnumerator switchWallsToo(string type)
    {
        
        switch(type)
        {
            case "forest":
                currentSetWall = forrestWallPrefab;
                break;
            case "dungeon":
                currentSetWall = dungeonWallPrefab;
                break; 
        }
        HideWalls();
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < wallParent.transform.childCount; i++)
        {
            Destroy(wallParent.transform.GetChild(i).gameObject);
        }
        //wallParent.transform.localPosition = Vector3.zero;
        //spawnAt = 0;
        curWallLeng = currentSetWall.transform.Find("Border").transform.lossyScale.x;
        for (int i = 0; i < 10; i++)
        {
            SpawnWall(-500);
        }
        yield return new WaitForSeconds(1f);
        ShowWalls();
    }




    // Update is called once per frame
    static float totalMag = 0f;
    static Vector3 lastColor = new Vector3(0,0,0);
    void Update()
    {

        //all the volume bar background effect stuff=================================================================
        float[] spectrum = new float[64];
        float totalMagInternal = 0;
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        //Debug.Log(spectrum[32]);
        Vector3 colorMag = Vector3.Lerp( lastColor, CurrentColorTarg * (totalMag / cureSpires.Length), utilityScript.Clamp((CurrentColorTarg * (totalMag / cureSpires.Length)).magnitude- lastColor.magnitude,100f, 1f) * 0.005f);
        
        Color curColor = new Color(colorMag.x, colorMag.y, colorMag.z);

        for (int i = 0; i < spectrum.Length; i++)
        {
            cureSpires[i].transform.localPosition = Vector3.Lerp(cureSpires[i].transform.localPosition, new Vector3(cureSpires[i].transform.localPosition.x, utilityScript.Clamp(-80 + spectrum[i] * 500, 50, -80), 0), 0.01f);
            totalMagInternal += (((spectrum[i] + 1) * (spectrum[i] + 1)) - 1) / 10;
            cureSpires[i].gameObject.GetComponent<Image>().color = curColor;

        }
        Color icolor = cureSpires[0].gameObject.GetComponent<Image>().color;
        lastColor = new Vector3(icolor.r /1.01f, icolor.g / 1.01f, icolor.b / 1.01f);
        totalMag = totalMagInternal;

        //============================================================================================================

        //wall mover==================================================================================================
        wallParent.transform.localPosition= Vector3.back * Time.time * 6;
        if(transform.position.z < -spawnAt)
        {
            //SpawnWall(0);
        }

        //============================================================================================================

    }
}
