using UnityEngine;

public class Map_EndPoint : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Canvas selectCanvasPrefabs;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerMoveController>(out var _))
        {
            Time.timeScale = 0f;
            Instantiate(selectCanvasPrefabs);
        }
    }
}
