using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using EasyTransition;

public class PauseManager : MonoBehaviour
{
    [Header("Contenedores")]
    public GameObject contenedorPausa;
    public GameObject contenedorControles;
    public GameObject contenedorOpciones;

    [Header("Primer botón seleccionado (para mando)")]
    public GameObject primerBotonPausa;
    public GameObject primerBotonControles;
    public GameObject primerBotonOpciones;

    [Header("Transición")]
    public TransitionSettings transicion;

    [Header("UI Fondo Blur")]
    public GameObject fondoBlurUI;

    private bool enPausa = false;
    private bool enSubMenu = false;

    void Start()
    {
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (fondoBlurUI != null) fondoBlurUI.SetActive(false);
        enPausa = false;
        enSubMenu = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (enSubMenu) VolverAPausa();
            else if (enPausa) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        enPausa = true;
        Time.timeScale = 0f;
        if (contenedorPausa != null) contenedorPausa.SetActive(true);
        if (fondoBlurUI != null) fondoBlurUI.SetActive(true);
        SeleccionarBoton(primerBotonPausa);
    }

    public void Reanudar()
    {
        enPausa = false;
        Time.timeScale = 1f;
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (fondoBlurUI != null) fondoBlurUI.SetActive(false);
    }

    public void MostrarControles()
    {
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(true);
        enSubMenu = true;
        SeleccionarBoton(primerBotonControles);
    }

    public void MostrarOpciones()
    {
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(true);
        enSubMenu = true;
        SeleccionarBoton(primerBotonOpciones);
    }

    public void VolverAPausa()
    {
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (contenedorPausa != null) contenedorPausa.SetActive(true);
        enSubMenu = false;
        SeleccionarBoton(primerBotonPausa);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        TransitionManager.Instance().Transition("Main_Menu", transicion, 0f);
    }


    public void Salir()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public bool EstaEnPausa() => enPausa;

    void SeleccionarBoton(GameObject boton)
    {
        if (boton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}
