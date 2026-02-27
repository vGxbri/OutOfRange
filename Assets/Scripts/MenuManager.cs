using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject contenedorBotones;
    public GameObject contenedorControles;
    public GameObject contenedorOpciones;
    public GameObject contenedorConfirmarNuevaPartida;
    public GameObject contenedorCreditos;

    [Header("Botones del menú principal")]
    public GameObject botonContinuar;
    public GameObject botonNuevaPartida;

    [Header("Transición")]
    public TransitionSettings transicion;

    private enum EstadoMenu { Principal, Controles, Opciones, ConfirmarNuevaPartida, Creditos }
    private EstadoMenu estadoActual = EstadoMenu.Principal;

    void Start()
    {
        Time.timeScale = 1f;
        OcultarTodosLosPaneles();
        MostrarMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (estadoActual == EstadoMenu.Controles || estadoActual == EstadoMenu.Opciones ||
                estadoActual == EstadoMenu.ConfirmarNuevaPartida || estadoActual == EstadoMenu.Creditos)
            {
                MostrarMenu();
            }
        }
    }

    public void MostrarMenu()
    {
        contenedorBotones.SetActive(true);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (contenedorConfirmarNuevaPartida != null) contenedorConfirmarNuevaPartida.SetActive(false);
        if (contenedorCreditos != null) contenedorCreditos.SetActive(false); // <-- AÑADIDO
        estadoActual = EstadoMenu.Principal;

        if (botonContinuar != null)
        {
            botonContinuar.SetActive(GestorGuardado.HayPartidaGuardada());
        }

        if (botonNuevaPartida != null)
        {
            botonNuevaPartida.SetActive(true);
        }

        // Seleccionar primer botón para mando/teclado con un pequeño retraso
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            if (botonContinuar != null && botonContinuar.activeSelf)
            {
                StartCoroutine(SeleccionarBotonConRetraso(botonContinuar));
            }
            else if (botonNuevaPartida != null && botonNuevaPartida.activeSelf)
            {
                StartCoroutine(SeleccionarBotonConRetraso(botonNuevaPartida));
            }
        }
    }

    public void MostrarControles()
    {
        contenedorBotones.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(true);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (contenedorConfirmarNuevaPartida != null) contenedorConfirmarNuevaPartida.SetActive(false);
        if (contenedorCreditos != null) contenedorCreditos.SetActive(false);
        estadoActual = EstadoMenu.Controles;

        // Aquí podrías seleccionar el primer botón del panel de controles si lo tuvieras referenciado,
        // por ejemplo un botón de "Volver"
        if (UnityEngine.EventSystems.EventSystem.current != null && contenedorControles != null)
        {
            UnityEngine.UI.Button primerBoton = contenedorControles.GetComponentInChildren<UnityEngine.UI.Button>();
            if (primerBoton != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(primerBoton.gameObject));
            }
        }
    }

    public void MostrarOpciones()
    {
        contenedorBotones.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(true);
        if (contenedorConfirmarNuevaPartida != null) contenedorConfirmarNuevaPartida.SetActive(false);
        if (contenedorCreditos != null) contenedorCreditos.SetActive(false);
        estadoActual = EstadoMenu.Opciones;

        // Aquí podrías seleccionar el primer botón del panel de opciones si lo tuvieras referenciado
        if (UnityEngine.EventSystems.EventSystem.current != null && contenedorOpciones != null)
        {
            UnityEngine.UI.Button primerBoton = contenedorOpciones.GetComponentInChildren<UnityEngine.UI.Button>();
            if (primerBoton != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(primerBoton.gameObject));
            }
        }
    }

    public void MostrarCreditos()
    {
        contenedorBotones.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (contenedorConfirmarNuevaPartida != null) contenedorConfirmarNuevaPartida.SetActive(false);

        // Activamos el panel de créditos
        if (contenedorCreditos != null) contenedorCreditos.SetActive(true);

        estadoActual = EstadoMenu.Creditos;

        // Aquí podrías seleccionar el primer botón del panel de créditos si lo tuvieras referenciado
        if (UnityEngine.EventSystems.EventSystem.current != null && contenedorCreditos != null)
        {
            UnityEngine.UI.Button primerBoton = contenedorCreditos.GetComponentInChildren<UnityEngine.UI.Button>();
            if (primerBoton != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(primerBoton.gameObject));
            }
        }
    }

    // --- BOTÓN "NUEVA PARTIDA" ---
    public void OnNuevaPartida()
    {
        if (GestorGuardado.HayPartidaGuardada())
        {
            // Hay partida guardada → preguntar confirmación
            contenedorBotones.SetActive(false);
            contenedorConfirmarNuevaPartida.SetActive(true);
            estadoActual = EstadoMenu.ConfirmarNuevaPartida;

            // Seleccionar primer botón para mando/teclado en confirmación (ej. botón Cancelar o Confirmar)
            // Asumiendo que podemos buscarlo en los hijos o simplemente seleccionamos el primero interactable
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.UI.Button primerBotonConfirmacion = contenedorConfirmarNuevaPartida.GetComponentInChildren<UnityEngine.UI.Button>();
                if (primerBotonConfirmacion != null)
                {
                    StartCoroutine(SeleccionarBotonConRetraso(primerBotonConfirmacion.gameObject));
                }
            }
        }
        else
        {
            // No hay partida → ir directo al tutorial
            IniciarNuevaPartida();
        }
    }

    // --- BOTONES DEL ContenedorConfirmarNuevaPartida ---
    public void ConfirmarNuevaPartida()
    {
        // El jugador dijo "Sí" → borrar y empezar
        GestorGuardado.BorrarPartida();
        IniciarNuevaPartida();
    }

    public void CancelarNuevaPartida()
    {
        // El jugador dijo "No" → volver al menú
        MostrarMenu();
    }

    // --- BOTÓN "CONTINUAR" ---
    public void OnContinuar()
    {
        string escena = GestorGuardado.ObtenerEscenaGuardada();
        TransitionManager.Instance().Transition(escena, transicion, 0f);
    }

    // --- BOTÓN "SALIR" ---
    public void Salir()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // --- UTILIDAD ---
    private void OcultarTodosLosPaneles()
    {
        if (contenedorBotones != null) contenedorBotones.SetActive(false);
        if (contenedorControles != null) contenedorControles.SetActive(false);
        if (contenedorOpciones != null) contenedorOpciones.SetActive(false);
        if (contenedorConfirmarNuevaPartida != null) contenedorConfirmarNuevaPartida.SetActive(false);
        if (contenedorCreditos != null) contenedorCreditos.SetActive(false);
    }

    void IniciarNuevaPartida()
    {
        GestorGuardado.GuardarProgreso("Tutorial", 0);
        TransitionManager.Instance().Transition("Tutorial", transicion, 0f);
    }

    private System.Collections.IEnumerator SeleccionarBotonConRetraso(GameObject boton)
    {
        // Esperamos un frame para asegurarnos que la UI se ha activado por completo
        // y el EventSystem ha limpiado el estado anterior.
        yield return null;
        if (UnityEngine.EventSystems.EventSystem.current != null && boton != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}
