using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public abstract class EnemyController : MonoBehaviour
{
    [Header("Attack Setting")]
    [SerializeField] private float attackDelay;
    [SerializeField] protected float attackAfterDelay;
    [SerializeField] private float maxDetectDistance;

    private Coroutine delayAttack = null;

    private bool isAttacked = false;

    private void Update()
    {
        DetectPlayer();

        OnUpdate();
    }

    private void DetectPlayer()
    {
        RaycastHit2D rayCastInfo = Physics2D.Raycast (
            transform.position, 
            (PlayerMoveController.Position - transform.position + new Vector3(0, 0.5f, 0)).normalized, 
            Mathf.Min(Vector2.Distance(transform.position, PlayerMoveController.Position), maxDetectDistance), 
            LayerInfo.Platform
        );

        if (rayCastInfo.collider != null) Debug.Log(rayCastInfo.collider.gameObject.name);

        if (rayCastInfo.collider == null && Vector2.Distance(transform.position, PlayerMoveController.Position) <= maxDetectDistance)
            OnDetected();
        else
            OnLostDetected();
    }

    protected virtual void OnUpdate()
    {

    }

    private void OnDetected()
    {
        if (delayAttack != null || isAttacked) return;

        delayAttack = StartCoroutine(DelayAttack());
        OnDetectedEvent();
    }

    protected virtual void OnDetectedEvent()
    {

    }

    private IEnumerator DelayAttack()
    {
        isAttacked = true;

        yield return new WaitForSeconds(attackDelay);

        OnAttack(PlayerMoveController.Position);
        StartCoroutine(DelayAfterAttack());
    }

    private IEnumerator DelayAfterAttack()
    {
        yield return new WaitForSeconds(attackAfterDelay);
        isAttacked = false;
    }

    private void OnLostDetected()
    {
        if (delayAttack == null) return;
        
        StopCoroutine(delayAttack);
        isAttacked = false;
        delayAttack = null;

        OnLostDetectedEvent();
    }

    protected virtual void OnLostDetectedEvent()
    {

    }

    protected abstract void OnAttack(Vector3 targetPosition);
}
