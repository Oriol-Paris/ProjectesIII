using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSequence : MonoBehaviour
{
    [SerializeField]
    public float totalTime = 5f; // Tiempo m�ximo disponible
    public float remainingTime;

    [SerializeField]
    private float velocity = 5f;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private PlayerBase playerBase;

    [SerializeField]
    private List<Action> actionList = new List<Action>(); // Lista de acciones visible en el editor

    private bool isExecuting = false;

    private Vector3 playerPosition;
    private List<Vector3> currentCurvePoints = new List<Vector3>();
    private float t;

    // Referencia al script shootPlayer
    [SerializeField]
    private shootPlayer shootScript; // Aqu� a�ades una referencia al script de disparo

    private bool isAiming = false;
    private Vector3 shootingDirection;
    private Vector3 lastMoveDestination;

    [SerializeField]
    private OG_MovementByMouse movementScript; // A�adimos la referencia a OG_MovementByMouse

    void Start()
    {
        remainingTime = totalTime;
        playerPosition = transform.position;
    }

    void Update()
    {
        // Planificaci�n de acciones mientras quede tiempo
        if (!isExecuting)
        {
            HandleInput();
        }

        // Comenzar ejecuci�n de acciones con un input espec�fico
        if (!isExecuting && actionList.Count > 0 && Input.GetKeyDown(KeyCode.Return)) // Enter para ejecutar
        {
            StartCoroutine(ExecuteActions());
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Planificar un movimiento
            Vector3 destination = GetMouseWorldPosition();
            if (AddAction("move", destination))
            {
                lastMoveDestination = destination;  // Guardar la �ltima posici�n de destino de movimiento
                Debug.Log($"Movimiento planificado hacia {destination}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Planificar un disparo
            if (AddAction("shoot"))
            {
                // Llamar al script shootPlayer para disparar, usando la �ltima posici�n de movimiento
                shootScript.PreShoot(lastMoveDestination);  // Apuntar desde la �ltima posici�n del movimiento
                isAiming = true;
                Debug.Log("Apuntando para disparar");
            }
        }
    }

    private bool AddAction(string actionType, Vector3? targetPosition = null)
    {
        if (remainingTime >= 1.0f) // Este es el costo de la acci�n (puedes ajustarlo seg�n el tipo de acci�n)
        {
            remainingTime -= 1.0f;
            actionList.Add(new Action(actionType, targetPosition)); // Agregar a la lista visible
            return true;
        }
        else
        {
            Debug.Log("No hay tiempo suficiente para esta acci�n");
            return false;
        }
    }

    private IEnumerator ExecuteActions()
    {
        isExecuting = true;

        while (actionList.Count > 0)
        {
            Action currentAction = actionList[0]; // Tomar la primera acci�n
            actionList.RemoveAt(0); // Eliminar la acci�n de la lista

            if (currentAction.Type == "move" && currentAction.TargetPosition.HasValue)
            {
                // Ejecutar movimiento
                yield return ExecuteMove(currentAction.TargetPosition.Value);
            }

            if (currentAction.Type == "shoot")
            {
                // El disparo ya se ha planeado con la posici�n de destino de la acci�n de movimiento anterior
                shootScript.Shoot();
                yield return new WaitForSeconds(0.5f); // Simular un peque�o retraso antes de pasar a la siguiente acci�n
            }
        }

        // Regenerar los puntos de acci�n al final del turno
        RegenerateActionPoints();
        isExecuting = false;
    }


    private IEnumerator ExecuteMove(Vector3 targetPosition)
    {
        movementScript.SetPositionDesired(targetPosition); // Establece el destino en el script de movimiento

        // Obtener la curva generada por OG_MovementByMouse
        List<Vector3> curvePoints = movementScript.GetCurvePoints();
        if (curvePoints.Count < 2) yield break;

        // Ejecutar movimiento a lo largo de la curva
        yield return MoveAlongCurveSequence(curvePoints, movementScript);
    }

    IEnumerator MoveAlongCurveSequence(List<Vector3> curvePoints, OG_MovementByMouse movementScript)
    {
        float t = 0f;
        float velocity = movementScript.velocity;
        int segmentCount = curvePoints.Count - 1;

        while (t < 1f)
        {
            // Calcular segmento actual
            float segmentLength = 1f / segmentCount;
            int currentSegment = Mathf.FloorToInt(t / segmentLength);

            if (currentSegment >= segmentCount - 1) break;

            // Puntos del segmento actual
            Vector3 p0 = curvePoints[currentSegment];
            Vector3 p1 = curvePoints[currentSegment + 1];
            Vector3 p2 = curvePoints[currentSegment + 2];

            // Tiempo local dentro del segmento
            float segmentT = (t - currentSegment * segmentLength) / segmentLength;

            // Calcular posici�n
            Vector3 newPosition = movementScript.CalculateQuadraticBezierPoint(segmentT, p0, p1, p2);
            movementScript.transform.position = newPosition;

            // Incrementar tiempo
            t += velocity * Time.deltaTime / segmentCount;
            yield return null; // Esperar al siguiente frame
        }

        // Finalizar movimiento
        movementScript.transform.position = curvePoints[curvePoints.Count - 1];
        movementScript.ResetMovementState(); // Agrega un m�todo en OG_MovementByMouse para reiniciar el estado
    }

    private void RegenerateActionPoints()
    {
        // Regenerar puntos de acci�n al finalizar el turno
        remainingTime = totalTime;
        Debug.Log("Puntos de acci�n regenerados.");
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = currentCurvePoints.Count;
        lineRenderer.SetPositions(currentCurvePoints.ToArray());
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return point;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private class Action
    {
        public string Type { get; }
        public Vector3? TargetPosition { get; }

        public Action(string type, Vector3? targetPosition = null)
        {
            Type = type;
            TargetPosition = targetPosition;
        }
    }
}
