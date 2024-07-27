using UnityEngine;

public class DetectableTest : MonoBehaviour
{
    public bool b;
    public Sound sound;
    private void OnEnable()
    {
        sound.data.source = transform;
    }
    private void Update()
    {
        if (b)
        {
            b = false;
            DetectionManager.instance.MakeNoise(sound);
        }
    }
}
