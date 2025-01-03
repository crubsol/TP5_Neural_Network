using System;
using UnityEngine;

public class NeuralNetwork
{
    // Hacer estos valores públicos para acceder desde otros scripts
    public int ni, nh, no; // nodos de entrada, ocultos y salida
    public float[] ai, ah, ao; // activaciones
    public float[,] wi, wo; // pesos
    public float[,] ci, co; // cambios previos (para el momentum)
    System.Random random = new System.Random();

    // Constructor modificado
    public NeuralNetwork(int ni, int nh, int no)
    {
        this.ni = ni + 1; // +1 para el nodo de sesgo (bias)
        this.nh = nh;
        this.no = no;

        // Inicializar activaciones
        ai = new float[this.ni];
        ah = new float[this.nh];
        ao = new float[this.no];

        // Inicializar matrices de pesos con valores aleatorios
        wi = new float[this.ni, this.nh];
        wo = new float[this.nh, this.no];
        for (int i = 0; i < this.ni; i++)
            for (int j = 0; j < this.nh; j++)
                wi[i, j] = Rand(-2.0f, 2.0f);
        for (int j = 0; j < this.nh; j++)
            for (int k = 0; k < this.no; k++)
                wo[j, k] = Rand(-2.0f, 2.0f);

        // Inicializar cambios previos
        ci = new float[this.ni, this.nh];
        co = new float[this.nh, this.no];
    }

    float Rand(float a, float b) => (float)(a + (b - a) * random.NextDouble());

    float Sigmoid(float x) => (float)Math.Tanh(x);

    float DSigmoid(float y) => 1.0f - y * y;

    public float[] Update(float[] inputs)
    {
        if (inputs.Length != ni - 1)
            throw new ArgumentException("Número incorrecto de entradas.");

        // Activaciones de entrada
        for (int i = 0; i < ni - 1; i++)
            ai[i] = inputs[i];
        ai[ni - 1] = 1.0f; // Nodo de sesgo

        // Activaciones ocultas
        for (int j = 0; j < nh; j++)
        {
            float sum = 0.0f;
            for (int i = 0; i < ni; i++)
                sum += ai[i] * wi[i, j];
            ah[j] = Sigmoid(sum);
        }

        // Activaciones de salida
        for (int k = 0; k < no; k++)
        {
            float sum = 0.0f;
            for (int j = 0; j < nh; j++)
                sum += ah[j] * wo[j, k];
            ao[k] = Sigmoid(sum);
        }

        return (float[])ao.Clone();
    }

    public float BackPropagate(float[] targets, float N, float M)
    {
        // Errores de salida
        float[] output_deltas = new float[no];
        for (int k = 0; k < no; k++)
        {
            float error = targets[k] - ao[k];
            output_deltas[k] = DSigmoid(ao[k]) * error;
        }

        // Errores ocultos
        float[] hidden_deltas = new float[nh];
        for (int j = 0; j < nh; j++)
        {
            float error = 0.0f;
            for (int k = 0; k < no; k++)
                error += output_deltas[k] * wo[j, k];
            hidden_deltas[j] = DSigmoid(ah[j]) * error;
        }

        // Actualizar pesos de salida
        for (int j = 0; j < nh; j++)
            for (int k = 0; k < no; k++)
            {
                float change = output_deltas[k] * ah[j];
                wo[j, k] += N * change + M * co[j, k];
                co[j, k] = change;
            }

        // Actualizar pesos de entrada
        for (int i = 0; i < ni; i++)
            for (int j = 0; j < nh; j++)
            {
                float change = hidden_deltas[j] * ai[i];
                wi[i, j] += N * change + M * ci[i, j];
                ci[i, j] = change;
            }

        // Calcular error
        float errorTotal = 0.0f;
        for (int k = 0; k < targets.Length; k++)
            errorTotal += 0.5f * (targets[k] - ao[k]) * (targets[k] - ao[k]);
        return errorTotal;
    }

    public void Train(float[][] inputs, float[][] targets, int iterations, float N, float M)
    {
        for (int i = 0; i < iterations; i++)
        {
            float error = 0.0f;
            for (int p = 0; p < inputs.Length; p++)
            {
                Update(inputs[p]);
                error += BackPropagate(targets[p], N, M);
            }
            if (i % 100 == 0)
                Debug.Log($"Iteración {i}, Error: {error}");
        }
    }
}
