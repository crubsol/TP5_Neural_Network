using UnityEngine;

public class XORTrainer : MonoBehaviour
{
    public GameObject cuboPrefab;       // Prefab del cubo
    public GameObject esferaPrefab;     // Prefab de la esfera
    public int numEntradas = 3;         // Número de entradas (cubos)
    public int numOcultas = 4;          // Número de neuronas ocultas (ajustar según sea necesario)
    public int numSalidas = 1;          // Número de salidas (esferas)

    private GameObject[] cubos;         // Array para almacenar los cubos
    private GameObject[] esferas;       // Array para almacenar las esferas (resultado de la red)
    private GameObject[] esferasEsperadas; // Array para las esferas de resultado esperado

    private NeuralNetwork nn;

    void Start()
    {
        // Crear la red neuronal con valores definidos por el inspector
        nn = new NeuralNetwork(numEntradas, numOcultas, numSalidas);

        // Crear y posicionar los cubos
        cubos = new GameObject[numEntradas];
        for (int i = 0; i < numEntradas; i++)
        {
            cubos[i] = Instantiate(cuboPrefab, new Vector3(i * 2.0f, 2.0f, 0), Quaternion.identity);
            cubos[i].GetComponent<CuboController>().xorTrainer = this;  // Asignar el XORTrainer al cubo
        }

        // Crear las esferas correspondientes a las salidas de la red neuronal
        esferas = new GameObject[numSalidas];
        for (int i = 0; i < numSalidas; i++)
        {
            esferas[i] = Instantiate(esferaPrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
        }

        // Crear la segunda fila de esferas para los resultados esperados
        esferasEsperadas = new GameObject[numSalidas];
        for (int i = 0; i < numSalidas; i++)
        {
            esferasEsperadas[i] = Instantiate(esferaPrefab, new Vector3(i * 2.0f, -2.0f, 0), Quaternion.identity);
        }

        // Generar las entradas y las salidas de entrenamiento para XOR
        float[][] inputs = GenerarEntradas(numEntradas);  // 8 combinaciones para XOR con 3 entradas
        float[][] targets = GenerarTargets(inputs);

        for (int i = 0; i < targets.Length; i++)
        {
            Debug.Log($"Target {i}: " + string.Join(", ", targets[i]));
        }

        // Entrenar la red neuronal
        nn.Train(inputs, targets, 10000, 0.5f, 0.1f);

        // Inicializar los colores de las esferas
        ActualizarColorEsferas();
    }

    // Genera todas las combinaciones posibles de entradas para los cubos
    float[][] GenerarEntradas(int numEntradas)
    {
        int numCombinaciones = (int)Mathf.Pow(2, numEntradas);
        float[][] entradas = new float[numCombinaciones][];

        for (int i = 0; i < numCombinaciones; i++)
        {
            entradas[i] = new float[numEntradas];
            for (int j = 0; j < numEntradas; j++)
            {
                entradas[i][j] = (i & (1 << j)) != 0 ? 1f : 0f;
            }
        }

        return entradas;
    }

    // Genera los targets correspondientes para un XOR generalizado en varias salidas
    float[][] GenerarTargets(float[][] inputs)
    {
        int numCombinaciones = inputs.Length;
        float[][] targets = new float[numCombinaciones][];

        for (int i = 0; i < numCombinaciones; i++)
        {
            targets[i] = new float[numSalidas];
            for (int salida = 0; salida < numSalidas; salida++)
            {
                float target = 0;
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    target += (salida % 2 == 0) ? inputs[i][j] : 1 - inputs[i][j];
                }
                targets[i][salida] = (target % 2 == 0) ? 0f : 1f;
            }
        }

        return targets;
    }

    // Actualiza el color de las esferas según las salidas de la red neuronal y el valor esperado
    public void ActualizarColorEsferas()
    {
        // Obtener los estados actuales de los cubos
        float[] inputs = new float[cubos.Length];
        for (int i = 0; i < cubos.Length; i++)
        {
            inputs[i] = cubos[i].GetComponent<CuboController>().ObtenerEstadoColor();
        }

        // Resultado de la red neuronal
        float[] result = nn.Update(inputs);

        for (int i = 0; i < esferas.Length; i++)
        {
            esferas[i].GetComponent<Renderer>().material.color = (result[i] > 0.5f) ? Color.black : Color.white;
        }

        // Resultado deseado
        float[][] inputsCombinations = GenerarEntradas(numEntradas);
        float[][] targets = GenerarTargets(inputsCombinations);

        // Calcular el índice del conjunto de entradas actual
        int index = CalcularIndice(inputs);

        // Actualizar el color de las esferas esperadas
        for (int i = 0; i < numSalidas; i++)
        {
            esferasEsperadas[i].GetComponent<Renderer>().material.color = (targets[index][i] > 0.5f) ? Color.black : Color.white;
        }

        // Imprimir en consola el target correspondiente al input actual
        Debug.Log($"Input actual: {string.Join(", ", inputs)} | Target esperado: {string.Join(", ", targets[index])}");
    }


    // Calcular el índice para el conjunto de entradas actual
    private int CalcularIndice(float[] inputs)
    {
        int indice = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == 1f)
            {
                indice += (1 << i);
            }
        }
        return indice;
    }
}
