using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTP3 {
public class MovementController : MonoBehaviour
{
    public float movementSpeed; //10
    public float rotationSpeed; //100
    public float scaleSpeed;
    public bool showLabels = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* TRANSLATE AWSD */ 
        float t = Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.Translate(new Vector3(-movementSpeed*t,0,0));

        if (Input.GetKey(KeyCode.D))
            transform.Translate(new Vector3(movementSpeed*t,0,0));

        if (Input.GetKey(KeyCode.W))
            transform.Translate(new Vector3(0,movementSpeed*t,0));

        if (Input.GetKey(KeyCode.S))
            transform.Translate(new Vector3(0,-movementSpeed*t,0));

        /* ROTATE Arrows */
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(new Vector3(0,rotationSpeed*t,0));

        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(new Vector3(0,-rotationSpeed*t,0));

        if (Input.GetKey(KeyCode.UpArrow))
            transform.Rotate(new Vector3(rotationSpeed*t,0,0));

        if (Input.GetKey(KeyCode.DownArrow))
            transform.Rotate(new Vector3(-rotationSpeed*t,0,0));

        /* SCALE QE */
        // if (Input.GetKey(KeyCode.Q))
        //     transform.localScale = Vector3.MoveTowards(transform.localScale, transform.localScale*2,scaleSpeed*t);
        // if (Input.GetKey(KeyCode.E))
        //     transform.localScale = Vector3.MoveTowards(transform.localScale, transform.localScale/2,scaleSpeed*t);
        if (Input.GetKey(KeyCode.Q))
            transform.localScale /= Mathf.Exp(Mathf.Log(scaleSpeed)*t);
        if (Input.GetKey(KeyCode.E))
            transform.localScale *= Mathf.Exp(Mathf.Log(scaleSpeed)*t);

        /* UN/HIDE LABELS L */
        if (Input.GetKeyDown(KeyCode.L)) {
            // // TODO si apretas la L lo desactiva pero despues si la apretas de nuevo tira nullpointer  
            // GameObject labels = GameObject.Find("Labels");
           
            // if(/*labels != null &&*/ labels.activeInHierarchy) { 
            //     Debug.Log("is active");
            //     labels.SetActive(false);
                
            // } else {
            //     labels.SetActive(true);
            // }
            foreach(GameObject labels in GameObject.FindGameObjectsWithTag("Labels")){
                foreach (Renderer labelRenderer in labels.GetComponentsInChildren(typeof(Renderer))){
                    labelRenderer.enabled = ! labelRenderer.enabled;
                }
            }
        }

    }
}
}