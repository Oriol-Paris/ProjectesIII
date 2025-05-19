using UnityEngine;
using UnityEngine.EventSystems;

public class ExitPlayerStatsMenu : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerStatsSlider playerStatsSlider = FindAnyObjectByType<PlayerStatsSlider>();
        if (playerStatsSlider != null)
        {
            playerStatsSlider.SlideIn();
        }
    }
}