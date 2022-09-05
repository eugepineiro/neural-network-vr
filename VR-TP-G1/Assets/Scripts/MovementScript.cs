using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float movementSpeed; //10
    public float rotationSpeed; //100
    public float scaleSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            //Debug.Log("A");
            transform.Translate(new Vector3(-movementSpeed*t,0,0));
        if (Input.GetKey(KeyCode.D))
            //Debug.Log("D");
            transform.Translate(new Vector3(movementSpeed*t,0,0));
        if (Input.GetKey(KeyCode.W))
            //Debug.Log("W");
            transform.Translate(new Vector3(0,movementSpeed*t,0));
        if (Input.GetKey(KeyCode.S))
            //Debug.Log("S");
            transform.Translate(new Vector3(0,-movementSpeed*t,0));

        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(new Vector3(0,rotationSpeed*t,0));
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(new Vector3(0,-rotationSpeed*t,0));
        if (Input.GetKey(KeyCode.UpArrow))
            transform.Rotate(new Vector3(rotationSpeed*t,0,0));
        if (Input.GetKey(KeyCode.DownArrow))
            transform.Rotate(new Vector3(-rotationSpeed*t,0,0));

        // if (Input.GetKey(KeyCode.Q))
        //     transform.localScale = Vector3.MoveTowards(transform.localScale, transform.localScale*2,scaleSpeed*t);
        // if (Input.GetKey(KeyCode.E))
        //     transform.localScale = Vector3.MoveTowards(transform.localScale, transform.localScale/2,scaleSpeed*t);
        if (Input.GetKey(KeyCode.Q))
            transform.localScale /= Mathf.Exp(Mathf.Log(scaleSpeed)*t);
        if (Input.GetKey(KeyCode.E))
            transform.localScale *= Mathf.Exp(Mathf.Log(scaleSpeed)*t);

    }
}
