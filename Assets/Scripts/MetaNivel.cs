using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class MetaNivel : MonoBehaviour
{
    private bool nivelTerminado = false;

    [Header("Configuración de Transición")]
    public TransitionSettings transition;
    public float delayCarga = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo actuamos si el Jugador toca la piedra y el nivel no ha acabado ya
        if (collision.CompareTag("Player") && !nivelTerminado)
        {
            nivelTerminado = true;

            int proximoIndice = SceneManager.GetActiveScene().buildIndex + 1;
            int totalEscenas = SceneManager.sceneCountInBuildSettings;

            TransitionManager tm = TransitionManager.Instance();
            if (tm == null)
            {
                tm = FindObjectOfType<TransitionManager>();
                if (tm == null)
                {
                    Debug.LogWarning("No se encontró TransitionManager. Cargando escena directamente.");
                    if (proximoIndice < totalEscenas)
                        SceneManager.LoadScene(proximoIndice);
                    else
                        SceneManager.LoadScene("Main_Menu");
                    return;
                }
            }

            if (proximoIndice < totalEscenas)
            {
                tm.Transition(proximoIndice, transition, delayCarga);
            }
            else
            {
                tm.Transition("Main_Menu", transition, delayCarga);
            }
        }
    }
}