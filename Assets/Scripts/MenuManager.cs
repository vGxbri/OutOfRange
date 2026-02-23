using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject contenedorBotones;
    public GameObject contenedorControles;

    private bool enControles = false;

    void Start()
    {
        MostrarMenu();
    }

    void Update()
    {
        if (enControles && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")))
        {
            MostrarMenu();
        }
    }

    public void MostrarMenu()
    {
        contenedorBotones.SetActive(true);
        contenedorControles.SetActive(false);
        enControles = false;
    }

    public void MostrarControles()
    {
        contenedorBotones.SetActive(false);
        contenedorControles.SetActive(true);
        enControles = true;
    }
}
