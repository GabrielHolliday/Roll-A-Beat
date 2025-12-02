using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public UnityEvent pingHealth;
    private System.Random random = new System.Random();
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

    public GameObject shatteredPlayer;

    public Vector3 dirHit;
    private GameObject shtrPlayer;

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
    private bool test = false;
    private IEnumerator TakeDamage()
    {
        health--;
        pingHealth.Invoke();
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
            if (test == true) yield break;
            test = true;
            shtrPlayer = Instantiate(shatteredPlayer, transform.position + Vector3.up * 0.5f, Quaternion.Euler(0, 0, 0));

            
            for (int i = 0; i < shtrPlayer.transform.childCount; i++)
            {
                shtrPlayer.transform.GetChild(i).GetComponent<Rigidbody>().AddForce(new Vector3(dirHit.z * random.Next(3,40), random.Next(-2,20), 0), ForceMode.Impulse);
            }
            
            Debug.LogWarning(dirHit);
            //shtrPlayer.transform.GetComponent<Rigidbody>().AddForce(new Vector3(dirHit.z * 200,0,0) );
            DeathCam.gameObject.SetActive(true);
            mainCam.gameObject.SetActive(false);
            GetComponent<Renderer>().enabled = false;
            
           
            StartCoroutine(UtilityScript.MuffleMusic());
            Time.timeScale = 0.3f;
            //songPlaying.Pause();


            yield return new WaitForSeconds(0.3f);

            StartCoroutine(UtilityScript.UnMuffleMusic());
            //songPlaying.Play();
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
        invunerabilityTime = 0;
        Time.timeScale = 1f;
        
        gameObject.transform.position = respawnPos;
        gameObject.SetActive(true);
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        movementX = 0;
        movementY = 0;
        health = 3;
        if (shtrPlayer != null) Destroy(shtrPlayer);
        GetComponent<Renderer>().enabled = true;
        StartCoroutine(UtilityScript.UnMuffleMusic());
        test = false;
        Debug.Log("Respawning");
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
            //Debug.Log("invunerable");
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
            dirHit = other.transform.forward;
            StartCoroutine(TakeDamage());
        }
    }
}
