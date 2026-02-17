using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject prefabZona; // Arrastra tu zona aquí
    public bool jugador1LlevaZona = true;

    private bool zonaCreada = false;

    void Start()
    {
        // Busca jugadores cada 0.5 segundos
        InvokeRepeating("BuscarJugadores", 0.5f, 0.5f);
    }

    void BuscarJugadores()
    {
        if (zonaCreada) return;

        // Busca las tarjetas de identidad
        IdentidadJugador[] jugadores = FindObjectsOfType<IdentidadJugador>();
        
        GameObject j1 = null;
        GameObject j2 = null;

        // Clasifica
        foreach (var id in jugadores)
        {
            if (id.numeroJugador == 1) j1 = id.gameObject;
            if (id.numeroJugador == 2) j2 = id.gameObject;
        }

        // Si están los dos, BOOM, creamos la zona
        if (j1 != null && j2 != null)
        {
            CancelInvoke("BuscarJugadores");
            CrearZona(j1, j2);
        }
    }

    void CrearZona(GameObject p1, GameObject p2)
    {
        zonaCreada = true;
        GameObject z = Instantiate(prefabZona);
        ControladorZona ctrl = z.GetComponent<ControladorZona>();

        if (jugador1LlevaZona) ctrl.ConfigurarZona(p1, p2);
        else ctrl.ConfigurarZona(p2, p1);
        
        Debug.Log("¡Zona creada y asignada!");
    }
}