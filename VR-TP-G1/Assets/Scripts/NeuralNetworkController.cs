using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace VRTP3 {
    public class NeuralNetworkController : MonoBehaviour
    {
        public List<int> network = new List<int>();

    private enum NetworkType { MLP, AUTOENCODER, KOHONEN };

    private NetworkType network_type = NetworkType.KOHONEN; // TODO esto deberia ser userInput 

    private int[,] kohonen_activations = { { 1, 2 }, { 3, 5 }}; // TODO esto deberia ser userInput 
     
    void Start()
    {   
        network.Add(2);  // TODO esto deberia ser userInput 
        network.Add(1);
        network.Add(3);
        network.Add(4);
        network.Add(4);
        network.Add(2); 

        switch (network_type) {

            case NetworkType.MLP: 
                BuildMLP(network);
                break; 
            
            case NetworkType.AUTOENCODER: 
                Debug.Log("AUTOENCODER");
                
                List<int> autoencoder = new List<int>();
                autoencoder.AddRange(network);
                network.RemoveAt(network.Count - 1); // latent space is not replicated
                network.Reverse();                   // mirror network
                autoencoder.AddRange(network);

                BuildMLP(autoencoder);
                break; 

            case NetworkType.KOHONEN: 
                BuildKohonen(kohonen_activations);
                break; 
        }     


    }

        
        void Update()
        {
            
        }
        
        private void BuildMLP(List<int> network) { 

            int layers_amount = network.Count;
            int max_neurons = 4; //network.Max(); TODO conseguir el maximo de una lista 
            int largest_layer_index = network.IndexOf(max_neurons);

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

            addConnections(1,2); // TODO for each layer 
        }

        private void addConnections(int first_layer, int second_layer) { 

                // Create Connection Parent 
                GameObject connections = new GameObject(string.Format("Connections {0} {1}", first_layer, second_layer));
                connections.transform.parent = transform; 
                int first_layer_neurons = network[first_layer]; 
                int second_layer_neurons = network[second_layer];
            
                for(int first_neuron_index = 0; first_neuron_index < network[first_layer]; first_neuron_index++) {
                    for(int second_neuron_index = 0; second_neuron_index < network[second_layer]; second_neuron_index++) {

                        GameObject connection = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                        connection.name = string.Format("Connection {0}{1}-{2}{3}", first_layer, first_neuron_index, second_layer, second_neuron_index);
                        connection.transform.parent = connections.transform; 
                        connection.transform.localScale = new Vector3(0.05F, 0.5F, 0.05F);

                        GameObject layer0 = GameObject.Find("Layer 0");
                        GameObject neuron00 = layer0.transform.Find("Neuron 0").gameObject; 

                        GameObject layer1 = GameObject.Find("Layer 1");
                        GameObject neuron10 = layer1.transform.Find("Neuron 0").gameObject; 
                        float x = (neuron10.transform.position.x + neuron00.transform.position.x) / 2.0F;
                        //connection.transform.position = new Vector3(0, x, 0);
                    } 
                }    
        }

        private JsonData LoadJsonData() {
            JsonData json = JsonUtility.FromJson<JsonData>(jsonFile.text);
            return json;
        }
    
    }
    
    private void BuildKohonen(int[,] activations) { 
        
        int neurons_amount = 0;
        int height = activations.GetLength(0);
        int width = activations.GetLength(1);

        for (int i=0; i < height; i++) {  // TODO tiene que haber mejor forma de sumar todo en C# 
            for (int j=0; j < width; j++) { 
                neurons_amount += activations[i,j];
            }
        }
        
        // Create Layer
        GameObject layer = new GameObject(string.Format("Layer 0"));
        layer.transform.parent = transform; 
        
        generateNeurons(layer, neurons_amount, 0,0);

        // Last Layer

        GameObject last_layer = new GameObject(string.Format("Last Layer"));
        last_layer.transform.parent = transform; 
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = last_layer.transform;

        plane.transform.localScale = new Vector3(height/10.0F, height/10.0F, height/10.0F); 
        plane.transform.localPosition = new Vector3(5, 0, 0); 
        plane.transform.localRotation = Quaternion.Euler(0, 0, 90); 

        GameObject neurons = new GameObject(string.Format("Last Neurons"));
        neurons.transform.parent = last_layer.transform;
        for (int i = 0; i < height; i++) { 
            generateNeurons(neurons, height, 5, i);
        }

    }

    private void generateNeurons(GameObject layer, int neurons_amount, int x, int z) { 
        
        for(int neuron_index = 0; neuron_index < neurons_amount; neuron_index++) {
            // Create Neurons    
            GameObject neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            neuron.name = string.Format("Neuron {0}", neuron_index);
            neuron.transform.parent = layer.transform; 
            neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F); 

            neuron.transform.localPosition = new Vector3(x, neuron_index, z); 
             
        }
    }
}