using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public Vector3 mousePosition; // Posición del ratón en el espacio mundial
    public Vector3 playerPosition; // Posición actual del jugador
    public Vector3 positionDesired; // La posición objetivo deseada
    [SerializeField] private LineRenderer lineRenderer; // Referencia al LineRenderer para dibujar la vista previa del camino
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
        // Actualizar el rango desde el PlayerBase
        range = playerStats.GetRange();

        // Actualizar la posición del jugador
        playerPosition = transform.position;

        // Obtener la posición del ratón en el espacio mundial
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        mousePosition.z = 0; // Mantener en el plano 2D

        // Calcular la distancia entre el jugador y el ratón
        float distanceToMouse = Vector3.Distance(playerPosition, mousePosition);

        // Limitar la posición deseada según el rango
        if (distanceToMouse > range)
        {
            Vector3 direction = (mousePosition - playerPosition).normalized;
            positionDesired = playerPosition + direction * range;
        }
        else
        {
            positionDesired = mousePosition;
        }

        // Mostrar la vista previa del camino
        ShowPathPreview();

        // Comenzar la vista previa al hacer clic con el botón izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            positionDesired = mousePosition; // Guardar la posición deseada al hacer clic
        }

        // Actualizar la vista previa mientras se arrastra el ratón
        if (Input.GetMouseButton(0))
        {
            ShowPathPreview(); // Continuar mostrando la vista previa
        }

        // Liberar el botón del ratón (opcional: puedes deshabilitar la vista previa aquí)
        if (Input.GetMouseButtonUp(0))
        {
            // Mantener la vista previa visible o realizar otras acciones al soltar el ratón
        }
    }

    // Función para mostrar la vista previa del camino con el LineRenderer
    void ShowPathPreview()
    {
        lineRenderer.enabled = true; // Asegurarse de que el LineRenderer esté habilitado

        // Configurar el LineRenderer para mostrar la línea desde el jugador hasta la posición deseada
        lineRenderer.positionCount = 2; // Dos puntos para una línea recta
        lineRenderer.SetPosition(0, playerPosition); // Punto inicial (posición del jugador)
        lineRenderer.SetPosition(1, positionDesired); // Punto final (posición deseada)
    }
}
