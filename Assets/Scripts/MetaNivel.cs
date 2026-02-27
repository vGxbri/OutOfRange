// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

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

    [Header("Efectos Visuales")]
    public GameObject animacionFondoDisponible;
    private bool animacionActivada = false;
    
    void Start()
    {
        tiempoInicio = Time.time;
        if (animacionFondoDisponible != null)
        {
            animacionFondoDisponible.SetActive(false);
        }
    }

    void Update()
    {
        if (!animacionActivada && animacionFondoDisponible != null)
        {
            VidaEnemigo[] todosLosEnemigos = FindObjectsOfType<VidaEnemigo>();
            bool quedanVivos = false;

            foreach (var e in todosLosEnemigos)
            {
                if (e != null && !e.EstaMuerto())
                {
                    quedanVivos = true;
                    break;
                }
            }

            if (!quedanVivos && Time.time - tiempoInicio > 1f)
            {
                animacionActivada = true;
                animacionFondoDisponible.SetActive(true);
                
                Animator animatorFondo = animacionFondoDisponible.GetComponent<Animator>();
                if (animatorFondo != null)
                {
                    animatorFondo.enabled = true;
                }
                
                Debug.Log("Todos los enemigos derrotados: Animación de la meta activada.");
            }
        }
    }

    public void ActivarMeta(GameObject atacante)
    {
        if (nivelTerminado) return;

        if (Time.time - tiempoInicio < 0.2f) 
        {
            Debug.LogWarning("MetaNivel ignorado por seguridad (demasiado pronto).");
            return;
        }

        VidaEnemigo[] todosLosEnemigos = FindObjectsOfType<VidaEnemigo>();
        int enemigosVivos = 0;
        
        foreach (var e in todosLosEnemigos)
        {
            if (e != null && !e.EstaMuerto())
            {
                enemigosVivos++;
            }
        }

        if (enemigosVivos > 0)
        {
            Debug.Log($"<color=yellow>¡Aún quedan {enemigosVivos} enemigos!</color> Debes derrotarlos a todos antes de pasar.");
            return;
        }

        Debug.Log("MetaNivel activada por un ataque de: " + atacante.name + " en pos: " + transform.position);
        nivelTerminado = true;

        MenuNivelCompletado menuCompletado = FindObjectOfType<MenuNivelCompletado>();
        if (menuCompletado != null)
        {
            menuCompletado.MostrarMenuCompletado();
            return;
        }

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