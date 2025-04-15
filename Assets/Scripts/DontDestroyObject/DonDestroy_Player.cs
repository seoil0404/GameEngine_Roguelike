using UnityEngine;

public class DonDestroy_Player : MonoBehaviour
{
    public static DonDestroy_Player Instance {  get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else Destroy(gameObject);
    }
}
