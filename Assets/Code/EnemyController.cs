using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Outlets
    private Rigidbody2D rigidbody2D;

    // Configurations
    //public float maxHP = 100f;
    public float curHP = 100f;
    public float getHitBackForce = 1f;

    // Methods
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        //curHP = maxHP;
    }

    private void Update()
    {
        
    }

    public void GetDamage(float damage, Vector3 sourcePosition)
    {
        // get damge
        curHP -= damage;
        print("Get damage, current HP: " + curHP);

        // hit back
        Vector3 damageDirection = (transform.position - sourcePosition).normalized;
        transform.position = transform.position + damageDirection * getHitBackForce;
    }
}
