using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar el nivel

public class ZonaMuerte : MonoBehaviour
{
    // Se activa cuando algo entra en el colisionador invisible
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si lo que cayó es el Jugador
        if (collision.CompareTag("Player"))
        {
            // Reinicia la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // Si tienes un sistema de vidas o respawn, 
            // aquí llamarías a la función de "PerderVida()"
        }
    }
}