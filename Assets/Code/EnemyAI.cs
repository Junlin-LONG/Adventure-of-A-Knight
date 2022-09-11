using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Outlets
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    public Transform playerTransform;
    private EnemyController enemyController;
    public CharacterController characterController;
    private Animator _animator;
    public AudioClip ishit;
    public AudioSource music;

    // Configurations
    private Vector3 startPosition;
    private float mapLength = 18f; // Map x range: -18f ~ 18f
    private float mapWidth = 10f; // Map y range: -10f ~ 10f
    public float alertRange = 5f;
    public float escapeRange = 10f;
    public float enemyDamage = 10f;

    // for Moving
    public float moveSpeed = 3f;

    // for Roaming
    private bool isRoaming = false;
    private Vector3 targetPosition;

    // for Attacking
    public float startAttackingRange = 1f;
    public float attackRange = 1f;
    private bool isAttacking = false;
    public float timeAttackInterval = 2f;

    // State Control
    public enum State
    {
        Roaming,
        Chase,
        Attack,
        Death,
    }
    State curState = State.Roaming;

    // Methods
    private void Start()
    {
        
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
        _animator = GetComponent<Animator>();
        music = GetComponent<AudioSource>();

        startPosition = transform.position;
        targetPosition = startPosition;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        mapLength = GameController.instance.enemyMovingAreaLength;
        mapWidth = GameController.instance.enemyMovingAreaWidth;
    }

    private void Update()
    {
        if (GameController.instance.isPause) { return; }
        // For any state, if hp<=0 , switch to Death
        if(enemyController.curHP <= 0)
        {
            curState = State.Death;
        }
        

        switch (curState)
        {
            case State.Roaming:
                Roaming();
                break;
            case State.Chase:
                Chasing();
                break;
            case State.Attack:
                Attacking();
                break;
            case State.Death:
                Death();
                break;
        }
    }

    // Methods for roaming
    private void Roaming()
    {
        /*** State switch part ***/
        // If player move into alert range, state switch to Chasing
        if ((transform.position - playerTransform.position).magnitude <= alertRange)
        {
            curState = State.Chase;
            isRoaming = false;
        }

        /**** Implement part ****/
        if (!isRoaming)
        {
            isRoaming = true;
            // Randomly roaming around start position
            targetPosition = GetRoamingPosition();
        }
        MoveToTarget(targetPosition);
        
        if ((targetPosition - transform.position).magnitude <= 0.1f)
        {
            isRoaming = false;
        }
    }

    private Vector3 GetRoamingPosition()
    {
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 randomPosition = startPosition + randomDirection * Random.Range(0f, 5f);
        if (randomPosition.x > mapLength/2 || randomPosition.x < -mapLength/2 || randomPosition.y > mapWidth/2 || randomPosition.y < -mapWidth/2)
            return startPosition;
        else
            return randomPosition;
    }

    // Methods for moving
    private void MoveToTarget(Vector3 targetPosition)
    {
        Vector3 moveDirection = targetPosition - transform.position;
        MoveToDirection(moveDirection);
    }

    private void MoveToDirection(Vector3 dir)
    {
        rigidbody2D.velocity = dir.normalized * moveSpeed;
        if(dir.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    
    // Methods for chasing
    private void Chasing()
    {
        /*** State switch part ***/
        // If player move away from escape range, state switch to Roaming
        if((transform.position - playerTransform.position).magnitude > escapeRange)
        {
            curState = State.Roaming;
        }
        // If player move into start attacking range, state switch to Attack
        if((transform.position - playerTransform.position).magnitude <= startAttackingRange)
        {
            curState = State.Attack;
        }

        /**** Implement part ****/
        MoveToTarget(playerTransform.position);
        _animator.SetFloat("monsterSpeed",rigidbody2D.velocity.magnitude);
    }


    // Methods for attacking
    private void Attacking()
    {
        /*** State switch part ***/
        // If player move away from attack range, state switch to Chase
        if((transform.position - playerTransform.position).magnitude > attackRange)
        {
            curState = State.Chase;
        }

        /**** Implement part ****/
        if (!isAttacking)
        {
            print("Enemy Attack");
            isAttacking = true;
            Vector3 attackDir = (playerTransform.position - transform.position).normalized;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, attackDir, attackRange);
            bool isHitPlayer = false;
            _animator.SetTrigger("monster attack");
            for(int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                if(hit.collider.gameObject.tag == "Player")
                {
                    isHitPlayer = true;
                }  
            }

            if (isHitPlayer)
            {
                print("Enemy hit player");
                characterController.GetDamage(enemyDamage, transform.position);
                music.clip = ishit;
                music.volume = 1f;
                music.Play();
            }
            StartCoroutine("EndAttack");
        }
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(timeAttackInterval);

        isAttacking = false;
    }

    public void SetEnemyAttack(float newEnemyDamage)
    {
        enemyDamage = newEnemyDamage;
    }

    // Methods for death
    private void Death()
    {

        Destroy(gameObject);


        GameController.instance.OneEnemyDie();
    }
}
