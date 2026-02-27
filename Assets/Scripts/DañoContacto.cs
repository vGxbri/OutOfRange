// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;

public class DañoContacto : MonoBehaviour
{
    [Header("Configuración")]
    public int daño = 1;
    public float cooldownDaño = 1f;
    public float fuerzaKnockbackJugador = 4f;

    private float ultimoTiempoDaño;

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (Time.time - ultimoTiempoDaño < cooldownDaño) return;

        Collider2D otro = colision.collider;

        if (otro.CompareTag("Player"))
        {
            if (VidaCompartida.Instancia != null)
            {
                VidaCompartida.Instancia.RecibirDaño(daño);
                ultimoTiempoDaño = Time.time;
            }

            MovimientoJugador movimiento = otro.GetComponent<MovimientoJugador>();
            if (movimiento != null)
            {
                movimiento.RecibirHit();
            }

            Rigidbody2D rbJugador = otro.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                float dirKnockback = Mathf.Sign(otro.transform.position.x - transform.position.x);
                rbJugador.velocity = new Vector2(dirKnockback * fuerzaKnockbackJugador, rbJugador.velocity.y);
            }
        }
    }
}

