using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyWandering : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform centrePoint;
    public Transform player;
    public float range;
    public float detectionRadius = 10f;

    // Nuevas variables para el cono de visión
    public float fieldOfViewAngle = 45f; // Ángulo del cono
    public float visionDistance = 10f;    // Distancia del cono
    private LineRenderer lineRenderer;
    public int segments = 50; // Cuántos puntos tendrá el arco

    bool founded = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Inicializar LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 2; // +2 para cerrar el cono
        lineRenderer.useWorldSpace = true; // Usar espacio mundial para la LineRenderer

        // Configurar propiedades del LineRenderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // Dibujar el círculo y el frustum inicial
        DrawDetectionRadius();
    }

    void Update()
    {
        float distancePlayer = Vector3.Distance(player.position, transform.position);

        if ((distancePlayer <= detectionRadius && IsPlayerInFrustum()) || founded)
        {
            FollowPlayer();
            // Comunicar a otros zombis que han detectado al jugador
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                go.BroadcastMessage("OnPlayerDetected", player.position, SendMessageOptions.DontRequireReceiver);
            }
        }

        if(!founded)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector3 punto;
                if (TryGetRandomPoint(centrePoint.position, range, out punto))
                {
                    agent.SetDestination(punto);
                }
            }
        }
        

        // Actualizar el círculo y el frustum
        DrawDetectionRadius();
        DrawFrustum();
    }

    bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 point)
    {
        Vector3 potentialPoint = center + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(potentialPoint, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
        {
            point = navHit.position;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    void FollowPlayer()
    {
        agent.destination = player.position;
    }

    // Método para verificar si el jugador está dentro del frustum
    private bool IsPlayerInFrustum()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfViewAngle / 2)
        {
            if (directionToPlayer.magnitude <= visionDistance)
            {
                return true;
            }
        }
        return false;
    }

    // Método para dibujar el círculo del radio de detección
    void DrawDetectionRadius()
    {
        float angle = 360f / segments;
        Vector3[] positions = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (i * angle);
            float x = Mathf.Sin(rad) * detectionRadius;
            float z = Mathf.Cos(rad) * detectionRadius;
            positions[i] = new Vector3(x, 0, z);
        }

        lineRenderer.SetPositions(positions);
    }

    // Método para dibujar el frustum en frente del zombie desde una vista superior
    private void DrawFrustum()
    {
        Vector3[] frustumPositions = new Vector3[segments + 2];

        // Establecer la posición del enemigo como el primer punto
        frustumPositions[0] = transform.position;

        // Calcular los límites del campo de visión
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

        // Conectar el último punto al enemigo
        frustumPositions[segments + 1] = transform.position;

        // Configurar el LineRenderer para dibujar el cono
        lineRenderer.positionCount = segments + 2; // +2 para cerrar el cono
        lineRenderer.SetPositions(frustumPositions);
    }

    // Método que se llama cuando el jugador es detectado
    private void OnPlayerDetected(Vector3 playerPosition)
    {
        Debug.Log("siguiendo");// Aquí puedes implementar la lógica para que el zombi actúe al ser notificado
         // Por ejemplo, seguir al jugador
        founded = true;
    }

    // Método para dibujar el Gizmo en la escena (Scene View)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (centrePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(centrePoint.position, range);
        }
    }
}
