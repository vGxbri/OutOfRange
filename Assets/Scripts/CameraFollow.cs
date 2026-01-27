using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Zona Muerta (Libertad de movimiento)")]
    public Vector2 deadZoneSize = new Vector2(3f, 2f);
    public float smoothSpeed = 5f;

    [Header("Límites de Movimiento (Bordes del Mapa)")]
    public float minWidth = -10f; // Límite izquierdo
    public float maxWidth = 50f;  // Límite derecho
    public float minHeight = -2f; // Límite inferior
    public float maxHeight = 10f; // Límite superior

    [Header("Ajustes Extra")]
    public Vector2 offset;

    private Vector3 targetPos;
    private float initialZ;

    void Start()
    {
        if (target == null) return;

        initialZ = transform.position.z;

        // Calculamos posición inicial respetando todos los límites
        Vector3 playerPos = target.position + (Vector3)offset;
        float startX = Mathf.Clamp(playerPos.x, minWidth, maxWidth);
        float startY = Mathf.Clamp(playerPos.y, minHeight, maxHeight);

        transform.position = new Vector3(startX, startY, initialZ);
        targetPos = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 playerPos = target.position + (Vector3)offset;

        // --- LÓGICA DE ZONA MUERTA ---
        float deltaX = playerPos.x - targetPos.x;
        if (Mathf.Abs(deltaX) > deadZoneSize.x)
        {
            targetPos.x += deltaX - (deadZoneSize.x * Mathf.Sign(deltaX));
        }

        float deltaY = playerPos.y - targetPos.y;
        if (Mathf.Abs(deltaY) > deadZoneSize.y)
        {
            targetPos.y += deltaY - (deadZoneSize.y * Mathf.Sign(deltaY));
        }

        // --- APLICAR LÍMITES TOTALES (CLAMP) ---
        float clampedX = Mathf.Clamp(targetPos.x, minWidth, maxWidth);
        float clampedY = Mathf.Clamp(targetPos.y, minHeight, maxHeight);

        Vector3 finalPosition = new Vector3(clampedX, clampedY, initialZ);

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Dibujamos la zona muerta en ROJO
        Gizmos.color = Color.red;
        Vector3 center = Application.isPlaying ? targetPos : transform.position;
        Gizmos.DrawWireCube(center, new Vector3(deadZoneSize.x * 2, deadZoneSize.y * 2, 0));

        // Dibujamos los límites del mapa en AMARILLO (Opcional, muy útil)
        Gizmos.color = Color.yellow;
        Vector3 limitCenter = new Vector3((minWidth + maxWidth) / 2, (minHeight + maxHeight) / 2, 0);
        Vector3 limitSize = new Vector3(maxWidth - minWidth, maxHeight - minHeight, 0);
        Gizmos.DrawWireCube(limitCenter, limitSize);
    }
}