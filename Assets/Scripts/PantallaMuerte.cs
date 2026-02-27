// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PantallaMuerte : MonoBehaviour
{
    [Header("UI")]
    public GameObject contenedorMuerte;
    public GameObject primerBoton;

    [Header("Fade In")]
    public float duracionFadeIn = 1f;
    public float esperaAntesDeMostrar = 1.5f;
    
    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoMuerte;

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (contenedorMuerte != null)
        {
            contenedorMuerte.SetActive(false);
            canvasGroup = contenedorMuerte.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = contenedorMuerte.AddComponent<CanvasGroup>();
        }

        if (VidaCompartida.Instancia != null)
            VidaCompartida.Instancia.OnGameOver += MostrarPantallaMuerte;
    }

    void OnDestroy()
    {
        if (VidaCompartida.Instancia != null)
            VidaCompartida.Instancia.OnGameOver -= MostrarPantallaMuerte;
    }

    void MostrarPantallaMuerte()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.DetenerMusica();
        }
        StartCoroutine(SecuenciaMuerte());
    }

    IEnumerator SecuenciaMuerte()
    {
        yield return new WaitForSeconds(esperaAntesDeMostrar);

        if (audioSource != null && sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        Time.timeScale = 0f;

        if (contenedorMuerte != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            contenedorMuerte.SetActive(true);
        }

        float timer = 0f;
        while (timer < duracionFadeIn)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = timer / duracionFadeIn;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (primerBoton != null && EventSystem.current != null)
        {
            StartCoroutine(SeleccionarBotonConRetraso(primerBoton));
        }
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Reiniciando nivel. Escena actual: " + currentScene);
        VidaCompartida.Instancia?.Reiniciar();
        SceneManager.LoadScene(currentScene);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_Menu");
    }

    private IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        yield return null;
        if (EventSystem.current != null && boton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}
