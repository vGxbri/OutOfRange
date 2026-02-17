using UnityEngine;
using UnityEngine.InputSystem;

public class UnirseMismoTeclado : MonoBehaviour
{
    [Header("Arrastra aquí tus DOS prefabs distintos")]
    public GameObject prefabJugador1; 
    public GameObject prefabJugador2; 

    private PlayerInputManager manager;

    void Awake()
    {
        manager = GetComponent<PlayerInputManager>();
        
        // Por defecto, cargamos el prefab del J1 para que esté listo
        if (prefabJugador1 != null)
        {
            manager.playerPrefab = prefabJugador1;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // JUGADOR 1 (ESPACIO)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (manager.playerCount == 0)
            {
                // Nos aseguramos de que el prefab activo sea el 1
                manager.playerPrefab = prefabJugador1;
                
                // Unimos al jugador
                manager.JoinPlayer(0, -1, "Teclado_Izquierda", Keyboard.current);
            }
        }

        // JUGADOR 2 (ENTER)
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (manager.playerCount == 1) // Solo entra si ya hay 1 jugador
            {
                // ¡EL TRUCO! Cambiamos el prefab al 2 justo antes de que entre
                manager.playerPrefab = prefabJugador2;

                // Unimos al jugador
                manager.JoinPlayer(1, -1, "Teclado_Derecha", Keyboard.current);
            }
        }
    }
}