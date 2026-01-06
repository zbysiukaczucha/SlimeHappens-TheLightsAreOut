using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Firefly : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float detectionRange = 6f;
    public Vector2 direction;
    private bool chasing = false;
    private Transform player;
    private PlayerCombat playerCombat;
    private PlayerMovementLO playerMovement;
    [SerializeField] private ParticleSystem explosionParticles;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCombat = player.GetComponent<PlayerCombat>();
        direction = transform.position.x > 0 ? Vector2.left : Vector2.right;
        playerMovement = player.GetComponent<PlayerMovementLO>();
        explosionParticles = transform.GetComponentInChildren<ParticleSystem>();
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
            SpawnExplosion();
        }
            
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if(!playerMovement.isDashing)
                playerCombat.TakeDamage(50);
            
            SpawnExplosion();
        }
    }
    
    private void SpawnExplosion()
    {
        if (explosionParticles == null) return;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<Light2D>().intensity = 0f;
        moveSpeed = 0f;
        explosionParticles.Play();
        Destroy(gameObject, 1.5f);
    }

}
