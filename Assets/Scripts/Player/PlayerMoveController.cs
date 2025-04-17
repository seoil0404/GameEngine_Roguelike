using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] private Collider2D _collider;

    [Header("Volumes")]
    [SerializeField] private Volume volume;

    [Header("Camera")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector2 cameraBasePosition;

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem swordEffectPrefab;
    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] private SwordCollider swordEffectColliderPrefab;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxJumpTime;

    [Header("HealthBar Setting")]
    [SerializeField] private int health;
    [SerializeField] private Image healthImagePrefab;
    [SerializeField] private Vector2 defaultPosition;
    [SerializeField] private float healthInterval;

    [Header("Sprite")]
    [SerializeField] private Sprite onHitImage;

    private Stack<Image> healths;

    private bool isJumping = false;
    private Coroutine delayJumpCoroutine;
    private float defaultGravityScale;

    private bool isAttacking = false;

    private Coroutine fadeColorCoroutine;
    private Coroutine timeCoroutine;

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
        if (Time.timeScale == 0) return;

        float horizontalRate = Input.GetAxisRaw("Horizontal");
        HandleMove(horizontalRate);

        if (Input.GetKeyDown(KeyData.JumpKey) && !isAttacking) OnJumpKeyDown();
        if (Input.GetKeyUp(KeyData.JumpKey)) OnJumpKeyUp();

        if (Input.GetKeyDown(KeyData.AttackKey)) OnAttack();
    }

    private void OnAttack()
    {
        if (Time.timeScale == 0) return;
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
        StartCoroutine(EndAttackWithDelay());

        rigid.linearVelocityY = 0;
        rigid.gravityScale = 0;
    }

    private IEnumerator EndAttackWithDelay()
    {
        yield return new WaitForSeconds(0.3f);
        OnEndAttack();
    }

    private void OnEndAttack()
    {
        isAttacking = false;
        rigid.gravityScale = defaultGravityScale;
    }

    private void HandleMove(float horizontalRate)
    {
        if (Time.timeScale == 0) return;
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
        if (Time.timeScale == 0) return;

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
        if (healths.Count <= 1)
        {
            OnDeath();
            return;
        }

        healths.Pop().transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true);

        animator.SetTrigger("OnHit");
        Instantiate(hitEffectPrefab).transform.position = transform.position;

        StartCoroutine(StopTime(0.25f));
    }

    private IEnumerator StopTime(float time)
    {
        ColorAdjustments color;
        volume.profile.TryGet(out color);

        _camera.transform.DOKill();
        _camera.transform.localPosition = new Vector3(cameraBasePosition.x, cameraBasePosition.y, _camera.transform.position.z);
        _camera.transform.DOShakePosition(
            duration: 0.15f,
            strength: new Vector3(0.5f, 0.5f, 0f),
            vibrato: 50,
            randomness: 90f,
            snapping: false,
            fadeOut: true
        ).SetUpdate(true);

        color.colorFilter.value = new Color(1, 0, 0);

        Time.timeScale = 0f;

        if(timeCoroutine != null) StopCoroutine(timeCoroutine);

        yield return new WaitForSecondsRealtime(time);

        color.colorFilter.value = new Color(1, 1, 1);

        Time.timeScale = 1;
    }

    private IEnumerator FadeColor(Color color, ColorAdjustments colorAdjustments)
    {
        yield return new WaitForSecondsRealtime(0.01f);

        color.g += 0.05f;
        color.b += 0.05f;
        
        colorAdjustments.colorFilter.value = color;

        if (color != Color.white) fadeColorCoroutine = StartCoroutine(FadeColor(color, colorAdjustments));
        else fadeColorCoroutine = null;
    }

    private void OnDeath()
    {
        SceneController.Instance.MoveDefeatScene();
    }
}
