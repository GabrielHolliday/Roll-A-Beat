using UnityEngine;
using System.Threading.Tasks;

public class GameManage : MonoBehaviour
{
    //Other scripts----
    public RythmEngine rythmEngine;
    public GroundMover groundMover;
    public PlayerController playerController;
    public UtilityScript utilityScript;
    public CameraController cameraController;


    //-----------------

    private async void wait(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    private async void mainRunner()
    {
        await Task.Delay(7000);
        rythmEngine.StartRound(0);
    }
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainRunner();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
