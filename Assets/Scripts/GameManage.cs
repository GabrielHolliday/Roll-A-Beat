using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.WSA;
using UnityEngine.Audio;
using System.IO;
using System;



public class Song
{
    public int songIndex;
    public string songName;
    public AudioResource songAudioTrack;
    public float songGroundSpeed;
    public int songLengthInBeats;
    public int songBPM;
    public List<char[]> songMap;
    public string songBackground;
    public string songFilePath; //just in case
}

public class GameManage : MonoBehaviour
{
    //Other scripts----
    public RythmEngine rythmEngine;
    public GroundMover groundMover;
    public PlayerController playerController;
    public UtilityScript utilityScript;
    public CameraController cameraController;
    public UIManager uiManager;
    private List<Song> songs = new List<Song>();

    //-----------------
    static CancellationTokenSource source;
    //

    //events---------------



    //
    //song indexes
    List<float> groundSpeeds;

    //

    //Game state stuff

    public enum GameState
    {
        MainMenu,
        LevelSelectMenu,
        PlayingAlive,
        PlayingDead
    }

    private void buildSongData()
    {
        UnityEngine.Object[] songFiles = Resources.LoadAll("rythmPatternFiles");
        Debug.Log(songFiles[0].name);
        for (int i = 0; i < songFiles.Length; i++)
        {
            Song curSong = new Song();

            TextAsset curFile = (TextAsset)songFiles[i];

            //VERY IMPORTANT STUFF


            //file contents============================================
            string[] tempStuff = curFile.text.Split("\n");
            //==========================================================

            //miscSet===================================================
            curSong.songIndex = Int32.Parse(tempStuff[1].Split('\t')[1]);
            curSong.songBPM = Int32.Parse(tempStuff[0].Split('\t')[1]);
            curSong.songBackground = tempStuff[2].Split('\t')[1];
            curSong.songAudioTrack = Resources.Load<AudioResource>(tempStuff[4].Split('\t')[1]);
            curSong.songName = tempStuff[3].Split('\t')[1];
            curSong.songLengthInBeats = tempStuff.Length - 9;
            curSong.songGroundSpeed = float.Parse(tempStuff[5].Split('\t')[1]);
            //==========================================================

            //getting the map============(and removing blank space)=====
            for (int j = 9; j < tempStuff.Length; j++)
            {
                tempStuff[j] = tempStuff[j].Replace("\t", string.Empty);
                curSong.songMap.Add(tempStuff[j].ToCharArray());
            }
            //==========================================================

            

        
        }
    }

    public GameState state = GameState.MainMenu;
    //
    private async void wait(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    public void requestRoundStart(int songIndex, int dificulty)
    {
        //unlock checking

        ///
        /// 
        /// 
        /// 


        if (state == GameState.LevelSelectMenu)
        {
            rythmEngine.StartRound(0, source.Token);
            //groundMover.Play()
        }

    }

    private async void mainRunner()
    {
        cameraController.bindTo("Ball");
        buildSongData();
        source = new CancellationTokenSource();
        uiManager.SwapTooAndCleanup(uiManager.MainCanvas);
        await Task.Delay(7000);
        //rythmEngine.StartRound(0, source.Token);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private async void StartRound(int mapID)
    {
        rythmEngine.StartRound(mapID, source.Token);
        playerController.speed = 0.2f;
        //maybe add some checking later?
        state = GameState.PlayingAlive;
        //
    }


    private void closingGame(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;
        //saving


        //
        groundMover.StopGround();
        rythmEngine.stopMusic();

        source.Cancel();

    }
    void Start()
    {
        EditorApplication.playModeStateChanged += closingGame;//only for dev, CHANGE FOR BUILD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        mainRunner();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
