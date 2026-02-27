// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

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
        if (contenedorCreditos != null) contenedorCreditos.SetActive(false);
        estadoActual = EstadoMenu.Principal;

        if (botonContinuar != null)
        {
            botonContinuar.SetActive(GestorGuardado.HayPartidaGuardada());
        }

        if (botonNuevaPartida != null)
        {
            botonNuevaPartida.SetActive(true);
        }

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

        if (contenedorCreditos != null) contenedorCreditos.SetActive(true);

        estadoActual = EstadoMenu.Creditos;

        if (UnityEngine.EventSystems.EventSystem.current != null && contenedorCreditos != null)
        {
            UnityEngine.UI.Button primerBoton = contenedorCreditos.GetComponentInChildren<UnityEngine.UI.Button>();
            if (primerBoton != null)
            {
                StartCoroutine(SeleccionarBotonConRetraso(primerBoton.gameObject));
            }
        }
    }

    public void OnNuevaPartida()
    {
        if (GestorGuardado.HayPartidaGuardada())
        {
            contenedorBotones.SetActive(false);
            contenedorConfirmarNuevaPartida.SetActive(true);
            estadoActual = EstadoMenu.ConfirmarNuevaPartida;

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
            IniciarNuevaPartida();
        }
    }

    public void ConfirmarNuevaPartida()
    {
        GestorGuardado.BorrarPartida();
        IniciarNuevaPartida();
    }

    public void CancelarNuevaPartida()
    {
        MostrarMenu();
    }

    public void OnContinuar()
    {
        string escena = GestorGuardado.ObtenerEscenaGuardada();
        TransitionManager.Instance().Transition(escena, transicion, 0f);
    }

    public void Salir()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif !UNITY_WEBGL
        Application.Quit();
        #else
        Debug.Log("Salir deshabilitado en WebGL");
        #endif
    }

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
        yield return null;
        if (UnityEngine.EventSystems.EventSystem.current != null && boton != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(boton);
        }
    }
}
