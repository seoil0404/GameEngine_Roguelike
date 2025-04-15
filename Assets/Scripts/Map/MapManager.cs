using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform startPoint;

    private void Start()
    {
        DonDestroy_Player.Instance.transform.position = startPoint.position;
    }
}
