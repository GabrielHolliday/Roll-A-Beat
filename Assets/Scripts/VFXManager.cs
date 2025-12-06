using System.Collections;

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

    public Camera backgroundCamera;

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

    //background colors
    private Color forrestSkyColor = new Color(21,9, 53) / 255f;
    private Color dungeonSkyColor = new Color(5,5,5) / 255f;
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
        StartCoroutine(utilityScript.Tween(gameObject, new Vector3(gameObject.transform.position.x, -500, gameObject.transform.position.z), 1000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.In, gameManage.source.Token));
    }

    public void ShowWalls()
    {
        StartCoroutine(utilityScript.Tween(gameObject, new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
    }

    private void SpawnWall(float heightModifier)
    {
        
        if (currentSetWall == null) return;
        
        GameObject lWall = Instantiate(currentSetWall, wallSpawnPointL + (Vector3.forward *spawnAt) + Vector3.up * heightModifier, Quaternion.Euler(new Vector3(0, 90, 0)));
        GameObject rWall =Instantiate(currentSetWall, wallSpawnPointR + (Vector3.forward * spawnAt) + Vector3.up * heightModifier, Quaternion.Euler(new Vector3(0, -90, 0)));
        rWall.transform.parent = wallParent.transform;
        rWall.transform.localPosition = wallSpawnPointR + (Vector3.forward * spawnAt);
        lWall.transform.parent = wallParent.transform;
        lWall.transform.localPosition = wallSpawnPointL + (Vector3.forward * spawnAt);
        spawnAt += curWallLeng;
    }

    private Coroutine changeCoroutine;
    private IEnumerator changeSkyColor(Color newColor)
    {
        float incrementynator5000 = 0f;
        while(incrementynator5000 <0.99f)
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, newColor, incrementynator5000);
            backgroundCamera.backgroundColor = Color.Lerp(backgroundCamera.backgroundColor, newColor, incrementynator5000);
            incrementynator5000 += 0.05f*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("finished");
    }

    private void TransitionToForrestBackGround()
    {
        changeSkyColor(forrestSkyColor);
        //rollInBackground();

    }

    
    public IEnumerator switchWallsToo(string type)
    {
        if(changeCoroutine != null) StopCoroutine(changeCoroutine);
        switch(type)
        {
            case "forest":
                //any lighting animations
                changeCoroutine = StartCoroutine(changeSkyColor(forrestSkyColor));
                //
                currentSetWall = forrestWallPrefab;
                break;
            case "dungeon":
                //lighting
                changeCoroutine = StartCoroutine(changeSkyColor(dungeonSkyColor));
                //
                currentSetWall = dungeonWallPrefab;
                break; 
        }
        HideWalls();
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < wallParent.transform.childCount; i++)
        {
            Destroy(wallParent.transform.GetChild(i).gameObject);
        }
        wallParent.transform.localPosition = Vector3.zero;
        spawnAt = 0;
        curWallLeng = currentSetWall.transform.Find("Border").transform.lossyScale.x;
        for (int i = 0; i < 10; i++)
        {
            //SpawnWall(-500);
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
        wallParent.transform.localPosition+=Vector3.back * 5*Time.deltaTime;
        //spawnAt-= 1*10*Time.deltaTime;
        if (wallParent.transform.localPosition.z < -spawnAt + 500)
        {
            if (wallParent.transform.childCount < 200)
            {
                    SpawnWall(transform.position.y);
            }
            else
            {
                    (GameObject, float) toDestryL = (null, 0f);
                    (GameObject, float) toDestryR = (null, 0f);
                    for (int i = 0; i < wallParent.transform.childCount; i++)
                    {

                        if (wallParent.transform.GetChild(i).transform.localPosition.x > 0 && (toDestryL.Item1 == null || wallParent.transform.GetChild(i).transform.localPosition.z < toDestryL.Item2))
                        {
                            toDestryL = (wallParent.transform.GetChild(i).gameObject, wallParent.transform.GetChild(i).transform.localPosition.z);
                        }
                        if (wallParent.transform.GetChild(i).transform.localPosition.x < 0 && (toDestryR.Item1 == null || wallParent.transform.GetChild(i).transform.localPosition.z < toDestryR.Item2))
                        {
                            toDestryR = (wallParent.transform.GetChild(i).gameObject, wallParent.transform.GetChild(i).transform.localPosition.z);
                        }
                    }
                    Destroy(toDestryR.Item1);
                    Destroy(toDestryL.Item1);

                    SpawnWall(transform.position.y);
            }
        }

        //============================================================================================================

    }
}
