using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Outlets
    private Animator animator;
    private AudioSource audio;
    public AudioClip audioAttack;
    
    // Configurations
    //public float playerDamage = 50f;
    public float timeAttackInterval = 0.5f;
    public float attackRange = 1f;
    public Vector3 attackDirection;

    // State Control
    public bool isAttacking = false;

    // Methods
    private void Start()
    {
        attackDirection = transform.right;
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameController.instance.isPause) { return; }
        Attack();
    }

    void Attack()
    {
        float playerDamage = GameController.instance.playerDamage; // change player attack dynamically
        
        Debug.DrawRay(transform.position, transform.right * attackRange, Color.red);
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                // print("Get left mouse button - Attack");
                isAttacking = true;
                //StartCoroutine("RaycastAttack");

                // Use raycast to hit enemies
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, attackDirection, attackRange);
                ArrayList hitEnemies = new();

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];

                    if (hit.collider.gameObject.tag == "Enemy")
                    {
                        if (!hitEnemies.Contains(hit.collider.gameObject))
                        {
                            // print("Raycast hit enemy " + hit.collider.name);
                            hit.collider.gameObject.GetComponent<EnemyController>().GetDamage(playerDamage, transform.position);
                            hitEnemies.Add(hit.collider.gameObject);
                        }
                    }
                }

                // Animation
                animator.SetTrigger("attack");

                // Sound
                audio.clip = audioAttack;
                audio.volume = 0.3f;
                audio.Play();

                StartCoroutine("EndAttack");
            }
        }       
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(timeAttackInterval);

        isAttacking = false;
        // print("Attack ends");
    }
}
