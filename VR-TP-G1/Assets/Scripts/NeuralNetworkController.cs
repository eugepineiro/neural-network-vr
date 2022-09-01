using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkController : MonoBehaviour
{
    public List<int> network = new List<int>();
     
    void Start()
    {   
        network.Add(2);
        network.Add(1);
        network.Add(3);
        network.Add(4);
        network.Add(4);
        network.Add(2);
        int layers_amount = network.Count;
        int max_neurons = 4; //network.Max(); TODO conseguir el maximo de una lista 
        int largest_layer_index = network.IndexOf(max_neurons);

        //int middle_layer_index = (int) Mathf.Floor(layers_amount / 2.0F);
        float increment = 0;
        for(int layer_index = 0; layer_index < layers_amount; layer_index++){
            
            // Create Layer
            GameObject layer = new GameObject(string.Format("Layer {0}", layer_index));
            layer.transform.parent = transform; 
            
            for(int neuron_index = 0; neuron_index < network[layer_index]; neuron_index++) {
                // Create Neurons    
                GameObject neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                neuron.name = string.Format("Neuron {0}", neuron_index);
                neuron.transform.parent = layer.transform; 
                neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F); 

                neuron.transform.localPosition = new Vector3(layer_index, neuron_index, 0); 
                increment = (max_neurons - network[layer_index])/2.0F; // for centering layers in Y axis
            }
            
            // Center Layer in Y axis            
            if(layer_index != largest_layer_index) { 
                layer.transform.localPosition = new Vector3(layer.transform.localPosition.x, increment , layer.transform.localPosition.z);
            }
        }

    }

    
    void Update()
    {
        
    }
}
