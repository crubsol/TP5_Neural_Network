using UnityEngine;

public class CuboController : MonoBehaviour
{
    public XORTrainer xorTrainer;  // Referencia al script XORTrainer para actualizar la esfera
    private int estadoColor = 0;   // 0 para blanco, 1 para negro

    void Start()
    {
        // Asegura que el cubo comience en blanco
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnMouseDown()
    {
        // Alternar el estado de color
        estadoColor = 1 - estadoColor;

        // Cambiar el color del cubo según el estado
        GetComponent<Renderer>().material.color = (estadoColor == 1) ? Color.black : Color.white;

        // Notificar al XORTrainer para actualizar las esferas de salida
        if (xorTrainer != null)
        {
            xorTrainer.ActualizarColorEsferas();
        }
    }

    // Método para obtener el estado como valor entero (0 o 1)
    public float ObtenerEstadoColor()
    {
        return (float)estadoColor;
    }

}
