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
    public int targetBpm;
    static private char[] enemyStates = { '0', '0', '0', '0', '0', '0' };
    static private List<char[]> songData = new List<char[]>();
    static int curSongLength = 0;

    public TextMeshProUGUI winMessage; 
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
        curEnemyBody.name = pos.ToString();
        activeEnemies[pos] = curEnemyBody;
        curEnemyBody.gameObject.transform.SetParent(enemyFolder.transform, true);
        int iSuckAtCoding = 0;
        float increase = 1;
        while (curEnemyBody.transform.position.y < 1) //fix later
        {
            increase = 1 - increase * 0.9f;
            iSuckAtCoding++;
            //Debug.Log(curEnemyBody.transform.position.y);
            curEnemyBody.transform.position = Vector3.Lerp(curEnemyBody.gameObject.transform.position, new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z), increase);
            if (iSuckAtCoding >= 100)
            {
                Debug.Log("Fuckass code lmao");
                break;
            }
        }
        curEnemyBody.transform.position = new Vector3(enemySpawnLocations[pos].x, 1, enemySpawnLocations[pos].z);
    }
    //--------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------called every beat, adjusts the enemy state table so the game knows what to do next
    private async void updateStates(double targTime, int beat)
    {
        actOnState((60 / targetBpm) / 5);
        while (AudioSettings.dspTime < targTime) await Task.Delay(1);

        for (int i = 0; i < 6; i++) // change length to 8 once we figure out boss stuff
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
        Debug.Log(parent.rotation.y);
        GameObject curLaser = Instantiate(laser, new Vector3(targPosForLaser, parent.position.y, parent.position.z), Quaternion.Euler(new Vector3(parent.rotation.eulerAngles.x, parent.rotation.eulerAngles.y, parent.rotation.eulerAngles.z)));
        curLaser.transform.SetParent(laserFolder.transform);
        curLaser.name = parent.name;
    }
    //-------------------------------------------------------------------------------

    enum easingStyle
    {
        None,
        Cube,
   
    }
    enum easingDirection
    {
        In,
        Out,
        
    }
    private async void Tween(GameObject item, Vector3 endPos, Vector3 endAngle, int milliseconds, easingStyle style, easingDirection direction)// for animating parts wee woo 
    {
        if (item == null) return;
        //Debug.Log("atleast we're here");
        List<Vector3> positionChart = new List<Vector3>(); //builds a list of points to hit, and then strikes them based on easing direction
        List<Vector3> angleChart = new List<Vector3>(); //does the same thing but for angles

        float xStep = (endPos.x - item.transform.position.x) / (milliseconds * 10);
        float yStep = (endPos.y - item.transform.position.y) / (milliseconds * 10);
        float zStep = (endPos.z - item.transform.position.z) / (milliseconds * 10);

        float xAStep = (endAngle.x - item.transform.rotation.x) / (milliseconds * 10);
        float yAStep = (endAngle.y - item.transform.rotation.y) / (milliseconds * 10);
        float zAStep = (endAngle.z - item.transform.rotation.z) / (milliseconds * 10);

        positionChart.Add(item.transform.position);
        angleChart.Add(item.transform.rotation.eulerAngles);

        for (int i = 0; i < milliseconds * 10; i++)
        {
            positionChart.Add(new Vector3(positionChart[i].x + xStep, positionChart[i].y + yStep, positionChart[i].z + zStep));
            angleChart.Add(new Vector3(angleChart[i].x + xAStep, angleChart[i].y + yAStep, angleChart[i].z + zAStep));
        }
        int jumpPos = 10;
        int modifier = 0;
        int progPos = 0;
        switch (style)
        {
            case easingStyle.None:
                break;
            case easingStyle.Cube:
                modifier = 1000;
                break;

        }
        switch (direction)
        {
            case easingDirection.In:
                jumpPos = 1000;
                modifier = math.abs(modifier);
                break;
            case easingDirection.Out:
                jumpPos = 1;
                modifier = math.abs(modifier) * -1;
                break;

        }
        Debug.Log(milliseconds);
        for (int i = 0; i < milliseconds; i++)
        {
            Debug.Log("aaa");
            item.transform.position = positionChart[progPos];
            item.transform.rotation = quaternion.Euler(angleChart[progPos]);
            jumpPos += modifier;
            progPos += jumpPos;
            await Task.Delay(1);
        }
        item.transform.rotation = quaternion.Euler(endAngle);


    }
    private async void rotateEnemy(GameObject enemy, int rotateAmmount)
    {
        int extra = 0;
        if (enemy.transform.position.x > 0)
        {
            rotateAmmount *= -1;
            extra = 180;
        }
        Vector3 targVec3 = new Vector3(0,rotateAmmount, extra);
        Debug.Log($"{enemy} {targVec3}");
        enemy.transform.rotation = Quaternion.Euler(targVec3);
        //Tween(enemy, enemy.transform.position, targVec3, (int)(60.0f / (float)targetBpm * 1000), easingStyle.None, easingDirection.Out);//; stil a wip


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
        for (int i = 0; i < 6; i++)
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

    private void Awake()
    {


    }
    void Start()
    {


        string filePath = "rythmPatternFiles/FluffADuck";


        buildMapFromFile(filePath);
        //Debug.Log("AAAAAAAAAA");
        
        bpmCoroutine = BPMManager(targetBpm, -50);
        StartCoroutine(bpmCoroutine);
        //Debug.Log("sdsd");
        Vector3 myCoolAngle = new Vector3(0, 0, 0);
        for (int i = 0; i < 6; i++)
        {
            if (i >2) myCoolAngle = new Vector3(0, 0, 180);
            spawnEnemy(i, myCoolAngle);
        }


    }
    private void Update()
    {
        

    }

}
