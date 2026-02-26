using UnityEngine;

public class AtaqueJugador : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public Transform puntoAtaque;
    public float rangoAtaque = 0.5f;
    public int dañoAtaque = 1;
    public int dañoAtaquePesado = 2;
    public float rangoAtaquePesado = 0.65f;
    public LayerMask capaEnemigos;

    [Header("Sonidos")]
    public AudioSource audioS;
    public AudioClip ataqueBasico;
    public AudioClip ataqueFuerte;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void EjecutarAtaque()
    {
        Golpear(dañoAtaque, rangoAtaque);
        audioS.PlayOneShot(ataqueBasico);
    }

    public void EjecutarAtaquePesado()
    {
        Golpear(dañoAtaquePesado, rangoAtaquePesado);
        audioS.PlayOneShot(ataqueFuerte);
    }

    void Golpear(int daño, float rango)
    {
        if (puntoAtaque == null) return;

        Vector3 posAtaque = ObtenerPosAtaque();

        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(
            posAtaque, rango, capaEnemigos);

        foreach (var enemigo in enemigosGolpeados)
        {
            VidaEnemigo vida = enemigo.GetComponent<VidaEnemigo>();
            if (vida != null)
            {
                vida.RecibirDaño(daño, transform.position);
            }
        }
    }

    Vector3 ObtenerPosAtaque()
    {
        Vector3 posAtaque = puntoAtaque.position;
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            float offsetX = puntoAtaque.localPosition.x;
            posAtaque = transform.position + new Vector3(-offsetX, puntoAtaque.localPosition.y, 0);
        }
        return posAtaque;
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(posAtaque, rangoAtaquePesado);
    }
}

