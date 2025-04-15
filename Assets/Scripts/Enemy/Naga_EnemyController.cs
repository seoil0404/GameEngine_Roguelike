using System.Collections;
using UnityEngine;

public class Naga_EnemyController : EnemyController
{
    [Header("Enemy MonoBehaviors")]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Prefabs")]
    [SerializeField] private Enemy_Projectile projectile;

    [Header("Move Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int firstMoveRate;

    private bool isDetected;

    private void Awake()
    {
        rigid.linearVelocityX = moveSpeed * firstMoveRate;
    }

    protected override void OnAttack(Vector3 targetPosition)
    {
        rigid.AddForce(new Vector3((targetPosition - transform.position).x, 0, 0).normalized * 500000);
        animator.SetTrigger("OnAttack");

        if ((targetPosition - transform.position).x > 0) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;

        Instantiate(projectile, transform);

        StartCoroutine(AttackDelay());
        StartCoroutine(InitializeVelocity());
    }

    private IEnumerator InitializeVelocity()
    {
        yield return new WaitForSeconds(0.5f);
        rigid.linearVelocityX = 0;
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackAfterDelay - 0.1f);
        animator.SetBool("IsDetected", false);
        isDetected = false;
    }

    protected override void OnDetectedEvent()
    {
        if (PlayerMoveController.Position.x >= transform.position.x) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;

        rigid.linearVelocityX = 0;

        animator.SetBool("IsDetected", true);

        isDetected = true;
    }

    protected override void OnLostDetectedEvent()
    {
        animator.SetBool("IsDetected", false);
        isDetected = false;
    }

    protected override void OnUpdate()
    {
        HandleMove();
    }

    private void HandleMove()
    {
        if (isDetected) return;

        if (rigid.linearVelocityX > 0)
        {
            spriteRenderer.flipX = true;

            bool changeDirection = new();

            RaycastHit2D rightRayCastInfo = Physics2D.Raycast(
                transform.position,
                new Vector3(1, 0, 0),
                1f,
                LayerInfo.Platform
            );

            if (rightRayCastInfo.collider != null) changeDirection = true;

            if (rightRayCastInfo.collider == null)
            {
                rightRayCastInfo = Physics2D.Raycast(
                    transform.position + new Vector3(1, 0, 0),
                    new Vector3(0, -1, 0),
                    2f,
                    LayerInfo.Platform
                );
            }

            if (rightRayCastInfo.collider == null) changeDirection = true;

            if (changeDirection)
            {
                rigid.linearVelocityX = -moveSpeed;
            }
            else rigid.linearVelocityX = moveSpeed;
        }
        else if (rigid.linearVelocityX < 0)
        {
            spriteRenderer.flipX = false;

            bool changeDirection = new();

            RaycastHit2D leftRayCastInfo = Physics2D.Raycast(
                transform.position,
                new Vector3(-1, 0, 0),
                1f,
                LayerInfo.Platform
            );

            if (leftRayCastInfo.collider != null) changeDirection = true;

            if (leftRayCastInfo.collider == null)
            {
                leftRayCastInfo = Physics2D.Raycast(
                    transform.position + new Vector3(-1, 0, 0),
                    new Vector3(0, -1, 0),
                    2f,
                    LayerInfo.Platform
                );
            }

            if (leftRayCastInfo.collider == null) changeDirection = true;

            if (changeDirection)
            {
                rigid.linearVelocityX = moveSpeed;
            }
            else rigid.linearVelocityX = -moveSpeed;
        }
        else rigid.linearVelocityX = moveSpeed * firstMoveRate;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
