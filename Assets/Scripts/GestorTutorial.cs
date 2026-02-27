using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GestorTutorial : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelModal;

    [Header("Textos")]
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoContenido;

    [Header("Imagen")]
    public Image imagenTutorial;
    public Sprite imagenPorDefecto;

    [Header("Animación")]
    [Range(0.1f, 1f)]
    public float duracionAnimacion = 0.3f;

    private bool estaMostrando = false; // Controla si el modal está activo

    void Update()
    {
        // Si el modal está activo y pulsamos Espacio o Enter, lo cerramos
        if (estaMostrando && panelModal.activeSelf)
        {
            // Usamos GetKeyDown para que solo responda al toque
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                CerrarModal();
            }
        }
    }

    public void MostrarModal(string titulo, string contenido, Sprite imagen = null)
    {
        estaMostrando = true;
        panelModal.SetActive(true);
        textoTitulo.text = titulo;
        textoContenido.text = contenido;

        // Configurar imagen del tutorial
        if (imagenTutorial != null)
        {
            Sprite spriteAMostrar = imagen != null ? imagen : imagenPorDefecto;
            if (spriteAMostrar != null)
            {
                imagenTutorial.sprite = spriteAMostrar;
                imagenTutorial.gameObject.SetActive(true);
            }
            else
            {
                imagenTutorial.gameObject.SetActive(false);
            }
        }

        // Animación de entrada (escala de 0 a 1)
        StopAllCoroutines();
        StartCoroutine(AnimarEntrada());

        // Pausamos el juego
        Time.timeScale = 0f;

        // Seleccionar automáticamente el propio panel o un botón imaginario (o nada si vamos por Input)
        // Para asegurar que si el juego tiene EventSystem, no se quede en un estado raro
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            // Si el modal tuviera un botón, lo seleccionaríamos aquí
            // UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(botonCerrar);
        }
    }

    public void CerrarModal()
    {
        if (!estaMostrando) return;
        estaMostrando = false;

        StopAllCoroutines();
        StartCoroutine(AnimarSalida());
    }

    private IEnumerator AnimarEntrada()
    {
        panelModal.transform.localScale = Vector3.zero;
        float tiempo = 0f;

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / duracionAnimacion;

            // Ease Out Back: da un efecto de "rebote" medieval
            float t = 1f - (1f - progreso) * (1f - progreso);
            t = t + (t - t * t) * 0.5f; // Pequeño overshoot

            panelModal.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        panelModal.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimarSalida()
    {
        float tiempo = 0f;

        while (tiempo < duracionAnimacion * 0.5f)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / (duracionAnimacion * 0.5f);
            float t = progreso * progreso; // Ease In

            panelModal.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        panelModal.transform.localScale = Vector3.one;
        panelModal.SetActive(false);

        // Reanudamos el juego
        Time.timeScale = 1f;
        
        // Limpiar selección
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}