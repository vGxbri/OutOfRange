// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

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
        int indice = playerInput.playerIndex;

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

        ConfigurarEsquemaDeControl(indice);
        MoverASpawn(indice);

        CameraFollow camara = FindObjectOfType<CameraFollow>();
        if (camara != null)
        {
            camara.AddTarget(this.transform);
        }

        RevertirLayer(gameObject, 7);
    }

    void RevertirLayer(GameObject obj, int capa)
    {
        obj.layer = capa;
        foreach (Transform child in obj.transform)
        {
            RevertirLayer(child.gameObject, capa);
        }
    }

    void ConfigurarEsquemaDeControl(int indice)
    {
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
                int otroControl = PlayerPrefs.GetInt(indice == 0 ? "ControlJ2" : "ControlJ1", 0);
                bool ambosUsanTeclado = (controlSeleccionado == 0 && otroControl == 0);
                
                string esquema;
                if (ambosUsanTeclado)
                    esquema = (indice == 0) ? "Teclado_Izquierda" : "Teclado_Derecha";
                else
                    esquema = "Teclado_Solo";
                
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
            Vector3 pos = spawn.transform.position;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}