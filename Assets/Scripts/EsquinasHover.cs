// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EsquinasHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Esquinas (TL, TR, BL, BR)")]
    public GameObject[] esquinas;

    [Header("Configuración")]
    public float duracionFade = 0.12f;

    private Coroutine fadeActual;

    void Start()
    {
        Ocultar(instantaneo: true);
    }

    void OnEnable()
    {
        Ocultar(instantaneo: true);
    }

    void Ocultar(bool instantaneo)
    {
        if (fadeActual != null) StopCoroutine(fadeActual);

        if (instantaneo)
        {
            foreach (var esq in esquinas)
            {
                if (esq == null) continue;
                esq.SetActive(false);
            }
        }
        else
        {
            fadeActual = StartCoroutine(FadeOut());
        }
    }

    void Mostrar()
    {
        if (fadeActual != null) StopCoroutine(fadeActual);
        foreach (var esq in esquinas)
        {
            if (esq == null) continue;
            esq.SetActive(true);
            Image img = esq.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }
        fadeActual = StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(t / duracionFade);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.unscaledDeltaTime;
            float alpha = 1f - Mathf.Clamp01(t / duracionFade);
            SetAlpha(alpha);
            yield return null;
        }
        foreach (var esq in esquinas)
        {
            if (esq != null) esq.SetActive(false);
        }
    }

    void SetAlpha(float alpha)
    {
        foreach (var esq in esquinas)
        {
            if (esq == null) continue;
            Image img = esq.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => Mostrar();
    public void OnPointerExit(PointerEventData eventData) => Ocultar(instantaneo: false);
    public void OnSelect(BaseEventData eventData) => Mostrar();
    public void OnDeselect(BaseEventData eventData) => Ocultar(instantaneo: false);
}
