using System.Collections;
using UnityEngine;

public class AutoDestroyer : MonoBehaviour
{
    [SerializeField] private float destroyDelay;

    private void Awake()
    {
        StartCoroutine(AutoDestroyByDelay());
    }

    private IEnumerator AutoDestroyByDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
