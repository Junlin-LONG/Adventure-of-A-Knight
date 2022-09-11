using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bornenemy : MonoBehaviour {
    //该出生点生成的怪物
    public GameObject targetEnemy;
    //生成怪物的总数量
    public int enemyTotalNum = 10;
    //生成怪物的时间间隔
    public float intervalTime = 3;
    //生成怪物的计数器
    private int enemyCounter;

    // Get scene size
    private float length = 16f;
    private float width = 8f;
 
    //生成怪物的计数器
 
	// Use this for initialization
	void Start () {
     
       
        //初始时，怪物计数为0；
        enemyCounter = 0;
        //重复生成怪物
        InvokeRepeating("CreatEnemy", 0.5F, intervalTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //方法，生成怪物
    private void CreatEnemy()
    {
        //如果玩家存活

        {
            //创建随机点
            Vector3 suiji= this.transform.position;
            suiji.x= this.transform.position.x + Random.Range(-length, length);
            suiji.y= this.transform.position.y + Random.Range(-width, width);
            suiji.z = 0;
            
            Vector3 suiji1 = this.transform.position;
            suiji1.x = this.transform.position.x + Random.Range(-length, length);
            suiji1.y = this.transform.position.y + Random.Range(-width, width);
            suiji1.z = 0;

            //生成一只怪物
            //if (Random.Range(0, 3) % 3 == 0)
            //{
                //Instantiate(targetEnemy, this.transform.position, Quaternion.identity);
                //enemyCounter++;
            //}
            if (Random.Range(0, 3) % 3 == 1)
            {
                Instantiate(targetEnemy,suiji, Quaternion.identity);
                enemyCounter++;
            }
            if (Random.Range(0, 3) % 3 == 2)
            {
                Instantiate(targetEnemy, suiji1, Quaternion.identity);
                enemyCounter++;
            }
            //如果计数达到最大值
            if (enemyCounter >= enemyTotalNum)
            {
                //停止刷新
                CancelInvoke();
            }
        }

    }
}

