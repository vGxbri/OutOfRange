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
        StartCoroutine(SecuenciaMuerte());
    }

    IEnumerator SecuenciaMuerte()
    {
        // Esperar a que se vea la animación de muerte
        yield return new WaitForSeconds(esperaAntesDeMostrar);

        Time.timeScale = 0f;

        if (contenedorMuerte != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            contenedorMuerte.SetActive(true);
        }

        // Fade in
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

        // Seleccionar primer botón para mando
        if (primerBoton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(primerBoton);
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
}
