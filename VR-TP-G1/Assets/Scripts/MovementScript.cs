using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float movementSpeed = 10;
    public float rotationSpeed = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.Translate(new Vector3(-movementSpeed*t,0,0),Space.World);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(new Vector3(movementSpeed*t,0,0),Space.World);
        if (Input.GetKey(KeyCode.W))
            transform.Translate(new Vector3(0,movementSpeed*t,0),Space.World);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(new Vector3(0,-movementSpeed*t,0),Space.World);

        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(new Vector3(0,rotationSpeed*t,0),Space.World);
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(new Vector3(0,-rotationSpeed*t,0),Space.World);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Rotate(new Vector3(rotationSpeed*t,0,0));
        if (Input.GetKey(KeyCode.DownArrow))
            transform.Rotate(new Vector3(-rotationSpeed*t,0,0));

        
    }
}
