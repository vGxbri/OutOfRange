using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Componentes")]
    public AudioSource audioSource;

    [Header("Música de Menús")]
    public AudioClip musicaMenu;
    public AudioClip musicaTutorial;

    [Header("Música de Niveles")]
    public AudioClip musicaNivel1;
    public AudioClip musicaNivel2;
    public AudioClip musicaNivel3;
    public AudioClip musicaNivel4;
    public AudioClip musicaNivel5;

    [Header("Ajustes")]
    public float tiempoFade = 1.0f; // Tiempo que tarda en cambiar de canción

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // ESTA LÍNEA ES LA MAGIA:
            // Busca automáticamente el componente en el mismo objeto si se te olvidó asignarlo
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // IMPORTANTE: Asegúrate de que los nombres coincidan exactamente con tus escenas
        switch (scene.name)
        {
            case "Main_Menu":
                StartCoroutine(CambiarMusicaSuave(musicaMenu));
                break;
            case "Tutorial":
                StartCoroutine(CambiarMusicaSuave(musicaTutorial));
                break;
            case "Lvl1":
                StartCoroutine(CambiarMusicaSuave(musicaNivel1));
                break;
            case "Lvl2":
                StartCoroutine(CambiarMusicaSuave(musicaNivel2));
                break;
            case "Lvl3":
                StartCoroutine(CambiarMusicaSuave(musicaNivel3));
                break;
            case "Lvl4":
                StartCoroutine(CambiarMusicaSuave(musicaNivel4));
                break;
            case "Lvl5":
                StartCoroutine(CambiarMusicaSuave(musicaNivel5));
                break;
        }
    }

    IEnumerator CambiarMusicaSuave(AudioClip nuevaCancion)
    {
        if (audioSource.clip == nuevaCancion) yield break;

        // Fade Out (Bajar volumen)
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / tiempoFade;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = nuevaCancion;
        audioSource.Play();

        // Fade In (Subir volumen)
        while (audioSource.volume < 1)
        {
            audioSource.volume += startVolume * Time.deltaTime / tiempoFade;
            yield return null;
        }
    }
    void Update()
    {
        // Obtenemos el nombre de la escena que se está viendo ahora mismo
        string nombreEscena = SceneManager.GetActiveScene().name;

        // Si la escena es Nivel1 pero la música que suena no es la del Nivel 1... ¡Cámbiala!
        if (nombreEscena == "Nivel1" && audioSource.clip != musicaNivel1)
        {
            StartCoroutine(CambiarMusicaSuave(musicaNivel1));
        }
        // Repite esto para los demás niveles importantes si ves que fallan
    }
}