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
    }

    public void EjecutarSonidoAtaque()
    {
        audioS.PlayOneShot(ataqueBasico);
    }

    public void EjecutarAtaquePesado()
    {
        Golpear(dañoAtaquePesado, rangoAtaquePesado);
    }

    public void EjecutarSonidoAtaquePesado()
    {
        audioS.PlayOneShot(ataqueFuerte);
    }

    void Golpear(int daño, float rango)
    {
        if (puntoAtaque == null) return;

        Vector3 posAtaque = ObtenerPosAtaque();

        // 1. Golpear Enemigos (usamos capaEnemigos para optimizar)
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

        // 2. Interaccionar con Objetos de Nivel (como la Piedra Meta)
        // Hacemos otra comprobación rápida sin máscara para pillar interacciones generales
        Collider2D[] interacciones = Physics2D.OverlapCircleAll(posAtaque, rango);
        foreach (var obj in interacciones)
        {
            MetaNivel meta = obj.GetComponent<MetaNivel>();
            if (meta != null)
            {
                meta.ActivarMeta(gameObject);
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

