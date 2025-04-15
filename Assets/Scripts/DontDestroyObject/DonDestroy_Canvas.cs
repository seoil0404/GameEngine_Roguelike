using UnityEngine;
using UnityEngine.UI;

public class DonDestroy_Canvas : MonoBehaviour
{
    [SerializeField] private Text clearRoomNumberText;

    public static DonDestroy_Canvas Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        clearRoomNumberText.text = SceneController.Instance.ClearRoomNumber.ToString();
    }
}
