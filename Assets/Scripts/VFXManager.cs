using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class VFXManager : MonoBehaviour
{

    //externals
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector3 CurrentColorTarg = new Vector3(255, 61, 61);
    private Canvas backGround;
    private Image spirePref;

    private AudioClip assumedClip;
    private GameObject[] cureSpires = new GameObject[64]; 
    
    void Start()
    {
        backGround = transform.Find("BackgroundEffect").GetComponent<Canvas>();
        spirePref = backGround.transform.Find("Spire").GetComponent<Image>();
        BuildSpires(CurrentColorTarg);
        
    }

    public void BuildSpires(Vector3 color)
    {
        CurrentColorTarg = color;
        Debug.Log("a");
        for (int i = 0; i < 64; i++)
        {
            GameObject curImg = Instantiate(spirePref.gameObject, backGround.transform);
            curImg.transform.localPosition = new Vector3(((float)i - 25) * 2, -15  + Mathf.Abs((( i -25)*(i-25)) * 0.01f), 0);
            cureSpires[i] = curImg;
        }
    }

    private void rebuildAudioData()
    { 
    }


    

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = new float[64];

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        Debug.Log(spectrum[32]);
        for (int i = 0; i < spectrum.Length; i++)
        {
            cureSpires[i].transform.localPosition = Vector3.Lerp(cureSpires[i].transform.localPosition, new Vector3(cureSpires[i].transform.localPosition.x, - 15 +  spectrum[i] * 500, 0), 0.02f) ;
        }

    }
}
