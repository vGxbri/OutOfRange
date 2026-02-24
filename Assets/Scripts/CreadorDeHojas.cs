using UnityEngine;

public class CreadorDeHojas : MonoBehaviour
{
    public GameObject prefabHoja;
    public float tiempoEntreHojas = 0.5f;

    [Header("Organización")]
    public Transform contenedorHojas; // Arrastra aquí el objeto padre

    void Start()
    {
        // Si olvidaste asignar un contenedor, usaremos este mismo objeto como padre
        if (contenedorHojas == null) contenedorHojas = this.transform;

        InvokeRepeating("SoltarHoja", 0, tiempoEntreHojas);
    }

    void SoltarHoja()
    {
        float xAleatorio = Random.Range(-10f, 10f);
        Vector3 pos = new Vector3(transform.position.x + xAleatorio, transform.position.y, 0);

        // Al añadir 'contenedorHojas' al final, se crean directamente ahí dentro
        Instantiate(prefabHoja, pos, Quaternion.identity, contenedorHojas);
    }
}