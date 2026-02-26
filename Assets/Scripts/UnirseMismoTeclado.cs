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

    // Control: cuántos jugadores se han unido de teclado (para saber si el siguiente es Izquierda o Derecha)
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
            // Es la primera vez que entramos a este nivel
            ultimoNivelCargado = escenaActual;
            
            if (contenedorComienzoPartida != null) contenedorComienzoPartida.SetActive(true);
            if (contenedorJ1 != null) contenedorJ1.SetActive(true);
            if (contenedorJ2 != null) contenedorJ2.SetActive(true);

            ConfigurarIconosUI();
        }
        else
        {
            // El nivel se ha reiniciado
            if (contenedorComienzoPartida != null) contenedorComienzoPartida.SetActive(false);
            IniciarAutomaticamente();
        }
    }

    void ConfigurarIconosUI()
    {
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        bool j1UsaEnter = (controlJ1 == 0 && controlJ2 == 1);

        // --- Configurar J1 ---
        if (j1ImagenTecla != null)
        {
            if (controlJ1 == 0) // Teclado
            {
                j1ImagenTecla.sprite = j1UsaEnter ? spriteEnter : spriteEspacio;
            }
            else // Mando
            {
                j1ImagenTecla.sprite = spriteMandoA;
            }
        }

        // --- Configurar J2 ---
        if (j2ImagenTecla != null)
        {
            if (controlJ2 == 0) // Teclado
            {
                j2ImagenTecla.sprite = spriteEnter;
            }
            else // Mando
            {
                j2ImagenTecla.sprite = spriteMandoA;
            }
        }
    }

    void IniciarAutomaticamente()
    {
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        // --- JUGADOR 1 ---
        if (controlJ1 == 0) // Teclado
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
        else // Mando
        {
            if (Gamepad.current != null)
            {
                manager.playerPrefab = prefabJugador1;
                manager.JoinPlayer(0, -1, "Mando", Gamepad.current);
                j1Unido = true;
            }
        }

        // --- JUGADOR 2 ---
        if (j1Unido)
        {
            if (controlJ2 == 0) // Teclado
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
            else // Mando
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
        // Leer selección del menú (0 = teclado, 1 = mando)
        int controlJ1 = PlayerPrefs.GetInt("ControlJ1", 0);
        int controlJ2 = PlayerPrefs.GetInt("ControlJ2", 0);

        // --- JUGADOR 1 ---
        if (!j1Unido)
        {
            if (controlJ1 == 0) // Teclado
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
            else // Mando
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

        // --- JUGADOR 2 ---
        if (!j2Unido && j1Unido)
        {
            if (controlJ2 == 0) // Teclado
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
            else // Mando
            {
                // Si J1 también usa mando, necesitamos un SEGUNDO mando
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

        // Ocultar contenedor global si ambos están unidos
        if (j1Unido && j2Unido && contenedorComienzoPartida != null && contenedorComienzoPartida.activeSelf)
        {
            contenedorComienzoPartida.SetActive(false);
        }
    }

    Gamepad ObtenerMandoParaJ2(int controlJ1)
    {
        if (controlJ1 == 0)
        {
            // J1 usa teclado → J2 puede usar cualquier mando
            return Gamepad.current;
        }
        else
        {
            // J1 también usa mando → J2 necesita un mando DIFERENTE
            // Si solo hay 1 mando conectado, Gamepad.all tendrá solo uno y no podrán ambos usar mando
            if (Gamepad.all.Count >= 2)
            {
                return Gamepad.all[1]; // El segundo mando
            }
            return null;
        }
    }
}