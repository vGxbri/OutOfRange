using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICorazones : MonoBehaviour
{
    [Header("Corazones Llenos (ordenados del 1 al 5)")]
    public GameObject[] corazonesLlenos; // Arrastra los objetos corazon_lleno aquí

    [Header("Efecto Parpadeo")]
    public int vecesParpadeo = 4;
    public float velocidadParpadeo = 0.1f;

    private int vidasAnterior;
    private Image[] imagenesCorazones;

    void Start()
    {
        // Obtener componentes Image de cada GameObject
        imagenesCorazones = new Image[corazonesLlenos.Length];
        for (int i = 0; i < corazonesLlenos.Length; i++)
        {
            if (corazonesLlenos[i] != null)
                imagenesCorazones[i] = corazonesLlenos[i].GetComponent<Image>();
        }

        if (VidaCompartida.Instancia != null)
        {
            vidasAnterior = VidaCompartida.Instancia.ObtenerVidas();
            VidaCompartida.Instancia.OnVidaCambiada += ActualizarCorazones;
            ActualizarCorazones(vidasAnterior);
        }
    }

    void OnDestroy()
    {
        if (VidaCompartida.Instancia != null)
            VidaCompartida.Instancia.OnVidaCambiada -= ActualizarCorazones;
    }

    void ActualizarCorazones(int vidasActuales)
    {
        for (int i = 0; i < imagenesCorazones.Length; i++)
        {
            if (imagenesCorazones[i] == null) continue;

            if (i < vidasActuales)
            {
                imagenesCorazones[i].enabled = true;
            }
            else if (i >= vidasActuales && i < vidasAnterior)
            {
                StartCoroutine(ParpadeoCorazon(imagenesCorazones[i]));
            }
            else
            {
                imagenesCorazones[i].enabled = false;
            }
        }

        vidasAnterior = vidasActuales;
    }

    IEnumerator ParpadeoCorazon(Image corazon)
    {
        for (int i = 0; i < vecesParpadeo; i++)
        {
            corazon.enabled = false;
            yield return new WaitForSecondsRealtime(velocidadParpadeo);
            corazon.enabled = true;
            yield return new WaitForSecondsRealtime(velocidadParpadeo);
        }
        corazon.enabled = false;
    }
}
