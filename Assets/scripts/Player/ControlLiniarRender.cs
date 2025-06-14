using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlLiniarRender : MonoBehaviour
{
    [Serializable]
    public class LineColor
    {
        public Color m_color;
        public PlayerBase.ActionEnum m_action;
    }
    public PlayerData playerData;
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;

    [SerializeField] private int curveResolution = 0;
    [SerializeField] private float maxReach = 10f;
    [SerializeField] private float tileScale = 1.0f;
    [SerializeField] private Material lineMaterial;
  

    [SerializeField] private bool animateLine = true;
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private bool scrollReverse = false;
    private float textureOffset = 0f;

    public LineRenderer lineRenderer;
    [SerializeField] private TimeSecuence time;
    public List<LineRenderer> lineList = new List<LineRenderer>();
    public GameObject endOfLineCursor;

    [SerializeField] public List<LineColor> lineColors = new List<LineColor>();

    private Color lineColor = Color.white;

    [SerializeField] private List<Vector3> curvePoints = new List<Vector3>();

    void Start()
    {
        maxReach = playerData.moveRange;

        playerPosition = transform.position;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        SetupLineMaterial();

        lineRenderer.enabled = true;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + Vector3.forward);

        if (endOfLineCursor == null)
        {
            Debug.LogWarning("No end of line cursor assigned. Create one in the inspector.");
        }
        else
        {
            UpdateCursorPosition();
        }
    }

    private void SetupLineMaterial()
    {
        if (lineMaterial == null)
        {
            Debug.LogWarning("No material assigned to the LineRenderer. Please assign a material with the TiledLineShader.");
            return;
        }

        Material materialInstance = new Material(lineMaterial);

        materialInstance.SetColor("_Color", lineColor);

        materialInstance.SetFloat("_TileScale", tileScale);

        lineRenderer.material = materialInstance;

        lineRenderer.textureMode = LineTextureMode.Tile;
    }

    void Update()
    {
        if (animateLine && lineRenderer != null && lineRenderer.material != null)
        {
            textureOffset += (scrollReverse ? -scrollSpeed : scrollSpeed) * Time.deltaTime;

            textureOffset = textureOffset % 1.0f;

            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(textureOffset, 0));
        }
    }

    private void UpdateCursorPosition()
    {
        if (endOfLineCursor != null && curvePoints.Count > 0)
        {
            endOfLineCursor.transform.position = new Vector3(curvePoints[curvePoints.Count - 1].x,-0.47f,curvePoints[curvePoints.Count - 1].z);
            
        }
    }

    public void Disable(bool isDisabled)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = !isDisabled;
        }

        if (endOfLineCursor != null)
        {
            endOfLineCursor.SetActive(!isDisabled);
        }
    }

    public void NextMov(bool isMoving)
    {
        if (isMoving)
        {
            playerPosition = positionDesired;
        }
    }

    public void ControlLiniarRenderer(bool _rang)
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;

        RaycastHit hit;

        mousePosition = Directorio.mousePosition();

        curvePoints.Clear();
        curvePoints.Add(playerPosition);

        Vector3 direction = (mousePosition - playerPosition).normalized;
        float distance = Vector3.Distance(playerPosition, mousePosition);

      
            if (distance > maxReach&&_rang)
            {
                positionDesired = playerPosition + direction * maxReach;
                distance = maxReach;
            }
            else
            {
                positionDesired = mousePosition;
            }
          
        
        if (Physics.Raycast(playerPosition, direction, out hit, distance))
        {

            if (hit.collider.CompareTag("envairoment"))
            {
                float margin = 0.2f;
                positionDesired = hit.point - (direction * margin);
            }

        }
      

        

     

        curvePoints.Add(positionDesired);

        UpdateLineRendererr();

        UpdateCursorPosition();
    }

    public void InstantiateLineMovment()
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;

        var newLine = Instantiate(lineRenderer, transform);

        if (curvePoints.Count > 0)
        {
            Vector3 offset = new Vector3(0f, -0.45f, 0f);
            Vector3[] adjustedPoints = curvePoints.Select(p => p + offset).ToArray();
            newLine.positionCount = adjustedPoints.Length;
            newLine.SetPositions(adjustedPoints);
            newLine.SetColors(Color.white, Color.white);
            lineList.Add(newLine);

            if (lineMaterial != null)
            {
                Material newMaterial = new Material(lineMaterial);
                newMaterial.SetColor("_Color", lineColor);
                newMaterial.SetFloat("_TileScale", tileScale);

                newMaterial.SetTextureOffset("_MainTex", new Vector2(textureOffset, 0));

                newLine.material = newMaterial;
            }
        }
    }

    public void UpdateLineRendererr()
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;

        if (curvePoints.Count > 0)
        {
            lineRenderer.positionCount = curvePoints.Count;

            // Crear nueva lista con y = -0.48f
            Vector3[] adjustedPoints = new Vector3[curvePoints.Count];
            for (int i = 0; i < curvePoints.Count; i++)
            {
                Vector3 p = curvePoints[i];
                adjustedPoints[i] = new Vector3(p.x, -0.48f, p.z);
            }

            lineRenderer.SetPositions(adjustedPoints);

            UpdateCursorPosition();
        }
        else
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, -0.47f, transform.position.z));
            lineRenderer.SetPosition(1, new Vector3(transform.position.x, -0.47f, transform.position.z + 1f));

            if (endOfLineCursor != null)
            {
                endOfLineCursor.transform.position = new Vector3(transform.position.x, -0.47f, transform.position.z + 1f);
            }
        }
    }


    public void UpdateCurve()
    {
        mousePosition = Directorio.mousePosition();

        if (curvePoints.Count < 2) return;

        Vector3 startPos = curvePoints[0];
        Vector3 endPos = curvePoints[curvePoints.Count - 1];

        Vector3 direction = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);
        if (distance > maxReach)
        {
            endPos = startPos + direction * maxReach;
            curvePoints[curvePoints.Count - 1] = endPos;
        }

        Vector3 centerPoint = (startPos + endPos) * 0.5f;
        Vector3 mainDirection = (endPos - startPos).normalized;
        Vector3 perpendicular = Vector3.Cross(mainDirection, Vector3.up).normalized;

        float lateralOffset = Vector3.Dot(mousePosition - centerPoint, perpendicular);
        Vector3 midPoint = centerPoint + perpendicular * lateralOffset * 0.5f;

        if (curvePoints.Count == 3)
        {
            curvePoints[1] = midPoint;
        }
        else if (curvePoints.Count > 3)
        {
            curvePoints[curvePoints.Count - 2] = midPoint;
        }
        else
        {
            curvePoints.Insert(1, midPoint);
        }

        UpdateLineRendererr();
    }

    public float CalculateCurveLength()
    {
        if (curvePoints.Count < 3) return 0;

        float length = 0f;
        Vector3 previousPoint = curvePoints[0];

        int resolution = curveResolution > 0 ? curveResolution : 50;
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 currentPoint = Directorio.BezierCurve(t, curvePoints[0], curvePoints[1], curvePoints[2]);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }

    public List<Vector3> getCurvePoint() { return curvePoints; }

    public void ResetControlLiniarRenderer()
    {
        curvePoints.Clear();
        foreach (var line in lineList)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        lineList.Clear();
        playerPosition = transform.position;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + Vector3.forward);
        lineRenderer.enabled = true;

        if (endOfLineCursor != null)
        {
            endOfLineCursor.transform.position = transform.position + Vector3.forward;
        }
    }

    public void ChangeLineColor(PlayerBase.Action action)
    {
        foreach (var color in lineColors)
        {
            if (color.m_action == action._action)
            {
                lineRenderer.material.SetColor("_Color", color.m_color);
                break;
            }
        }
    }
}