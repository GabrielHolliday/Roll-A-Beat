using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;

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

        //getting children 

        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < toSwap.transform.childCount; i++)
        {
            children.Add(curCanvas.transform.GetChild(i).gameObject);
        }
        switch(curCanvas.gameObject.name)
        {
            case "MainMenu(Clone)":
                MainCanvasControl(children);
                break;
        }
    }

    static void MainCanvasControl(List<GameObject> children)
    {
        if (curCanvas == null) return;
        //UnityEngine.Debug.Log("uuu");

        Button playButton = null;
        Button settingsButton = null;
        GameObject blackScreen = null;

        
        for (int i = 0; i < children.Count; i++)
        {
            //UnityEngine.Debug.Log(children[i].name);
            switch(children[i].name)
            {
                case "PlayButton":
                    //UnityEngine.Debug.Log("WELL we found the playbutton"); 
                    playButton = children[i].GetComponent<Button>();
                    break;
                case "SettingsButton":
                    settingsButton = children[i].GetComponent<Button>();
                    break;
                case "BlackScreen":
                    blackScreen = (GameObject)children[i];
                    break;
            }
        }

        if (playButton == null)
        {
            UnityEngine.Debug.Log("playbuttonWasNull");
            return;
        }
        UnityEngine.Debug.Log(playButton.name);

        
        playButton.onClick.AddListener(printy);
        UnityEngine.Debug.Log(playButton.onClick.GetPersistentEventCount());

     


    }

    static void printy()
    {
        UnityEngine.Debug.Log("AAAAAAAA");
    }

    static void LevelSelectCanvasControl(List<GameObject> children)
    {

    }

    static void PostGameCanvasControl(List<GameObject> children)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
