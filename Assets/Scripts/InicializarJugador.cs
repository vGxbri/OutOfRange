using UnityEngine;
using UnityEngine.InputSystem;

public class InicializadorJugador : MonoBehaviour
{
    [Header("Configuración por Jugador")]
    public RuntimeAnimatorController animatorJ1;
    public RuntimeAnimatorController animatorJ2;

    private MovimientoJugador movimiento;
    private Animator anim;
    private PlayerInput playerInput;

    void Awake()
    {
        movimiento = GetComponent<MovimientoJugador>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        int indice = playerInput.playerIndex; // 0 para J1, 1 para J2

        // 1. ASIGNACIÓN DE LÓGICA Y ANIMATOR
        if (indice == 0)
        {
            anim.runtimeAnimatorController = animatorJ1;
            movimiento.tieneDash = false;
        }
        else
        {
            anim.runtimeAnimatorController = animatorJ2;
            movimiento.tieneDash = true;
        }

        // 2. CONFIGURACIÓN DE CONTROLES (Sin errores de "Invalid User")
        ConfigurarEsquemaDeControl(indice);

        // 3. POSICIONAMIENTO (Spawn)
        MoverASpawn(indice);

        // --- NUEVA LÓGICA: REGISTRARSE EN LA CÁMARA ---
        CameraFollow camara = FindObjectOfType<CameraFollow>();
        if (camara != null)
        {
            camara.AddTarget(this.transform);
        }
    }

    void ConfigurarEsquemaDeControl(int indice)
    {
        // El esquema ya se asigna al hacer JoinPlayer en UnirseMismoTeclado.
        // Este método es un fallback por si algo falla.
        if (playerInput.devices.Count > 0)
        {
            InputDevice dispositivo = playerInput.devices[0];
            int controlSeleccionado = PlayerPrefs.GetInt(indice == 0 ? "ControlJ1" : "ControlJ2", 0);

            if (dispositivo is Gamepad)
            {
                playerInput.SwitchCurrentControlScheme("Mando", dispositivo);
            }
            else if (dispositivo is Keyboard)
            {
                // Si ambos usan teclado, J1=Izquierda, J2=Derecha
                // Si solo uno usa teclado, ese usa Izquierda
                int otroControl = PlayerPrefs.GetInt(indice == 0 ? "ControlJ2" : "ControlJ1", 0);
                bool ambosUsanTeclado = (controlSeleccionado == 0 && otroControl == 0);
                
                string esquema;
                if (ambosUsanTeclado)
                    esquema = (indice == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                else
                    esquema = "Teclado_Solo"; // Solo uno usa teclado, usa A/D + W(salto) + Enter(ataque)
                
                playerInput.SwitchCurrentControlScheme(esquema, dispositivo);
            }
        }
    }

    void MoverASpawn(int indice)
    {
        string nombreSpawn = (indice == 0) ? "Spawn_J1" : "Spawn_J2";
        GameObject spawn = GameObject.Find(nombreSpawn);

        if (spawn != null)
        {
            // Solo copiar X e Y, mantener Z original para no salir del clipping de la cámara
            Vector3 pos = spawn.transform.position;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}