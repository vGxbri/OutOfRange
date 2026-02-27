// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;

public class DisparadorTutorial : MonoBehaviour
{
    public string titulo;
    [TextArea] public string mensaje;
    public Sprite imagenTutorial;
    private bool yaSeUso = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !yaSeUso)
        {
            FindObjectOfType<GestorTutorial>().MostrarModal(titulo, mensaje, imagenTutorial);
            yaSeUso = true;
        }
    }
}