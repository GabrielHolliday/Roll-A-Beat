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
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Audio;

public class RythmEngine : MonoBehaviour
{
    //NOTE ON BUILDING LEVELS!!!!!!!!! 
    /*
     * Q = 65 degrees Down
     * A = 45 Degrees Down
     * Z = 25 Degrees Down
     * 
     * 
     * E = 65 Degrees Up
     * D = 45 Degrees Up
     * C = 25 Degrees Up 
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
    private int targetBpm;
    static private char[] enemyStates = { '0', '0', '0', '0' };
    static private List<char[]> songData = new List<char[]>();
    static int curSongLength = 0;

    public TextMeshProUGUI winMessage;
    double bpmTargTime = 0;
    public UtilityScript utilityScript;
    private bool ableToStartSong = false;

    static GameObject[] activeEnemies = new GameObject[4]; // will add a seperate table for the forward facing ones, cause theyre boss specific


    //song file stuff
    private string[] songFilePaths = {"Music/Fluffing a Duck - Kevin Macleod (youtube).mp3"};
    private string[] songsMapFilePaths = {"rythmPatternFiles/FluffADuck" };
    private int[] songBPMs = { 142 };



    //----------------------

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

        utilityScript.Tween(curEnemyBody, targVec3, angle, 2000, UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out);




        //curEnemyBody.transform.position = new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z);
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------called every beat, adjusts the enemy state table so the game knows what to do next
    private async void updateStates(double targTime, int beat)
    {
        actOnState((60 / targetBpm) / 5);
        while (AudioSettings.dspTime < targTime) await Task.Delay(1);

        for (int i = 0; i < 4; i++) // change length to 8 once we figure out boss stuff
        {
            //Debug.Log(beat);
            //Debug.Log(songData[beat][i]);
            enemyStates[i] = songData[beat][i];
            if (songData[beat][i] == '1')
            {
                //Debug.Log($"Setting {i} to {enemyStates[i]}");
            }

        }

    }
    //---------------------------------------------------------------

    //FIRIN THHHAAA LAAAAAAZZZZERRRRR~!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!-----------
    private void fireLaser(Transform parent)
    {
        if (parent == null) return;
        float targPosForLaser = parent.position.x;
        //if (parent.position.x > 0) targPosForLaser = parent.position.x - laser.transform.lossyScale.y;
        //else targPosForLaser = parent.position.x + laser.transform.lossyScale.y;
        //Debug.Log(parent.rotation.y);
        GameObject curLaser = Instantiate(laser, new Vector3(targPosForLaser, parent.position.y, parent.position.z), Quaternion.Euler(new Vector3(parent.rotation.eulerAngles.x, parent.rotation.eulerAngles.y, parent.rotation.eulerAngles.z)));
        curLaser.transform.SetParent(laserFolder.transform);
        curLaser.name = parent.name;
    }
    //-------------------------------------------------------------------------------


    private async void rotateEnemy(GameObject enemy, int rotateAmmount)
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
        utilityScript.Tween(enemy, enemy.transform.position, targVec3, (int)(30.0f / (float)targetBpm * 1000), UtilityScript.easingStyle.Cube, UtilityScript.easingDirection.Out);//; stil a wip


    }

    public async void stopMusic()
    {
        runMetronome = false;
        for (int i = 0; i < 1100; i++)
        {
            track.pitch -= 0.001f;
            track.volume -= 0.001f;
            await Task.Delay(2);
        }
        track.Stop();
        track.pitch = 1;
        track.volume = 1;
    }
    private async void actOnState(double targTime)
    {
        while (AudioSettings.dspTime < targTime) await Task.Delay(1);
        for (int i = 0; i < 4; i++)
        {
            GameObject curEnemy = activeEnemies[i];
            Transform chargePiece = curEnemy.transform.Find("FrontThing");//fix (should work maybe?)
            int addToAngle = 0;
            switch (enemyStates[i])
            {
                case '0':
                    break;
                case '1':
                    chargePiece.gameObject.SetActive(true);
                    break;
                case '2':
                    fireLaser(curEnemy.transform);
                    break;
                case '3':
                    chargePiece.gameObject.SetActive(false);
                    if (laserFolder.transform.Find(curEnemy.name))
                    {
                        Destroy(laserFolder.transform.Find(curEnemy.name).gameObject);
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
    static public int beat = 0;
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
                    Debug.Log("we out");
                    runMetronome = false;
                    winMessage.text = "You win!";
                    break;

                }
                bpmTargTime += bpsAddon;
                beat += 1;
                metronomes[cycleInt].PlayScheduled(bpmTargTime);
                updateStates(bpmTargTime, beat);

                //-----------------------------------------
                if (cycleInt == 3)
                {
                    cycleInt = 0;
                    ableToStartSong = true;
                    if (track.isPlaying == false) track.PlayScheduled(bpmTargTime + ((float)(offset)) / 1000);
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
    private void buildMapFromFile(string theFile)
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

    public async void StartRound(int songIndex)
    {
        if (songsMapFilePaths[songIndex] == null) return;
        targetBpm = songBPMs[songIndex];
        track.resource = Resources.Load<AudioResource>(songFilePaths[songIndex]);
        buildMapFromFile(songsMapFilePaths[songIndex]);
        
        //Debug.Log("sdsd");
        Vector3 myCoolAngle = new Vector3(0, 0, 0);
        for (int i = 0; i < 4; i++)
        {
            await Task.Delay(500);
            if (i > 1) myCoolAngle = new Vector3(0, 0, 180);
            spawnEnemy(i, myCoolAngle);
        }
        await Task.Delay(2000);
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
