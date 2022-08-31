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
        network.Add(5);
        network.Add(3);
        network.Add(2);
        int layers_amount = network.Count;

        for(int layer_index = 0; layer_index < layers_amount; layer_index++){

            GameObject layer = new GameObject(string.Format("Layer {0}", layer_index));
            layer.transform.parent = transform; 
            
            for(int neuron_index = 0; neuron_index < network[layer_index]; neuron_index++) {

                GameObject neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                neuron.name = string.Format("Neuron {0}", neuron_index);
                neuron.transform.parent = layer.transform; 
                neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F); 

                neuron.transform.localPosition = new Vector3(layer_index, neuron_index, 0); 
                
            }
        }



    }

    
    void Update()
    {
        
    }
}
