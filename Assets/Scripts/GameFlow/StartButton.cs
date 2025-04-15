using UnityEngine;

public class StartButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneController.Instance.StartGame();
    }
}
