using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using System.Collections.Generic;

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
    private Vector3 redSparkleIrisBaseSize = new Vector3(0.06f,1, 0.05f);
    //

    //big boss face
    private GameObject bossFaceParent;
    private Vector3 intendedPos;
    private Vector3 intendedPosP1;
    private Vector3 intendedPosP2;
    private Vector3 basePos;

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


        bossFaceParent = gameObject;
     
        basePos = bossFaceParent.transform.position;

        intendedPosP1 = new Vector3(0, 0, 0);
        intendedPosP2 = new Vector3(0, 0, -0.5f);


        bossFaceParent.transform.position = new Vector3(0, -40, 0);
        intendedPos = bossFaceParent.transform.position;
        TestFunctions();
       
        
    }


    private async void TestFunctions()
    {
        await Task.Delay(2000);
        ChangeBossFace("SkullFace");
        //StartIdleBounce();
        await Task.Delay(4000);
        SparkleAndAppear(gameManage.source.Token);
    }

    private void adjustEyes()
    {
        if (rEye == null | lEye == null | rIris == null | lIris == null) return;
        Vector2 lTargPos = Vector2.up;
        Vector2 rTargPos = Vector2.up;
        switch (eyesToFollow)
        {
            case "Mouse":
                break;
            case "Player":
                lEye.transform.LookAt(player.transform);
                rEye.transform.LookAt(player.transform);
                break;
        }
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
        if (intendedPos == Vector3.zero | intendedPos == bossFaceParent.transform.position) return;
        Vector3 moveVel = intendedPos - bossFaceParent.transform.position;
        bossFaceParent.transform.position = Vector3.SmoothDamp(bossFaceParent.transform.position, intendedPos, ref moveVel, 0.1f, 80f);

        //p1
        if (currentBossFace == null) return;
        if (intendedPosP1 == Vector3.zero | intendedPosP1 == currentBossFace.transform.Find("Part1").transform.position) return;
        Vector3 moveVelP1 = intendedPosP1 - currentBossFace.transform.Find("Part1").transform.position;
        currentBossFace.transform.Find("Part1").transform.position = Vector3.SmoothDamp(currentBossFace.transform.Find("Part1").transform.position, intendedPosP1, ref moveVelP1, 0.05f, 80f);

        //p2
        if (intendedPosP2 == Vector3.zero | intendedPosP2 == currentBossFace.transform.Find("Part2").transform.position) return;
        Vector3 moveVelP2 = intendedPosP2 - currentBossFace.transform.Find("Part2").transform.position;
        currentBossFace.transform.Find("Part2").transform.position = Vector3.SmoothDamp(currentBossFace.transform.Find("Part2").transform.position, intendedPosP2, ref moveVelP1, 0.05f, 80f);
    }



    //ALL THE STUFF THAT OTHER SCRIPTS CAN TELL THE BOSS FACE TO DO!!!!======================================

    private bool stop =false;
    private bool flip =false;

    public async void StartIdleBounce()
    {
        while(!stop)
        {
            if (flip) intendedPos = new Vector3(intendedPos.x, intendedPos.y + 3, intendedPos.z);
            else intendedPos = new Vector3(intendedPos.x, intendedPos.y - 3, intendedPos.z);
            flip = !flip;
            await Task.Delay(5000);

        }
        stop = false;

    }

    public async void StopIdleBounce()
    {
        stop = true;
    }



    public bool requestReset()
    {
        bossFaceParent.transform.position = basePos;
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

    public async void SparkleAndAppear(CancellationToken token)
    {
        Sparkle(token);
        await Task.Delay(150);
        lIris.transform.Find("Standard").gameObject.SetActive(true);
        rIris.transform.Find("Standard").gameObject.SetActive(true);
    }


    public async void Sparkle(CancellationToken token)
    {

        GameObject lSpark = lIris.transform.Find("Sparkle").gameObject;
        GameObject rSpark = rIris.transform.Find("Sparkle").gameObject;
        Debug.Log(lSpark.name);
        lSpark.transform.localScale = Vector3.up;
        rSpark.transform.localScale = Vector3.up;
        lSpark.SetActive(true);
        rSpark.SetActive(true);

        utilityScript.Tween(lSpark, lSpark.transform.localPosition, lSpark.transform.localEulerAngles, sparkleIrisBaseSize, 100, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token);
        await utilityScript.Tween(rSpark, rSpark.transform.localPosition, rSpark.transform.localEulerAngles, sparkleIrisBaseSize, 100, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token);
        Debug.Log("You're here");
        await Task.Delay(300);
        if (token.IsCancellationRequested) return;

        utilityScript.Tween(lSpark, lSpark.transform.localPosition, lSpark.transform.localEulerAngles, Vector3.up, 1000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token);
        await utilityScript.Tween(rSpark, rSpark.transform.localPosition, rSpark.transform.localEulerAngles, Vector3.up, 1000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, gameManage.source.Token);
        if (token.IsCancellationRequested) return;
        lSpark.SetActive(false);
        rSpark.SetActive(false);

    }//need to update tween to be able to scale size (Done!! Yahoo!)


    public async void SetToRedSparkle()
    {

    }



    public void SetAgroAnim()
    {
        //skull anim
        switch(currentBossFace.name)
        {
            case "SkullFace":
                intendedPosP1 = new Vector3(0, 0, -1);
                break;
        }
        //wendigoAnim
    }

    public void SetNormalAnim()
    {
        switch (currentBossFace.name)
        {
            case "SkullFace":
                intendedPosP1 = new Vector3(0, 0, 0);
                break;
        }
    }

    private List<CancellationTokenSource> sources = new List<CancellationTokenSource>();

    public async void ChangeBossFace(string changeTo)
    {
        if (currentBossFace != null && currentBossFace.name == changeTo) return;
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

        intendedPos = new Vector3(0, -40, 0);
        Debug.Log("you made it here");
        await Task.Delay(1000);

        if (token.IsCancellationRequested) return;
        Debug.Log("now here");
        for (int i = 0; i < bossFaceParent.transform.childCount; i++)
        {
            GameObject curChild = bossFaceParent.transform.GetChild(i).gameObject;
            if (curChild.name == "RightEye" | curChild.name == "LeftEye") continue;
            curChild.SetActive(false);
        }
        currentBossFace = bossFaceParent.transform.Find(changeTo).gameObject;
        currentBossFace.SetActive(true);

        //any eye moving for different facePoses

        //

        intendedPos = basePos;
        Debug.Log("and even here");
        await Task.Delay(1000);
        if (token.IsCancellationRequested) return;
        sources.Remove(curSc);
    }





    //=======================================================================================================

    // Update is called once per frame
    void Update()
    {
        shiftFace();
        if(following) adjustEyes();


    }
}
