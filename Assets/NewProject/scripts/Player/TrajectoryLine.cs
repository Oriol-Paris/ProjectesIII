using UnityEngine;
using System.Collections;

public class TrajectoryLine : MonoBehaviour
{
    #region VARIABLES

    [Header("References")]
    [SerializeField] private OG_MovementByMouse m_MovementByMouse;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float curveResolution = 3.5f;
    [Header("Trajectory Line Smoothness/Length")]
    [SerializeField] private int segmentCount = 50;
    private Vector3[] segments;
    private LineRenderer lineRenderer;
    private float speed;

    #endregion
    

    void Start()
    {
        segments=new Vector3[segmentCount];

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;

        speed = m_MovementByMouse.velocity;

        
    }

    // Update is called once per frame
    void Update()
    {
        //Set starting position of line renderer
        Vector3 startPos = m_MovementByMouse.GetPosition();
        segments[0] = startPos;
        lineRenderer.SetPosition(0,startPos);

        //set starting velocity
        Vector3 startVelocity = m_MovementByMouse.GetPosition() * m_MovementByMouse.velocity;
        for(int i = 0;i<segmentCount;i++)
        {
            float timeOffset = (i * Time.fixedDeltaTime * curveResolution);

            segments[i] = segments[0] + startVelocity * timeOffset;
            //lineRenderer.SetPosition(i, segments[i]);
        }
    }
}
