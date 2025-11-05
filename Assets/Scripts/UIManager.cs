using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using Unity.Android.Gradle;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas MainCanvas;
    public Canvas LevelSelCanvas;
    public Canvas PostGameCanvas;
    public Canvas playModeCanvas;
    public Canvas onlyShowBoss;

    public GameManage gameManager;
    public CameraController cameraController;
    public BossController bossController;
    public RythmEngine rythmEngine;

    static Canvas curCanvas; //how this will work is we'll clone one of the canvases when we need it, so the man canvases are more prefabs.
    void Start()
    {
        //SwapTooAndCleanup(MainCanvas);
    }


    //for cancelaiton

    

    static CancellationTokenSource source = new CancellationTokenSource();

    //data
  
    private string[] bossFaceNames = {"SkullFace" , "Wendigo"};
    //
    
    public async void SwapTooAndCleanup(Canvas toSwap)
    {
        if (curCanvas != null) Destroy(curCanvas.gameObject);
        source.Cancel();
        source = new CancellationTokenSource();
        curCanvas = Instantiate(toSwap);
        curCanvas.gameObject.transform.parent = gameObject.transform;
        curCanvas.gameObject.SetActive(true);
        //UnityEngine.Debug.Log(curCanvas.gameObject.name);

        //getting children 

        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < curCanvas.transform.childCount; i++)
        {
            children.Add(curCanvas.transform.GetChild(i).gameObject);
        }
        switch (curCanvas.gameObject.name)
        {
            case "MainMenu(Clone)":
                cameraController.bindTo("Mouse");
                bossController.SetLookTarg("Mouse");
                gameManager.state = GameManage.GameState.MainMenu;
                bossController.SparkleAndAppear(gameManager.source.Token);
                MainCanvasControl(children, source.Token);
                break;
            case "LevelSelect(Clone)":
                cameraController.bindTo("Ball");
                rythmEngine.ClearScreen();
                bossController.requestReset();
                
                gameManager.state = GameManage.GameState.LevelSelectMenu;
                LevelSelectCanvasControl(children, source.Token);
                break;
            case "PostGame(Clone)":
                cameraController.bindTo("Ball");
                
                PostGameCanvasControl(children, source.Token);
                break;
        }
    }

    //EVERYTHING THAT HAS TO DO WITH THE MAIN CANVAS---------------------------------------
    //-------------------------------------------------------------------------------------

    private void MainCanvasControl(List<GameObject> children, CancellationToken token)
    {
        if (curCanvas == null) return;
        //UnityEngine.Debug.Log("uuu");

        Button playButton = null;
        Button settingsButton = null;
        Image blackScreen = null;
  


        for (int i = 0; i < children.Count; i++)
        {
            //UnityEngine.Debug.Log(children[i].name);
            switch (children[i].name)
            {
                case "PlayButton":
                    //UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    playButton = children[i].GetComponent<Button>();
                    break;
                case "SettingButton":
                    settingsButton = children[i].GetComponent<Button>();
                    break;
                case "BlackScreen":
                    blackScreen = children[i].GetComponent<Image>();
                    break;
            }
        }

        if (playButton == null)
        {
            UnityEngine.Debug.Log("playbuttonWasNull");
            return;
        }

        blackScreen.CrossFadeAlpha(0, 0.5f, true);

        async void transitionToLevelSelect()
        {
            UnityEngine.Debug.Log("e");
            playButton.onClick.RemoveAllListeners();
            playButton.gameObject.SetActive(false);
            settingsButton.gameObject.SetActive(false);
            onlyShowBoss.gameObject.SetActive(true);
            bossController.Sparkle(gameManager.source.Token);
            //bossController.SetLookTarg("Camera");
            await Task.Delay(2000);
            blackScreen.CrossFadeAlpha(1, 0.7f, true);
            //play laugh sound

            //

            //the animation 
            await Task.Delay(2000);
            onlyShowBoss.gameObject.SetActive(false);
            if (token.IsCancellationRequested) return;
            //

            //actual stuff
            SwapTooAndCleanup(LevelSelCanvas);
            //
        }
        
        playButton.onClick.AddListener(transitionToLevelSelect);
        blackScreen.CrossFadeAlpha(0, 0.7f, true);
        UnityEngine.Debug.Log(playButton.onClick.GetPersistentEventCount());
        
    }






    //-----------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------



    //LEVEL SELECT STUFF--------------------------------------------
    //-------------------------------------------------------------
    private int index = 1;
    private void LevelSelectCanvasControl(List<GameObject> children, CancellationToken token)
    {
        
        Image blackScreen = null;
        Button startRound = null;
        Button back = null;
        Button left = null;
        Button right = null;
        TextMeshProUGUI levelName = null;

        //note -- though the ui knows the song, and whatnot, the ONLY thing it gives the game manager is the index (and eventually dificulty)
        //the game manager is responisible for double checking that the player has the song unlocked


        List<string> levelNames = new List<string>();

        /*
        //temp list ----
        string[] temp = { "Dungeon", "Forrest", "High Seas" };

        //-------------

        


        for (int i = 0; i < 3; i++)
        {
            levelNames.Add(temp[i]);
        }
        */

        for (int i = 0; i < gameManager.songs.Count; i++)
        {
            levelNames.Add(gameManager.songs[i].songBackground);
        }
        


        //loady the data from the files to display

        //
        for (int i = 0; i < children.Count; i++)
        {
            //UnityEngine.Debug.Log(children[i].name);
            switch (children[i].name)
            {
                case "StartButton":
                    //UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    startRound = children[i].GetComponent<Button>();
                    break;
                case "Back":
                    back = children[i].GetComponent<Button>();
                    break;
                case "Left":
                    left = children[i].GetComponent<Button>();
                    break;
                case "Right":
                    right = children[i].GetComponent<Button>();
                    break;
                case "BlackScreen":
                    blackScreen = children[i].GetComponent<Image>();
                    break;
                case "LevelName":
                    levelName = children[i].GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        if (blackScreen == null) return;

        levelName.text = levelNames[index - 1];

        bool ignore = true;
        void loadLeft()
        {
            if (index <= 1)
            {
                //play an error sound or sum
                left.gameObject.SetActive(false);
                return;
            }
            right.gameObject.SetActive(true);

            index -= 1;
            levelName.text = levelNames[index - 1];
            if(!ignore) bossController.ChangeBossFace(bossFaceNames[index - 1]);
            //maybe another function in here, oh yea one that interacts with ground mover for the backrounds, eventually

            if (index <= 1)
            {
                left.gameObject.SetActive(false);
            }
        }

        void loadRight()
        {
            if (index >= levelNames.Count)
            {
                //play an error sound or sum
                right.gameObject.SetActive(false);
                return;
            }
            left.gameObject.SetActive(true);
            index += 1;

            levelName.text = levelNames[index - 1];
            if(!ignore) bossController.ChangeBossFace(bossFaceNames[index - 1]);
            //maybe another function in here, oh yea one that interacts with ground mover for the backrounds, eventually



            if (index >= levelNames.Count)
            {
                right.gameObject.SetActive(false);
            }


        }

        async void LoadBack()
        {
            blackScreen.CrossFadeAlpha(1, 0.7f, true);
            await Task.Delay(1500);
            if (token.IsCancellationRequested) return;
            SwapTooAndCleanup(MainCanvas);
        }

        async void LoadPlay()
        {
            startRound.onClick.RemoveAllListeners();
            for (int i = 0; i < children.Count; i++)
            {
                children[i].gameObject.GetComponent<Image>().CrossFadeAlpha(0, 0.7f, true);
                await Task.Delay(700);
                if (token.IsCancellationRequested) return;
                gameManager.requestRoundStart(index, 1);
                SwapTooAndCleanup(playModeCanvas);
            }
            
        }
        loadRight();
        loadLeft();
        loadLeft();
        loadLeft();
        loadLeft();
        index = 1;
        bossController.ChangeBossFace(bossFaceNames[0]);
        ignore = false;
        bossController.SetLookTarg("Player");
        left.onClick.AddListener(loadLeft);
        right.onClick.AddListener(loadRight);
        back.onClick.AddListener(LoadBack);
        startRound.onClick.AddListener(LoadPlay);


        blackScreen.CrossFadeAlpha(0, 2f, true);
    }
    private bool yPress = false;
    private bool nPress = false;
    private async Task PostGameCanvasControl(List<GameObject> children, CancellationToken token)
    {
        if (curCanvas == null) return;
        //UnityEngine.Debug.Log("uuu");

        TextMeshProUGUI internalText = null;
        TextMeshProUGUI winText = null;
        



        for (int i = 0; i < children.Count; i++)
        {
            //UnityEngine.Debug.Log(children[i].name);
            switch (children[i].name)
            {
                case "TryAgainText":
                    //UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    internalText = children[i].GetComponent<TextMeshProUGUI>();
                    break;
                case "WinText":
                    //UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    winText = children[i].GetComponent<TextMeshProUGUI>();
                    break;
            }
        }

        yPress = false;
        nPress = false;
        bool flipFlop = false;
        bool stillInMenu = true;

        if(gameManager.state != GameManage.GameState.PlayingWin)
        {
            gameManager.state = GameManage.GameState.PlayingDead;
            internalText.gameObject.SetActive(true);
            async void flickerText()
            {
                while (stillInMenu)
                {
                    internalText.text = "Give Up?\r\n(Press Y or N)";
                    if (flipFlop) internalText.CrossFadeAlpha(0.3f, 1f, true);
                    else internalText.CrossFadeAlpha(1f, 1f, true);
                    flipFlop = !flipFlop;
                    await Task.Delay(1000);
                    if (token.IsCancellationRequested) return;
                }

            }


            async void Retry()
            {
                stillInMenu = false;
                internalText.CrossFadeAlpha(0.3f, 0.7f, true);
                await Task.Delay(700);
                if (token.IsCancellationRequested) return;
                gameManager.requestRoundStart(index, 1);
                SwapTooAndCleanup(playModeCanvas);
            }

            async void checky()
            {
                while (stillInMenu)
                {
                    if (token.IsCancellationRequested) return;
                    //UnityEngine.Debug.Log("Buffeting...");
                    if (nPress) Retry();
                    else if (yPress) ExitToMenu();
                    await Task.Delay(1);
                }
            }


            async void ExitToMenu()
            {
                stillInMenu = false;
                SwapTooAndCleanup(LevelSelCanvas);
            }

            checky();
            flickerText();
        }
        else
        {
            //UnityEngine.Debug.Log("a");
            gameManager.state = GameManage.GameState.PlayingDead;
            winText.gameObject.SetActive(true);
            //gameManager.WinRound();
            await Task.Delay(3000);
            if (token.IsCancellationRequested) return;
            SwapTooAndCleanup(LevelSelCanvas);
            

        }
        
    }
    
    // Update is called once per frame
    
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            //UnityEngine.Debug.Log("aa");
            yPress = true;
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            //UnityEngine.Debug.Log("bb");
            nPress = true;
        }
        if(gameManager.state == GameManage.GameState.LevelSelectMenu)
        {
            //if()




        }
    }
}
