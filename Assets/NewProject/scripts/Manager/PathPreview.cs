using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public Vector3 mousePositionWorld; // Posici�n del rat�n proyectada al espacio mundial
    public Vector3 playerPosition; // Posici�n actual del jugador
    public Vector3 positionDesired; // La posici�n objetivo deseada
    [SerializeField] private LineRenderer lineRenderer1; // Primer LineRenderer
    [SerializeField] private LineRenderer lineRenderer2; // Segundo LineRenderer
    [SerializeField] private LayerMask groundLayer; // Capa para determinar la superficie del suelo
    private float range = 5f; // Rango m�ximo de movimiento para el jugador
    private PlayerBase playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerBase>();
        playerPosition = transform.position; // Inicializar la posici�n del jugador
        lineRenderer1.enabled = false; // Comenzar con el primer LineRenderer deshabilitado
        lineRenderer2.enabled = false; // Comenzar con el segundo LineRenderer deshabilitado
    }

    void Update()
    {
        // Actualizar el rango desde PlayerBase
        range = playerStats.GetRange();

        // Actualizar la posici�n del jugador
        playerPosition = transform.position;

        // Proyectar la posici�n del rat�n al espacio mundial usando un rayo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            mousePositionWorld = hit.point; // Obtener la posici�n del rat�n en el espacio mundial
        }

        // Calcular la distancia entre el jugador y el punto del rat�n
        float distanceToMouse = Vector3.Distance(playerPosition, mousePositionWorld);

        // Limitar la posici�n deseada seg�n el rango
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

        // Ocultar las l�neas si se suelta el bot�n izquierdo (opcional)
        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer1.enabled = false;
            lineRenderer2.enabled = false;
        }
    }

    // Funci�n para mostrar la vista previa del camino con ambos LineRenderers
    void ShowPathPreview()
    {
        lineRenderer1.enabled = true; // Habilitar el primer LineRenderer
        lineRenderer2.enabled = true; // Habilitar el segundo LineRenderer

        // Configurar ambos LineRenderers para mostrar la l�nea desde el jugador hasta la posici�n deseada
        lineRenderer1.positionCount = 2; // Dos puntos para una l�nea recta
        lineRenderer1.SetPosition(0, playerPosition); // Punto inicial (posici�n del jugador)
        lineRenderer1.SetPosition(1, positionDesired); // Punto final (posici�n deseada)

        lineRenderer2.positionCount = 2; // Dos puntos para una l�nea recta
        //lineRenderer2.SetPosition(0, playerPosition); // Punto inicial (posici�n del jugador)
        lineRenderer2.SetPosition(1, positionDesired); // Punto final (posici�n deseada)
    }
}
