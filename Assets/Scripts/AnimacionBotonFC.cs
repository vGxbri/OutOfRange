using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AnimacionBotonFC : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    [Header("Configuración")]
    public RectTransform lineaSubrayado;
    public float velocidadAnimacion = 10f;

    [Header("Opcional: Cambio de Color Texto")]
    public TextMeshProUGUI textoBoton;
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = Color.yellow;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoHover;
    public AudioClip sonidoClick;

    private float anchoObjetivo = 0f;

    void Start()
    {
        if (lineaSubrayado != null)
        {
            lineaSubrayado.localScale = new Vector3(0, 1, 1);

            if (lineaSubrayado.sizeDelta.y < 4f)
            {
                lineaSubrayado.sizeDelta = new Vector2(lineaSubrayado.sizeDelta.x, 4f);
            }
        }

        if (textoBoton != null) colorNormal = textoBoton.color;
        
        // Autocompletar AudioSource si no está asignado
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        anchoObjetivo = 0f;
        if (lineaSubrayado != null)
        {
            lineaSubrayado.localScale = new Vector3(0, 1, 1);
        }
        if (textoBoton != null) textoBoton.color = colorNormal;
    }

    void Update()
    {
        if (lineaSubrayado != null)
        {
            float nuevoX = Mathf.Lerp(lineaSubrayado.localScale.x, anchoObjetivo, Time.unscaledDeltaTime * velocidadAnimacion);
            lineaSubrayado.localScale = new Vector3(nuevoX, 1, 1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivarEfecto();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DesactivarEfecto();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ActivarEfecto();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DesactivarEfecto();
    }

    // --- DETECTAR EL CLICK ---
    public void OnPointerClick(PointerEventData eventData)
    {
        ReproducirClick();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ReproducirClick();
    }

    void ActivarEfecto()
    {
        anchoObjetivo = 1f;
        if (textoBoton != null) textoBoton.color = colorSeleccionado;
        
        if (audioSource != null && sonidoHover != null)
        {
            audioSource.PlayOneShot(sonidoHover);
        }
    }

    void DesactivarEfecto()
    {
        anchoObjetivo = 0f;
        if (textoBoton != null) textoBoton.color = colorNormal;
    }

    void ReproducirClick()
    {
        if (audioSource != null && sonidoClick != null)
        {
            audioSource.PlayOneShot(sonidoClick);
        }
    }
}