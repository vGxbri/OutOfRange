// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class CreditosScroll : MonoBehaviour
{
    [Header("Configuración de Velocidad")]
    public float velocidadNormal = 50f;
    public float multiplicadorVelocidadRapida = 3f;

    [Header("Referencias UI")]
    public RectTransform contenidoCreditos; 
    public RectTransform mascaraVisible;    

    [Header("Eventos")]
    public UnityEvent AlTerminarCreditos;

    private float posicionActualY;
    private float limiteSuperior;
    private bool animando = false;

    void OnEnable()
    {
        animando = false;
        StartCoroutine(PrepararYEmpezar());
    }

    IEnumerator PrepararYEmpezar()
    {
        if (contenidoCreditos == null || mascaraVisible == null) yield break;

        contenidoCreditos.anchoredPosition = new Vector2(contenidoCreditos.anchoredPosition.x, -10000f);

        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(contenidoCreditos);
        limiteSuperior = contenidoCreditos.rect.height;
        
        posicionActualY = -mascaraVisible.rect.height;
        contenidoCreditos.anchoredPosition = new Vector2(contenidoCreditos.anchoredPosition.x, posicionActualY);
        
        animando = true;
    }

    void Update()
    {
        if (!animando || contenidoCreditos == null) return;

        float velocidadActual = velocidadNormal;
        if (Input.GetButton("Submit") || Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            velocidadActual *= multiplicadorVelocidadRapida;
        }

        posicionActualY += velocidadActual * Time.unscaledDeltaTime; 
        contenidoCreditos.anchoredPosition = new Vector2(contenidoCreditos.anchoredPosition.x, posicionActualY);

        if (posicionActualY >= limiteSuperior)
        {
            TerminarCreditos();
        }

        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape))
        {
            TerminarCreditos();
        }
    }

    public void TerminarCreditos()
    {
        if (animando)
        {
            animando = false;
            AlTerminarCreditos.Invoke();
        }
    }
}
