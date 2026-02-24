using UnityEngine;

public class AtaqueJugador : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public Transform puntoAtaque;
    public float rangoAtaque = 0.5f;
    public int dañoAtaque = 1;
    public LayerMask capaEnemigos;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Llamado desde Animation Event o desde el script de movimiento
    public void EjecutarAtaque()
    {
        if (puntoAtaque == null) return;

        // Calcular posición real del ataque (espejada si mira a la izquierda)
        Vector3 posAtaque = puntoAtaque.position;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            // Espejar la X del punto de ataque respecto al centro del jugador
            float offsetX = puntoAtaque.localPosition.x;
            posAtaque = transform.position + new Vector3(-offsetX, puntoAtaque.localPosition.y, 0);
        }

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(
            posAtaque, rangoAtaque, capaEnemigos);

        foreach (var enemigo in enemigosGolpeados)
        {
            VidaEnemigo vida = enemigo.GetComponent<VidaEnemigo>();
            if (vida != null)
            {
                vida.RecibirDaño(dañoAtaque, transform.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null) return;

        Vector3 posAtaque = puntoAtaque.position;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            float offsetX = puntoAtaque.localPosition.x;
            posAtaque = transform.position + new Vector3(-offsetX, puntoAtaque.localPosition.y, 0);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(posAtaque, rangoAtaque);
    }
}

