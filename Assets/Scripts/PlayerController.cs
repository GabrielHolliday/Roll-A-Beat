using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 0.0f;
    public TextMeshProUGUI scoreGui;
    public TextMeshProUGUI winMessage;
    public GameManage gameManage;

  
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    public float groundSpeed = 0; //0.2 feels natural

    public RythmEngine rythmEngine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    
        winMessage.text = " ";
    }

    
    // Update is called once per frame


    void OnMove(InputValue movementValue)
    {
        Debug.Log("hhhehehehe");
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
            gameObject.SetActive(false);
            winMessage.text = "You Loose!";
            rythmEngine.stopMusic(gameManage.source.Token);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Laser"))
        {
            gameObject.SetActive(false);
            winMessage.text = "You Loose!";
            rythmEngine.stopMusic(gameManage.source.Token);
        }
    }
}
