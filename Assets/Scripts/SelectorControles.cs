using UnityEngine;
using UnityEngine.UI;

public class SelectorControles : MonoBehaviour
{
    [Header("Jugador 1")]
    public Image imgTecladoJ1;
    public Image imgMandoJ1;

    [Header("Jugador 2")]
    public Image imgTecladoJ2;
    public Image imgMandoJ2;

    [Header("Indicadores de Selección (esquinas)")]
    public RectTransform[] esquinasTecladoJ1; // 4 esquinas: TL, TR, BL, BR
    public RectTransform[] esquinasMandoJ1;
    public RectTransform[] esquinasTecladoJ2;
    public RectTransform[] esquinasMandoJ2;

    [Header("Configuración Visual")]
    public float velocidadAnimacion = 8f;
    public float escalaActiva = 1f;
    public float escalaInactiva = 0.5f;

    // 0 = Teclado, 1 = Mando
    private int seleccionJ1 = 0;
    private int seleccionJ2 = 0;

    // Escala actual para animación suave
    private float[] escalaActualTecladoJ1 = new float[4];
    private float[] escalaActualMandoJ1 = new float[4];
    private float[] escalaActualTecladoJ2 = new float[4];
    private float[] escalaActualMandoJ2 = new float[4];

    void Start()
    {
        // Cargar selección guardada (0 = teclado por defecto)
        seleccionJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        seleccionJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        // Inicializar escalas inmediatamente (sin animación)
        InicializarEscalas();
        AplicarSeleccion();
    }

    void Update()
    {
        // Animar esquinas suavemente
        AnimarEsquinas(esquinasTecladoJ1, escalaActualTecladoJ1, seleccionJ1 == 0);
        AnimarEsquinas(esquinasMandoJ1, escalaActualMandoJ1, seleccionJ1 == 1);
        AnimarEsquinas(esquinasTecladoJ2, escalaActualTecladoJ2, seleccionJ2 == 0);
        AnimarEsquinas(esquinasMandoJ2, escalaActualMandoJ2, seleccionJ2 == 1);
    }

    void InicializarEscalas()
    {
        for (int i = 0; i < 4; i++)
        {
            escalaActualTecladoJ1[i] = seleccionJ1 == 0 ? escalaActiva : escalaInactiva;
            escalaActualMandoJ1[i] = seleccionJ1 == 1 ? escalaActiva : escalaInactiva;
            escalaActualTecladoJ2[i] = seleccionJ2 == 0 ? escalaActiva : escalaInactiva;
            escalaActualMandoJ2[i] = seleccionJ2 == 1 ? escalaActiva : escalaInactiva;
        }
    }

    void AnimarEsquinas(RectTransform[] esquinas, float[] escalasActuales, bool activo)
    {
        if (esquinas == null) return;

        float objetivo = activo ? escalaActiva : escalaInactiva;
        float alphaObjetivo = activo ? 1f : 0f;

        for (int i = 0; i < esquinas.Length && i < 4; i++)
        {
            if (esquinas[i] == null) continue;

            // Animar escala
            escalasActuales[i] = Mathf.Lerp(escalasActuales[i], objetivo, Time.unscaledDeltaTime * velocidadAnimacion);
            esquinas[i].localScale = Vector3.one * escalasActuales[i];

            // Animar color (solo opacidad)
            Image img = esquinas[i].GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = Mathf.Lerp(c.a, alphaObjetivo, Time.unscaledDeltaTime * velocidadAnimacion);
                img.color = c;
            }
        }
    }

    void AplicarSeleccion()
    {
        // Ajustar opacidad de las imágenes principales
        SetAlpha(imgTecladoJ1, seleccionJ1 == 0 ? 1f : 0.4f);
        SetAlpha(imgMandoJ1, seleccionJ1 == 1 ? 1f : 0.4f);
        SetAlpha(imgTecladoJ2, seleccionJ2 == 0 ? 1f : 0.4f);
        SetAlpha(imgMandoJ2, seleccionJ2 == 1 ? 1f : 0.4f);
    }

    void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    // --- Métodos públicos para los botones ---

    public void SeleccionarTecladoJ1()
    {
        seleccionJ1 = 0;
        GuardarYAplicar();
    }

    public void SeleccionarMandoJ1()
    {
        seleccionJ1 = 1;
        GuardarYAplicar();
    }

    public void SeleccionarTecladoJ2()
    {
        seleccionJ2 = 0;
        GuardarYAplicar();
    }

    public void SeleccionarMandoJ2()
    {
        seleccionJ2 = 1;
        GuardarYAplicar();
    }

    void GuardarYAplicar()
    {
        PlayerPrefs.SetInt("ControlJ1", seleccionJ1);
        PlayerPrefs.SetInt("ControlJ2", seleccionJ2);
        PlayerPrefs.Save();
        AplicarSeleccion();
    }

    // --- Acceso público para otros scripts ---

    public bool Jugador1UsaMando() => seleccionJ1 == 1;
    public bool Jugador2UsaMando() => seleccionJ2 == 1;
    public int GetSeleccionJ1() => seleccionJ1;
    public int GetSeleccionJ2() => seleccionJ2;
}
