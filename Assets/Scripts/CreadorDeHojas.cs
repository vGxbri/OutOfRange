// Gabriel Francisque Almarcha Martínez
// Jorge Maqueda Miguel

using UnityEngine;

public class CreadorDeHojas : MonoBehaviour
{
    public GameObject prefabHoja;
    public float tiempoEntreHojas = 0.5f;

    [Header("Organización")]
    public Transform contenedorHojas;

    void Start()
    {
        if (contenedorHojas == null) contenedorHojas = this.transform;

        InvokeRepeating("SoltarHoja", 0, tiempoEntreHojas);
    }

    void SoltarHoja()
    {
        float xAleatorio = Random.Range(-10f, 10f);
        Vector3 pos = new Vector3(transform.position.x + xAleatorio, transform.position.y, 0);

        Instantiate(prefabHoja, pos, Quaternion.identity, contenedorHojas);
    }
}