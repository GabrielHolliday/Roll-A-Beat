using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class RythmEngine : MonoBehaviour
{

    private bool runMetronome = true;
    public AudioSource metronome1;
    public AudioSource metronome2;
    public AudioSource metronome3;
    public AudioSource metronome4;

    public GameObject enemyBody;
    public AudioSource track;
    public int targetBpm;

    double bpmTargTime = 0;

    private bool ableToStartSong = false;


    IEnumerator bpmCoroutine;


    static Vector3[] enemySpawnLocations = new[] {
        new Vector3(-11, -10, 5),
        new Vector3(-11, -10, 0),
        new Vector3(-11, -10, -5),
        new Vector3(11, -10, 5),
        new Vector3(11, -10, 0),
        new Vector3(11, -10, -5)
    };

    IEnumerator spawnEnemy(int pos, Quaternion angle, double riseTime, double chargeUpTime, double shootTime, double shootDuration)
    {

        GameObject curEnemyBody = Instantiate(enemyBody, enemySpawnLocations[pos], angle);
        while (curEnemyBody.transform.position.y > 1)
        {
            
        }


    }

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
                bpmTargTime += bpsAddon;
                Debug.Log($"ts WORKING {bpmTargTime - AudioSettings.dspTime}");
                //RUNNING ALL THE EFFECTS FOR THIS BEAT
                metronomes[cycleInt].PlayScheduled(bpmTargTime);






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
    





    private void Awake()
    {


    }
    void Start()
    {
        Debug.Log("AAAAAAAAAA");
        
        bpmCoroutine = BPMManager(targetBpm, 75);
        StartCoroutine(bpmCoroutine);
        Debug.Log("sdsd");
    }
    private void Update()
    {

    }

}
