using System.Collections;
using UnityEngine;

public class Enemy_Naga_Projectile : Enemy_Projectile
{
    [Header("Destroy Setting")]
    [SerializeField] private bool isAutoDestroy;
    [SerializeField] private float autoDestroyDelay;

    private void Awake()
    {
        transform.localPosition = Vector3.zero;
        if (isAutoDestroy) StartCoroutine(AutoDestroyByDelay());
    }

    private IEnumerator AutoDestroyByDelay()
    {
        yield return new WaitForSeconds(autoDestroyDelay);
        Destroy(gameObject);
    }

    protected override void HandleMove()
    {
        
    }
}
