using UnityEngine;

public class HojaEfecto : MonoBehaviour
{
    private Rigidbody2D rb;
    private float variacionViento;
    private float velocidadCaida;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Les damos un giro inicial para que cada una empiece diferente
        rb.rotation = Random.Range(0, 360f);
        rb.AddTorque(Random.Range(-2f, 2f)); // Giro lento constante

        // Creamos una variación única para esta hoja
        variacionViento = Random.Range(0.5f, 2f);
        velocidadCaida = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        // Movimiento de vaivén (Seno) para simular planeo
        float movimientoX = Mathf.Sin(Time.time * variacionViento) * 2f;

        // Aplicamos una fuerza lateral constante (viento base) + el vaivén
        // El "1.5f" es la fuerza del viento hacia la derecha, cámbialo a negativo para la izquierda
        rb.velocity = new Vector2(1.5f + movimientoX, -velocidadCaida);

        // Destruir si sale de la pantalla
        if (transform.position.y < -10f) Destroy(gameObject);
    }
}