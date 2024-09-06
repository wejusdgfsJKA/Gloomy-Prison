using System;
using TMPro;
using UnityEngine;

public class ButtonAdd : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    public void Change(int value)
    {
        int a = Int32.Parse(text.text);
        a += value;
        text.text = a.ToString();
    }
    public void Exit()
    {
        Application.Quit();
    }
}
