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

    [Header("Efectos Visuales")]
    public Animator animatorCarga;

    [Header("Configuración de Dash")]
    public bool tieneDash;
    public float velocidadDash = 10f;
    public float duracionDash = 0.2f;

    [Header("Sonidos")]
    public AudioSource audioS;
    public AudioSource audioPasos; // Canal dedicado para pasos
    public AudioClip sonidoSalto;
    public AudioClip sonidoDash;
    public AudioClip pasosHierba;
    public AudioClip pasosPlataforma;
    public float intervaloPasos = 0.3f;
    private float contadorPasos;
    private string tipoSueloActual = "Plataforma";

    // Componentes
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera camaraPrincipal;

    // Variables de estado
    private Vector2 inputMovimiento;
    private bool tocandoSuelo;
    private float tiempoCoyote = 0.15f;
    private float contadorCoyote;
    private bool estaMuerto = false;
    private float tiempoPresionado;
    private bool bloqueadoPorAtaque = false;
    private bool bloqueadoPorHit = false;
    private bool estaDasheando = false;
    private float mitadAnchoJugador;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Referencia a la cámara y cálculo de bordes del sprite
        camaraPrincipal = Camera.main;
        if (spriteRenderer != null)
        {
            mitadAnchoJugador = spriteRenderer.bounds.extents.x;
        }
    }

    void Update()
    {
        // Si el juego se pausa (ej: tutorial), detenemos el sonido de los pasos inmediatamente
        if (Time.timeScale == 0f)
        {
            if (audioPasos != null && audioPasos.isPlaying)
            {
                audioPasos.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        // 1. GESTIÓN DE BLOQUEOS
        if (estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit || estaDasheando)
        {
            if (!estaDasheando) rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetFloat("VelocidadVertical", rb.velocity.y);
            return;
        }

        // 2. DETECCIÓN DE SUELO (Solo detectamos suelo si no estamos subiendo)
        if (rb.velocity.y <= 0.1f)
        {
            Collider2D col = Physics2D.OverlapBox(detectorSuelo.position, tamañoCajaDeteccion, 0f, capaSuelo);
            tocandoSuelo = col != null;

            if (tocandoSuelo)
            {
                if (col.CompareTag("Hierba")) tipoSueloActual = "Hierba";
                else tipoSueloActual = "Plataforma";
            }
        }
        else
        {
            tocandoSuelo = false;
        }

        // 3. LÓGICA DE COYOTE TIME
        if (tocandoSuelo) contadorCoyote = tiempoCoyote;
        else contadorCoyote -= Time.fixedDeltaTime;

        // 4. GRAVEDAD DINÁMICA
        if (!estaDasheando)
        {
            rb.gravityScale = (rb.velocity.y < 0) ? 1.2f : 0.7f;
        }

        // 5. MOVIMIENTO HORIZONTAL
        float entradaActualX = GameManager.JuegoIniciado ? inputMovimiento.x : 0f;
        rb.velocity = new Vector2(entradaActualX * velocidad, rb.velocity.y);

        // 6. ACTUALIZAR ANIMATOR
        animator.SetBool("enSuelo", tocandoSuelo);
        animator.SetFloat("Velocidad", Mathf.Abs(entradaActualX));
        animator.SetFloat("VelocidadVertical", rb.velocity.y);

        // 7. GIRAR SPRITE
        if (entradaActualX > 0) spriteRenderer.flipX = false;
        else if (entradaActualX < 0) spriteRenderer.flipX = true;

        // --- LÓGICA DE PASOS MEJORADA ---
        bool seEstaMoviendo = Mathf.Abs(rb.velocity.x) > 0.1f;

        if (tocandoSuelo && seEstaMoviendo && !estaDasheando)
        {
            contadorPasos -= Time.fixedDeltaTime;
            if (contadorPasos <= 0)
            {
                ReproducirSonidoPaso();
                contadorPasos = intervaloPasos; 
            }
        }
        else
        {
            // Ahora si paramos de andar, solo paramos el canal de pasos
            if (audioPasos != null && audioPasos.isPlaying)
            {
                audioPasos.Stop();
            }
            contadorPasos = 0; 
        }
    }

    private void ReproducirSonidoPaso()
    {
        AudioClip clipAElegir = (tipoSueloActual == "Hierba") ? pasosHierba : pasosPlataforma;
        
        if (audioPasos && clipAElegir)
        {
            // Usamos un canal independiente para que el Stop() no mate ataques/saltos
            audioPasos.clip = clipAElegir;
            audioPasos.pitch = Random.Range(0.9f, 1.1f);
            audioPasos.Play();
        }
    }

    void LateUpdate()
    {
        // --- RESTRICCIÓN DE PANTALLA ---
        if (!estaMuerto && camaraPrincipal != null)
        {
            float limiteIzquierdo = camaraPrincipal.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
            float limiteDerecho = camaraPrincipal.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

            float xClamped = Mathf.Clamp(transform.position.x, limiteIzquierdo + mitadAnchoJugador, limiteDerecho - mitadAnchoJugador);

            if (xClamped != transform.position.x)
            {
                transform.position = new Vector3(xClamped, transform.position.y, transform.position.z);
                if (estaDasheando) rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    public void AlMover(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f) 
        {
            inputMovimiento = Vector2.zero;
            return;
        }
        inputMovimiento = context.ReadValue<Vector2>();
    }

    public void AlSaltar(InputAction.CallbackContext context)
    {
        if (!GameManager.JuegoIniciado || estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit || Time.timeScale == 0f) return;

        if (context.performed && contadorCoyote > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            animator.SetTrigger("Saltar");
            contadorCoyote = -0.5f;
            if (audioS && sonidoSalto) audioS.PlayOneShot(sonidoSalto);
        }
    }

    public void AlAtacar(InputAction.CallbackContext context)
    {
        if (!GameManager.JuegoIniciado || estaMuerto || bloqueadoPorAtaque || bloqueadoPorHit) return;

        // Si el juego está pausado (ej. tutorial abierto), evitamos procesar y cancelamos cargas activas
        if (Time.timeScale == 0f)
        {
            animatorCarga.SetBool("isCharging", false);
            return;
        }

        if (context.started)
        {
            tiempoPresionado = Time.time;
            animatorCarga.SetBool("isCharging", true);
            animatorCarga.speed = 1f / tiempoCargaNecesario;
        }

        if (context.canceled)
        {
            // Solo atacamos si de verdad estábamos cargando el golpe (evita ataques al soltar la tecla tras cerrar el tutorial)
            if (!animatorCarga.GetBool("isCharging")) return;

            float duracionFinal = Time.time - tiempoPresionado;
            animatorCarga.SetBool("isCharging", false);
            animatorCarga.speed = 1f;

            if (duracionFinal >= tiempoCargaNecesario)
            {
                if (tieneDash) StartCoroutine(SecuenciaDash());
                else StartCoroutine(SecuenciaAtaquePesado());
            }
            else
            {
                animator.SetTrigger("Atacar");
            }
        }
    }

    IEnumerator SecuenciaAtaquePesado()
    {
        bloqueadoPorAtaque = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetTrigger("Atacar2");
        yield return new WaitForSeconds(0.3f);
        GetComponent<AtaqueJugador>().EjecutarAtaquePesado();
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.3f);
        bloqueadoPorAtaque = false;
    }

    IEnumerator SecuenciaDash()
    {
        estaDasheando = true;
        if (audioS && sonidoDash) audioS.PlayOneShot(sonidoDash);
        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0;

        float direccion = spriteRenderer.flipX ? -1f : 1f;
        rb.velocity = new Vector2(direccion * velocidadDash, 0);

        animator.SetTrigger("Dash");
        yield return new WaitForSeconds(duracionDash);

        rb.velocity = Vector2.zero;
        rb.gravityScale = gravedadOriginal;
        estaDasheando = false;
    }

    IEnumerator SecuenciaHit()
    {
        bloqueadoPorHit = true;
        animator.SetTrigger("Recibir_Hit");
        yield return new WaitForSeconds(tiempoBloqueoHit);
        bloqueadoPorHit = false;
    }

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
