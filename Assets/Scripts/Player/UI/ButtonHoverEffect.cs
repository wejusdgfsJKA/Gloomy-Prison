using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Original size of the button.
    /// </summary>
    protected Vector3 ogButtonSize;
    /// <summary>
    /// How much the button will increase in size when hovered on.
    /// </summary>
    [SerializeField]
    protected float scalar = 1.1f;
    private void OnEnable()
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
