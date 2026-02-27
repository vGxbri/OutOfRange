// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnirseMismoTeclado : MonoBehaviour
{
    [Header("Arrastra aquí tus DOS prefabs distintos")]
    public GameObject prefabJugador1; 
    public GameObject prefabJugador2; 

    [Header("UI Comienzo Nivel")]
    public GameObject contenedorComienzoPartida;
    public GameObject contenedorJ1;
    public GameObject contenedorJ2;

    [Header("Iconos Teclas UI")]
    public Image j1ImagenTecla;
    public Image j2ImagenTecla;

    [Header("Sprites de Teclas")]
    public Sprite spriteEspacio;
    public Sprite spriteEnter;
    public Sprite spriteMandoA;

    private PlayerInputManager manager;

    private int jugadoresTecladoUnidos = 0;
    private bool j1Unido = false;
    private bool j2Unido = false;

    private static string ultimoNivelCargado = "";

    void Awake()
    {
        manager = GetComponent<PlayerInputManager>();
        
        if (prefabJugador1 != null)
        {
            manager.playerPrefab = prefabJugador1;
        }
    }

    void Start()
    {
        string escenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (ultimoNivelCargado != escenaActual)
        {
            ultimoNivelCargado = escenaActual;
            
            if (contenedorComienzoPartida != null) contenedorComienzoPartida.SetActive(true);
            if (contenedorJ1 != null) contenedorJ1.SetActive(true);
            if (contenedorJ2 != null) contenedorJ2.SetActive(true);

            ConfigurarIconosUI();
        }
        else
        {
            if (contenedorComienzoPartida != null) contenedorComienzoPartida.SetActive(false);
            IniciarAutomaticamente();
        }
    }

    void ConfigurarIconosUI()
    {
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        bool j1UsaEnter = (controlJ1 == 0 && controlJ2 == 1);

        if (j1ImagenTecla != null)
        {
            if (controlJ1 == 0)
            {
                j1ImagenTecla.sprite = j1UsaEnter ? spriteEnter : spriteEspacio;
            }
            else
            {
                j1ImagenTecla.sprite = spriteMandoA;
            }
        }

        if (j2ImagenTecla != null)
        {
            if (controlJ2 == 0)
            {
                j2ImagenTecla.sprite = spriteEnter;
            }
            else
            {
                j2ImagenTecla.sprite = spriteMandoA;
            }
        }
    }

    void IniciarAutomaticamente()
    {
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        if (controlJ1 == 0)
        {
            if (Keyboard.current != null)
            {
                manager.playerPrefab = prefabJugador1;
                string esquema = (jugadoresTecladoUnidos == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                manager.JoinPlayer(0, -1, esquema, Keyboard.current);
                jugadoresTecladoUnidos++;
                j1Unido = true;
            }
        }
        else
        {
            if (Gamepad.current != null)
            {
                manager.playerPrefab = prefabJugador1;
                manager.JoinPlayer(0, -1, "Mando", Gamepad.current);
                j1Unido = true;
            }
        }

        if (j1Unido)
        {
            if (controlJ2 == 0)
            {
                if (Keyboard.current != null)
                {
                    manager.playerPrefab = prefabJugador2;
                    string esquema = (jugadoresTecladoUnidos == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                    manager.JoinPlayer(1, -1, esquema, Keyboard.current);
                    jugadoresTecladoUnidos++;
                    j2Unido = true;
                }
            }
            else
            {
                Gamepad mando = ObtenerMandoParaJ2(controlJ1);
                if (mando != null)
                {
                    manager.playerPrefab = prefabJugador2;
                    manager.JoinPlayer(1, -1, "Mando", mando);
                    j2Unido = true;
                }
            }
        }
    }

    void Update()
    {
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        if (!GameManager.JuegoIniciado)
        {
            ConfigurarIconosUI();
        }

        if (!j1Unido)
        {
            if (controlJ1 == 0)
            {
                bool j1UsaEnter = (controlJ1 == 0 && controlJ2 == 1);
                bool presionoBoton = false;

                if (Keyboard.current != null)
                {
                    presionoBoton = j1UsaEnter ? Keyboard.current.enterKey.wasPressedThisFrame : Keyboard.current.spaceKey.wasPressedThisFrame;
                }

                if (presionoBoton)
                {
                    manager.playerPrefab = prefabJugador1;
                    string esquema = (jugadoresTecladoUnidos == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                    manager.JoinPlayer(0, -1, esquema, Keyboard.current);
                    jugadoresTecladoUnidos++;
                    j1Unido = true;
                    if (contenedorJ1 != null) contenedorJ1.SetActive(false);
                }
            }
            else
            {
                if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    manager.playerPrefab = prefabJugador1;
                    manager.JoinPlayer(0, -1, "Mando", Gamepad.current);
                    j1Unido = true;
                    if (contenedorJ1 != null) contenedorJ1.SetActive(false);
                }
            }
        }

        if (!j2Unido && j1Unido)
        {
            if (controlJ2 == 0)
            {
                if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    manager.playerPrefab = prefabJugador2;
                    string esquema = (jugadoresTecladoUnidos == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                    manager.JoinPlayer(1, -1, esquema, Keyboard.current);
                    jugadoresTecladoUnidos++;
                    j2Unido = true;
                    if (contenedorJ2 != null) contenedorJ2.SetActive(false);
                }
            }
            else
            {
                Gamepad mando = ObtenerMandoParaJ2(controlJ1);
                if (mando != null && mando.buttonSouth.wasPressedThisFrame)
                {
                    manager.playerPrefab = prefabJugador2;
                    manager.JoinPlayer(1, -1, "Mando", mando);
                    j2Unido = true;
                    if (contenedorJ2 != null) contenedorJ2.SetActive(false);
                }
            }
        }

        if (j1Unido && j2Unido && contenedorComienzoPartida != null && contenedorComienzoPartida.activeSelf)
        {
            contenedorComienzoPartida.SetActive(false);
        }
    }

    Gamepad ObtenerMandoParaJ2(int controlJ1)
    {
        if (controlJ1 == 0)
        {
            return Gamepad.current;
        }
        else
        {
            if (Gamepad.all.Count >= 2)
            {
                return Gamepad.all[1];
            }
            return null;
        }
    }
}