// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (VidaCompartida.Instancia != null)
            {
                VidaCompartida.Instancia.RecibirDaño(VidaCompartida.Instancia.ObtenerVidas());
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            VidaEnemigo vidaEnemigo = collision.GetComponent<VidaEnemigo>();
            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(9999);
            }
        }
    }
}