using UnityEngine;

public class IAEnemigo : MonoBehaviour
{
    private enum Estado { Patrullar, IdlePatrulla, Perseguir, PreAtaque, Atacar, Retroceder, Golpeado }

    [Header("Movimiento")]
    public float velocidadPatrulla = 1.5f;
    public float velocidadPersecucion = 2.5f;
    public float velocidadRetroceso = 1.5f;

    [Header("Detección")]
    public float rangoDeteccion = 5f;
    public float rangoDeteccionTrasera = 1.5f;
    public Vector2 offsetDeteccionTrasera = Vector2.zero;
    public float alturaDeteccion = 1f;
    public float rangoAtaque = 1.2f;
    public Vector2 offsetAtaque = Vector2.zero;
    public LayerMask capaJugador;

    [Header("Patrulla")]
    public float distanciaDeteccionBorde = 0.5f;
    public float distanciaDeteccionPared = 0.3f;
    public LayerMask capaSuelo;
    public float tiempoIdleMin = 1f;
    public float tiempoIdleMax = 3f;
    public float tiempoCaminarMin = 2f;
    public float tiempoCaminarMax = 5f;

    [Header("Ataque")]
    public float cooldownAtaque = 1.5f;
    public float duracionPreAtaque = 0.4f;
    public float duracionAtaque = 0.6f;
    public float duracionRetroceso = 0.5f;
    public int dañoAtaque = 1;
    public float fuerzaKnockbackAtaque = 5f;

    [Header("Agresión")]
    public float memoriaAgresion = 2.5f;

    [Header("Golpe")]
    public float tiempoStun = 0.4f;

    [Header("Variación (%)")]
    [Range(0, 50)]
    public int variacionTiempos = 20;

    private Estado estadoActual = Estado.Patrullar;
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private int direccion = 1;
    private Transform objetivoActual;

    // Timers
    private float timerEstado;
    private float tiempoUltimoAtaque;
    private float tiempoPerdiendoObjetivo;
    private bool objetivoFueraDeRango = false;

    // Cooldown global de giro para evitar oscilación en espacios reducidos
    private const float COOLDOWN_GIRO = 0.6f;
    private float tiempoUltimoGiro;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        tiempoUltimoAtaque = -cooldownAtaque;
        timerEstado = VariarTiempo(Random.Range(tiempoCaminarMin, tiempoCaminarMax));

        // Masa alta para que los jugadores no lo empujen
        if (rb != null) rb.mass = 100f;

        // Fricción 0 para que los jugadores no se queden enganchados al saltar
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D matSinFriccion = new PhysicsMaterial2D("SinFriccion");
            matSinFriccion.friction = 0f;
            matSinFriccion.bounciness = 0f;
            col.sharedMaterial = matSinFriccion;
        }

        // Los enemigos no colisionan entre sí
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
    }

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.collider.CompareTag("Player")) return;

        bool esPared = ((1 << colision.gameObject.layer) & capaSuelo) != 0;

        if (esPared)
        {
            foreach (ContactPoint2D contacto in colision.contacts)
            {
                if (Mathf.Abs(contacto.normal.x) > 0.5f)
                {
                    if (IntentarGirar())
                    {
                        if (estadoActual == Estado.Perseguir)
                        {
                            objetivoActual = null;
                            CambiarEstado(Estado.Patrullar);
                        }
                    }
                    break;
                }
            }
        }
    }

    void Update()
    {
        switch (estadoActual)
        {
            case Estado.Golpeado:
                if (Time.time >= timerEstado)
                    CambiarEstado(Estado.Patrullar);
                return;

            case Estado.PreAtaque:
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (Time.time >= timerEstado)
                    EjecutarAtaque();
                return;

            case Estado.Atacar:
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (Time.time >= timerEstado)
                    CambiarEstado(Estado.Retroceder);
                return;

            case Estado.Retroceder:
                Retroceder();
                if (Time.time >= timerEstado)
                    CambiarEstado(Estado.Patrullar);
                return;

            case Estado.IdlePatrulla:
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (Time.time >= timerEstado)
                {
                    if (Random.value < 0.3f) IntentarGirar();
                    CambiarEstado(Estado.Patrullar);
                }
                BuscarJugador();
                return;

            case Estado.Patrullar:
                Patrullar();
                if (Time.time >= timerEstado)
                    CambiarEstado(Estado.IdlePatrulla);
                BuscarJugador();
                return;

            case Estado.Perseguir:
                Perseguir();
                return;
        }

        ActualizarAnimaciones();
        ActualizarDireccionVisual();
    }

    void BuscarJugador()
    {
        // Detección rectangular: ancho = rango de detección, alto = alturaDeteccion
        Vector2 tamañoCaja = new Vector2(rangoDeteccion * 2f, alturaDeteccion * 2f);
        Collider2D[] jugadores = Physics2D.OverlapBoxAll(transform.position, tamañoCaja, 0f, capaJugador);

        float distanciaMinima = float.MaxValue;
        Transform masCercano = null;

        foreach (var col in jugadores)
        {
            float distX = Mathf.Abs(col.transform.position.x - transform.position.x);
            float dirHaciaJugador = col.transform.position.x - transform.position.x;
            bool estaDelante = (dirHaciaJugador * direccion) > 0;

            // Detección direccional: rango completo delante, rango reducido detrás
            if (estaDelante || distX <= rangoDeteccionTrasera)
            {
                if (distX < distanciaMinima)
                {
                    distanciaMinima = distX;
                    masCercano = col.transform;
                }
            }
        }

        if (masCercano != null)
        {
            objetivoActual = masCercano;
            objetivoFueraDeRango = false;
            CambiarEstado(Estado.Perseguir);
        }
    }

    void Patrullar()
    {
        Vector2 posicionBorde = (Vector2)transform.position + new Vector2(direccion * distanciaDeteccionPared, 0);
        RaycastHit2D hitSuelo = Physics2D.Raycast(posicionBorde, Vector2.down, distanciaDeteccionBorde, capaSuelo);
        RaycastHit2D hitPared = Physics2D.Raycast(transform.position, Vector2.right * direccion, distanciaDeteccionPared, capaSuelo);

        if (!hitSuelo.collider || hitPared.collider)
        {
            if (!IntentarGirar())
            {
                // No puede girar (cooldown activo) → está en un espacio reducido, esperar
                CambiarEstado(Estado.IdlePatrulla);
                return;
            }
        }

        rb.velocity = new Vector2(direccion * velocidadPatrulla, rb.velocity.y);
        ActualizarAnimaciones();
        ActualizarDireccionVisual();
    }

    void Perseguir()
    {
        if (objetivoActual == null)
        {
            CambiarEstado(Estado.Patrullar);
            return;
        }

        float distancia = Vector2.Distance(transform.position, objetivoActual.position);

        // ¿Está en rango de ataque?
        if (distancia <= rangoAtaque)
        {
            if (Time.time - tiempoUltimoAtaque >= cooldownAtaque)
            {
                CambiarEstado(Estado.PreAtaque);
                return;
            }
        }

        // ¿Perdimos al objetivo?
        float dirHaciaJugador = objetivoActual.position.x - transform.position.x;
        bool estaDelante = (dirHaciaJugador * direccion) > 0;

        if (distancia > rangoDeteccion && !(distancia <= rangoDeteccionTrasera))
        {
            if (!objetivoFueraDeRango)
            {
                objetivoFueraDeRango = true;
                tiempoPerdiendoObjetivo = Time.time;
            }

            // Memoria de agresión: sigue persiguiendo unos segundos
            if (Time.time - tiempoPerdiendoObjetivo > memoriaAgresion)
            {
                objetivoActual = null;
                CambiarEstado(Estado.Patrullar);
                return;
            }
        }
        else
        {
            objetivoFueraDeRango = false;
        }

        // Moverse hacia el objetivo
        direccion = dirHaciaJugador > 0 ? 1 : -1;

        // Comprobar pared y borde antes de moverse
        Vector2 posicionBorde = (Vector2)transform.position + new Vector2(direccion * distanciaDeteccionPared, 0);
        RaycastHit2D hitSuelo = Physics2D.Raycast(posicionBorde, Vector2.down, distanciaDeteccionBorde, capaSuelo);
        RaycastHit2D hitPared = Physics2D.Raycast(transform.position, Vector2.right * direccion, distanciaDeteccionPared, capaSuelo);

        if (!hitSuelo.collider || hitPared.collider)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (hitPared.collider)
            {
                objetivoActual = null;
                IntentarGirar();
                CambiarEstado(Estado.Patrullar);
                return;
            }
        }
        else
        {
            rb.velocity = new Vector2(direccion * velocidadPersecucion, rb.velocity.y);
        }

        ActualizarAnimaciones();
        ActualizarDireccionVisual();
    }

    void EjecutarAtaque()
    {
        estadoActual = Estado.Atacar;
        tiempoUltimoAtaque = Time.time;
        timerEstado = Time.time + VariarTiempo(duracionAtaque);

        if (animator != null) animator.SetTrigger("Attack");

        // Calcular posición de ataque con offset
        Vector2 posAtaque = (Vector2)transform.position + offsetAtaque;

        // Detectar y dañar jugadores en rango de ataque
        Collider2D[] jugadores = Physics2D.OverlapCircleAll(posAtaque, rangoAtaque, capaJugador);
        foreach (var col in jugadores)
        {
            // Daño compartido
            if (VidaCompartida.Instancia != null)
                VidaCompartida.Instancia.RecibirDaño(dañoAtaque);

            // Hit animation en el jugador
            MovimientoJugador mov = col.GetComponent<MovimientoJugador>();
            if (mov != null) mov.RecibirHit();

            // Knockback al jugador
            Rigidbody2D rbJugador = col.GetComponent<Rigidbody2D>();
            if (rbJugador != null)
            {
                float dir = Mathf.Sign(col.transform.position.x - transform.position.x);
                rbJugador.velocity = new Vector2(dir * fuerzaKnockbackAtaque, rbJugador.velocity.y);
            }
        }
    }

    void Retroceder()
    {
        rb.velocity = new Vector2(-direccion * velocidadRetroceso, rb.velocity.y);
        ActualizarAnimaciones();
    }

    // Llamado desde VidaEnemigo
    public void RecibirGolpe()
    {
        estadoActual = Estado.Golpeado;
        timerEstado = Time.time + VariarTiempo(tiempoStun);
        // No reseteamos velocidad para que el knockback de VidaEnemigo se aplique
    }

    void CambiarEstado(Estado nuevo)
    {
        estadoActual = nuevo;

        switch (nuevo)
        {
            case Estado.Patrullar:
                timerEstado = Time.time + VariarTiempo(Random.Range(tiempoCaminarMin, tiempoCaminarMax));
                break;
            case Estado.IdlePatrulla:
                timerEstado = Time.time + VariarTiempo(Random.Range(tiempoIdleMin, tiempoIdleMax));
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case Estado.PreAtaque:
                timerEstado = Time.time + VariarTiempo(duracionPreAtaque);
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case Estado.Retroceder:
                timerEstado = Time.time + VariarTiempo(duracionRetroceso);
                break;
        }
    }

    /// Intenta girar respetando el cooldown global. Devuelve true si giró.
    bool IntentarGirar()
    {
        if (Time.time - tiempoUltimoGiro < COOLDOWN_GIRO)
            return false;

        direccion *= -1;
        offsetAtaque.x = -offsetAtaque.x;
        tiempoUltimoGiro = Time.time;
        return true;
    }

    void ActualizarDireccionVisual()
    {
        float escalaObjetivo = Mathf.Abs(transform.localScale.x) * direccion;

        // Compensar posición solo cuando realmente cambia la dirección
        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(escalaObjetivo) && capsuleCollider != null)
        {
            float compensacion = 2f * capsuleCollider.offset.x * transform.localScale.x;
            transform.position += new Vector3(compensacion, 0f, 0f);
        }

        Vector3 escala = transform.localScale;
        escala.x = escalaObjetivo;
        transform.localScale = escala;
    }

    void ActualizarAnimaciones()
    {
        if (animator == null) return;
        float velocidadAbs = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", velocidadAbs);
    }

    float VariarTiempo(float tiempo)
    {
        float variacion = tiempo * (variacionTiempos / 100f);
        return tiempo + Random.Range(-variacion, variacion);
    }

    public bool EstaAtacando() => estadoActual == Estado.Atacar;

    void OnDrawGizmosSelected()
    {
        float dir = Application.isPlaying ? direccion : (transform.localScale.x > 0 ? 1f : -1f);

        // Rango de detección frontal (solo hacia delante)
        Gizmos.color = Color.yellow;
        Vector3 centroFrontal = transform.position + new Vector3(dir * rangoDeteccion / 2f, 0f, 0f);
        Gizmos.DrawWireCube(centroFrontal, new Vector3(rangoDeteccion, alturaDeteccion * 2f, 0f));

        // Rango de detección trasera (solo hacia atrás)
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Vector3 centroTrasero = transform.position + new Vector3(-dir * rangoDeteccionTrasera / 2f + offsetDeteccionTrasera.x, offsetDeteccionTrasera.y, 0f);
        Gizmos.DrawWireCube(centroTrasero, new Vector3(rangoDeteccionTrasera, alturaDeteccion * 2f, 0f));

        // Rango de ataque
        Gizmos.color = Color.red;
        Vector2 posAtaque = (Vector2)transform.position + offsetAtaque;
        Gizmos.DrawWireSphere(posAtaque, rangoAtaque);

        // Detector de borde
        Gizmos.color = Color.green;
        Vector2 posicionBorde = (Vector2)transform.position + new Vector2(dir * distanciaDeteccionPared, 0);
        Gizmos.DrawLine(posicionBorde, posicionBorde + Vector2.down * distanciaDeteccionBorde);
    }
}
