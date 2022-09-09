using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Material neuronMaterial;

    void Start()
    {
        GameObject neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      
        neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F); 
        neuron.GetComponent<MeshRenderer>().material = neuronMaterial;
        neuron.transform.localPosition = new Vector3(0,0,0);
 
    
        Color color = new Color(88, 24, 69, 1.0f);
        neuron.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
