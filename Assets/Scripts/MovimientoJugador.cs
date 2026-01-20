using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 1.2f;
    public float fuerzaSalto = 4.2f;

    [Header("Detección de Suelo")]
    public Transform detectorSuelo;
    public Vector2 tamañoCajaDeteccion = new Vector2(0.2f, 0.05f);
    public LayerMask capaSuelo;

    [Header("Ajustes de Combate")]
    public float tiempoCargaNecesario = 0.3f;
    public float tiempoBloqueoHit = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 inputMovimiento;

    // Variables de estado
    private bool tocandoSuelo;
    private float tiempoCoyote = 0.15f;
    private float contadorCoyote;
    private bool estaMuerto = false;
    private float tiempoPresionado;
    private bool bloqueadoPorAtaque = false;
    private bool bloqueadoPorHit = false;

    [Header("Efectos Visuales")]
    public Animator animatorCarga;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // 1. GESTIÓN DE BLOQUEOS
        // Si estamos muertos, atacando o heridos, frenamos y salimos del FixedUpdate
        if (estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            // Actualizamos velocidad vertical para que Jump/Fall funcionen si nos pegan en el aire
            animator.SetFloat("VelocidadVertical", rb.velocity.y);
            return;
        }

        tocandoSuelo = Physics2D.OverlapBox(detectorSuelo.position, tamañoCajaDeteccion, 0f, capaSuelo);

        // 3. LÓGICA DE COYOTE TIME
        if (tocandoSuelo)
        {
            contadorCoyote = tiempoCoyote;
        }
        else
        {
            contadorCoyote -= Time.fixedDeltaTime;
        }

        // 4. GRAVEDAD PRO
        if (rb.velocity.y < 0) rb.gravityScale = 2f; // Cae más pesado
        else rb.gravityScale = 1f;

        // 5. MOVIMIENTO HORIZONTAL
        rb.velocity = new Vector2(inputMovimiento.x * velocidad, rb.velocity.y);

        // 6. ACTUALIZAR ANIMATOR
        animator.SetBool("enSuelo", tocandoSuelo);
        animator.SetFloat("Velocidad", Mathf.Abs(inputMovimiento.x));
        animator.SetFloat("VelocidadVertical", rb.velocity.y);

        // 7. GIRAR SPRITE
        if (inputMovimiento.x > 0) spriteRenderer.flipX = false;
        else if (inputMovimiento.x < 0) spriteRenderer.flipX = true;
    }

    // --- ENTRADAS DE CONTROL (INPUT SYSTEM) ---

    public void AlMover(InputAction.CallbackContext context)
    {
        inputMovimiento = context.ReadValue<Vector2>();
    }

    public void AlSaltar(InputAction.CallbackContext context)
    {
        if (estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit) return;

        if (context.performed && contadorCoyote > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            animator.SetTrigger("Saltar");
            contadorCoyote = 0;
        }
    }

    public void AlAtacar(InputAction.CallbackContext context)
    {
        if (estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit) return;

        if (context.started)
        {
            tiempoPresionado = Time.time;

            // 1. Activamos el efecto visual
            animatorCarga.SetBool("isCharging", true);

            // 2. Sincronizamos la velocidad: 
            // Si la animación dura 1s y tu carga es de 0.3s, la velocidad debe ser 1/0.3
            animatorCarga.speed = 1f / tiempoCargaNecesario;
        }

        if (context.canceled)
        {
            float duracionFinal = Time.time - tiempoPresionado;

            // Desactivamos el efecto y reseteamos velocidad
            animatorCarga.SetBool("isCharging", false);
            animatorCarga.speed = 1f;

            if (duracionFinal >= tiempoCargaNecesario)
            {
                StartCoroutine(SecuenciaAtaquePesado());
            }
            else
            {
                animator.SetTrigger("Atacar");
            }

            spriteRenderer.color = Color.white;
        }
    }

    // --- SECUENCIAS (CORRUTINAS) ---

    IEnumerator SecuenciaAtaquePesado()
    {
        bloqueadoPorAtaque = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetTrigger("Atacar2");

        yield return new WaitForSeconds(0.6f); // Duración animación
        yield return new WaitForSeconds(0.3f); // Recuperación extra

        bloqueadoPorAtaque = false;
    }

    IEnumerator SecuenciaHit()
    {
        bloqueadoPorHit = true;
        animator.SetTrigger("Recibir_Hit"); // Asegúrate de que el Trigger se llame "Hit" en el Animator
        yield return new WaitForSeconds(tiempoBloqueoHit);

        bloqueadoPorHit = false;
    }

    // --- FUNCIONES PÚBLICAS ---

    public void RecibirHit()
    {
        if (estaMuerto || bloqueadoPorHit) return;
        StartCoroutine(SecuenciaHit());
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        animator.SetTrigger("Morir");
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = tocandoSuelo ? Color.green : Color.red;
            Gizmos.DrawWireCube(detectorSuelo.position, tamañoCajaDeteccion);
        }
    }
}