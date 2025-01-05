using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public Vector3 mousePositionWorld; // Posición del ratón proyectada al espacio mundial
    public Vector3 playerPosition; // Posición actual del jugador
    public Vector3 positionDesired; // La posición objetivo deseada
    [SerializeField] private LineRenderer lineRenderer; // Referencia al LineRenderer para dibujar la vista previa del camino
    [SerializeField] private LayerMask groundLayer; // Capa para determinar la superficie del suelo
    private float range = 5f; // Rango máximo de movimiento para el jugador
    private PlayerBase playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerBase>();
        playerPosition = transform.position; // Inicializar la posición del jugador
        lineRenderer.enabled = false; // Comenzar con el LineRenderer deshabilitado
    }

    void Update()
    {

        // Actualizar el rango desde PlayerBase
        range = playerStats.GetRange();

        // Actualizar la posición del jugador
        playerPosition = transform.position;

        // Proyectar la posición del ratón al espacio mundial usando un rayo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            mousePositionWorld = hit.point; // Obtener la posición del ratón en el espacio mundial
        }

        // Calcular la distancia entre el jugador y el punto del ratón
        float distanceToMouse = Vector3.Distance(playerPosition, mousePositionWorld);

        // Limitar la posición deseada según el rango
        if (distanceToMouse > range)
        {
            Vector3 direction = (mousePositionWorld - playerPosition).normalized;
            positionDesired = playerPosition + direction * range;
        }
        else
        {
            positionDesired = mousePositionWorld;
        }

        // Mostrar la vista previa al arrastrar o al hacer clic
        if (Input.GetMouseButton(0))
        {
            ShowPathPreview();
        }

        // Ocultar la línea si se suelta el botón izquierdo (opcional)
        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;
        }
    }

    // Función para mostrar la vista previa del camino con el LineRenderer
    void ShowPathPreview()
    {
        lineRenderer.enabled = true; // Habilitar el LineRenderer

        // Configurar el LineRenderer para mostrar la línea desde el jugador hasta la posición deseada
        lineRenderer.positionCount = 2; // Dos puntos para una línea recta
        lineRenderer.SetPosition(0, playerPosition); // Punto inicial (posición del jugador)
        lineRenderer.SetPosition(1, positionDesired); // Punto final (posición deseada)
    }
}
