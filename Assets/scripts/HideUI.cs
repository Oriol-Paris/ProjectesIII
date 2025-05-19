using UnityEngine;
using UnityEngine.EventSystems;

public class HideUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float transparentAlpha = 0.5f;

    private CanvasGroup canvasGroup;
    private float originalAlpha;

    [SerializeField] private PauseMenu pauseMenu;

    void Awake()
    {
        // Asegura que haya un CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalAlpha = canvasGroup.alpha;

        // Busca el PauseMenu en la escena
       
        if (pauseMenu == null)
            Debug.LogWarning("No se encontró el PauseMenu en la escena.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pauseMenu != null && pauseMenu.pauseMenu.activeInHierarchy)
            return;

        canvasGroup.alpha = transparentAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (pauseMenu != null && pauseMenu.pauseMenu.activeInHierarchy)
            return;

        canvasGroup.alpha = originalAlpha;
    }
}
