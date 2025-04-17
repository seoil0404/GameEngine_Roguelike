using UnityEngine;

public class SelectView : MonoBehaviour
{
    private static SelectView instance = null;

    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else instance = this;
    }

    public void Continue()
    {
        Time.timeScale = 1.0f;
        instance = null;
        SceneController.Instance.MoveNextStage();
    }

    public void Boss()
    {
        Time.timeScale = 1.0f;
        instance = null;
        SceneController.Instance.MoveBoss();
    }
}
