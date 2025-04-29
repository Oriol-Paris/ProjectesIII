using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_EntityPosition");
    public static int SizeID = Shader.PropertyToID("_Size");
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        
        var dir = gameCamera.transform.position - transform.position;
        var ray = new Ray (transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, layerMask))
        {
            wallMaterial.SetFloat(SizeID, 0.5f);
        }
        else
        {
            wallMaterial.SetFloat (SizeID, 0);
        }

        var view = gameCamera.WorldToViewportPoint(transform.position);
        wallMaterial.SetVector(PosID, view);
    }
}
