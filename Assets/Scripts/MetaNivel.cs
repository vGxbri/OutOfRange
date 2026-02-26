using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class MetaNivel : MonoBehaviour
{
    private bool nivelTerminado = false;

    [Header("Configuración de Transición")]
    public TransitionSettings transition;
    public float delayCarga = 0.5f;

    private float tiempoInicio;
    
    void Start()
    {
        tiempoInicio = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo actuamos si el Jugador toca la piedra y el nivel no ha acabado ya
        if (collision.CompareTag("Player") && !nivelTerminado)
        {
            // Evitar disparos accidentales al spawnear (ej: si el trigger está muy cerca del inicio)
            if (Time.time - tiempoInicio < 0.2f) 
            {
                Debug.LogWarning("MetaNivel ignorado por seguridad (demasiado pronto).");
                return;
            }

            // --- NUEVA LÓGICA: COMPROBAR ENEMIGOS ---
            VidaEnemigo[] todosLosEnemigos = FindObjectsOfType<VidaEnemigo>();
            int enemigosVivos = 0;
            
            foreach (var e in todosLosEnemigos)
            {
                // Solo contamos los que no están muertos ni en proceso de morir
                if (e != null && !e.EstaMuerto())
                {
                    enemigosVivos++;
                }
            }

            if (enemigosVivos > 0)
            {
                Debug.Log($"<color=yellow>¡Aún quedan {enemigosVivos} enemigos!</color> Debes derrotarlos a todos antes de pasar.");
                // Opcional: Aquí podrías activar un mensaje en pantalla para el usuario
                return;
            }

            Debug.Log("MetaNivel activada por: " + collision.gameObject.name + " en pos: " + transform.position);
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