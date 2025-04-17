using UnityEngine;

public class RestartManager : MonoBehaviour
{
    private void Awake()
    {
        Destroy(DonDestroy_Canvas.Instance.gameObject);
    }

    public void ReStart()
    {
        Destroy(DonDestroy_Player.Instance.gameObject);
        SceneController.Instance.MoveStartScene();
    }
}
