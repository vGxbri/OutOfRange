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

    // --- METODO MODIFICADO: Ahora se llama cuando un jugador "ataca" la piedra ---
    public void ActivarMeta(GameObject atacante)
    {
        if (nivelTerminado) return;

        // Evitar disparos accidentales al spawnear (ej: si el ataque ocurre muy pronto)
        if (Time.time - tiempoInicio < 0.2f) 
        {
            Debug.LogWarning("MetaNivel ignorado por seguridad (demasiado pronto).");
            return;
        }

        // --- LÓGICA: COMPROBAR ENEMIGOS ---
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

        Debug.Log("MetaNivel activada por un ataque de: " + atacante.name + " en pos: " + transform.position);
        nivelTerminado = true;

        // --- LÓGICA: MOSTRAR UI DE NIVEL COMPLETADO ---
        MenuNivelCompletado menuCompletado = FindObjectOfType<MenuNivelCompletado>();
        if (menuCompletado != null)
        {
            // Si existe el menú en la escena, le decimos que se muestre y él se encarga de cargar niveles
            menuCompletado.MostrarMenuCompletado();
            return;
        }

        // --- LÓGICA ANTIGUA (Fallback si no hay menú de nivel completado en la escena) ---
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