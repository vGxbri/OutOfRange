using UnityEngine;
using UnityEngine.UI;

public class ManagerOpciones : MonoBehaviour
{
    [Header("Switch Música")]
    public GameObject switchMusicaOn;
    public GameObject switchMusicaOff;

    [Header("Switch Efectos")]
    public GameObject switchEfectosOn;
    public GameObject switchEfectosOff;

    [Header("Switch Pantalla Completa")]
    public GameObject switchPantallaOn;
    public GameObject switchPantallaOff;

    private bool musicaActiva;
    private bool efectosActivos;
    private bool pantallaCompleta;

    void Start()
    {
        // Cargar preferencias guardadas (todo activo por defecto)
        musicaActiva = PlayerPrefs.GetInt("Musica", 1) == 1;
        efectosActivos = PlayerPrefs.GetInt("Efectos", 1) == 1;
        pantallaCompleta = PlayerPrefs.GetInt("PantallaCompleta", Screen.fullScreen ? 1 : 0) == 1;

        AplicarMusica();
        AplicarEfectos();
        AplicarPantallaCompleta();
        ActualizarSwitches();
    }

    // --- Métodos públicos para los botones ---

    public void ToggleMusica()
    {
        musicaActiva = !musicaActiva;
        PlayerPrefs.SetInt("Musica", musicaActiva ? 1 : 0);
        PlayerPrefs.Save();
        AplicarMusica();
        ActualizarSwitches();
    }

    public void ToggleEfectos()
    {
        efectosActivos = !efectosActivos;
        PlayerPrefs.SetInt("Efectos", efectosActivos ? 1 : 0);
        PlayerPrefs.Save();
        AplicarEfectos();
        ActualizarSwitches();
    }

    public void TogglePantallaCompleta()
    {
        pantallaCompleta = !pantallaCompleta;
        PlayerPrefs.SetInt("PantallaCompleta", pantallaCompleta ? 1 : 0);
        PlayerPrefs.Save();
        AplicarPantallaCompleta();
        ActualizarSwitches();
    }

    // --- Aplicar cambios ---

    void AplicarMusica()
    {
        // Buscar todas las AudioSources con tag "Music" o ajustar volumen global
        AudioSource[] fuentes = FindObjectsOfType<AudioSource>();
        foreach (var fuente in fuentes)
        {
            if (fuente.CompareTag("Music"))
                fuente.mute = !musicaActiva;
        }
    }

    void AplicarEfectos()
    {
        // Los efectos de sonido se comprueban antes de reproducir
        // Guardamos en PlayerPrefs y los scripts de sonido lo leen
        AudioListener.volume = efectosActivos ? 1f : 0f;

        // Si la música está activa, restaurar sus fuentes
        if (musicaActiva)
        {
            AudioSource[] fuentes = FindObjectsOfType<AudioSource>();
            foreach (var fuente in fuentes)
            {
                if (fuente.CompareTag("Music"))
                    fuente.mute = false;
            }
        }
    }

    void AplicarPantallaCompleta()
    {
        Screen.fullScreen = pantallaCompleta;
    }

    // --- Actualizar visuales ---

    void ActualizarSwitches()
    {
        if (switchMusicaOn != null) switchMusicaOn.SetActive(musicaActiva);
        if (switchMusicaOff != null) switchMusicaOff.SetActive(!musicaActiva);

        if (switchEfectosOn != null) switchEfectosOn.SetActive(efectosActivos);
        if (switchEfectosOff != null) switchEfectosOff.SetActive(!efectosActivos);

        if (switchPantallaOn != null) switchPantallaOn.SetActive(pantallaCompleta);
        if (switchPantallaOff != null) switchPantallaOff.SetActive(!pantallaCompleta);
    }

    // --- Acceso público ---
    public bool MusicaActiva() => musicaActiva;
    public bool EfectosActivos() => efectosActivos;
}
