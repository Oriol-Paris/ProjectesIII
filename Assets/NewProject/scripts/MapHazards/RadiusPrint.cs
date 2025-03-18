using Unity.VisualScripting;
using UnityEngine;

public class RadiusPrint : MonoBehaviour
{

    private ExplosionHazard parentScript;
    public Material Material;
    private LineRenderer lineRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        parentScript = GetComponentInParent<ExplosionHazard>();
        DrawCircle( parentScript.explosionRadius);
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            lineRenderer.enabled = !lineRenderer.enabled;
        }
        
    }

    private void DrawCircle(float r)
    {
        int size = 63;
        float x, z;
        float theta_scale = 0.1f;  // Circle resolution

        
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(UnityEngine.Color.yellow, UnityEngine.Color.yellow);
        lineRenderer.SetWidth(0.1F, 0.1F);
        lineRenderer.material = Material;
        lineRenderer.sortingLayerName = "hazards";
        lineRenderer.SetVertexCount(size);

        int i = 0;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += theta_scale)
        {

            x = (r * Mathf.Cos(theta)) + transform.position.x;
            z = (r * Mathf.Sin(theta)) + transform.position.z;
            

            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            i += 1;
        }
    }
}
