using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivos")]
    public List<Transform> targets = new List<Transform>();

    [Header("Configuración")]
    public float smoothSpeed = 5f;
    public Vector2 offset;

    [Header("Límites del Mapa")]
    public float minWidth = -10f;
    public float maxWidth = 50f;
    public float minHeight = -2f;
    public float maxHeight = 10f;

    private Camera cam;
    private float initialZ;

    void Start()
    {
        cam = GetComponent<Camera>();
        initialZ = transform.position.z;

        // Intento inicial de buscar jugadores si ya están en la escena
        EncontrarJugadores();
    }

    void LateUpdate()
    {
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
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
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
}