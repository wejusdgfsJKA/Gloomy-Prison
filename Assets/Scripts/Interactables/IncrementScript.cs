using TMPro;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class IncrementScript : MonoBehaviour
{
    Interactable interactable;
    [SerializeField] TextMeshProUGUI text;
    public void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteract += (Transform t) =>
        {
            int a;
            System.Int32.TryParse(text.text, out a);
            a++;
            text.text = a.ToString();
        };
    }
}
