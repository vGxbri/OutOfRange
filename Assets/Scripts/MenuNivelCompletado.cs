using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition; // Asegúrate de tener el paquete de transiciones
using UnityEngine.EventSystems; // Para limpiar selección de UI y soporte de mandos

public class MenuNivelCompletado : MonoBehaviour
{
    [Header("UI del Nivel Completado")]
    public GameObject contenedorPrincipal; // Asigna aquí el ContenedorNivelCompletado
    public GameObject botonSiguienteNivel; // El botón para que se seleccione automáticamente (Mando/Teclado)

    [Header("Efecto de Aparición (Fade-In)")]
    public bool usarFadeIn = true;
    public float tiempoFadeIn = 0.5f;

    [Header("Configuración de Transición")]
    public TransitionSettings transicion;
    public float delayCarga = 0.5f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoVictoria;

    private bool nivelCompletado = false;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Nos aseguramos de que el contenedor esté oculto al iniciar el nivel
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.SetActive(false);

            if (usarFadeIn)
            {
                // Obtenemos o añadimos automáticamente el CanvasGroup para controlar la transparencia
                canvasGroup = contenedorPrincipal.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = contenedorPrincipal.AddComponent<CanvasGroup>();
                }
                
                // Empezamos totalmente transparentes
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void MostrarMenuCompletado()
    {
        if (nivelCompletado) return;
        nivelCompletado = true;

        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.SetActive(true);

            if (usarFadeIn && canvasGroup != null)
            {
                StartCoroutine(FadeInRutina());
            }

            // Reproducir sonido de victoria
            if (audioSource != null && sonidoVictoria != null)
            {
                audioSource.PlayOneShot(sonidoVictoria);
            }

            // Seleccionar botón por defecto para la navegación con mando o teclado
            if (EventSystem.current != null && botonSiguienteNivel != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(botonSiguienteNivel));
            }
        }
    }

    private IEnumerator FadeInRutina()
    {
        float tiempoPasado = 0f;
        while (tiempoPasado < tiempoFadeIn)
        {
            tiempoPasado += Time.unscaledDeltaTime; // Por si el juego llegase a pausarse
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, tiempoPasado / tiempoFadeIn);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // --- MÉTODOS PARA LOS BOTONES ---

    public void SiguienteNivel()
    {
        Time.timeScale = 1f; // Restaurar el tiempo por si acaso lo habías pausado

        int proximoIndice = SceneManager.GetActiveScene().buildIndex + 1;
        int totalEscenas = SceneManager.sceneCountInBuildSettings;

        TransitionManager tm = TransitionManager.Instance();
        if (tm == null)
        {
            tm = FindObjectOfType<TransitionManager>();
        }

        if (proximoIndice < totalEscenas)
        {
            if (tm != null && transicion != null)
            {
                tm.Transition(proximoIndice, transicion, delayCarga);
            }
            else
            {
                SceneManager.LoadScene(proximoIndice);
            }
        }
        else
        {
            // Si ya no hay más niveles (es el último), volver al menú principal
            MenuPrincipal();
        }
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f; // Restaurar el tiempo

        TransitionManager tm = TransitionManager.Instance();
        if (tm == null)
        {
            tm = FindObjectOfType<TransitionManager>();
        }

        if (tm != null && transicion != null)
        {
            tm.Transition("Main_Menu", transicion, delayCarga);
        }
        else
        {
            SceneManager.LoadScene("Main_Menu");
        }
    }

    private System.Collections.IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        yield return null;
        if (EventSystem.current != null && boton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}
