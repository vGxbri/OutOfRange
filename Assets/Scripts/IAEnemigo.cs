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
    public float rangoAtaque = 1.2f;
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
    private int direccion = 1;
    private Transform objetivoActual;

    // Timers
    private float timerEstado;
    private float tiempoUltimoAtaque;
    private float tiempoPerdiendoObjetivo;
    private bool objetivoFueraDeRango = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
    }

    private float ultimoGiroColision;

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.collider.CompareTag("Player")) return;
        if (Time.time - ultimoGiroColision < 0.5f) return;

        bool esPared = ((1 << colision.gameObject.layer) & capaSuelo) != 0;
        IAEnemigo otroEnemigo = colision.collider.GetComponent<IAEnemigo>();

        if (esPared || otroEnemigo != null)
        {
            foreach (ContactPoint2D contacto in colision.contacts)
            {
                if (Mathf.Abs(contacto.normal.x) > 0.5f)
                {
                    GirarPorColision();

                    // Forzar que el otro enemigo también gire
                    if (otroEnemigo != null)
                        otroEnemigo.GirarPorColision();

                    break;
                }
            }
        }
    }

    public void GirarPorColision()
    {
        if (Time.time - ultimoGiroColision < 0.5f) return;
        Girar();
        ultimoGiroColision = Time.time;
        if (estadoActual == Estado.Perseguir)
        {
            objetivoActual = null;
            CambiarEstado(Estado.Patrullar);
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
                    // A veces gira al reanudar la patrulla
                    if (Random.value < 0.3f) Girar();
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
        Collider2D[] jugadores = Physics2D.OverlapCircleAll(transform.position, rangoDeteccion, capaJugador);

        float distanciaMinima = float.MaxValue;
        Transform masCercano = null;

        foreach (var col in jugadores)
        {
            float dist = Vector2.Distance(transform.position, col.transform.position);
            float dirHaciaJugador = col.transform.position.x - transform.position.x;
            bool estaDelante = (dirHaciaJugador * direccion) > 0;

            // Detección direccional: rango completo delante, rango reducido detrás
            if (estaDelante || dist <= rangoDeteccionTrasera)
            {
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
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
        // Detectar borde de plataforma
        Vector2 posicionBorde = (Vector2)transform.position + new Vector2(direccion * distanciaDeteccionPared, 0);
        RaycastHit2D hitSuelo = Physics2D.Raycast(posicionBorde, Vector2.down, distanciaDeteccionBorde, capaSuelo);
        RaycastHit2D hitPared = Physics2D.Raycast(transform.position, Vector2.right * direccion, distanciaDeteccionPared, capaSuelo);

        if (!hitSuelo.collider || hitPared.collider)
        {
            Girar();
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
            // Si hay pared, dejar de perseguir
            if (hitPared.collider)
            {
                objetivoActual = null;
                CambiarEstado(Estado.Patrullar);
                Girar();
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

        // Detectar y dañar jugadores en rango de ataque
        Collider2D[] jugadores = Physics2D.OverlapCircleAll(transform.position, rangoAtaque, capaJugador);
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

    void Girar()
    {
        direccion *= -1;
    }

    void ActualizarDireccionVisual()
    {
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
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
        // Rango de detección frontal
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        // Rango de detección trasera
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, rangoDeteccionTrasera);

        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        // Detector de borde
        Gizmos.color = Color.green;
        float dir = transform.localScale.x > 0 ? 1 : -1;
        Vector2 posicionBorde = (Vector2)transform.position + new Vector2(dir * distanciaDeteccionPared, 0);
        Gizmos.DrawLine(posicionBorde, posicionBorde + Vector2.down * distanciaDeteccionBorde);
    }
}
