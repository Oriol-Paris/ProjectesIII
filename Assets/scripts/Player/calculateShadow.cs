using UnityEngine;

public class calculateShadow : MonoBehaviour
{
    public GameObject shadowPrefab;
    private float shadowHeight = -0.6f;
    void Update()
    {
        Vector3 spritePosition = transform.position;

        Vector3 shadowPosition = new Vector3(spritePosition.x, shadowHeight, spritePosition.z);

        shadowPrefab.transform.position = shadowPosition;
    }
}