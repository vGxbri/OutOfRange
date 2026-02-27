// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;

public static class GestorGuardado
{
    private const string CLAVE_PARTIDA_EXISTE = "PartidaExiste";
    private const string CLAVE_NIVEL_ACTUAL = "NivelActual";
    private const string CLAVE_NOMBRE_ESCENA = "EscenaActual";

    public static bool HayPartidaGuardada()
    {
        return PlayerPrefs.GetInt(CLAVE_PARTIDA_EXISTE, 0) == 1;
    }

    public static void GuardarProgreso(string nombreEscena, int nivelNumero)
    {
        PlayerPrefs.SetInt(CLAVE_PARTIDA_EXISTE, 1);
        PlayerPrefs.SetString(CLAVE_NOMBRE_ESCENA, nombreEscena);
        PlayerPrefs.SetInt(CLAVE_NIVEL_ACTUAL, nivelNumero);
        PlayerPrefs.Save();
    }

    public static string ObtenerEscenaGuardada()
    {
        return PlayerPrefs.GetString(CLAVE_NOMBRE_ESCENA, "Tutorial");
    }

    public static int ObtenerNivelActual()
    {
        return PlayerPrefs.GetInt(CLAVE_NIVEL_ACTUAL, 0);
    }

    public static void BorrarPartida()
    {
        PlayerPrefs.DeleteKey(CLAVE_PARTIDA_EXISTE);
        PlayerPrefs.DeleteKey(CLAVE_NIVEL_ACTUAL);
        PlayerPrefs.DeleteKey(CLAVE_NOMBRE_ESCENA);
        PlayerPrefs.Save();
    }
}
