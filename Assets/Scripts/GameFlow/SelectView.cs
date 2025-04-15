using UnityEngine;

public class SelectView : MonoBehaviour
{
    public void Continue()
    {
        Time.timeScale = 1.0f;
        SceneController.Instance.MoveNextStage();
    }

    public void Boss()
    {
        Time.timeScale = 1.0f;
        SceneController.Instance.MoveBoss();
    }
}
