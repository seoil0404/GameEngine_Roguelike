using System.Collections;
using TMPro;
using UnityEngine;

public class Crocodile_EnemyController : EnemyController
{
    [Header("Enemy MonoBehaviors")]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Prefabs")]
    [SerializeField] private Enemy_Projectile attack1Projectile;
    [SerializeField] private Enemy_Projectile attack2Projectile;

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
        if (Vector2.Distance(transform.position, targetPosition) < 7)
        {
            OnAttack1(targetPosition);
        }
        else
        {
            OnAttack2();
        }

        StartCoroutine(AttackDelay());
    }

    private void OnAttack2()
    {
        StartCoroutine(Attack2());
    }

    private IEnumerator Attack2()
    {
        for(int index = 0; index < 5; index++)
        {
            Instantiate(attack2Projectile).transform.position = transform.position - new Vector3(0, 0.9f, 0);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnAttack1(Vector3 targetPosition)
    {
        rigid.AddForce(new Vector3((targetPosition - transform.position).x, 0, 0).normalized * 500);
        animator.SetTrigger("OnAttack1");

        if ((targetPosition - transform.position).x > 0)
        {
            spriteRenderer.flipX = false;
            Instantiate(attack1Projectile, transform).transform.localPosition = new Vector3(0.75f, -1.75f, 0);
        }
        else
        {
            spriteRenderer.flipX = true;
            Instantiate(attack1Projectile, transform).transform.localPosition = new Vector3(-0.75f, -1.75f, 0);
        }

        StartCoroutine(InitializeVelocity());
    }

    private IEnumerator InitializeVelocity()
    {
        yield return new WaitForSeconds(0.3f);
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
        if (PlayerMoveController.Position.x >= transform.position.x) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;

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

    protected override void OnDeath()
    {
        Instantiate(deadEffect).transform.position = effectInstanceTransform.transform.position;
        SceneController.Instance.ClearGame();
    }

    private void HandleMove()
    {
        if (isDetected) return;

        if (PlayerMoveController.Position.x > transform.position.x)
        {
            rigid.linearVelocityX = moveSpeed;
            spriteRenderer.flipX = false;
        }
        else
        {
            rigid.linearVelocityX = -moveSpeed;
            spriteRenderer.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
