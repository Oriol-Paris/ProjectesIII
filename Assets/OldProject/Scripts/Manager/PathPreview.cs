using UnityEngine;

public class PathPreview : MonoBehaviour
{
    public Vector3 mousePosition; // Posici�n del rat�n en el espacio mundial
    public Vector3 playerPosition; // Posici�n actual del jugador
    public Vector3 positionDesired; // La posici�n objetivo deseada
    [SerializeField] private LineRenderer lineRenderer; // Referencia al LineRenderer para dibujar la vista previa del camino
    private float range = 5f; // Rango m�ximo de movimiento para el jugador
    private PlayerBase playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerBase>();
        playerPosition = transform.position; // Inicializar la posici�n del jugador
        lineRenderer.enabled = false; // Comenzar con el LineRenderer deshabilitado
    }

    void Update()
    {
        // Actualizar el rango desde el PlayerBase
        range = playerStats.GetRange();

        // Actualizar la posici�n del jugador
        playerPosition = transform.position;

        // Obtener la posici�n del rat�n en el espacio mundial
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        mousePosition.z = 0; // Mantener en el plano 2D

        // Calcular la distancia entre el jugador y el rat�n
        float distanceToMouse = Vector3.Distance(playerPosition, mousePosition);

        // Limitar la posici�n deseada seg�n el rango
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

        // Comenzar la vista previa al hacer clic con el bot�n izquierdo del rat�n
        if (Input.GetMouseButtonDown(0))
        {
            positionDesired = mousePosition; // Guardar la posici�n deseada al hacer clic
        }

        // Actualizar la vista previa mientras se arrastra el rat�n
        if (Input.GetMouseButton(0))
        {
            ShowPathPreview(); // Continuar mostrando la vista previa
        }

        // Liberar el bot�n del rat�n (opcional: puedes deshabilitar la vista previa aqu�)
        if (Input.GetMouseButtonUp(0))
        {
            // Mantener la vista previa visible o realizar otras acciones al soltar el rat�n
        }
    }

    // Funci�n para mostrar la vista previa del camino con el LineRenderer
    void ShowPathPreview()
    {
        lineRenderer.enabled = true; // Asegurarse de que el LineRenderer est� habilitado

        // Configurar el LineRenderer para mostrar la l�nea desde el jugador hasta la posici�n deseada
        lineRenderer.positionCount = 2; // Dos puntos para una l�nea recta
        lineRenderer.SetPosition(0, playerPosition); // Punto inicial (posici�n del jugador)
        lineRenderer.SetPosition(1, positionDesired); // Punto final (posici�n deseada)
    }
}
