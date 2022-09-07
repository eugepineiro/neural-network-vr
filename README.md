# VR-TP3

Se desarrolló una representación en Realidad Virtual de distintos modelos de Redes Neuronales. 

El objetivo principal del proyecto consiste en que el usuario pueda aprender a distinguir arquitecturas de Redes Neuronales. La visualización en 3D de la totalidad de la red facilita la comprensión de la misma, teniendo en cuenta la cantidad de capas ocultas, la cantidad de neuronas en cada capa, cómo es la capa de salida y cómo son las conexiones entre las neuronas.

Además, se puede aprender de una forma interactiva e inmersiva, lo cual hace al aprendizaje más dinámico y entretenido para el usuario. 

## Modo de Uso 

Se recibe un archivo de configuración `data.json` con las características de la red 

| Parámetro| Descripción                    | Opciones|
| ------------- | ------------------------------ | ------------- |
|"nn_type"     |  Modelo de Red Neuronal  | MLP, KOHONEN, AUTOENCODER |
|"layers"     |  Array donde el índice representa la capa y el valor la cantidad de neuronas en esa capa  | int array |


## Ejemplos 

### Perceptron Simple 
Red con 5 neuronas en la capa de entrada y una en la capa de salida.
```json
{
    "nn_type": "MLP",
    "layers": [5, 1],
}
```

### Perceptron Multicapa
Red con 10 neuronas en la capa de entrada, dos capas intermedias con 20 y 30 neuronas respectivamente y 40 neuronas en la capa de salida.
```json
{
    "nn_type": "MLP",
    "layers": [10,20,30,40],
}
```

### Autoencoder
Red con 10 neuronas en la capa de entrada, dos capas intermedias con 20 y 30 neuronas respectivamente, 2 neuronas en el espacio latente. Luego, la red se genera espejada quedando de la siguiente forma: [10,20,30,2,30, 20,10]. No es necesario por las capas del Decoder.
```json
{
    "nn_type": "AUTOENCODER",
    "layers": [10,20,30,2],
}
```

### Kohonen
Red de Kohonen
```json
 
```

## Interacción con Teclas
- W: Trasladar la red hacia arriba 
- A: Trasladar  la red hacia la izquierda 
- S: Trasladar  la red hacia abajo
- D: Trasladar  la red hacia la derecha 

- Flechas: Rotar la red respecto de su centro 

- R: Volver la red a la posición inicial 
- L: Mostrar y ocultar las etiquetas de cada neurona