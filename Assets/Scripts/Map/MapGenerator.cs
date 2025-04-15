using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Scriptable")]
    [SerializeField] private MapData mapData;

    private void Awake()
    {
        int random = Random.Range(0, mapData.MapManagers.Length);

        Instantiate(mapData.MapManagers[random]);
    }
}
