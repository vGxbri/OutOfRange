// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

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
    public float tiempoFade = 1.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

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
        switch (scene.name)
        {
            case "Main_Menu":
                StartCoroutine(CambiarMusicaSuave(musicaMenu));
                break;
            case "Tutorial":
                StartCoroutine(CambiarMusicaSuave(musicaTutorial));
                break;
            case "(Lvl1)":
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
        if (audioSource.clip == nuevaCancion && audioSource.isPlaying) yield break;

        float startVolume = 1f;
        if (audioSource.isPlaying)
        {
            startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.unscaledDeltaTime / tiempoFade;
                yield return null;
            }
        }

        audioSource.Stop();
        audioSource.clip = nuevaCancion;
        audioSource.Play();

        audioSource.volume = 0f;
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.unscaledDeltaTime / tiempoFade;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    public void DetenerMusica()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutDetener());
    }

    IEnumerator FadeOutDetener()
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.unscaledDeltaTime / tiempoFade;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 1f;
    }
}