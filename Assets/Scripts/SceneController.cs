using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private int clearRoomNumber = 0;

    public int ClearRoomNumber => clearRoomNumber;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        clearRoomNumber++;
    }

    public void MoveBoss()
    {
        SceneManager.LoadScene("BossScene");
    }

    public void MoveNextStage()
    {
        SceneManager.LoadScene("GameScene");
        clearRoomNumber++;
    }

    public void ClearGame()
    {
        SceneManager.LoadScene("ClearScene");
    }

    public void MoveStartScene()
    {
        clearRoomNumber = 0;
        SceneManager.LoadScene("StartScene");
        Destroy(DonDestroy_Canvas.Instance);
    }
}
