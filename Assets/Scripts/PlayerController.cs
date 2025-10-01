using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 0.0f;
    public TextMeshProUGUI scoreGui;
    public TextMeshProUGUI winMessage; 

    private short count = 0;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        setScoreText();
        winMessage.text = " ";
    }

    
    // Update is called once per frame
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;   
        movementY = movementVector.y;   
    }
    private void setScoreText()
    {
        scoreGui.text = "Count: " + count.ToString();
        if(count >= 9)
        {
            winMessage.text = "You Win!";
        }
    }

    private void FixedUpdate()
    {
        Vector3 movementForce = new Vector3(movementX, -10.0f, movementY);
        rb.AddForce(movementForce * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            setScoreText();
        }
        else if (other.gameObject.CompareTag("Laser"))
        {
            gameObject.SetActive(false);
            winMessage.text = "You Loose!";
        }
    }
}
