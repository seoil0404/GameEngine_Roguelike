using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy_Projectile : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float moveSpeed;

    [Header("Effect Prefabs")]
    [SerializeField] private ParticleSystem explosionEffect;

    protected virtual void Update()
    {
        HandleMove();
    }

    protected virtual void HandleMove()
    {
        Vector3 playerPosition = PlayerMoveController.Position + new Vector3(0, 0.3f, 0);

        Vector2 direction = playerPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = Vector3.Lerp(transform.position, playerPosition, moveSpeed);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<EnemyController>(out var _))
        {
            if(collision.gameObject.TryGetComponent<PlayerMoveController>(out var _))
                Instantiate(explosionEffect).transform.position = PlayerMoveController.Position;
            else
                Instantiate(explosionEffect).transform.position = transform.position;
            Destroy();
        }
    }

    private void Destroy()
    {
        
        Destroy(gameObject);
    }
}
