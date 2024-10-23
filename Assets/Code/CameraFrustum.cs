using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFrustumVisualizer : MonoBehaviour
{
    public float fieldOfViewAngle = 45f; // �ngulo del cono
    public float visionDistance = 10f;    // Distancia del cono
    public int segments = 50;              // Cu�ntos segmentos tendr� el arco
    private LineRenderer lineRenderer;

    void Start()
    {
        // Inicializar LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 2; // +2 para cerrar el cono
        lineRenderer.useWorldSpace = true; // Usar espacio mundial

        // Configurar propiedades del LineRenderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.red; // Color del cono de visi�n
        lineRenderer.endColor = Color.red;

        // Dibujar el frustum inicial
        DrawFrustum();
    }

    void Update()
    {
        // Actualizar el frustum en cada frame
        DrawFrustum();
    }

    // M�todo para dibujar el frustum en frente de la c�mara
    private void DrawFrustum()
    {
        Vector3[] frustumPositions = new Vector3[segments + 2];

        // Establecer la posici�n de la c�mara como el primer punto
        frustumPositions[0] = transform.position;

        // Calcular los l�mites del campo de visi�n
        float halfAngle = fieldOfViewAngle / 2;
        Vector3 leftLimit = Quaternion.Euler(0, -halfAngle, 0) * transform.forward * visionDistance;
        Vector3 rightLimit = Quaternion.Euler(0, halfAngle, 0) * transform.forward * visionDistance;

        // Dibujar el arco
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            Vector3 point = Quaternion.Euler(0, angle, 0) * transform.forward * visionDistance;
            frustumPositions[i + 1] = transform.position + point;
        }

        // Conectar el �ltimo punto al enemigo
        frustumPositions[segments + 1] = transform.position;

        // Configurar el LineRenderer para dibujar el cono
        lineRenderer.positionCount = segments + 2; // +2 para cerrar el cono
        lineRenderer.SetPositions(frustumPositions);
    }
}
