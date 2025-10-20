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


    //-----------------
    static CancellationTokenSource source;
    //

    private async void wait(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    private async void mainRunner()
    {
        //EditorApplication.playModeStateChanged += closingGame;
        source = new CancellationTokenSource();
        await Task.Delay(7000);
        rythmEngine.StartRound(0, source.Token);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    static void closingGame()
    {
        //saving






        //

    }
    void Start()
    {
        mainRunner();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
