using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necesario para detectar el ratón/selección
using TMPro;

public class AnimacionBotonFC : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Configuración")]
    public RectTransform lineaSubrayado; // Arrastra aquí la imagen de la línea
    public float velocidadAnimacion = 10f; // Qué tan rápido se abre la línea

    [Header("Opcional: Cambio de Color Texto")]
    public TextMeshProUGUI textoBoton;
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = Color.yellow; // O el color que quieras

    private float anchoObjetivo = 0f; // 0 = invisible, 1 = ancho completo

    void Start()
    {
        // Al empezar, nos aseguramos de que la línea no se vea
        if (lineaSubrayado != null)
        {
            lineaSubrayado.localScale = new Vector3(0, 1, 1);
        }

        // Guardamos el color original si asignaste el texto
        if (textoBoton != null) colorNormal = textoBoton.color;
    }

    void Update()
    {
        if (lineaSubrayado != null)
        {
            // Lerp hace que el cambio sea suave en lugar de instantáneo
            float nuevoX = Mathf.Lerp(lineaSubrayado.localScale.x, anchoObjetivo, Time.deltaTime * velocidadAnimacion);
            lineaSubrayado.localScale = new Vector3(nuevoX, 1, 1);
        }
    }

    // --- EVENTOS DE RATÓN (HOVER) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivarEfecto();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DesactivarEfecto();
    }

    // --- EVENTOS DE MANDO/TECLADO (SELECCIÓN) ---
    public void OnSelect(BaseEventData eventData)
    {
        ActivarEfecto();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DesactivarEfecto();
    }

    // --- LÓGICA ---
    void ActivarEfecto()
    {
        anchoObjetivo = 1f; // Expandir a escala 1
        if (textoBoton != null) textoBoton.color = colorSeleccionado;
    }

    void DesactivarEfecto()
    {
        anchoObjetivo = 0f; // Contraer a escala 0
        if (textoBoton != null) textoBoton.color = colorNormal;
    }
}