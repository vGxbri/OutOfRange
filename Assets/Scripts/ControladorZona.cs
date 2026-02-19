using UnityEngine;

public class ControladorZona : MonoBehaviour
{
    // Opciones para elegir en el inspector
    public enum FormaZona { Circulo, Cuadrado }

    [Header("Configuración")]
    public FormaZona formaActual = FormaZona.Circulo;
    public float radio = 5f;               // Tamaño si es círculo
    public Vector2 tamanoBox = new Vector2(8, 6); // Tamaño si es cuadrado

    [Header("Referencias (Arrastra aquí)")]
    public CircleCollider2D colCirculo;
    public BoxCollider2D colCuadrado;
    public Transform visuales; // El objeto hijo que tiene el Sprite

    // Variables privadas (el juego las rellena solas)
    private GameObject jugadorVictima; 

    // --- FUNCIÓN PRINCIPAL DE CONFIGURACIÓN ---
    // Esta función la llamará el GameManager al arrancar
    public void ConfigurarZona(GameObject dueño, GameObject victima)
    {
        // 1. Guardamos quién va a morir si sale
        jugadorVictima = victima;

        // 2. Nos pegamos al dueño (nos hacemos sus hijos)
        transform.SetParent(dueño.transform);
        transform.localPosition = Vector3.zero; // Nos centramos en él

        // 3. Aplicamos la forma y tamaño correctos
        ActualizarForma();
    }

    [Header("Visuales")]
    public LineRenderer lineaBorde;
    public Color colorRelleno = new Color(1, 0, 0, 0.12f);
    public Color colorBorde = new Color(1, 0, 0, 0.5f);
    public float anchoLinea = 0.01f;
    public float tamanoGuion = 1f;
    public float espacioGuion = 1f;

    public string nombreCapaOrden = "Zona Jugador";
    public int ordenCapa = 10;

    // ... (rest of the code)

    void DibujarCirculo()
    {
        if (!lineaBorde) return;
        if (_materialCache == null) ConfigurarLineRenderer();
        if (lineaBorde.sharedMaterial != _materialCache) lineaBorde.sharedMaterial = _materialCache;

        int segmentos = 60;
        lineaBorde.positionCount = segmentos;
        
        float angulo = 0f;
        for (int i = 0; i < segmentos; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angulo) * radio;
            float y = Mathf.Cos(Mathf.Deg2Rad * angulo) * radio;
            
            lineaBorde.SetPosition(i, new Vector3(x, y, 0));
            angulo += (360f / segmentos);
        }
        
        // Ajustar tiling para dashes perfectos
        float densidad = 8f; 
        if(_materialCache) _materialCache.mainTextureScale = new Vector2(densidad, 1); 
    }

    void DibujarCuadrado()
    {
        if (!lineaBorde) return;
        if (_materialCache == null) ConfigurarLineRenderer();
        if (lineaBorde.sharedMaterial != _materialCache) lineaBorde.sharedMaterial = _materialCache;

        lineaBorde.positionCount = 4;
        
        float w = tamanoBox.x / 2;
        float h = tamanoBox.y / 2;
        
        // Esquinas (sentido horario o antihorario)
        lineaBorde.SetPosition(0, new Vector3(-w, -h, 0));
        lineaBorde.SetPosition(1, new Vector3(-w, h, 0));
        lineaBorde.SetPosition(2, new Vector3(w, h, 0));
        lineaBorde.SetPosition(3, new Vector3(w, -h, 0));
        
        // Tiling perfecto
        float densidad = 8f;
        if(_materialCache) _materialCache.mainTextureScale = new Vector2(densidad, 1);
    }

    // Material caché para evitar instanciar copias constantemente
    private Material _materialCache;
    private Texture2D texturaLinea;

    private void Awake()
    {
        // Corregir valores antiguos
        if (string.IsNullOrEmpty(nombreCapaOrden)) nombreCapaOrden = "Zona Jugador";
        if (ordenCapa == 0) ordenCapa = 10;

        if (lineaBorde == null)
        {
            lineaBorde = GetComponent<LineRenderer>();
            if (lineaBorde == null) lineaBorde = gameObject.AddComponent<LineRenderer>();
        }

        ConfigurarLineRenderer();
        ActualizarForma();
    }

    void ConfigurarLineRenderer()
    {
        lineaBorde.loop = true;
        lineaBorde.useWorldSpace = false;
        lineaBorde.startWidth = anchoLinea;
        lineaBorde.endWidth = anchoLinea;
        
        lineaBorde.sortingLayerName = nombreCapaOrden;
        lineaBorde.sortingOrder = ordenCapa;

        // Limpieza de material previo si existe
        if (_materialCache == null)
        {
            _materialCache = new Material(Shader.Find("Custom/DashedLine"));
        }
        lineaBorde.sharedMaterial = _materialCache;
        
        // Configuración para dashes consistentes
        lineaBorde.textureMode = LineTextureMode.Tile; 

        // Textura: 16px (8 white, 8 clear)
        if (texturaLinea == null)
        {
            texturaLinea = new Texture2D(16, 1);
            texturaLinea.filterMode = FilterMode.Point;
            texturaLinea.wrapMode = TextureWrapMode.Repeat;
            for (int i = 0; i < 16; i++) {
                texturaLinea.SetPixel(i, 0, (i < 8) ? Color.white : Color.clear);
            }
            texturaLinea.Apply();
        }
        
        _materialCache.mainTexture = texturaLinea;
        _materialCache.color = colorBorde;
    }

    void ActualizarForma()
    {
        if (_materialCache == null) ConfigurarLineRenderer();

        // Apagamos todo primero para no tener bugs
        if(colCirculo) colCirculo.enabled = false;
        if(colCuadrado) colCuadrado.enabled = false;

        // Asegurar configuración del borde
        if (lineaBorde) 
        {
             lineaBorde.startWidth = anchoLinea;
             lineaBorde.endWidth = anchoLinea;
             lineaBorde.sortingLayerName = nombreCapaOrden;
             lineaBorde.sortingOrder = ordenCapa;
             
             if(_materialCache != null) _materialCache.color = colorBorde;
        }

        // Actualizar color de relleno del Sprite
        if (visuales)
        {
            SpriteRenderer sr = visuales.GetComponent<SpriteRenderer>();
            if (sr == null) sr = visuales.gameObject.AddComponent<SpriteRenderer>();
            
            sr.color = colorRelleno;
            // Asegurar que el relleno esté por debajo del borde
            sr.sortingLayerName = nombreCapaOrden;
            sr.sortingOrder = ordenCapa - 1; 

            // Generar y asignar sprite procedimental
            if (formaActual == FormaZona.Circulo)
            {
                sr.sprite = GenerarSpriteCirculo();
            }
            else
            {
                sr.sprite = GenerarSpriteCuadrado();
            }
        }

        if (formaActual == FormaZona.Circulo)
        {
            // FÍSICA
            colCirculo.enabled = true;
            colCirculo.radius = radio;
            
            // VISUAL (El círculo se escala por Diámetro = Radio * 2)
            if(visuales) visuales.localScale = new Vector3(radio * 2, radio * 2, 1);

            // BORDE (Círculo)
            DibujarCirculo();
        }
        else // Cuadrado
        {
            // FÍSICA
            colCuadrado.enabled = true;
            colCuadrado.size = tamanoBox;

            // VISUAL
            if(visuales) visuales.localScale = new Vector3(tamanoBox.x, tamanoBox.y, 1);

            // BORDE (Cuadrado)
            DibujarCuadrado();
        }
    }

    Sprite GenerarSpriteCirculo()
    {
        int res = 256; // Resolución de la textura
        Texture2D tex = new Texture2D(res, res);
        tex.filterMode = FilterMode.Bilinear; // Suave
        tex.wrapMode = TextureWrapMode.Clamp;
        
        Color[] colors = new Color[res * res];
        float center = res / 2f;
        float radius = res / 2f;
        float radiusSqr = radius * radius;

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distSqr = dx*dx + dy*dy;
                
                // Antialiasing simple
                float alpha = 1f;
                if (distSqr > radiusSqr) 
                    alpha = 0f;
                else if (distSqr > (radius - 1) * (radius - 1)) // Borde suave de 1 pixel
                    alpha = Mathf.Clamp01(radius - Mathf.Sqrt(distSqr));

                colors[y * res + x] = new Color(1, 1, 1, alpha);
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        // Crear sprite. Pivot en el centro (0.5, 0.5). PixelsPerUnit = res para que mida 1x1 unidad.
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    Sprite GenerarSpriteCuadrado()
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.filterMode = FilterMode.Point;
        Color[] colors = new Color[4] {Color.white, Color.white, Color.white, Color.white};
        tex.SetPixels(colors);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 2);
    }



    /*
    // --- LÓGICA DE MUERTE ---
    private void OnTriggerExit2D(Collider2D other)
    {
        // Si todavía no se ha configurado la partida, ignorar
        if (jugadorVictima == null) return;

        // Si lo que sale es LA VÍCTIMA (y no el dueño ni una pared)
        if (other.gameObject == jugadorVictima)
        {
            Debug.Log("¡El jugador ha salido de la zona! Muriendo...");
            Destroy(jugadorVictima); 
            // Aquí luego pondrás tu lógica de Fin de Partida
        }
    }
    */
}