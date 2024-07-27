using TMPro;
using UnityEngine;

public class BTDebug : MonoBehaviour
{
    [SerializeField] BTree LinkedBT;
    [SerializeField] TextMeshProUGUI LinkedDebugText;

    // Start is called before the first frame update
    void Start()
    {
        LinkedDebugText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        LinkedDebugText.text = LinkedBT.GetDebugText();
    }
}
