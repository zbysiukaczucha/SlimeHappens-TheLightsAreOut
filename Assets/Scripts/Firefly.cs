using System.Drawing;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float detectionRange = 6f;
    public Vector2 direction;
    private bool chasing = false;
    private Transform player;
    private PlayerCombat playerCombat;
    [SerializeField] private ParticleSystem explosionPrefab;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCombat = player.GetComponent<PlayerCombat>();
        direction = transform.position.x > 0 ? Vector2.left : Vector2.right;
    }

    void Update()
    {
        if (!chasing && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= detectionRange)
            {
                direction = (player.position - transform.position).normalized;
                chasing = true;
                moveSpeed = 7.5f;
                var sr = GetComponent<SpriteRenderer>();
                if (sr) sr.color = new UnityEngine.Color(1f, 0.39f, 0.39f);
            }
        }

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        if (transform.position.y < 0 || System.Math.Abs(transform.position.x) > 75)
        {
            Destroy(gameObject);
            SpawnExplosion();
        }
            
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerCombat.TakeDamage(50);
            
            SpawnExplosion();
            Destroy(gameObject);
        }
    }
    
    private void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            var ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }

}
