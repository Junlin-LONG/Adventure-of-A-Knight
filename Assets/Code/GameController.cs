using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    // Outlets
    public GameObject enemyPrefab;
    public Transform bornPosition;
    public GameObject map;
    public GameObject boundaryPrefab;
    public AudioSource music;

    public TMP_Text textScore;
    public TMP_Text textSkillPoint;   
    public TMP_Text textHighestScoreInEndMenu;

    public float playerDamage = 50f;
    public float enemyInitialDamage = 10f;

    public CharacterController player;

    // Configurations
    public float mapLength;
    public float mapWidth;
    public float boundaryWidth;
    private Vector3 enemyScale; // Size of enemy prefab collision box 
    public float enemyMovingAreaLength;
    public float enemyMovingAreaWidth;

    public int enemyMaxNum = 5;
    public float intervalTime = 3f;

    // Counters
    private int enemyCurNum; // Monitoring current number of enemies in scene
    private int enemyTotalCounter; // Counting total number of enemies spawn in scene. wave number = enemyTotalNumber / 5
    public int score;
    private int skillExperience; // Progress of earning skill points
    public int skillPoint;

    // Pause control
    public bool isPause = true;

    // Methods
    private void Awake()
    {
        instance = this;
        mapLength = map.GetComponent<SpriteRenderer>().bounds.size.x;
        mapWidth = map.GetComponent<SpriteRenderer>().bounds.size.y;
        //print(mapLength);
        //print(mapWidth);

        GenerateBoundaries();

        /************************************** RUN TIHS CODE BEFORE BUILDING GAME**************************************/

        //PlayerPrefs.DeleteKey("BestScore"); //  This code refreshes best score in saved data. RUN AND ONLY RUN it before building game!!!
        
        /************************************** RUN TIHS CODE BEFORE BUILDING GAME**************************************/
    }

    private void Start()
    {
        enemyCurNum = 0;
        enemyTotalCounter = 0;
        score = 0;
        skillExperience = 0;
        skillPoint = 0;

        StartCoroutine("EnemySpawnTimer");
        music = GetComponent<AudioSource>();

        enemyScale = enemyPrefab.GetComponent<SpriteRenderer>().bounds.size; // size of enemy prefab. When spawning enemyPrefable, avoid enemy born at wrong place
        enemyMovingAreaLength = mapLength - enemyScale.x;
        enemyMovingAreaWidth = mapWidth - enemyScale.y;
    }

    private void Update()
    {
        UpdateUIText();
        UpdateSkillPoint();

        if (isPause) { return; }
        UpdateSkillMenu();
    }

    // Initialization
    private void GenerateBoundaries()
    {
        Vector3 pos = new Vector3(0, (mapWidth + boundaryWidth) / 2f, 0);
        GameObject northBoundary = Instantiate(boundaryPrefab, pos, Quaternion.identity);
        northBoundary.GetComponent<BoxCollider2D>().size = new Vector2(mapLength, boundaryWidth);

        pos = new Vector3(0, -(mapWidth + boundaryWidth) / 2f, 0);
        GameObject southBoundary = Instantiate(boundaryPrefab, pos, Quaternion.identity);
        southBoundary.GetComponent<BoxCollider2D>().size = new Vector2(mapLength, boundaryWidth);

        pos = new Vector3((mapLength + boundaryWidth) / 2f, 0, 0);
        GameObject eastBoundary = Instantiate(boundaryPrefab, pos, Quaternion.identity);
        eastBoundary.GetComponent<BoxCollider2D>().size = new Vector2(boundaryWidth, mapWidth);

        pos = new Vector3(-(mapLength + boundaryWidth) / 2f, 0, 0);
        GameObject westBoundary = Instantiate(boundaryPrefab, pos, Quaternion.identity);
        westBoundary.GetComponent<BoxCollider2D>().size = new Vector2(boundaryWidth, mapWidth);
    }

    // Game flow control
    private void SpawnEnemy()
    {
        Vector3 randomSpawnPosition = bornPosition.position
            + new Vector3(Random.Range(-enemyMovingAreaLength / 2 + 0.1f, enemyMovingAreaLength / 2 - 0.1f),
                          Random.Range(-enemyMovingAreaWidth / 2 + 0.1f, enemyMovingAreaWidth / 2 - 0.1f),
                          0);

        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPosition, Quaternion.identity);
        enemyCurNum++;
        enemyTotalCounter++;

        // Adjust difficulty dynamically
        newEnemy.GetComponent<EnemyController>().curHP += (Mathf.FloorToInt(enemyTotalCounter / 5) * 10);
        newEnemy.GetComponent<EnemyAI>().SetEnemyAttack(enemyInitialDamage + Mathf.FloorToInt(enemyTotalCounter / 5) * 5);
        enemyMaxNum = 5 + Mathf.FloorToInt(enemyTotalCounter / 10);
    }

    IEnumerator EnemySpawnTimer()
    {
        yield return new WaitForSeconds(intervalTime);

        if (enemyCurNum < enemyMaxNum)
        {
            SpawnEnemy();
        }

        StartCoroutine("EnemySpawnTimer");
    }

    public void PlayerDeath()
    {
        // Highest Score
        if (PlayerPrefs.HasKey("BestScore")){
            if(score > PlayerPrefs.GetInt("BestScore"))
            {
                PlayerPrefs.SetInt("BestScore", score);
            }
        }
        else 
        { 
            PlayerPrefs.SetInt("BestScore", score);
        }
        textHighestScoreInEndMenu.text = "Higest score: " + PlayerPrefs.GetInt("BestScore");

        MenuController.instance.ShowEndMenu();
    }

    public void OneEnemyDie()
    {
        score += 1;
        enemyCurNum--;
        skillExperience++;
        music.Play();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // UI control
    private void UpdateUIText()
    {
        textScore.text = "Your Score: " + score;
        textSkillPoint.text = "Skill Points: " + skillPoint;
    }

    private void UpdateSkillPoint()
    {
        if (skillExperience >= 5)
        {
            skillExperience -= 5;
            skillPoint++;
        }
    }

    private void UpdateSkillMenu()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            MenuController.instance.ShowSkillMenu();
        }
    }

    public void AttackBuff()
    {
        if(skillPoint > 0)
        {
            playerDamage += 10f;
            skillPoint--;
        }
        print("current attack: " + playerDamage);
    }

    public void MaxHealthBuff()
    {
        if(skillPoint > 0)
        {
            player.MaxHPBuff(20f);
            skillPoint--;
        }
    }

    public void RecoverHealth()
    {
        if(skillPoint > 0)
        {
            player.RecoverHalfHealth();
            skillPoint--;
        }
    }
}
