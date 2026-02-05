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
        // Revisamos qué dispositivo se le asignó a este jugador al nacer
        if (playerInput.devices.Count > 0)
        {
            InputDevice dispositivo = playerInput.devices[0];

            if (dispositivo is Keyboard)
            {
                // Si es teclado, asignamos Izquierda o Derecha según el índice
                string esquema = (indice == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                playerInput.SwitchCurrentControlScheme(esquema, dispositivo);
            }
            else if (dispositivo is Gamepad)
            {
                // Si es mando, usamos el esquema "Mando" (o el nombre que tengas en el Asset)
                playerInput.SwitchCurrentControlScheme("Mando", dispositivo);
            }
        }
    }

    void MoverASpawn(int indice)
    {
        string nombreSpawn = (indice == 0) ? "Spawn_J1" : "Spawn_J2";
        GameObject spawn = GameObject.Find(nombreSpawn);

        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
}