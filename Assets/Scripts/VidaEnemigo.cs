using System.Collections;
using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuración")]
    public int vidaMaxima = 2;
    public float fuerzaKnockback = 5f;

    [Header("Efecto de Muerte")]
    public float duracionMuerte = 0.6f;
    public Color colorMuerte = Color.red;

    private int vidaActual;
    private Animator animator;
    private IAEnemigo ia;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool muerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        ia = GetComponent<IAEnemigo>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RecibirDaño(int cantidad = 1)
    {
        RecibirDaño(cantidad, Vector2.zero);
    }

    public void RecibirDaño(int cantidad, Vector2 posicionAtacante)
    {
        if (muerto) return;

        vidaActual -= cantidad;
        Debug.Log($"{gameObject.name} recibió daño. Vida: {vidaActual}/{vidaMaxima}");

        // Knockback
        if (rb != null && posicionAtacante != Vector2.zero)
        {
            float dirKnockback = Mathf.Sign(transform.position.x - posicionAtacante.x);
            rb.velocity = new Vector2(dirKnockback * fuerzaKnockback, rb.velocity.y + 1f);
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            if (animator != null) animator.SetTrigger("Hit");
            if (ia != null) ia.RecibirGolpe();

            // Flash rojo breve al recibir daño
            if (spriteRenderer != null) StartCoroutine(FlashDaño());
        }
    }

    IEnumerator FlashDaño()
    {
        spriteRenderer.color = colorMuerte;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Morir()
    {
        muerto = true;
        Debug.Log($"{gameObject.name} ha muerto!");

        // Desactivar IA y Animator
        if (ia != null) ia.enabled = false;
        if (animator != null) animator.enabled = false;

        // Desactivar todas las colisiones
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders) col.enabled = false;

        // Efecto visual de muerte
        StartCoroutine(EfectoMuerte());
    }

    IEnumerator EfectoMuerte()
    {
        // 1. Flash rojo instantáneo
        if (spriteRenderer != null) spriteRenderer.color = colorMuerte;

        // Pequeña pausa para que se note el flash
        yield return new WaitForSeconds(0.15f);

        // 2. Fade out + encogerse
        Vector3 escalaInicial = transform.localScale;
        Color colorInicial = spriteRenderer != null ? spriteRenderer.color : Color.red;
        float timer = 0f;

        while (timer < duracionMuerte)
        {
            timer += Time.deltaTime;
            float t = timer / duracionMuerte;

            // Encogerse
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, t);

            // Fade out
            if (spriteRenderer != null)
            {
                Color c = colorInicial;
                c.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public bool EstaMuerto() => muerto;
}
