using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 0.0f;
    public TextMeshProUGUI scoreGui;
    public TextMeshProUGUI winMessage;
    public GameManage gameManage;
    private Vector3 respawnPos;

    public int health;
  
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    private Renderer plrMat;

    public float groundSpeed = 0; //0.2 feels natural

    public RythmEngine rythmEngine;

    public bool godMode = false;

    public UtilityScript UtilityScript;
    private float invunerabilityTime;

    public AudioSource songPlaying;

    public Camera DeathCam;
    public Camera mainCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        respawnPos = transform.position + Vector3.up;
        winMessage.text = " ";
        plrMat = GetComponent<Renderer>();
        if (health == 0) health = 3;
 
    }

    
    // Update is called once per frame

    private IEnumerator TakeDamage()
    {
        health--;
        if(health > 0)
        {
            //all the death effects
            invunerabilityTime = 1.5f;
            StartCoroutine(UtilityScript.MuffleMusic());
            while (Time.timeScale > 0.1f)
            {
                Time.timeScale -= Time.deltaTime * 10;
                yield return null;
            }

            

            yield return new WaitForSeconds(0.05f);

            StartCoroutine(UtilityScript.UnMuffleMusic());
            while (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * 10;
                yield return null;
            }
            Time.timeScale = 1f;
        }
        else
        {
            DeathCam.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(false);
            
            StartCoroutine(UtilityScript.MuffleMusic());
            Time.timeScale = 0.1f;
            songPlaying.Pause();


            yield return new WaitForSeconds(0.1f);

            StartCoroutine(UtilityScript.UnMuffleMusic());
            songPlaying.Play();
            Time.timeScale = 1f;
            transform.position = new Vector3(0, 1, 0);
            DeathCam.gameObject.SetActive(false);
            mainCam.gameObject.SetActive(true);
            gameObject.SetActive(false);
            gameManage.PlayerDied();
        }
    }

   

    public void Respawn()
    {
        gameObject.transform.position = respawnPos;
        gameObject.SetActive(true);
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        movementX = 0;
        movementY = 0;
        health = 3;

    }
    void OnMove(InputValue movementValue)
    {
        //Debug.Log("hhhehehehe");
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;   
        movementY = movementVector.y;   
    }


    private void FixedUpdate()
    {
        Vector3 movementForce = new Vector3(movementX, -10.0f, movementY - groundSpeed);
        rb.AddForce(movementForce * speed);
        if (transform.position.y < -3)
        {
            gameManage.PlayerDied();
            Debug.Log("player hit");
            transform.position = new Vector3(0, 1, 0);
            gameObject.SetActive(false);
            
        }
    }

    private void Update()
    {
        if(invunerabilityTime > 0)
        {
            Debug.Log("invunerable");
            plrMat.material.color = new Color(plrMat.material.color.r, plrMat.material.color.g, plrMat.material.color.b, Mathf.Clamp(Mathf.Sin(Time.time * 10), 0.1f, 0.4f));
            invunerabilityTime -= Time.deltaTime;
        }
        else
        {
            plrMat.material.color = new Color(plrMat.material.color.r, plrMat.material.color.g, plrMat.material.color.b,1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Laser") && godMode == false && invunerabilityTime <= 0)
        {
            StartCoroutine(TakeDamage());
        }
    }
}
