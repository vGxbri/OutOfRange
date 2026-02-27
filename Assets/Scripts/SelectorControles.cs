// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectorControles : MonoBehaviour
{
    [Header("Jugador 1")]
    public Image imgTecladoJ1;
    public Image imgMandoJ1;

    [Header("Jugador 2")]
    public Image imgTecladoJ2;
    public Image imgMandoJ2;

    [Header("Indicadores de Selección (esquinas)")]
    public RectTransform[] esquinasTecladoJ1;
    public RectTransform[] esquinasMandoJ1;
    public RectTransform[] esquinasTecladoJ2;
    public RectTransform[] esquinasMandoJ2;

    [Header("Configuración Visual")]
    public float velocidadAnimacion = 8f;
    public float escalaActiva = 1f;
    public float escalaInactiva = 0.5f;

    private int seleccionJ1 = 0;
    private int seleccionJ2 = 0;

    private float[] escalaActualTecladoJ1 = new float[4];
    private float[] escalaActualMandoJ1 = new float[4];
    private float[] escalaActualTecladoJ2 = new float[4];
    private float[] escalaActualMandoJ2 = new float[4];

    void Start()
    {
        seleccionJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        seleccionJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        InicializarEscalas();
        AplicarSeleccion();
    }

    void Update()
    {
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

            escalasActuales[i] = Mathf.Lerp(escalasActuales[i], objetivo, Time.unscaledDeltaTime * velocidadAnimacion);
            esquinas[i].localScale = Vector3.one * escalasActuales[i];

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
        AplicarControlesAJugadores();
    }

    void AplicarControlesAJugadores()
    {
        PlayerInput[] jugadores = FindObjectsOfType<PlayerInput>();
        foreach (var pi in jugadores)
        {
            int indice = pi.playerIndex;
            int control = (indice == 0) ? seleccionJ1 : seleccionJ2;
            MovimientoJugador mov = pi.GetComponent<MovimientoJugador>();

            if (control == 1)
            {
                if (Gamepad.current != null)
                {
                    pi.SwitchCurrentControlScheme("Mando", Gamepad.current);
                    if (mov != null) mov.enabled = true;
                }
                else
                {
                    if (mov != null) mov.enabled = false;
                    Debug.Log($"Jugador {indice + 1}: esperando mando...");
                }
            }
            else
            {
                if (Keyboard.current != null)
                {
                    bool ambosUsanTeclado = (seleccionJ1 == 0 && seleccionJ2 == 0);
                    string esquema;
                    if (ambosUsanTeclado)
                        esquema = (indice == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                    else
                        esquema = "Teclado_Solo";
                    pi.SwitchCurrentControlScheme(esquema, Keyboard.current);
                    if (mov != null) mov.enabled = true;
                }
            }
        }
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange -= OnDispositivoCambiado;
        InputSystem.onDeviceChange += OnDispositivoCambiado;
    }

    void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDispositivoCambiado;
    }

    void OnDispositivoCambiado(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad && change == InputDeviceChange.Added)
        {
            Debug.Log("Mando conectado, aplicando controles...");
            AplicarControlesAJugadores();
        }
    }

    public bool Jugador1UsaMando() => seleccionJ1 == 1;
    public bool Jugador2UsaMando() => seleccionJ2 == 1;
    public int GetSeleccionJ1() => seleccionJ1;
    public int GetSeleccionJ2() => seleccionJ2;
}
