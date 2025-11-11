using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Timeline;
//using UnityEngine.UIElements;
using TMPro;
//using UnityEngine.Audio;

public class RythmEngine : MonoBehaviour
{
    //NOTE ON BUILDING LEVELS!!!!!!!!! 
    /*
     * Q = 65 degrees UP
     * A = 45 Degrees UP
     * Z = 25 Degrees UP
     * 
     * 
     * E = 65 Degrees DOWN
     * D = 45 Degrees DOWN
     * C = 25 Degrees DOWN 

     * M = NORMAL
     */

    private bool runMetronome = true;
    public AudioSource metronome1;
    public AudioSource metronome2;
    public AudioSource metronome3;
    public AudioSource metronome4;

    public GameObject enemyFolder;
    public GameObject laserFolder;


    public GameObject enemyBody;
    public GameObject laser;

    public AudioSource track;
    public int targetBpm;
    static private char[] enemyStates = { '0', '0', '0', '0' };
    static private List<char[]> songData = new List<char[]>();
    static int curSongLength = 0;

    public TextMeshProUGUI winMessage;
    double bpmTargTime = 0;
    public UtilityScript utilityScript;
    public CameraController cameraController;
    public BossController bossController;
    public GameManage gameManage;
    private bool ableToStartSong = false;

    //public Song song;

    static GameObject[] activeEnemies = new GameObject[4]; // will add a seperate table for the forward facing ones, cause theyre boss specific


    //old song file stuff, keeping for good measure (for now)
    private string[] songFilePaths = {"Music/Fluffing a Duck - Kevin Macleod (youtube).mp3"};
    private string[] songsMapFilePaths = {"rythmPatternFiles/FluffADuck" };
    private int[] songBPMs = { 142 };



    //----------------------


    //cancelation stuff

    static CancellationTokenSource source;


    //-----------------

    IEnumerator bpmCoroutine;


    static Vector3[] enemySpawnLocations = new[] {
        new Vector3(-11, -10, 2.5f),
        new Vector3(-11, -10, -7.5f),

        new Vector3(11, -10, 7.5f),
        new Vector3(11, -10, -2.5f),
    };


    //-------------------------------------------------ONLY USED at the start of the game, change to make new ones each fight later on----
    void spawnEnemy(int pos, Vector3 angle)
    {

        GameObject curEnemyBody = Instantiate(enemyBody, enemySpawnLocations[pos], Quaternion.Euler(angle));
        curEnemyBody.name = pos.ToString();
        activeEnemies[pos] = curEnemyBody;
        curEnemyBody.gameObject.transform.SetParent(enemyFolder.transform, true);

        Vector3 targVec3 = new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z);
        //Debug.Log(curEnemyBody.transform.position.y);

        StartCoroutine(utilityScript.Tween(curEnemyBody, targVec3, angle, 700, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, source.Token));




        //curEnemyBody.transform.position = new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z);
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------called every beat, adjusts the enemy state table so the game knows what to do next
    private IEnumerator updateStates(double targTime, int beat)
    {
        
        while (AudioSettings.dspTime < targTime) yield return null;
        //Debug.Log("state trigger" + beat);
        
        for (int i = 0; i < 4; i++) // 
        {
            //Debug.Log(beat);
            //Debug.Log($"{beat}, {i}");
            //Debug.Log(songData[beat][i]);
            enemyStates[i] = songData[beat][i];
            if (songData[beat][i] == '1')
            {
                //Debug.Log($"Setting {i} to {enemyStates[i]}");
            }

        }
        actOnState((60 / targetBpm) / 5);

    }
    //---------------------------------------------------------------
    private IEnumerator Womp(double targTime, CancellationToken token, int beat)//tells other scripts when a beat happens, and any info on what they need to do that beat (ie boss state, works like a foriegn affairs officer)
    {
        
        while (AudioSettings.dspTime < targTime) yield return null;
        
        if (token.IsCancellationRequested | beat < 0) yield break;
        Debug.Log("womp trigger re" + beat);
        bossController.WompRecieve((int)char.GetNumericValue(songData[beat][4]), beat);


    }

    //FIRIN THHHAAA LAAAAAAZZZZERRRRR~!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!-----------
    private void fireLaser(Transform parent)
    {
        if (parent == null) return;
        float targPosForLaser = parent.position.x;
        //if (parent.position.x > 0) targPosForLaser = parent.position.x - laser.transform.lossyScale.y;
        //else targPosForLaser = parent.position.x + laser.transform.lossyScale.y;
        //Debug.Log(parent.rotation.y);
        //camera angle stuff
        
        //if (targPosForLaser < 0 ) cameraController.angleWeight += 2;
        //else cameraController.angleWeight -= 2;
        
        //
        //cameraController.addRumble(10);
        GameObject curLaser = Instantiate(laser, new Vector3(targPosForLaser, parent.position.y, parent.position.z), Quaternion.Euler(new Vector3(parent.rotation.eulerAngles.x, parent.rotation.eulerAngles.y, parent.rotation.eulerAngles.z)));
        curLaser.transform.SetParent(laserFolder.transform);
        curLaser.name = parent.name;
    }
    //-------------------------------------------------------------------------------


    private void rotateEnemy(GameObject enemy, int rotateAmmount)
    {
        int extra = 0;
        if (enemy.transform.position.x > 0)
        {
            rotateAmmount *= -1;
            extra = 180;
        }
        Vector3 targVec3 = new Vector3(0, rotateAmmount, extra);
        Debug.Log($"{enemy} {targVec3}");
        //enemy.transform.rotation = Quaternion.Euler(targVec3);
        StartCoroutine(utilityScript.Tween(enemy, enemy.transform.position, targVec3, (int)(28f / (float)targetBpm * 1000), UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out, source.Token));//; stil a wip


    }

    private bool forceUnquiet = false;
    public IEnumerator stopMusic(CancellationToken token)
    {
        forceUnquiet = false;
        source.Cancel();
        runMetronome = false;
        while(track.pitch > 0.01f)
        {
            
            if (forceUnquiet == true)
            {
                track.Stop();
                break;
            }
            track.pitch -= 0.01f * Time.deltaTime;
            track.volume -= 0.01f * Time.deltaTime;
            yield return null;
        }
        track.Stop();
        track.pitch = 1;
        track.volume = 1;
    }
    private void actOnState(double targTime)
    {
        //while (AudioSettings.dspTime < targTime) await Task.Delay(1);
        for (int i = 0; i < 4; i++)
        {
            GameObject curEnemy = activeEnemies[i];
            Transform chargePiece = curEnemy.transform.Find("FrontThing");//fix (should work maybe?)

            switch (enemyStates[i])
            {
                case '0':
                    break;
                case '1':
                    Debug.Log("charging");
                    chargePiece.gameObject.SetActive(true);
                    break;
                case '2':
                    fireLaser(curEnemy.transform);
                    break;
                case '3':
                    chargePiece.gameObject.SetActive(false);
                    if (laserFolder.transform.Find(curEnemy.name))
                    {
                        for (int h = 0; h < laserFolder.transform.childCount; h++)
                        {
                            if (laserFolder.transform.GetChild(h).name == curEnemy.name)
                            {
                                Destroy(laserFolder.transform.GetChild(h).gameObject);
                                Debug.Log($"Destroying... {curEnemy.name}");
                                Debug.Log(laserFolder.transform.childCount);
                                if (laserFolder.transform.childCount <= 1)
                                {
                                    cameraController.rumblin = false;
                                    cameraController.angleWeight = 0;
                                }
                            }
                        }
                    }
                    break;
                case 'Q':
                    rotateEnemy(curEnemy, -65);
                    break;
                case 'A':
                    rotateEnemy(curEnemy, -45);
                    break;
                case 'Z':
                    rotateEnemy(curEnemy, -25);
                    break;
                case 'E':
                    rotateEnemy(curEnemy, 65);
                    break;
                case 'D':
                    rotateEnemy(curEnemy, 45);
                    break;
                case 'C':
                    rotateEnemy(curEnemy, 25);
                    break;
                case 'M':
                    rotateEnemy(curEnemy, 0);
                    break;

            }

        }
        
    }

    //ye olle bread and butter, meant to be a READ ONLY type jawn, pings beat changes------------------------- BE VERY CAREFULL CHANGING CODE, WILL BREAK WHOLE GAME IF NO WORK
    static public int beat = -4;
    private bool up = false;
    IEnumerator BPMManager(int bpm, int offset)
    {


        double bpsAddon;
        bpsAddon = (double)60 / (double)(bpm);
        AudioSource[] metronomes = { metronome1, metronome2, metronome3, metronome4 };
        short cycleInt = 0;
        bpmTargTime = AudioSettings.dspTime + bpsAddon;

        while (runMetronome)
        {

            //waiting using bps time
            if (AudioSettings.dspTime > bpmTargTime)
            {
                if (beat >= curSongLength)
                {
                    //song end
                    Debug.Log("we out");
                    runMetronome = false;
                    StartCoroutine(stopMusic(gameManage.source.Token));
                    gameManage.WinRound();
                    break;

                }
                bpmTargTime += bpsAddon;
                beat += 1;
                metronomes[cycleInt].PlayScheduled(bpmTargTime);
                StartCoroutine(Womp(bpmTargTime, gameManage.source.Token, beat - 1));
                if(beat >= 0) StartCoroutine(updateStates(bpmTargTime, beat -1));
                //-----------------------------------------
                if (cycleInt == 3)
                {
                    cycleInt = 0;
                    ableToStartSong = true;
                    if (track.isPlaying == false) track.PlayScheduled(bpmTargTime + ((float)(offset)) / 1000);
                    if (!up)
                    {
                        //bossController.bobHeadUp();
                    }
                    else
                    {
                        //bossController.bobHeadDown();
                    }
                    up = !up;
                }
                else cycleInt += 1;

                while (bpmTargTime <= AudioSettings.dspTime)
                {
                    bpmTargTime += bpsAddon;
                }
            }
            yield return null;
        }
        //

    }

    //---------------------------------------------------------------------------------------------------------------------


    //filea reading 
    private void buildMapFromFile(string theFile)//old, unused
    {
        string[] tempStuff = Resources.Load<TextAsset>(theFile).text.Split("\n");

        //string[] tempStuff = File.ReadAllLines(theFile);
        for (int i = 1; i < tempStuff.Length; i++)
        {
            tempStuff[i] = tempStuff[i].Replace("\t", string.Empty);
            songData.Add(tempStuff[i].ToCharArray());
            //Debug.Log($"{songData[songData.Count - 1][0]}");

        }
        curSongLength = songData.Count;

    }

    public void ClearScreen()
    {
        for (int i = 0; i < enemyFolder.transform.childCount; i++)
        {
            Destroy(enemyFolder.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < laserFolder.transform.childCount; i++)
        {
            Destroy(laserFolder.transform.GetChild(i).gameObject);
        }
    }

    public IEnumerator StartRound(Song song, CancellationToken token)
    {
        if (song == null) yield break;
        forceUnquiet = true;
        source = new CancellationTokenSource();
        targetBpm = song.songBPM;
        track.resource = song.songAudioTrack;
        songData = song.songMap;
        curSongLength = song.songLengthInBeats;
        track.pitch =1;
        track.volume =1;

        ClearScreen();
        Debug.Log("sdsd");
        Vector3 myCoolAngle = new Vector3(0, 0, 0);
        //var resets
        runMetronome = true;
        beat = -4;
        //--
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (i > 1) myCoolAngle = new Vector3(0, 0, 180);
            spawnEnemy(i, myCoolAngle);
        }
        yield return new WaitForSeconds(2f);
        bpmCoroutine = BPMManager(targetBpm, -50);
        StartCoroutine(bpmCoroutine);
    }

    private void Awake()
    {


    }
    void Start()
    {


        


    }
    private void Update()
    {
        

    }

}
