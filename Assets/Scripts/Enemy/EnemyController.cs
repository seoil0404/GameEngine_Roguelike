using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public abstract class EnemyController : MonoBehaviour
{
    [Header("Attack Setting")]
    [SerializeField] private float attackDelay;
    [SerializeField] protected float attackAfterDelay;
    [SerializeField] private float maxDetectDistance;

    [Header("Enemy Health Setting")]
    [SerializeField] private int enemyHealth;

    [Header("Effect")]
    [SerializeField] protected ParticleSystem deadEffect;
    [SerializeField] protected Transform effectInstanceTransform;

    public Vector2 Position => effectInstanceTransform.position;

    private Coroutine delayAttack = null;

    private bool isAttacked = false;
    private bool isAttackDelaying = false;

    public void DecreaseHealth()
    {
        enemyHealth--;
        if(enemyHealth == 0)
        {
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        Instantiate(deadEffect).transform.position = effectInstanceTransform.transform.position;
        Destroy(gameObject);
    }

    private void Update()
    {
        DetectPlayer();

        OnUpdate();
    }

    private void DetectPlayer()
    {
        RaycastHit2D rayCastInfo = Physics2D.Raycast (
            transform.position, 
            (PlayerMoveController.Position - transform.position + new Vector3(0, 0.6f, 0)).normalized, 
            Mathf.Min(Vector2.Distance(transform.position, PlayerMoveController.Position), maxDetectDistance), 
            LayerInfo.Platform
        );

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

        delayAttack = null;
        OnAttack(PlayerMoveController.Position);
        StartCoroutine(DelayAfterAttack());
    }

    private IEnumerator DelayAfterAttack()
    {
        isAttackDelaying = true;

        yield return new WaitForSeconds(attackAfterDelay);

        isAttackDelaying = false;
        isAttacked = false;
    }

    private void OnLostDetected()
    {
        if (delayAttack == null || isAttackDelaying) return;

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
