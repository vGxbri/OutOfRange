// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using System.Collections;

public class ControladorZona : MonoBehaviour
{
    public enum FormaZona { Circulo, Cuadrado }

    [Header("Configuración")]
    public FormaZona formaActual = FormaZona.Circulo;
    public float radio = 5f;
    public Vector2 tamanoBox = new Vector2(8, 6);

    [Header("Referencias (Arrastra aquí)")]
    public CircleCollider2D colCirculo;
    public BoxCollider2D colCuadrado;
    public Transform visuales;

    private GameObject jugadorVictima; 

    public void ConfigurarZona(GameObject dueño, GameObject victima)
    {
        jugadorVictima = victima;

        transform.SetParent(dueño.transform);
        transform.localPosition = Vector3.zero;

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

        float perimetro = 2f * Mathf.PI * radio;
        
        float densidad = perimetro / espacioGuion;
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
        
        lineaBorde.SetPosition(0, new Vector3(-w, -h, 0));
        lineaBorde.SetPosition(1, new Vector3(-w, h, 0));
        lineaBorde.SetPosition(2, new Vector3(w, h, 0));
        lineaBorde.SetPosition(3, new Vector3(w, -h, 0));
        
        float perimetro = (tamanoBox.x * 2f) + (tamanoBox.y * 2f);
        float densidad = perimetro / espacioGuion;
        if(_materialCache) _materialCache.mainTextureScale = new Vector2(densidad, 1);
    }

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
        lineaBorde.numCornerVertices = 4; // Añade geometría en las esquinas para suavizar el giro
        lineaBorde.numCapVertices = 4;    // Suaviza los extremos de los guiones
        
        lineaBorde.sortingLayerName = nombreCapaOrden;
        lineaBorde.sortingOrder = ordenCapa;

        // Limpieza de material previo si existe
        if (_materialCache == null)
        {
            Shader shader = Shader.Find("Custom/DashedLine");
            if (shader == null)
            {
                // Si falla (por ejemplo, en build porque se omitió el shader),
                // intentamos usar un shader basico para evitar errores NullReference.
                Debug.LogWarning("Shader Custom/DashedLine no encontrado. Usando Sprites/Default como fallback.");
                shader = Shader.Find("Sprites/Default");
            }
            _materialCache = new Material(shader);
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
            // Sumamos el anchoLinea para que el relleno muerda el borde y no queden huecos
            float escalaReal = (radio * 2) + anchoLinea;
            if(visuales) visuales.localScale = new Vector3(escalaReal, escalaReal, 1);

            // BORDE (Círculo)
            DibujarCirculo();
        }
        else // Cuadrado
        {
            // FÍSICA
            colCuadrado.enabled = true;
            colCuadrado.size = tamanoBox;

            // VISUAL
            if(visuales) visuales.localScale = new Vector3(tamanoBox.x + anchoLinea, tamanoBox.y + anchoLinea, 1);

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


    // --- LÓGICA DE DAÑO FUERA DE ZONA ---
    [Header("Daño Fuera de Zona")]
    public float primerDañoDelay = 0.5f;
    public float intervaloDaño = 1.5f;
    public int dañoPorTick = 1;

    private bool victimaFuera = false;
    private Coroutine corrutinaDaño;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!gameObject.activeInHierarchy) return;
        if (jugadorVictima == null) return;
        if (other.gameObject != jugadorVictima) return;

        victimaFuera = true;
        if (corrutinaDaño == null)
            corrutinaDaño = StartCoroutine(DañoFueraDeZona());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jugadorVictima == null) return;
        if (other.gameObject != jugadorVictima) return;

        victimaFuera = false;
        if (corrutinaDaño != null)
        {
            StopCoroutine(corrutinaDaño);
            corrutinaDaño = null;
        }
    }

    IEnumerator DañoFueraDeZona()
    {
        // Primer daño rápido (500ms)
        yield return new WaitForSeconds(primerDañoDelay);

        while (victimaFuera)
        {
            if (VidaCompartida.Instancia != null)
                VidaCompartida.Instancia.RecibirDaño(dañoPorTick);

            // Hit visual en la víctima
            MovimientoJugador mov = jugadorVictima.GetComponent<MovimientoJugador>();
            if (mov != null) mov.RecibirHit();

            yield return new WaitForSeconds(intervaloDaño);
        }

        corrutinaDaño = null;
    }
}