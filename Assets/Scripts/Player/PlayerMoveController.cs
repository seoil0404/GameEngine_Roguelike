using System.Collections;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private static PlayerMoveController instance;
    public static Vector3 Position => instance.transform.position;

    [Header("MonoBehaviors")]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;

    private bool isJumping = false;
    private Coroutine delayJumpCoroutine;
    private float defaultGravityScale;

    private bool isAttacking = false;

    private void Awake()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((1 << collision.gameObject.layer) == LayerInfo.Platform) OnEndJump();
    }
}
