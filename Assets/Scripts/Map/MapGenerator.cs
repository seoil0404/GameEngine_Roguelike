using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Scriptable")]
    [SerializeField] private MapData mapData;

    private static int mapIndex = 0;

    private void Awake()
    {
        int random = Random.Range(0, mapData.MapManagers.Length);
        while (mapIndex == random) random = Random.Range(0, mapData.MapManagers.Length);

        mapIndex = random;

        Instantiate(mapData.MapManagers[random]);
    }
}
