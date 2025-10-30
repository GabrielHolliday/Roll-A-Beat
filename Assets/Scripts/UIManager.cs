using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using UnityEditor;


public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas MainCanvas;
    public Canvas LevelSelCanvas;
    public Canvas PostGameCanvas;
    public Canvas playModeCanvas;

    public GameManage gameManager;
    public CameraController cameraController;

    static Canvas curCanvas; //how this will work is we'll clone one of the canvases when we need it, so the man canvases are more prefabs.
    void Start()
    {
        //SwapTooAndCleanup(MainCanvas);
    }


    //for cancelaiton

    

    static CancellationTokenSource source = new CancellationTokenSource();
    
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
        for (int i = 0; i < toSwap.transform.childCount; i++)
        {
            children.Add(curCanvas.transform.GetChild(i).gameObject);
        }
        switch (curCanvas.gameObject.name)
        {
            case "MainMenu(Clone)":
                cameraController.bindTo("Mouse");
                gameManager.state = GameManage.GameState.MainMenu;
                MainCanvasControl(children, source.Token);
                break;
            case "LevelSelect(Clone)":
                cameraController.bindTo("Ball");
                gameManager.state = GameManage.GameState.LevelSelectMenu;
                LevelSelectCanvasControl(children, source.Token);
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
                case "SettingsButton":
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
        UnityEngine.Debug.Log(playButton.name);
        blackScreen.CrossFadeAlpha(0, 0.5f, true);

        async void transitionToLevelSelect()
        {
            playButton.onClick.RemoveAllListeners();
            blackScreen.CrossFadeAlpha(1, 0, true);
            //play laugh sound

            //

            //the animation 
            await Task.Delay(2000);
            if (token.IsCancellationRequested) return;
            //

            //actual stuff
            SwapTooAndCleanup(LevelSelCanvas);
            //
        }
        playButton.onClick.AddListener(transitionToLevelSelect);
        UnityEngine.Debug.Log(playButton.onClick.GetPersistentEventCount());
        
    }






    //-----------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------



    //LEVEL SELECT STUFF--------------------------------------------
    //-------------------------------------------------------------
    private void LevelSelectCanvasControl(List<GameObject> children, CancellationToken token)
    {
        int index = 1;
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

        foreach (Song song in gameManager.songs)
        {
            levelNames.Add(song.songBackground);
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

        left.onClick.AddListener(loadLeft);
        right.onClick.AddListener(loadRight);
        back.onClick.AddListener(LoadBack);
        startRound.onClick.AddListener(LoadPlay);


        blackScreen.CrossFadeAlpha(0, 2f, true);
    }

    private void PostGameCanvasControl(List<GameObject> children)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.state == GameManage.GameState.LevelSelectMenu)
        {
            //if()




        }
    }
}
