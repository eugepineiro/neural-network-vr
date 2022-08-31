using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkController : MonoBehaviour
{
    public List<int> network = new List<int>();
     
    void Start()
    {   
        network.Add(2);
        network.Add(3);
        network.Add(3);
        network.Add(3);
        network.Add(2);
        int layers_amount = network.Count;

        for(int layer = 0; layer < layers_amount; layer++){

            for(int neuron_index = 0; neuron_index < network[layer]; neuron_index++) {

                
                GameObject neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                neuron.transform.parent = transform; 
                neuron.transform.localPosition = new Vector3(layer, neuron_index,0); 
                neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F); 
            }
        }



    }

    
    void Update()
    {
        
    }
}
