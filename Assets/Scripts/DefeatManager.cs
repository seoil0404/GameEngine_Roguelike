using UnityEngine;

public class DefeatManager : MonoBehaviour
{
    
    private void Awake()
    {
        Destroy(DonDestroy_Canvas.Instance.gameObject);
    }

    public void OnClick()
    {
        Destroy(DonDestroy_Player.Instance.gameObject);
        SceneController.Instance.MoveStartScene();
    }
}
