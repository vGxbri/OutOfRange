using UnityEngine;

public static class GestorGuardado
{
    private const string CLAVE_PARTIDA_EXISTE = "PartidaExiste";
    private const string CLAVE_NIVEL_ACTUAL = "NivelActual";
    private const string CLAVE_NOMBRE_ESCENA = "EscenaActual";

    /// <summary>
    /// Comprueba si hay una partida guardada.
    /// </summary>
    public static bool HayPartidaGuardada()
    {
        return PlayerPrefs.GetInt(CLAVE_PARTIDA_EXISTE, 0) == 1;
    }

    /// <summary>
    /// Guarda el progreso actual (llama a esto al empezar un nivel).
    /// </summary>
    public static void GuardarProgreso(string nombreEscena, int nivelNumero)
    {
        PlayerPrefs.SetInt(CLAVE_PARTIDA_EXISTE, 1);
        PlayerPrefs.SetString(CLAVE_NOMBRE_ESCENA, nombreEscena);
        PlayerPrefs.SetInt(CLAVE_NIVEL_ACTUAL, nivelNumero);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Devuelve el nombre de la escena donde se quedó el jugador.
    /// </summary>
    public static string ObtenerEscenaGuardada()
    {
        return PlayerPrefs.GetString(CLAVE_NOMBRE_ESCENA, "Tutorial");
    }

    /// <summary>
    /// Devuelve el número de nivel guardado.
    /// </summary>
    public static int ObtenerNivelActual()
    {
        return PlayerPrefs.GetInt(CLAVE_NIVEL_ACTUAL, 0);
    }

    /// <summary>
    /// Borra toda la partida guardada.
    /// </summary>
    public static void BorrarPartida()
    {
        PlayerPrefs.DeleteKey(CLAVE_PARTIDA_EXISTE);
        PlayerPrefs.DeleteKey(CLAVE_NIVEL_ACTUAL);
        PlayerPrefs.DeleteKey(CLAVE_NOMBRE_ESCENA);
        PlayerPrefs.Save();
    }
}
