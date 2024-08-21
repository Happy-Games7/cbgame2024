using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
//using UnityEditor.Overlays;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using Unity.Burst.Intrinsics;

public class GameManager : MonoBehaviour
{
    WaitForSeconds waitForSec = new WaitForSeconds(0.05f);    
    public Transform[] spawnTr;    

    int gameTime = 0;        
    int wave = 1;
    int waveTime = 15;

    public enum GameState 
    {
        Title,
        GamePlay,
        GameEnd        
    }

    public float magneticItemDropInterval;
    public float boomItemDropInterval;
    public float potionDropInterval;    

    public GameState gameState = GameState.Title;

    public bool Boss1_Die { get; set; } = false;
    public bool Boss2_Die { get; set; } = false;
    public bool Boss3_Die { get; set; } = false;
    
    private void Awake()
    {
        GameData.Instance.gameManager = this;
    }

    void Start()
    {        
        if(gameState == GameState.Title)
        {
            GameData.Instance.uiManager.Show_UI_Title(true);
        }            
    }       

    void WaveStart()
    {       
        switch (wave)
        {
            case 1:
                StartCoroutine(SpawnMonster(112, 25, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));                
                break;
            case 2:                
                StartCoroutine(SpawnMonster(112, 30, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(116, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                break;
            case 3:
                StartCoroutine(SpawnMonster(112, 35, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(116, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(9, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(3, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 4:
                StartCoroutine(SpawnMonster(112, 40, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(116, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(9, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(3, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));                
                break;
            case 5:                                               
                StartCoroutine(SpawnMonster(112, 45, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(537, 1, MonsterController.MonsterType.Boss, MonsterController.MonsterAttackType.Range));
                GameData.Instance.uiManager.bossHpbarObj1.SetActive(true);
                break;
            case 6:                
                StartCoroutine(SpawnMonster(9, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));   
                
                StartCoroutine(SpawnMonster(3, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 7:                
                StartCoroutine(SpawnMonster(9, 4, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));                
                StartCoroutine(SpawnMonster(10, 1, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 1, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                
                StartCoroutine(SpawnMonster(3, 22, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(53, 50, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));                
                break;
            case 8:              
                StartCoroutine(SpawnMonster(9, 5, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(3, 24, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 4, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(52, 55, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(10, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 1, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));                                        
                
                break;
            case 9:            
                StartCoroutine(SpawnMonster(9, 6, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(10, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 1, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(3, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));                
                StartCoroutine(SpawnMonster(51, 60, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 10:                              
                StartCoroutine(SpawnMonster(3, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 4, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(557, 1, MonsterController.MonsterType.Boss, MonsterController.MonsterAttackType.Melee));
                GameData.Instance.uiManager.bossHpbarObj2.SetActive(true);
                break;
            case 11:
                StartCoroutine(SpawnMonster(3, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 1, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 4, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 12:                
                StartCoroutine(SpawnMonster(3, 10, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(4, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(43, 3, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 5, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(538, 1, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                break;
            case 13:
                StartCoroutine(SpawnMonster(10, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(3, 5, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 12, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(55, 70, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 14:
                StartCoroutine(SpawnMonster(3, 6, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 3, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 15, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(10, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 2, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(55, 80, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                break;
            case 15:
                StartCoroutine(SpawnMonster(10, 3, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(126, 3, MonsterController.MonsterType.Elite, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(3, 6, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                //StartCoroutine(SpawnMonster(4, 2, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                //StartCoroutine(SpawnMonster(43, 4, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Range));
                StartCoroutine(SpawnMonster(138, 15, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));
                StartCoroutine(SpawnMonster(140, 20, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(55, 90, MonsterController.MonsterType.Normal, MonsterController.MonsterAttackType.Melee));

                StartCoroutine(SpawnMonster(513, 1, MonsterController.MonsterType.Boss, MonsterController.MonsterAttackType.Range));
                GameData.Instance.uiManager.bossHpbarObj3.SetActive(true);
                break;
        }        
    }
    
    public void GameStart()
    {
        // BGM START
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("BGM051");

        WaveStart();        

        Item.canDropBoom = false;
        Item.canDropMagnetic = false;
        Item.canDropPotion = false;

        // UI
        CancelInvoke("TimeUpdate");
        InvokeRepeating("TimeUpdate", 1f, 1f);
        
        // Item
        CancelInvoke("CanDropMagneticTrue");
        InvokeRepeating("CanDropMagneticTrue", magneticItemDropInterval, magneticItemDropInterval);

        CancelInvoke("CanDropBoomTrue");
        InvokeRepeating("CanDropBoomTrue", boomItemDropInterval, boomItemDropInterval);

        CancelInvoke("CanDropPotionTrue");
        InvokeRepeating("CanDropPotionTrue", potionDropInterval, potionDropInterval);
    }
   
    public void TimeUpdate()
    {
        if (Time.timeScale == 0) return;
        gameTime += 1;
        int min = gameTime / 60;
        int second = gameTime % 60;
        GameData.Instance.uiManager.gameTime_Text.text = $"{min.ToString("00")} : {second.ToString("00")}";

        // Wave        
        if (wave <= 15) waveTime--;
        if (waveTime <= 0)
        {                                    
            wave++;
            
            if(wave <= 15)
            {
                waveTime = 15;
                GameData.Instance.uiManager.wave_Text.text = $"Wave {wave}";
                WaveStart();
            }
            else // wave 16으로 넘어간 상태
            {
                waveTime = 0;
                GameData.Instance.uiManager.waveTime_Text.text = $"남은 시간 : {waveTime}초";                               
            }
        }
        if (wave <= 15) GameData.Instance.uiManager.waveTime_Text.text = $"남은 시간 : {waveTime}초";

        if (wave >= 15 && Boss1_Die && Boss2_Die && Boss3_Die) // 게임 클리어
        {
            int monsterCount = 0;

            foreach (Transform tr in GameData.Instance.poolManager.transform)
            {
                if (tr.CompareTag("Monster"))
                {                    
                    if (tr.gameObject.activeInHierarchy)
                    {
                        monsterCount++;

                        if (transform.position.x < -19.2f
                            || transform.position.x > 19.2f
                            || transform.position.y < -14.4f
                            || transform.position.y > 14.4f) tr.gameObject.SetActive(false);
                    }
                }
            }

            if (monsterCount == 0) GameData.Instance.uiManager.Show_GameClear(true);
        }

        if (GameData.Instance.playerContoller.skill_hadLearn[11]) // 매초마다 플레이어 체력 회복하는 패시브 스킬
        {
            if (GameData.Instance.playerContoller.current_HP < GameData.Instance.playerContoller.max_HP)
                GameData.Instance.playerContoller.current_HP += (GameData.Instance.playerContoller.skill_Lv[11] * 2f);
            if (GameData.Instance.playerContoller.current_HP > GameData.Instance.playerContoller.max_HP)
                GameData.Instance.playerContoller.current_HP = GameData.Instance.playerContoller.max_HP;
        }
    }

    void CanDropMagneticTrue()
    {
        Item.canDropMagnetic = true;
    }

    void CanDropBoomTrue()
    {
        Item.canDropBoom = true;
    }

    void CanDropPotionTrue()
    {
        Item.canDropPotion = true;
    }
        
    public void SceneMove(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 6.0f);
    }

    IEnumerator SpawnMonster(int prefabNum, int createMonNum, MonsterController.MonsterType mt, MonsterController.MonsterAttackType mat)
    {
        for(int i = 0; i < createMonNum; i++)
        {
            // 랜덤 생성 위치 지정           
            float posX = Random.Range(PlayerContoller.map_Limit_Min_X + 0.7f, PlayerContoller.map_Limit_Max_X - 0.7f);
            float posY = Random.Range(PlayerContoller.map_Limit_Min_Y + 0.7f, PlayerContoller.map_Limit_Max_Y - 0.7f);

            // 생성 위치가 플레이어 위치와 너무 가깝다면 위치 수정
            if (Vector2.Distance(new Vector2(posX, posY), (Vector2)GameData.Instance.playerContoller.transform.position) <= 9.2f)
            {
                List<Vector3> canPos = new List<Vector3>();

                foreach (Transform tr in spawnTr)
                {
                    if (tr.position.x < PlayerContoller.map_Limit_Max_X
                        && tr.position.x > PlayerContoller.map_Limit_Min_X
                        && tr.position.y < PlayerContoller.map_Limit_Max_Y
                        && tr.position.y > PlayerContoller.map_Limit_Min_Y)
                    {
                        canPos.Add(tr.position);
                    }
                }

                if (canPos.Count > 0)
                {
                    Vector3 createPos = canPos[Random.Range(0, canPos.Count)];
                    posX = createPos.x;
                    posY = createPos.y;

                    GameObject enemy = GameData.Instance.poolManager.Get(0);
                    enemy.transform.position = new Vector3(posX, posY, 0);
                    enemy.GetComponent<MonsterController>().InitSetting(prefabNum, mt, mat);

                    canPos.Clear();
                }
            }
            else
            {
                GameObject enemy = GameData.Instance.poolManager.Get(0);
                enemy.transform.position = new Vector3(posX, posY, 0);
                enemy.GetComponent<MonsterController>().InitSetting(prefabNum, mt, mat);
            }
            
            yield return waitForSec;                                   
        }               
    }
       
    // Test ---------------------------------
    void BossSpawn()
    {
        Transform ranTr = spawnTr[Random.Range(0, spawnTr.Length)];

        // 만약 몬스터 생성위치가 맵의 바깥쪽이라면, 안쪽인 위치들을 찾아서 그 중에서 랜덤한 곳으로 지정 
        if (ranTr.position.x >= PlayerContoller.map_Limit_Max_X
            || ranTr.position.x <= PlayerContoller.map_Limit_Min_X
            || ranTr.position.y >= PlayerContoller.map_Limit_Max_Y
            || ranTr.position.y <= PlayerContoller.map_Limit_Min_Y)
        {
            List<Transform> canTr = new List<Transform>();
            foreach (Transform tr in spawnTr)
            {
                if (tr.position.x < PlayerContoller.map_Limit_Max_X
                    && tr.position.x > PlayerContoller.map_Limit_Min_X
                    && tr.position.y < PlayerContoller.map_Limit_Max_Y
                    && tr.position.y > PlayerContoller.map_Limit_Min_Y)
                {
                    canTr.Add(tr);
                }
            }

            if (canTr.Count > 0)
            {
                ranTr = canTr[Random.Range(0, canTr.Count)];
                canTr.Clear();
            }
        }

        GameObject enemy = GameData.Instance.poolManager.Get(0);        
        enemy.transform.position = ranTr.position;
        enemy.GetComponent<MonsterController>().InitSetting(513, MonsterController.MonsterType.Boss, MonsterController.MonsterAttackType.Range);
    }
}