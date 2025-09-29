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

public class RythmEngine : MonoBehaviour
{

    private bool runMetronome = true;
    public AudioSource metronome1;
    public AudioSource metronome2;
    public AudioSource metronome3;
    public AudioSource metronome4;

    public GameObject enemyFolder;

    

    public GameObject enemyBody;
    public AudioSource track;
    public int targetBpm;
    static private string[] enemyStates = {"0", "0", "0", "0", "0", "0"};
    static private List<string[]> songData = new List<string[]>();

    double bpmTargTime = 0;

    private bool ableToStartSong = false;

    static GameObject[] activeEnemies = new GameObject[6]; // will add a seperate table for the forward facing ones, cause theyre boss specific


    IEnumerator bpmCoroutine;


    static Vector3[] enemySpawnLocations = new[] {
        new Vector3(-11, -10, 5),
        new Vector3(-11, -10, 0),
        new Vector3(-11, -10, -5),
        new Vector3(11, -10, 5),
        new Vector3(11, -10, 0),
        new Vector3(11, -10, -5)
    };


    //-------------------------------------------------ONLY USED at the start of the game, change to make new ones each fight later on----
    void spawnEnemy(int pos, Vector3 angle)
    {
        
        GameObject curEnemyBody = Instantiate(enemyBody, enemySpawnLocations[pos], Quaternion.Euler(angle));
        activeEnemies[pos] = curEnemyBody;
        curEnemyBody.gameObject.transform.SetParent(enemyFolder.transform, true);
        int iSuckAtCoding = 0;
        float increase = 1;
        while (curEnemyBody.transform.position.y < 1) //fix later
        {
            increase = 1 - increase * 0.9f;
            iSuckAtCoding++;
            //Debug.Log(curEnemyBody.transform.position.y);
            curEnemyBody.transform.position = Vector3.Lerp(curEnemyBody.gameObject.transform.position, new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z),increase);
            if (iSuckAtCoding >= 100)
            {
                Debug.Log("Fuckass code lmao");
                break;
            }
        }
        curEnemyBody.transform.position = new Vector3(enemySpawnLocations[pos].x,1, enemySpawnLocations[pos].z );
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------called every beat, adjusts the enemy state table so the game knows what to do next
    static async void updateStates(double targTime, int beat)
    {
        Debug.Log("yobbo");
        while (AudioSettings.dspTime < targTime) await Task.Yield();
        //await Task.Delay((int)(waitTime * 1000));
        for (int i = 0; i < 6; i++) // change length to 8 once we figure out boss stuff
        {
            enemyStates[i] = songData[beat][i];
            Debug.Log($"Setting {i} to {enemyStates[i]}");
        }
        actOnState();
    }
    //---------------------------------------------------------------

    static async void actOnState()
    {
        
        for (int i = 0; i < 6; i++)
        {
            GameObject curEnemy = activeEnemies[i];
            Transform chargePiece = curEnemy.transform.Find("FrontThing");//fix
            switch (enemyStates[i])
            {
                case "0":
                    chargePiece.gameObject.SetActive(false);
                    break;

                case "1":
                    chargePiece.gameObject.SetActive(true);
                    break;

                case "2":

                    break;
            }
                
        }
    }

    //ye olle bread and butter, meant to be a READ ONLY type jawn, pings beat changes------------------------- BE VERY CAREFULL CHANGING CODE, WILL BREAK WHOLE GAME IF NO WORK
    static public int beat = 1;
    IEnumerator BPMManager(int bpm, int offset)
    {
        beat += 1;
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
                bpmTargTime += bpsAddon;
                //Debug.Log($"ts WORKING {bpmTargTime - AudioSettings.dspTime}");
                //RUNNING ALL THE EFFECTS FOR THIS BEAT
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




    private void Awake()
    {


    }
    void Start()
    {

        string filePath = @"C:\UnityProjects\Roll-A-Beat\Assets\rythmPatternFiles\Test.txt";
        string[] tempStuff = File.ReadAllLines(filePath);


        //Debug.Log("AAAAAAAAAA");
        
        bpmCoroutine = BPMManager(targetBpm, 75);
        StartCoroutine(bpmCoroutine);
        //Debug.Log("sdsd");
        Vector3 myCoolAngle = new Vector3(0, 0, 0);
       
        for (int i = 0; i < 6; i++)
        {
            spawnEnemy(i, myCoolAngle);
        }

        for (int i = 1; i < tempStuff.Length; i++)
        {
            songData.Add(tempStuff[i].Split("\t"));
            
        }

    }
    private void Update()
    {
        

    }

}
