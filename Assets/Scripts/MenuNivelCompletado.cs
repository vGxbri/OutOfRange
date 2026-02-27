// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition; // Asegúrate de tener el paquete de transiciones
using UnityEngine.EventSystems; // Para limpiar selección de UI y soporte de mandos

public class MenuNivelCompletado : MonoBehaviour
{
    [Header("UI del Nivel Completado")]
    public GameObject contenedorPrincipal;
    public GameObject botonSiguienteNivel;

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
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.SetActive(false);

            if (usarFadeIn)
            {
                canvasGroup = contenedorPrincipal.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = contenedorPrincipal.AddComponent<CanvasGroup>();
                }
                
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

            if (audioSource != null && sonidoVictoria != null)
            {
                audioSource.PlayOneShot(sonidoVictoria);
            }

            if (EventSystem.current != null && botonSiguienteNivel != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(botonSiguienteNivel));
            }

            int proximoIndice = SceneManager.GetActiveScene().buildIndex + 1;
            int totalEscenas = SceneManager.sceneCountInBuildSettings;

            if (proximoIndice < totalEscenas)
            {
                string proximaEscenaRuta = SceneUtility.GetScenePathByBuildIndex(proximoIndice);
                string nombreProximaEscena = System.IO.Path.GetFileNameWithoutExtension(proximaEscenaRuta);
                
                GestorGuardado.GuardarProgreso(nombreProximaEscena, proximoIndice);
                Debug.Log($"Nivel completado. Guardando progreso para el siguiente nivel: {nombreProximaEscena} (Índice {proximoIndice})");
            }
            else
            {
                Debug.Log("Último nivel completado. No hay más niveles por delante.");
            }
        }
    }

    private IEnumerator FadeInRutina()
    {
        float tiempoPasado = 0f;
        while (tiempoPasado < tiempoFadeIn)
        {
            tiempoPasado += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, tiempoPasado / tiempoFadeIn);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;

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
            MenuPrincipal();
        }
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f;

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
