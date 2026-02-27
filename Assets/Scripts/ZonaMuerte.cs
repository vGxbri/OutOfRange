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
            if (VidaCompartida.Instancia != null)
            {
                // Quitar todas las vidas para provocar el Game Over y mostrar el ContenedorMuerte
                VidaCompartida.Instancia.RecibirDaño(VidaCompartida.Instancia.ObtenerVidas());
            }
            else
            {
                // Fallback por si no hay VidaCompartida en la escena
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            // Si no es el jugador, comprobamos si es un enemigo para matarlo
            VidaEnemigo vidaEnemigo = collision.GetComponent<VidaEnemigo>();
            if (vidaEnemigo != null)
            {
                // Le hacemos daño masivo para asegurar que se ejecute su efecto de muerte
                vidaEnemigo.RecibirDaño(9999);
            }
        }
    }
}