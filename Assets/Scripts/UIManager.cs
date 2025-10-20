using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Canvas MainCanvas;
    public Canvas LevelSelCanvas;
    public Canvas PostGameCanvas;

    static Canvas curCanvas; //how this will work is we'll clone one of the canvases when we need it, so the man canvases are more prefabs.
    void Start()
    {
        SwapTooAndCleanup(MainCanvas);
    }

    static async void SwapTooAndCleanup(Canvas toSwap)
    {
        Destroy(curCanvas);
        curCanvas = Instantiate(toSwap);
        curCanvas.gameObject.SetActive(true);
        //UnityEngine.Debug.Log(curCanvas.gameObject.name);
        switch(curCanvas.gameObject.name)
        {
            case "MainMenu(Clone)":
                MainCanvasControl();
                break;
        }
    }

    static void MainCanvasControl()
    {
        if (curCanvas == null) return;
        UnityEngine.Debug.Log("uuu");

        Button playButton = null;
        Button settingsButton = null;
        GameObject blackScreen = null;

        Object[] stuff = curCanvas.GetComponents<GameObject>();
        for (int i = 0; i < stuff.Length; i++)
        {
            UnityEngine.Debug.Log(stuff[i].name);
            switch(stuff[i].name)
            {
                case "PlayButton":
                    UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    playButton = (Button)stuff[i];
                    break;
                case "SettingsButton":
                    settingsButton = (Button)stuff[i];
                    break;
                case "BlackScreen":
                    blackScreen = (GameObject)stuff[i];
                    break;
            }
        }

        if (playButton == null) return;

        playButton.onClick.AddListener(printy);


     


    }

    static void printy()
    {
        UnityEngine.Debug.Log("AAAAAAAA");
    }

    static void LevelSelectCanvasControl()
    {

    }

    static void PostGameCanvasControl()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
