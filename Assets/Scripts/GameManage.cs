using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UnityEditor;

public class GameManage : MonoBehaviour
{
    //Other scripts----
    public RythmEngine rythmEngine;
    public GroundMover groundMover;
    public PlayerController playerController;
    public UtilityScript utilityScript;
    public CameraController cameraController;
    public UIManager uiManager;


    //-----------------
    static CancellationTokenSource source;
    //

    //events---------------



    //

    //Game state stuff

    enum GameState
    {
        MainMenu,
        LevelSelectMenu,
        PlayingAlive,
        PlayingDead
    }

    static GameState state = GameState.MainMenu;
    //
    private async void wait(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    private async void mainRunner()
    {
        cameraController.bindTo("Ball");
        source = new CancellationTokenSource();
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
