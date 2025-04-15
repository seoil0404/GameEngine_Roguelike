using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveController : MonoBehaviour
{
    private static PlayerMoveController instance;
    public static Vector3 Position => instance.transform.position;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;

    [Header("MonoBehaviors")]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem swordEffectPrefab;
    [SerializeField] private SwordCollider swordEffectColliderPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;

    [Header("HealthBar Setting")]
    [SerializeField] private int health;
    [SerializeField] private Image healthImagePrefab;
    [SerializeField] private Vector2 defaultPosition;
    [SerializeField] private float healthInterval;

    private Stack<Image> healths;

    private bool isJumping = false;
    private Coroutine delayJumpCoroutine;
    private float defaultGravityScale;

    private bool isAttacking = false;

    private void Awake()
    {
        healths = new();

        for (int index = 0; index < health; index++)
        {
            healths.Push(Instantiate(healthImagePrefab, canvas.transform));

            healths.Peek().rectTransform.anchoredPosition = defaultPosition + new Vector2(healthInterval * index, 0);
        }

        if(instance == null) instance = this;

        defaultGravityScale = rigid.gravityScale;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float horizontalRate = Input.GetAxisRaw("Horizontal");
        HandleMove(horizontalRate);

        if (Input.GetKeyDown(KeyData.JumpKey) && !isAttacking) OnJumpKeyDown();
        if (Input.GetKeyUp(KeyData.JumpKey)) OnJumpKeyUp();

        if (Input.GetKeyDown(KeyData.AttackKey)) OnAttack();
    }

    private void OnAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        if(spriteRenderer.flipX == true)
        {
            ParticleSystem temp = Instantiate(swordEffectPrefab);
            temp.transform.position = transform.position + new Vector3(1f, 0.6f, 0);

            Instantiate(swordEffectColliderPrefab).transform.position = transform.position + new Vector3(1f, 0.6f, 0);
        }
        else
        {
            ParticleSystem temp = Instantiate(swordEffectPrefab);
            temp.transform.localScale = new Vector3(temp.transform.localScale.x * -1, temp.transform.localScale.y, temp.transform.localScale.z);
            temp.transform.position = transform.position + new Vector3(-1f, 0.6f, 0);

            Instantiate(swordEffectColliderPrefab).transform.position = transform.position + new Vector3(-1f, 0.6f, 0);
        }

        animator.SetTrigger("OnAttack");

        rigid.linearVelocityY = 0;
        rigid.gravityScale = 0;
    }

    private void OnEndAttack()
    {
        isAttacking = false;
        rigid.gravityScale = defaultGravityScale;
    }

    private void HandleMove(float horizontalRate)
    {
        if (isAttacking) horizontalRate = 0;

        if (horizontalRate > 0) spriteRenderer.flipX = true;
        else if (horizontalRate < 0) spriteRenderer.flipX = false;

        if (horizontalRate == 0) animator.SetBool("IsRunning", false);
        else animator.SetBool("IsRunning", true);

        rigid.linearVelocityX = horizontalRate * moveSpeed;
    }

    private void OnJumpKeyDown()
    {
        if (isJumping) return;

        isJumping = true;

        animator.SetBool("IsJumping", true);
        
        rigid.linearVelocityY = jumpForce;
        rigid.gravityScale = 0;
        
        delayJumpCoroutine = StartCoroutine(DelayJump());
    }

    private IEnumerator DelayJump()
    {
        yield return new WaitForSeconds(maxJumpTime);

        if(!isAttacking) rigid.gravityScale = defaultGravityScale;
        
        delayJumpCoroutine = null;
    }

    private void OnJumpKeyUp()
    {
        if (delayJumpCoroutine == null) return;

        StopCoroutine(delayJumpCoroutine);
        delayJumpCoroutine = null;

        if(!isAttacking) rigid.gravityScale = defaultGravityScale;
    }

    private void OnEndJump()
    {
        isJumping = false;
        
        rigid.gravityScale = defaultGravityScale;

        animator.SetBool("IsJumping", false);

        if (delayJumpCoroutine != null) StopCoroutine(delayJumpCoroutine);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyController>(out var _)) OnHit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy_Projectile>(out var _)) OnHit();
        if ((1 << collision.gameObject.layer) == LayerInfo.Platform) OnEndJump();
    }

    private void OnHit()
    {
        if (healths.Count <= 0)
        {
            OnDeath();
            return;
        }

        Destroy(healths.Pop());
    }

    private void OnDeath()
    {

    }
}
