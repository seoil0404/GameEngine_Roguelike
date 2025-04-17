using UnityEngine;
using UnityEngine.UI;

public class ClearTextManager : MonoBehaviour
{
    [SerializeField] private Text clearText;
    private void Awake()
    {
        clearText.text = "Thanks to Playing my game!\nCleared Rooms : " + SceneController.Instance.ClearRoomNumber;
    }
}
