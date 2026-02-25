using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivos")]
    public List<Transform> targets = new List<Transform>();

    [Header("Configuraci�n")]
    public float smoothSpeed = 5f;
    public Vector2 offset;

    [Header("L�mites del Mapa")]
    public float minWidth = -10f;
    public float maxWidth = 50f;
    public float minHeight = -2f;
    public float maxHeight = 10f;

    private Camera cam;
    private float initialZ;
    private bool primeraVez = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        initialZ = transform.position.z;

        // Intento inicial de buscar jugadores si ya están en la escena
        EncontrarJugadores();
    }

    void LateUpdate()
    {
        if (!GameManager.JuegoIniciado) return;

        // Si no hay nadie a quien seguir, buscamos. Si sigue sin haber nadie, paramos.
        if (targets.Count == 0 || targets[0] == null)
        {
            EncontrarJugadores();
            return;
        }

        // 1. Calcular punto medio
        Vector3 centerPoint = GetCenterPoint();
        Vector3 targetPos = centerPoint + (Vector3)offset;

        // 2. Límites de la cámara
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // IMPORTANTE: Si maxWidth es muy pequeño, el Clamp puede bloquear la cámara.
        // Asegúrate de que (maxWidth - minWidth) sea mayor que el ancho de la cámara.
        float clampedX = Mathf.Clamp(targetPos.x, minWidth + camWidth, maxWidth - camWidth);
        float clampedY = Mathf.Clamp(targetPos.y, minHeight + camHeight, maxHeight - camHeight);

        Vector3 finalPosition = new Vector3(clampedX, clampedY, initialZ);

        // 3. Movimiento
        if (primeraVez)
        {
            transform.position = finalPosition;
            primeraVez = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
        }
    }

    private void EncontrarJugadores()
    {
        // Busca cualquier objeto que tenga el script MovimientoJugador
        MovimientoJugador[] jugadores = FindObjectsOfType<MovimientoJugador>();
        foreach (var j in jugadores)
        {
            if (!targets.Contains(j.transform)) targets.Add(j.transform);
        }
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null) bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    public void AddTarget(Transform newTarget)
    {
        if (targets == null) targets = new List<Transform>(); // Evita error de lista nula
        if (!targets.Contains(newTarget))
        {
            targets.Add(newTarget);
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar una caja roja para visualizar los límites en los que se puede mover la cámara (su centro)
        Gizmos.color = Color.red;
        
        // El ancho total jugable es (maxWidth - minWidth) y la altura es (maxHeight - minHeight)
        float width = maxWidth - minWidth;
        float height = maxHeight - minHeight;
        Vector3 center = new Vector3(minWidth + (width / 2f), minHeight + (height / 2f), transform.position.z);
        
        Gizmos.DrawWireCube(center, new Vector3(width, height, 0));
    }
}