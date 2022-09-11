using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    // Outlets
    private Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    public GameObject maxHPImg;
    public GameObject curHPImg;


    //public AudioClip Attack;
    public AudioClip Dash;
    public AudioClip CharacterDie;
    public AudioSource music;
    public PlayerAttack playerAttack;
    private Animator _animator;

    // Configurations
    private float maxHP = 100f;
    public float curHP = 100f;
    public float getHitBackForce = 1f;
    
    private const float MOVE_SPEED = 8f;
    [SerializeField] private LayerMask dashLayerMask;
    private Vector3 moveDir;

    // State Control
    private bool isDashButtonDown;

    
    // Methods
    private void Awake()
    {
         
    }

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        music = GetComponent<AudioSource>();
        playerAttack = GetComponent<PlayerAttack>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameController.instance.isPause) {return;}
        
        // Movement
        float moveX = 0f;
        float moveY = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            spriteRenderer.flipX = true;
            playerAttack.attackDirection = -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
            spriteRenderer.flipX = false;
            playerAttack.attackDirection = transform.right;
        }
        moveDir = new Vector3(moveX, moveY).normalized;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDashButtonDown = true;
            music.clip = Dash;
            music.volume = 1f;
            music.Play();
        }

        /*if (Input.GetMouseButtonDown(0))
        {
            if (playerAttack.isAttacking) { 
                music.clip = Attack;
                music.volume = 0.3f;
                music.Play();
                _animator.SetTrigger("attack");
            }
        }*/
    }

    private void FixedUpdate()
    {
        // Dash
        rigidbody2D.velocity = moveDir * MOVE_SPEED;
        _animator.SetFloat("Speed",rigidbody2D.velocity.magnitude);
        if (isDashButtonDown)
          {
             float dashAmount = 2f;
             Vector3 dashPosition = transform.position + moveDir * dashAmount;
             RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, moveDir, dashAmount, dashLayerMask);
             if (raycastHit2D.collider != null)
             {
                dashPosition = raycastHit2D.point;
             }
             rigidbody2D.MovePosition(dashPosition);
             isDashButtonDown = false;
        }
    }

    public void GetDamage(float damage, Vector3 sourcePosition)
    {
        // get damage
        curHP -= damage;
        if (curHP <= 0)
        {
            GameController.instance.PlayerDeath();
            music.clip = CharacterDie;
            music.volume = 1f;
            music.Play();
        }

        curHPImg.GetComponent<Image>().fillAmount = curHP / maxHP;

        // get hit back
        Vector3 damageDirection = (transform.position - sourcePosition).normalized;
        transform.position = transform.position + damageDirection * getHitBackForce;
    }

    public void MaxHPBuff(float someMaxHPIncrease)
    {
        float ratio = curHP / maxHP;
        maxHP += someMaxHPIncrease;
        curHP = ratio * maxHP;

        maxHPImg.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHP, 15);
        curHPImg.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHP, 15);
        curHPImg.GetComponent<Image>().fillAmount = curHP / maxHP;
    }

    public void RecoverHalfHealth()
    {
        if(curHP > 0)
        {
            curHP += (maxHP / 2);
            if(curHP >= maxHP)
            {
                curHP = maxHP;
            }

            curHPImg.GetComponent<Image>().fillAmount = curHP / maxHP;
        }
    }
}
