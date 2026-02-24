using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class PauseManager : MonoBehaviour
{
    [Header("Contenedores")]
    public GameObject contenedorPausa;
    public GameObject contenedorControles;

    [Header("Transición")]
    public TransitionSettings transicion;

    private bool enPausa = false;
    private bool enControles = false;
    private BlurPausa blur;

    void Start()
    {
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        enPausa = false;
        enControles = false;

        Camera cam = Camera.main;
        if (cam != null) blur = cam.GetComponent<BlurPausa>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (enControles) VolverAPausa();
            else if (enPausa) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        enPausa = true;
        Time.timeScale = 0f;
        if (contenedorPausa != null) contenedorPausa.SetActive(true);
        if (blur != null) blur.ActivarBlur();
    }

    public void Reanudar()
    {
        enPausa = false;
        Time.timeScale = 1f;
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (blur != null) blur.DesactivarBlur();
    }

    public void MostrarControles()
    {
        if (contenedorPausa != null) contenedorPausa.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(true);
        enControles = true;
    }

    public void VolverAPausa()
    {
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorPausa != null) contenedorPausa.SetActive(true);
        enControles = false;
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
}
