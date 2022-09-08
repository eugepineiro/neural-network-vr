using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VRTP3 {
public class NeuralNetworkController : MonoBehaviour{

    public List<int> network = new List<int>();
    public Material connectionMaterial;
    public Material neuronMaterial;
    public Material quadMaterial;
    public Material planeMaterial;
    public Material pinkMaterial;
    public Material purpleMaterial;
    public Material orangeMaterial;
    public Material blueMaterial;
	public GameObject selfConnection;
    public TextAsset jsonFile;
    public Transform target;

    public const float MAX_RADIUS = 2;
    
    private NetworkType network_type = NetworkType.MLP;
 
    private enum NetworkType { MLP, AUTOENCODER, KOHONEN };

    private bool low_cost = false;
        
    void Start() {   

        JsonData j = JsonUtility.FromJson<JsonData>(jsonFile.text);
        low_cost = j.improve_performance;
        Debug.Log(low_cost);
        if (Enum.IsDefined(typeof(NetworkType), j.nn_type)) {
            network_type = (NetworkType)Enum.Parse(typeof(NetworkType), j.nn_type);
        }
        foreach (int number in j.layers) {
            network.Add(number);
        }

        switch (network_type) {

            case NetworkType.MLP:
				Debug.Log("MLP");
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
                if (j.kohonen != null) {
                    int[,] kohonen_activations; 
                    int dimension = j.kohonen.grid_dimension;
                    bool has_activations; 

                    if (j.kohonen.activations != null) {
                        kohonen_activations = activationsParser(j.kohonen.activations, j.kohonen.grid_dimension);
                        has_activations = true;
                    } else { 
                        kohonen_activations = new int[dimension, dimension]; 
                        for (int i =0; i < dimension; i++) { 
                            for (int k=0; k< dimension; k++) { 
                                kohonen_activations[i,k] = 1;
                            }
                        }
                        has_activations = false;
                    }
                    BuildKohonen(kohonen_activations, j.kohonen.input_dimension, has_activations);
                    
                } else {
                    Debug.Log("Faltan parametros para Kohonen");
                }
                break; 
        }
    }
    
    private void BuildMLP(List<int> network)
    {
        int layers_amount = network.Count;
		int tallestLayer = 0;
        GameObject labels = new GameObject();
        labels.transform.parent = transform;
        labels.name = "Labels";
        labels.tag = "Labels";
        for(int layer_index = 0; layer_index < layers_amount; layer_index++){
            GameObject layer = createLayer(layer_index, network[layer_index], labels);
			if (layer.transform.childCount > tallestLayer)
				tallestLayer = layer.transform.childCount;
		}
        for(int l=0; l < layers_amount-1; l++)
            addConnections(l, l+1);
        float height = network.Max();
        float width = layers_amount-1;
        float s = 1;
        if (width >= height && width > MAX_RADIUS)
        {
            s = 2*(MAX_RADIUS / width);
            this.transform.localScale = new Vector3(s, s, s);
        }
        else if(height >= width && height > MAX_RADIUS)
        {
            s = 2*(MAX_RADIUS / height);
            this.transform.localScale = new Vector3(s,s,s);
        }
        this.transform.position = new Vector3(-s*(width/2.0f), 0, 0); // Si quiero que quede apoyado, y=(tallestLayer/2.0f)
        PivotTo(new Vector3(0,0,0));
    }

    public void PivotTo(Vector3 position)
    {
        Vector3 offset = transform.position - position;
        foreach (Transform child in transform)
            child.transform.position += offset;
        transform.position = position;
    }

    private GameObject createLayer(int layer_index, int neurons_amount, GameObject labels)
    {
        
        GameObject layer = new GameObject(string.Format("Layer {0}", layer_index));
        layer.transform.parent = transform;

      
        for (int neuron_index = 0; neuron_index < neurons_amount; neuron_index++)  
        {   
            GameObject neuron;
            if (low_cost) { 
                GameObject parentLookAt = new GameObject(); 
                parentLookAt.transform.parent = layer.transform;
                parentLookAt.name = string.Format("Neuron {0}", neuron_index);
                neuron = GameObject.CreatePrimitive(PrimitiveType.Quad);
                neuron.GetComponent<MeshRenderer>().material = quadMaterial;
                neuron.transform.localRotation = Quaternion.Euler(0, 180, 0);
                neuron.transform.parent = parentLookAt.transform; 
                neuron.name = string.Format("Neuron Child {0}", neuron_index);
                
                parentLookAt.transform.localPosition = new Vector3(layer_index, neuron_index - (neurons_amount-1)/2.0f, 0);
                LookAtController script = parentLookAt.AddComponent<LookAtController>();
                script.target = target;
            
            } else {
                neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                neuron.GetComponent<MeshRenderer>().material = neuronMaterial;
                neuron.transform.parent = layer.transform;
                neuron.name = string.Format("Neuron {0}", neuron_index);
                neuron.transform.localPosition = new Vector3(layer_index, neuron_index - (neurons_amount-1)/2.0f, 0);
            }
            
            
            neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F);
            

            generateLabels(labels, neuron,  string.Format("({0};{1})", layer_index, neuron_index), new Color(0,0,0,1));
        }
        return layer;
    }

    private void addConnections(int first_layer, int second_layer) { 
            // Create Connection Parent 
            GameObject layer1 = GameObject.Find("Layer " + first_layer);
            GameObject layer2 = GameObject.Find("Layer " + second_layer);
            GameObject connections = new GameObject(string.Format("Connections {0}-{1}", first_layer, second_layer));
            connections.transform.parent = transform;

            for (int first_neuron_index = 0; first_neuron_index < layer1.transform.childCount; first_neuron_index++) {
                for(int second_neuron_index = 0; second_neuron_index < layer2.transform.childCount; second_neuron_index++)
                {
                    GameObject connection = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    GameObject neuronA = layer1.transform.Find("Neuron " + first_neuron_index).gameObject;
                    GameObject neuronB = layer2.transform.Find("Neuron " + second_neuron_index).gameObject;
                    Vector3 p1 = neuronA.transform.position;
                    Vector3 p2 = neuronB.transform.position;

                    connection.GetComponent<MeshRenderer>().material = connectionMaterial;
                    connection.name = string.Format("Connection {0}.{1}-{2}.{3}", first_layer, first_neuron_index, second_layer, second_neuron_index);
                    connection.transform.parent = connections.transform;
                    connection.transform.localScale = new Vector3(0.02F, Vector3.Distance(p1, p2)/2, 0.02F);
                    connection.transform.position = (p2 + p1) / 2.0F;
                    connection.transform.up = p2-p1;
                } 
            }    
    }

    private JsonData LoadJsonData() {
        JsonData json = JsonUtility.FromJson<JsonData>(jsonFile.text);
        return json;
    }

    private void generateLabels(GameObject parent, GameObject neuron, string text, Color color){ 
            
            GameObject label = new GameObject();
         
            label.transform.parent = parent.transform;
            label.name = "Label";

            //Create TextMesh and modify its properties
            TextMesh textMesh = label.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.characterSize = 0.1F;
            textMesh.color = color;
            if(low_cost) 
                textMesh.offsetZ = -0.3F;

            //Set postion of the TextMesh same as Neuron
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.transform.position = new Vector3(neuron.transform.position.x, neuron.transform.position.y, neuron.transform.position.z);
            
    }
    
    private void BuildKohonen(int[,] activations, int input_dimension, bool has_activations) { 
        int height = activations.GetLength(0);
        int width = activations.GetLength(1);
        GameObject labels = new GameObject();
        labels.transform.parent = transform;
        labels.name = "Labels";
        labels.tag = "Labels";
        // First Layer
        GameObject layer = createLayer(0, input_dimension, labels);

        // Outside-facing plane
        GameObject last_layer = new GameObject(string.Format("Last Layer"));
        last_layer.transform.parent = transform;
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plane.transform.parent = last_layer.transform;
        plane.transform.localScale = new Vector3(height, 0.001f, width);
        plane.transform.localPosition = new Vector3(5, 0, 0);
        plane.transform.localRotation = Quaternion.Euler(0, 0, 90);
        plane.GetComponent<MeshRenderer>().material = planeMaterial;

        // Final Neurons
        GameObject top_neurons = new GameObject(string.Format("Layer 1"));
        top_neurons.transform.parent = last_layer.transform;
 
        for (int i = 0; i < width; i++)
            generateKohonenTopLayerColumn(top_neurons, height, i, width, activations,labels, has_activations);

		addKohonenConnections(top_neurons, width, height);
        addConnections(0, 1);
        float network_h = Mathf.Max(network[0], height);
        float network_w = 5;
        float s = 1;
        if (network_w >= network_h && network_w > MAX_RADIUS)
        {
            s = 2 * (MAX_RADIUS / network_w);
            this.transform.localScale = new Vector3(s, s, s);
        }
        else if (network_h >= network_w && network_h > MAX_RADIUS)
        {
            s = 2 * (MAX_RADIUS / network_h);
            this.transform.localScale = new Vector3(s, s, s);
        }
        this.transform.position = new Vector3(-s*2.5f, 0, 0); // Si quiero que quede apoyado, y=(tallestLayer/2.0f)
        PivotTo(new Vector3(0, 0, 0));
     }

    private void generateKohonenTopLayerColumn(GameObject layer, int height, int column, int width,  int[,] kohonen_activations, GameObject labels, bool has_activations)
    {   
        Color[] activation_colors = GetColors();
        
        for(int neuron_index = 0; neuron_index < height; neuron_index++)
		{   
            GameObject neuron;
            if (low_cost) { 
                GameObject parentLookAt = new GameObject(); 
                parentLookAt.transform.parent = layer.transform;
                neuron = GameObject.CreatePrimitive(PrimitiveType.Quad);
                neuron.GetComponent<MeshRenderer>().material = quadMaterial;
                //neuron.transform.localRotation = Quaternion.Euler(0, 180, 0);
                neuron.transform.parent = parentLookAt.transform;
                
                parentLookAt.transform.localPosition = new Vector3(5, neuron_index - (height - 1)/2.0f, column - (width - 1) / 2.0f); 
                parentLookAt.name = string.Format("Neuron {0}", neuron_index+column* height);
                neuron.name = string.Format("Neuron Child {0}", neuron_index+column* height);
                
            } else {
                neuron = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                neuron.GetComponent<MeshRenderer>().material = neuronMaterial;
                neuron.transform.parent = layer.transform; 
                neuron.transform.localPosition = new Vector3(5, neuron_index - (height - 1)/2.0f, column - (width - 1) / 2.0f);
                neuron.name = string.Format("Neuron {0}", neuron_index+column* height);

            }
            neuron.transform.localScale = new Vector3(0.2F, 0.2F, 0.2F);  

            string label_txt;
            Color label_color;
            if (has_activations) { 
                label_txt = string.Format("{0}", kohonen_activations[column, neuron_index]);
                int min_value = getMinValue(kohonen_activations); 
                int max_value = getMaxValue(kohonen_activations); 

                /*Color color = GetColor( min_value,  max_value, kohonen_activations[column, neuron_index], activation_colors);
                neuron.GetComponent<Renderer>().material.SetColor("_Color", new Color(200, 30, 150, 1.0f));*/ 
                neuron.GetComponent<MeshRenderer>().material = GetMaterial(min_value,max_value, kohonen_activations[column, neuron_index]);
                label_color = new Color(255,255,255,1);
            } else { 
                label_txt = string.Format("(1;{0};{1})", column, neuron_index);
                label_color = new Color(0,0,0,1);
                
            }
            generateLabels(labels, neuron, label_txt, label_color);

            
        }
    }

	private void addKohonenConnections(GameObject layer, int width, int height){
		GameObject connections = new GameObject(string.Format("Connections K"));
		for (int col = 0; col < height; col++)
			for (int row = 0; row < width; row++)
				addKohonenConnection(row, col, width, height, layer, connections);
	}


	private void addKohonenConnection(int row, int col, int width, int height, GameObject layer, GameObject connections) {
		int indexA = row + col*height;
		GameObject neuronA = layer.transform.Find("Neuron " + indexA).gameObject;
		connections.transform.parent = transform;

		Vector3 loop_position = neuronA.transform.localPosition + new Vector3(0.1f, 0, 0);
		GameObject loop = Instantiate(selfConnection, loop_position, Quaternion.identity);
		loop.transform.parent = connections.transform;
		loop.name = string.Format("Connection K{0}", indexA);

		for (int dc = -1; dc <= 1; dc++)
		{
			for (int dr = -1; dr <= 0; dr++)
			{
				int indexB = (row+dr) + (col+dc)*width;
				if (indexA!=indexB && (row + dr)>=0 && indexB >= 0 && indexB < layer.transform.childCount && (dr!=0 || dc<=0)){
					GameObject neuronB = layer.transform.Find("Neuron " + indexB).gameObject;
					GameObject connection = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
					Vector3 p1 = neuronA.transform.position;
					Vector3 p2 = neuronB.transform.position;

					connection.GetComponent<MeshRenderer>().material = connectionMaterial;
					connection.name = string.Format("Connection K{0}-K{1}", indexA, indexB);
					connection.transform.parent = connections.transform;
					connection.transform.localScale = new Vector3(0.02F, Vector3.Distance(p1, p2) / 2, 0.02F);
					connection.transform.position = (p2 + p1) / 2.0F;
					connection.transform.up = p2 - p1;
				}
			}
		}
	}

	private Color[] GetColors() { 

        Color[] colors = { 
            new Color(255, 195, 0, 1.0f), 
            new Color(255, 87, 51 , 1.0f),
            new Color(199, 0, 57, 1.0f),
            new Color(144, 12, 63, 1.0f),
            new Color(88, 24, 69, 1.0f),
            new Color(0, 0, 0, 1.0f)
        };

        return colors;
        
    }

    private Color GetColor(int min_value, int max_value, int value, Color[] colors) {  
        int step = max_value / colors.Count();
        int delta = (int) Mathf.Floor((max_value - min_value) / step);
        return colors[delta];
    }

    private Material GetMaterial(int min_value, int max_value, int value) {
        Material[] materials = { blueMaterial, purpleMaterial, pinkMaterial, orangeMaterial};  

        int step = max_value / materials.Count();
        int delta = (int) Mathf.Floor((max_value - value) / step);
         
        return materials[delta];
    }

    private int[,] activationsParser(string activations, int dimension) {
        int[,] result = new int[dimension,dimension];
        string num = "";
        bool isOpen = false;
        int row = 0;
        int column = 0;
        for (int i = 0; i < activations.Length; i++)
        {
            if (activations[i] == '[') {
                isOpen = true;
                column = 0;
                num = "";
            } else if (activations[i] == ']') {
                if (isOpen) {
                    if (num != "") {
                        result[row,column] = int.Parse(num);
                    }
                    row++;
                }
                isOpen = false;
            } else if (activations[i] == ',' && isOpen) {
                result[row,column] = int.Parse(num);
                column++;
                num = "";
            } else if (Char.IsDigit(activations[i])) {
                num += activations[i];
            }
        }
        return result;
    }

    private int getMinValue(int[,] matrix) { 

        int height = matrix.GetLength(0);
        int width =matrix.GetLength(1);

        int min = matrix[0,0];
        for (int i =0; i < width; i++) { 
            for (int j=0; j < height; j++) { 
                if (matrix[i,j] < min) 
                    min = matrix[i,j];
            }
        }

        return min;
    } 

    private int getMaxValue(int[,] matrix) { 
        
        int height = matrix.GetLength(0);
        int width = matrix.GetLength(1);

        int max = 0;
        for (int i =0; i < width; i++) { 
            for (int j=0; j < height; j++) { 
                if (matrix[i,j] > max) 
                    max = matrix[i,j];
            }
        }

        return max;
    } 
    
}
}