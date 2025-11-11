using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
//using UnityEditor;
using System.Collections.Generic;

using UnityEngine.Audio;

using System;
using System.Collections;



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
    public BossController boardController;
    

    //-----------------
    public CancellationTokenSource source;
    //

    //events---------------

    //

    //Game state stuff
    public List<Song> songs = new List<Song>();

    public enum GameState
    {
        MainMenu,
        LevelSelectMenu,
        PlayingAlive,
        PlayingDead,
        PlayingWin,
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
            curSong.songAudioTrack = Resources.Load<AudioResource>("Music/" + tempStuff[4].Split('\t')[1].Trim());
            curSong.songName = tempStuff[3].Split('\t')[1];
            curSong.songLengthInBeats = tempStuff.Length - 9;
            curSong.songGroundSpeed = float.Parse(tempStuff[5].Split('\t')[1]);
            //==========================================================

            //debug=====================================================
            Debug.Log(curSong.songIndex);
            Debug.Log(curSong.songBPM);
            Debug.Log(curSong.songAudioTrack);
            Debug.Log(tempStuff[4].Split('\t')[1]);
            Debug.Log(curSong.songBackground);
            Debug.Log(curSong.songName);
            Debug.Log(curSong.songLengthInBeats);
            Debug.Log(curSong.songGroundSpeed);
            //==========================================================

            //getting the map============(and removing blank space)=====
            curSong.songMap = new List<char[]>();
            for (int j = 9; j < tempStuff.Length; j++)
            {
                tempStuff[j] = tempStuff[j].Replace("\t", string.Empty);
                curSong.songMap.Add(tempStuff[j].ToCharArray());
                //Debug.Log(tempStuff[j]);
            }
            //==========================================================
            songs.Add(curSong);
            

            
        
        }
    }

    public GameState state = GameState.MainMenu;
    
    //
    

    //external requests

    public void requestRoundStart(int songIndex, int dificulty)
    {
        //unlock checking

        ///
        /// 
        /// 
        /// 
        /// 
        


        if (state == GameState.LevelSelectMenu || state == GameState.PlayingDead)
        {

            StartRound(songIndex);
            //rythmEngine.StartRound(curSong, source.Token);
            //groundMover.Play()
        }

    }
    private void PlrRespawn()
    {
        //playerController.gameObject.SetActive(true);
        //playerController.Respawn();
    }

    public IEnumerator PlayerDied()
    {
        if (state != GameState.PlayingAlive) yield break;
        StartCoroutine(rythmEngine.stopMusic(source.Token));
        yield return new WaitForSeconds(1f);
        uiManager.SwapTooAndCleanup(uiManager.PostGameCanvas);

    }

    //

    private void mainRunner()
    {
        
        cameraController.bindTo("Mouse");
        boardController.SetLookTarg("Mouse");
        buildSongData();
        source = new CancellationTokenSource();
        uiManager.SwapTooAndCleanup(uiManager.MainCanvas);
        
        //rythmEngine.StartRound(0, source.Token);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void WinRound()
    {
        state = GameState.PlayingWin;
        uiManager.SwapTooAndCleanup(uiManager.PostGameCanvas);
        playerController.gameObject.SetActive(false);
        //any unlocking or whatnot
    }    

    private void StartRound(int songIndex)
    {
        playerController.Respawn();
        cameraController.bindTo("Ball");
        Song curSong = songs.Find(Song => Song.songIndex == songIndex);
        StartCoroutine(rythmEngine.StartRound(curSong, source.Token));
        boardController.requestReset();
        rythmEngine.ClearScreen();
        //playerController.speed = 0.2f;
        //maybe add some checking later?
        StartCoroutine(boardController.SparkleAndAppear(source.Token));
        groundMover.Play(curSong.songGroundSpeed, curSong.songBackground);
        state = GameState.PlayingAlive;
        //
    }

    /*
    private void closingGame(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;
        //saving


        //
        groundMover.StopGround();
        rythmEngine.stopMusic(source.Token);

        source.Cancel();

    }
    */

    private void Awake()
    {

        Application.targetFrameRate = 360;
    }
    void Start()
    {
        //EditorApplication.playModeStateChanged += closingGame;//only for dev, CHANGE FOR BUILD!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        
        mainRunner();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
