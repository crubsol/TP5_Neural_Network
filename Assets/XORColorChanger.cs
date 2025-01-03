using UnityEngine;

public class XORColorChanger : MonoBehaviour
{
    public GameObject cuboA;
    public GameObject cuboB;
    public GameObject esfera;

    void Start()
    {
        // Configuramos los colores iniciales de los cubos (negro representa 1, blanco representa 0)
        cuboA.GetComponent<Renderer>().material.color = Color.black;
        cuboB.GetComponent<Renderer>().material.color = Color.white;

        // Actualizamos el color de la esfera al inicio
        ActualizarColorEsfera();
    }

    void Update()
    {
        // Actualiza el color de la esfera en cada frame para reflejar los cambios en los cubos
        ActualizarColorEsfera();
    }

    void ActualizarColorEsfera()
    {
        // Obtenemos los colores de los cubos
        Color colorCuboA = cuboA.GetComponent<Renderer>().material.color;
        Color colorCuboB = cuboB.GetComponent<Renderer>().material.color;

        // Determinamos si los cubos son negros (representan "1")
        bool esNegroA = colorCuboA == Color.black;
        bool esNegroB = colorCuboB == Color.black;

        // Aplicamos la lógica XOR para cambiar el color de la esfera
        if (esNegroA ^ esNegroB) // Si uno es negro y el otro no, la esfera se vuelve negra (resultado 1)
        {
            esfera.GetComponent<Renderer>().material.color = Color.black;
        }
        else // Si ambos son iguales (ambos negros o ambos blancos), la esfera se vuelve blanca (resultado 0)
        {
            esfera.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    // Método que se ejecuta cuando hacemos clic sobre un objeto
    void OnMouseDown()
    {
        // Cambia el color del cubo que ha sido clickeado
        Renderer renderer = GetComponent<Renderer>();

        // Alterna entre negro y blanco
        if (renderer.material.color == Color.black)
        {
            renderer.material.color = Color.white;
        }
        else
        {
            renderer.material.color = Color.black;
        }

        // Actualiza el color de la esfera según la lógica XOR
        ActualizarColorEsfera();
    }
}
