using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

public class BossController : MonoBehaviour
{

    //externals
    public GameObject player;
    public RythmEngine rythmEngine;
    public Camera camera;
    public UtilityScript utilityScript;
    public GameManage gameManage;
    //

    //eye looking stuff
    private Vector2 rEyeBasePosSkull;
    private Vector2 lEyeBasePosSkull;

    private GameObject lEye;
    private GameObject rEye;

    private GameObject lIris;
    private GameObject rIris;

    private string eyesToFollow = "Player";

    bool following = true;

    private Vector3 normalIrisBaseSize = new Vector3(0.01f,1, 0.01f);
    private Vector3 sparkleIrisBaseSize = new Vector3(0.1f, 1, 0.1f);
    private Vector3 redSparkleIrisBaseSize = new Vector3(0.06f, 1, 0.05f);

    private GameObject lStar;
    private GameObject rStar;
    //

    //big boss face
    private GameObject bossFaceParent;
    private Vector3 intendedPos;
    private Vector3 intendedRot;
    private Vector3 intendedPosP1;
    private Vector3 intendedPosP2;
    private Vector3 basePos;
    private Vector3 baseRot;

    //

    //
    private GameObject currentBossFace;
    //







    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rEyeBasePosSkull = new Vector2(transform.Find("RightEye").transform.position.x, transform.Find("RightEye").transform.position.y);
        lEyeBasePosSkull = new Vector2(transform.Find("LeftEye").transform.position.x, transform.Find("LeftEye").transform.position.y);

       
        rEye = transform.Find("RightEye").gameObject;
        lEye = transform.Find("LeftEye").gameObject;

        if (rEye != null) rIris = rEye.transform.Find("Iris").gameObject;
        if (lEye != null) lIris = lEye.transform.Find("Iris").gameObject;

        lStar = lIris.transform.Find("RedSparkle").gameObject;
        rStar = rIris.transform.Find("RedSparkle").gameObject;


        bossFaceParent = gameObject;
     
        basePos = bossFaceParent.transform.position;
        baseRot = new Vector3(-15, 0, 0);

        intendedPosP2 = new Vector3(0, 0, 0);
        intendedPosP1 = new Vector3(0, 0, -0.5f);


        bossFaceParent.transform.position = new Vector3(0, -40, 0);

        intendedPos = bossFaceParent.transform.position;
        intendedRot = new Vector3(-15, 0, 0);
        StartCoroutine(TestFunctions());
       
        
    }


    private IEnumerator TestFunctions()
    {
        HideEyes();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ChangeBossFace("SkullFace"));
        //StartIdleBounce();
        yield return new WaitForSeconds(4f);
        StartCoroutine(SparkleAndAppear(gameManage.source.Token));
    }

    private void adjustEyes()
    {
        if (rEye == null | lEye == null | rIris == null | lIris == null) return;
        Vector2 lTargPos = Vector2.up;
        Vector2 rTargPos = Vector2.up;
        switch (eyesToFollow)
        {
            case "Mouse":
                //Debug.Log("following mouse");
                lEye.transform.localRotation = Quaternion.Euler(Input.mousePosition.y / (Screen.height / 20) + 180, -(Input.mousePosition.x - Screen.width / 2) / (Screen.width / 20), 0);
                rEye.transform.localRotation = Quaternion.Euler(Input.mousePosition.y / (Screen.height / 20)  + 180, -(Input.mousePosition.x - Screen.width / 2) / (Screen.width / 20), 0);
                break;
            case "Player":
                //Debug.Log("following plr");
                lEye.transform.LookAt(player.transform);
                rEye.transform.LookAt(player.transform);
                break;
            case "Camera":
                //Debug.Log("following camera");
                lEye.transform.LookAt(camera.gameObject.transform);
                rEye.transform.LookAt(camera.gameObject.transform);
                break;
        }

        rStar.transform.Find("Spot Light").transform.LookAt(player.transform);
        lStar.transform.Find("Spot Light").transform.LookAt(player.transform);

        lIris.transform.LookAt(camera.gameObject.transform);
        //lIris.transform.localPosition = new Vector3(lIris.transform.localPosition.x, lIris.transform.localPosition.y, 0);

        rIris.transform.LookAt(camera.gameObject.transform);
        //rIris.transform.localPosition = new Vector3(rIris.transform.localPosition.x, rIris.transform.localPosition.y, 0);

        /*
        lEye.transform.position = new Vector3(lTargPos.x, lTargPos.y, lEye.transform.position.z);
        rEye.transform.position = new Vector3(rTargPos.x, rTargPos.y, rEye.transform.position.z);
        */
    }

    private void shiftFace()
    {

        //main stuff
        if (intendedPos != bossFaceParent.transform.position)
        {
            Vector3 moveVel = intendedPos - bossFaceParent.transform.position;
            bossFaceParent.transform.position = Vector3.SmoothDamp(bossFaceParent.transform.position, intendedPos, ref moveVel, 15.0f * Time.deltaTime, 80f);

            bossFaceParent.transform.rotation = Quaternion.RotateTowards(bossFaceParent.transform.rotation, Quaternion.Euler(intendedRot), 0.03f);

        }
        

        //p1
        if (currentBossFace == null) return;
        if (intendedPosP1 != currentBossFace.transform.Find("Part1").transform.localPosition)
        {
            Vector3 moveVelP1 = intendedPosP1 - currentBossFace.transform.Find("Part1").transform.localPosition ;
            currentBossFace.transform.Find("Part1").transform.localPosition = Vector3.SmoothDamp(currentBossFace.transform.Find("Part1").transform.localPosition, intendedPosP1, ref moveVelP1, 50f * Time.deltaTime, 80f);
        }
        
        //p2
        if (intendedPosP2 != currentBossFace.transform.Find("Part2").transform.localPosition)
        {
            Vector3 moveVelP2 = intendedPosP2 - currentBossFace.transform.Find("Part2").transform.localPosition;
            currentBossFace.transform.Find("Part2").transform.localPosition = Vector3.SmoothDamp(currentBossFace.transform.Find("Part2").transform.localPosition, intendedPosP2, ref moveVelP2, 50f * Time.deltaTime, 80f);
        }
     
        
    }

    public void WompRecieve(int state, int beat)//ran by rythm manager every beat, using a function and not an event cause data needs to be passed
    {
        //Debug.Log("WompRecieve" + beat);
        mode = state;
        switch (state)//for special animations
        {
            case 1:
                SetNormalAnim();
                break;
            case 2:
                SetAgroAnim1();
                break;
            case 3:
                SetAgroAnim2();
                break;
        }
    }



    //ALL THE STUFF THAT OTHER SCRIPTS CAN TELL THE BOSS FACE TO DO!!!!======================================
    private int mode = 1;

    private bool stop =false;
    private bool flip =false;

    public IEnumerator StartIdleBounce()
    {
        while(!stop)
        {
            if (flip) intendedPos = new Vector3(intendedPos.x, intendedPos.y + 3, intendedPos.z);
            else intendedPos = new Vector3(intendedPos.x, intendedPos.y - 3, intendedPos.z);
            flip = !flip;
            yield return new WaitForSeconds(5f);

        }
        stop = false;

    }

    public void StopIdleBounce()
    {
        stop = true;
    }



    public bool requestReset()
    {
        SetNormalAnim();
        StopIdleBounce();
        HideEyes();
        bossFaceParent.transform.position = basePos;
        bossFaceParent.transform.rotation = Quaternion.Euler(baseRot);
        return true;
    }
    public void bobHeadUp()
    {
        intendedPos = new Vector3(intendedPos.x, intendedPos.y + 3, intendedPos.z);
    }

    public void bobHeadDown()
    {
        intendedPos = new Vector3(intendedPos.x, intendedPos.y - 3, intendedPos.z);
    }

    public void bobHeadUp(bool stopBounce)
    {
        intendedPos = new Vector3(intendedPos.x, intendedPos.y + 3, intendedPos.z);
        StopIdleBounce();
    }

    public void bobHeadDown(bool stopBounce)
    {
        intendedPos = new Vector3(intendedPos.x, intendedPos.y - 3, intendedPos.z);
        StopIdleBounce();
    }

    public IEnumerator SparkleAndAppear(CancellationToken token)
    {
        StartCoroutine(Sparkle(token));
        yield return new WaitForSeconds(0.15f);
        lIris.transform.Find("Standard").gameObject.SetActive(true);
        rIris.transform.Find("Standard").gameObject.SetActive(true);
    }


    public IEnumerator Sparkle(CancellationToken token)
    {

        GameObject lSpark = lIris.transform.Find("Sparkle").gameObject;
        GameObject rSpark = rIris.transform.Find("Sparkle").gameObject;
        Debug.Log(lSpark.name);
        lSpark.transform.localScale = Vector3.up;
        rSpark.transform.localScale = Vector3.up;
        lSpark.SetActive(true);
        rSpark.SetActive(true);

        StartCoroutine(utilityScript.Tween(lSpark, lSpark.transform.localPosition, lSpark.transform.localEulerAngles, sparkleIrisBaseSize, 300, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
        yield return StartCoroutine(utilityScript.Tween(rSpark, rSpark.transform.localPosition, rSpark.transform.localEulerAngles, sparkleIrisBaseSize, 300, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
        Debug.Log("You're here");
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(utilityScript.Tween(lSpark, lSpark.transform.localPosition, lSpark.transform.localEulerAngles, Vector3.up, 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
        yield return StartCoroutine(utilityScript.Tween(rSpark, rSpark.transform.localPosition, rSpark.transform.localEulerAngles, Vector3.up, 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token));
        
        lSpark.SetActive(false);
        rSpark.SetActive(false);

    }//need to update tween to be able to scale size (Done!! Yahoo!)


    



    public void SetAgroAnim1()
    {
        //skull anim
        switch (currentBossFace.name)
        {
            case "SkullFace":
                intendedPosP2 = new Vector3(0, 0, -1);
                lIris.transform.Find("Standard").gameObject.SetActive(true);
                rIris.transform.Find("Standard").gameObject.SetActive(true);
                lStar.SetActive(false);
                rStar.SetActive(false);
                intendedRot = new Vector3(-25, 0, 0);
                intendedPos = basePos + new Vector3(0, 2, -8);
                break;
        }
        Debug.Log("anim1");
        //wendigoAnim
    }
    
    public void SetAgroAnim2()
    {
        //skull anim
        switch(currentBossFace.name)
        {
            case "SkullFace":
                intendedPosP2 = new Vector3(0, 0, -2);
                lIris.transform.Find("Standard").gameObject.SetActive(false);
                rIris.transform.Find("Standard").gameObject.SetActive(false);
                lStar.SetActive(true);
                rStar.SetActive(true);
                intendedRot = new Vector3(-30, 0, 0);
                intendedPos = basePos + new Vector3(0, 4, -15);
                break;
        }
        //wendigoAnim
    }

    public void SetNormalAnim()
    {
        switch (currentBossFace.name)
        {
            case "SkullFace":
                intendedPosP2 = new Vector3(0, 0, 0);
                lStar.SetActive(false);
                rStar.SetActive(false);
                lIris.transform.Find("Standard").gameObject.SetActive(true);
                rIris.transform.Find("Standard").gameObject.SetActive(true);
                intendedRot = new Vector3(-15, 0, 0);
                intendedPos = basePos;
                break;
        }
    }

   public void HideEyes()
   {
        rIris.transform.Find("Standard").gameObject.SetActive(false);
        lIris.transform.Find("Standard").gameObject.SetActive(false);
    }

    public void SetLookTarg(string at)
    {
        eyesToFollow = at;
    }

    private List<CancellationTokenSource> sources = new List<CancellationTokenSource>();

    public IEnumerator ChangeBossFace(string changeTo)
    {
        //if (currentBossFace != null) yield break;
        //Cancelations
        for (int i = 0; i < sources.Count; i++)
        {
            sources[sources.Count - 1].Cancel();
            sources.RemoveAt(sources.Count - 1);
        }

        CancellationTokenSource curSc = new CancellationTokenSource();
        sources.Add(curSc);
        CancellationToken token = curSc.Token;
        //

        intendedPos = new Vector3(0, -100, 0);
        //Debug.Log("you made it here");
        yield return new WaitForSeconds(1f);
        if (token.IsCancellationRequested) yield break;
        
        //Debug.Log("now here");
        for (int i = 0; i < bossFaceParent.transform.childCount; i++)
        {
            GameObject curChild = bossFaceParent.transform.GetChild(i).gameObject;
            if (curChild.name == "RightEye" | curChild.name == "LeftEye") continue;
            curChild.SetActive(false);
        }
        currentBossFace = bossFaceParent.transform.Find(changeTo).gameObject;
        currentBossFace.SetActive(true);

        //any eye moving for different facePoses
        lEye.transform.position = currentBossFace.transform.Find("lEye").transform.position;
        rEye.transform.position = currentBossFace.transform.Find("rEye").transform.position;
        //

        intendedPos = basePos;

        intendedPosP2 = currentBossFace.transform.Find("Part2").transform.localPosition;
        intendedPosP1 = currentBossFace.transform.Find("Part1").transform.localPosition;
        //Debug.Log("and even here");
        yield return new WaitForSeconds(1f);
        if (token.IsCancellationRequested) yield break;
        sources.Remove(curSc);
    }

    public void rotateEyes()
    {
        

        rStar.transform.Rotate(new Vector3(0, rythmEngine.targetBpm, 0) * Time.deltaTime);
        lStar.transform.Rotate(new Vector3(0, rythmEngine.targetBpm, 0) * Time.deltaTime);
    }





    //=======================================================================================================

    // Update is called once per frame
    private IEnumerator FixedPerBPM()
    {
        int bpm = 0;
        if (rythmEngine.targetBpm < 60) bpm = 60;
        else bpm = rythmEngine.targetBpm;

        yield return new WaitForSeconds(0.1f / bpm);
       
        //everything you need to do


        //
    }
    void Update()
    {
        shiftFace();
        rotateEyes();
        if(following) adjustEyes();


    }
}
