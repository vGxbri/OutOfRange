using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 1.5f;
    public float fuerzaSalto = 4.2f;

    [Header("Detección de Suelo")]
    public Transform detectorSuelo;
    public float radioDeteccion = 0.3f;
    public LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 inputMovimiento;

    // Variables de estado
    private bool tocandoSuelo;
    private float tiempoCoyote = 0.15f;
    private float contadorCoyote;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Se activa al pulsar W (Configurado en el Player Input)
    public void AlSaltar(InputAction.CallbackContext context)
    {
        // Solo saltamos en el momento exacto de pulsar (performed)
        // y si el contador de Coyote aún es mayor a cero
        if (context.performed && contadorCoyote > 0)
        {
            // Aplicamos fuerza vertical pura (Salto constante)
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);

            // Disparamos la animación
            animator.SetTrigger("Saltar");

            // Gastamos el coyote time para no saltar dos veces
            contadorCoyote = 0;
        }
    }

    public void AlMover(InputAction.CallbackContext context)
    {
        inputMovimiento = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // 1. ACTUALIZAR DETECCIÓN DE SUELO
        // Comprobamos si el círculo toca algo en la capa Suelo
        tocandoSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);

        // 2. LÓGICA DE COYOTE TIME
        if (tocandoSuelo)
        {
            contadorCoyote = tiempoCoyote;
        }
        else
        {
            contadorCoyote -= Time.fixedDeltaTime;
        }

        // 3. GRAVEDAD PRO (Cae más rápido para que no sea "lunar")
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = 1f;
        }
        else
        {
            rb.gravityScale = 2f;
        }

        // 4. MOVIMIENTO HORIZONTAL
        rb.velocity = new Vector2(inputMovimiento.x * velocidad, rb.velocity.y);

        // 5. ACTUALIZAR ANIMATOR
        animator.SetBool("enSuelo", tocandoSuelo);
        animator.SetFloat("Velocidad", Mathf.Abs(inputMovimiento.x));

        // 6. GIRAR SPRITE
        if (inputMovimiento.x > 0) spriteRenderer.flipX = false;
        else if (inputMovimiento.x < 0) spriteRenderer.flipX = true;

        // ENVIAR VELOCIDAD VERTICAL AL ANIMATOR
        // rb.velocity.y será positivo al subir y negativo al bajar
        animator.SetFloat("VelocidadVertical", rb.velocity.y);
    }

    // Dibuja el círculo en la escena para ayudarte a colocarlo
    private void OnDrawGizmos()
    {
        if (detectorSuelo != null)
        {
            Gizmos.color = tocandoSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        }
    }
}