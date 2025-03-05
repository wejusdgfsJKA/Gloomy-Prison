using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 ogButtonSize;
    public float scalar = 1.1f; //growth factor

    private void Start()
    {
        ogButtonSize = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) //hovering
    {
        transform.localScale = ogButtonSize * scalar; //button gets a lil excited
    }

    public void OnPointerExit(PointerEventData eventData) //not hovering
    {
        transform.localScale = ogButtonSize; //reset size
    }
}
