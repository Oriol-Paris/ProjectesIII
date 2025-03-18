using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool white = true;

    public bool isItem;
    public bool isReroll;
    public Sprite whiteBG;
    public Sprite blackBG;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (white)
            ChangeToBlack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!white)
            ChangeToWhite();
    }

    void ChangeToWhite()
    {
        //BG
        this.GetComponent<Image>().sprite = whiteBG;

        if(isItem)
        {
            //Item Name
            this.transform.Find("Item Name").GetComponent<TextMeshProUGUI>().color = Color.white;

            //Item Price
            this.transform.Find("Price").GetComponent<TextMeshProUGUI>().color = Color.white;

            //Item Image
            this.transform.Find("Item Image").GetComponent<Image>().color = Color.white;
        }
        else if (isReroll)
        {
            //Name
            this.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = Color.white;

            //Price
            this.transform.Find("Price").GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            //Name
            this.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        white = true;
    }

    void ChangeToBlack()
    {
        //BG
        this.GetComponent<Image>().sprite = blackBG;

        if (isItem)
        {
            //Item Name
            this.transform.Find("Item Name").GetComponent<TextMeshProUGUI>().color = Color.black;

            //Item Price
            this.transform.Find("Price").GetComponent<TextMeshProUGUI>().color = Color.black;

            //Item Image
            this.transform.Find("Item Image").GetComponent<Image>().color = Color.black;
        }
        else if (isReroll)
        {
            //Name
            this.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = Color.black;

            //Price
            this.transform.Find("Price").GetComponent<TextMeshProUGUI>().color = Color.black;
        }
        else
        {
            //Name
            this.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = Color.black;
        }

        white = false;
    }
}
