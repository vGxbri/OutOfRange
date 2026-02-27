// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using System;

public class VidaCompartida : MonoBehaviour
{
    public static VidaCompartida Instancia { get; private set; }

    [Header("Configuración")]
    public int vidasMaximas = 5;

    private int vidasActuales;

    public event Action<int> OnVidaCambiada;
    public event Action OnGameOver;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        vidasActuales = vidasMaximas;
    }

    public void RecibirDaño(int cantidad = 1)
    {
        if (vidasActuales <= 0) return;

        vidasActuales -= cantidad;
        vidasActuales = Mathf.Max(vidasActuales, 0);

        Debug.Log($"Vidas restantes: {vidasActuales}/{vidasMaximas}");
        OnVidaCambiada?.Invoke(vidasActuales);

        if (vidasActuales <= 0)
        {
            Debug.Log("GAME OVER");

            MovimientoJugador[] jugadores = FindObjectsOfType<MovimientoJugador>();
            foreach (var j in jugadores)
            {
                j.Morir();
            }

            OnGameOver?.Invoke();
        }
    }

    public void Curar(int cantidad = 1)
    {
        vidasActuales = Mathf.Min(vidasActuales + cantidad, vidasMaximas);
        OnVidaCambiada?.Invoke(vidasActuales);
    }

    public void Reiniciar()
    {
        vidasActuales = vidasMaximas;
        OnVidaCambiada?.Invoke(vidasActuales);
    }

    public int ObtenerVidas() => vidasActuales;
    public bool EstaVivo() => vidasActuales > 0;
}
