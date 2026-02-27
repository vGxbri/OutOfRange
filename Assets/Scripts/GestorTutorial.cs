// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

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

    private bool estaMostrando = false;

    void Update()
    {
        if (estaMostrando && panelModal.activeSelf)
        {
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

        StopAllCoroutines();
        StartCoroutine(AnimarEntrada());

        Time.timeScale = 0f;

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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

            float t = 1f - (1f - progreso) * (1f - progreso);
            t = t + (t - t * t) * 0.5f;

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
            float t = progreso * progreso;

            panelModal.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        panelModal.transform.localScale = Vector3.one;
        panelModal.SetActive(false);

        Time.timeScale = 1f;
        
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}