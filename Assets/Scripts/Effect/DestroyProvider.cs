using UnityEngine;

public class DestroyProvider : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private void Destroy()
    {
        Destroy(canvas);
    }
}
