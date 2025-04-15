using UnityEngine;

public class Soul_EnemyController : EnemyController
{
    [Header("Enemy MonoBehaviors")]
    [SerializeField] private Animator animator;

    [Header("Prefabs")]
    [SerializeField] private Enemy_Projectile projectilePrefab;

    protected override void OnAttack(Vector3 targetPosition)
    {
        animator.SetBool("IsDetected", false);
        animator.SetTrigger("OnAttack");
        Instantiate(projectilePrefab).transform.position = transform.position;
    }

    protected override void OnDetectedEvent()
    {
        animator.SetBool("IsDetected", true);
    }

    protected override void OnLostDetectedEvent()
    {
        animator.SetBool("IsDetected", false);
    }
}
