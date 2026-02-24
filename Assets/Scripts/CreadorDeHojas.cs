using UnityEngine;
public class CreadorDeHojas : MonoBehaviour
{
    public GameObject prefabHoja;
    public float tiempoEntreHojas = 0.5f;

    void Start()
    {
        InvokeRepeating("SoltarHoja", 0, tiempoEntreHojas);
    }

    void SoltarHoja()
    {
        float xAleatorio = Random.Range(-10f, 10f); // Ajusta según el ancho de tu nivel
        Vector3 pos = new Vector3(transform.position.x + xAleatorio, transform.position.y, 0);
        Instantiate(prefabHoja, pos, Quaternion.identity);
    }
}