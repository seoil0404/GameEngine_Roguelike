using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [Header("Particle Prefabs")]
    [SerializeField] private ParticleSystem parryingEffect;
    [SerializeField] private ParticleSystem attackEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if(collision.TryGetComponent<Enemy_Projectile>(out var _))
        {
            Instantiate(parryingEffect).transform.position = collision.transform.position;
        }

        EnemyController enemyController;
        if(collision.TryGetComponent(out enemyController))
        {
            enemyController.DecreaseHealth();
            Instantiate(attackEffect).transform.position = enemyController.Position;
        }
    }
}
