using UnityEngine;
using UnityEngine.InputSystem;

public class UnirseMismoTeclado : MonoBehaviour
{
    [Header("Prefab 'JugadorBase'")]
    public GameObject prefabJugador; 

    private PlayerInputManager manager;

    void Awake()
    {
        manager = GetComponent<PlayerInputManager>();

        if (prefabJugador != null)
        {
            manager.playerPrefab = prefabJugador;
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
                manager.JoinPlayer(0, -1, "Teclado_Izquierda", Keyboard.current);
            }
        }

        // JUGADOR 2 (ENTER)
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (manager.playerCount == 1)
            {
                manager.JoinPlayer(1, -1, "Teclado_Derecha", Keyboard.current);
            }
        }
    }
}