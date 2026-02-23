using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject contenedorBotones;
    public GameObject contenedorControles;
    public GameObject contenedorNuevaPartida;

    [Header("Botones del menú principal")]
    public GameObject botonContinuar;

    private enum EstadoMenu { Principal, Controles, ConfirmarNuevaPartida }
    private EstadoMenu estadoActual = EstadoMenu.Principal;

    void Start()
    {
        MostrarMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (estadoActual == EstadoMenu.Controles || estadoActual == EstadoMenu.ConfirmarNuevaPartida)
            {
                MostrarMenu();
            }
        }
    }

    public void MostrarMenu()
    {
        contenedorBotones.SetActive(true);
        contenedorControles.SetActive(false);
        if (contenedorNuevaPartida != null) contenedorNuevaPartida.SetActive(false);
        estadoActual = EstadoMenu.Principal;

        // Mostrar u ocultar el botón de continuar según si hay partida guardada
        if (botonContinuar != null)
        {
            botonContinuar.SetActive(GestorGuardado.HayPartidaGuardada());
        }
    }

    public void MostrarControles()
    {
        contenedorBotones.SetActive(false);
        contenedorControles.SetActive(true);
        if (contenedorNuevaPartida != null) contenedorNuevaPartida.SetActive(false);
        estadoActual = EstadoMenu.Controles;
    }

    // --- BOTÓN "NUEVA PARTIDA" ---
    public void OnNuevaPartida()
    {
        if (GestorGuardado.HayPartidaGuardada())
        {
            // Hay partida guardada → preguntar confirmación
            contenedorBotones.SetActive(false);
            contenedorNuevaPartida.SetActive(true);
            estadoActual = EstadoMenu.ConfirmarNuevaPartida;
        }
        else
        {
            // No hay partida → ir directo al tutorial
            IniciarNuevaPartida();
        }
    }

    // --- BOTONES DEL ContenedorNuevaPartida ---
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
        SceneManager.LoadScene(escena);
    }

    // --- UTILIDAD ---
    void IniciarNuevaPartida()
    {
        GestorGuardado.GuardarProgreso("Tutorial", 0);
        SceneManager.LoadScene("Tutorial");
    }
}
