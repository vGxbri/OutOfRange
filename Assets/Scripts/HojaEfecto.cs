// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;

public class HojaEfecto : MonoBehaviour
{
    private Rigidbody2D rb;
    private float variacionViento;
    private float velocidadCaida;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.rotation = Random.Range(0, 360f);
        rb.AddTorque(Random.Range(-2f, 2f));

        variacionViento = Random.Range(0.5f, 2f);
        velocidadCaida = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        float movimientoX = Mathf.Sin(Time.time * variacionViento) * 2f;

        rb.velocity = new Vector2(1.5f + movimientoX, -velocidadCaida);

        if (transform.position.y < -10f) Destroy(gameObject);
    }
}