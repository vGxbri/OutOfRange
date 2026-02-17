using UnityEngine;

public class ControladorZona : MonoBehaviour
{
    // Opciones para elegir en el inspector
    public enum FormaZona { Circulo, Cuadrado }

    [Header("Configuración")]
    public FormaZona formaActual = FormaZona.Circulo;
    public float radio = 5f;               // Tamaño si es círculo
    public Vector2 tamanoBox = new Vector2(8, 6); // Tamaño si es cuadrado

    [Header("Referencias (Arrastra aquí)")]
    public CircleCollider2D colCirculo;
    public BoxCollider2D colCuadrado;
    public Transform visuales; // El objeto hijo que tiene el Sprite

    // Variables privadas (el juego las rellena solas)
    private GameObject jugadorVictima; 

    // --- FUNCIÓN PRINCIPAL DE CONFIGURACIÓN ---
    // Esta función la llamará el GameManager al arrancar
    public void ConfigurarZona(GameObject dueño, GameObject victima)
    {
        // 1. Guardamos quién va a morir si sale
        jugadorVictima = victima;

        // 2. Nos pegamos al dueño (nos hacemos sus hijos)
        transform.SetParent(dueño.transform);
        transform.localPosition = Vector3.zero; // Nos centramos en él

        // 3. Aplicamos la forma y tamaño correctos
        ActualizarForma();
    }

    void ActualizarForma()
    {
        // Apagamos todo primero para no tener bugs
        if(colCirculo) colCirculo.enabled = false;
        if(colCuadrado) colCuadrado.enabled = false;

        if (formaActual == FormaZona.Circulo)
        {
            // FÍSICA
            colCirculo.enabled = true;
            colCirculo.radius = radio;
            
            // VISUAL (El círculo se escala por Diámetro = Radio * 2)
            if(visuales) visuales.localScale = new Vector3(radio * 2, radio * 2, 1);
        }
        else // Cuadrado
        {
            // FÍSICA
            colCuadrado.enabled = true;
            colCuadrado.size = tamanoBox;

            // VISUAL
            if(visuales) visuales.localScale = new Vector3(tamanoBox.x, tamanoBox.y, 1);
        }
    }

    // --- LÓGICA DE MUERTE ---
    private void OnTriggerExit2D(Collider2D other)
    {
        // Si todavía no se ha configurado la partida, ignorar
        if (jugadorVictima == null) return;

        // Si lo que sale es LA VÍCTIMA (y no el dueño ni una pared)
        if (other.gameObject == jugadorVictima)
        {
            Debug.Log("¡El jugador ha salido de la zona! Muriendo...");
            Destroy(jugadorVictima); 
            // Aquí luego pondrás tu lógica de Fin de Partida
        }
    }
}