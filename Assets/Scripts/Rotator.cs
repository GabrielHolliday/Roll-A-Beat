using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(15, 45, 15) * Time.deltaTime); 
    }
}
