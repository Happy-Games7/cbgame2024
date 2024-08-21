using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
    [Header("UI Title")]    
    public GameObject pressGameStart;

    [Header("UI Game")]
    public Slider bossHpBar_Slider;
    public Slider bossHpBar2_Slider;
    public Slider bossHpBar3_Slider;

    public GameObject bossHpbarObj1;
    public GameObject bossHpbarObj2;
    public GameObject bossHpbarObj3;
    public GameObject pauseCtrlObj;

    public Slider expBar_Slider;
    public TMP_Text playerLevelText;
    public GameObject gamePanel;
    public TMP_Text gameTime_Text;
    public TMP_Text wave_Text;
    public TMP_Text waveTime_Text;
    
    [Header("UI End")]
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

    [Header("UI Skill Select")]
    public GameObject skillCardSelectObj;
    public GameObject aliveSkills;

    [Header("Skill Card 1")]
    public GameObject[] skillCard_1;
    public GameObject[] star_1;
    public GameObject activeBg_1;
    public GameObject passiveBg_1;
    public TextMeshProUGUI skillName_1;
    public TextMeshProUGUI info_1;

    [Header("Skill Card 2")]
    public GameObject[] skillCard_2;
    public GameObject[] star_2;
    public GameObject activeBg_2;
    public GameObject passiveBg_2;
    public TextMeshProUGUI skillName_2;
    public TextMeshProUGUI info_2;

    [Header("Skill Card 3")]
    public GameObject[] skillCard_3;
    public GameObject[] star_3;
    public GameObject activeBg_3;
    public GameObject passiveBg_3;
    public TextMeshProUGUI skillName_3;
    public TextMeshProUGUI info_3;

    [Header("Alive Active Skill")]
    public GameObject[] activeSkill_1;
    public TextMeshProUGUI activeSkill_1_Lv;

    public GameObject[] activeSkill_2;
    public TextMeshProUGUI activeSkill_2_Lv;

    public GameObject[] activeSkill_3;
    public TextMeshProUGUI activeSkill_3_Lv;

    public GameObject[] activeSkill_4;
    public TextMeshProUGUI activeSkill_4_Lv;

    public GameObject[] activeSkill_5;
    public TextMeshProUGUI activeSkill_5_Lv;

    public GameObject[] activeSkill_6;
    public TextMeshProUGUI activeSkill_6_Lv;

    [Header("Alive passive Skill")]
    public GameObject[] passiveSkill_1;
    public TextMeshProUGUI passiveSkill_1_Lv;

    public GameObject[] passiveSkill_2;
    public TextMeshProUGUI passiveSkill_2_Lv;

    public GameObject[] passiveSkill_3;
    public TextMeshProUGUI passiveSkill_3_Lv;

    public GameObject[] passiveSkill_4;
    public TextMeshProUGUI passiveSkill_4_Lv;

    public GameObject[] passiveSkill_5;
    public TextMeshProUGUI passiveSkill_5_Lv;

    public GameObject[] passiveSkill_6;
    public TextMeshProUGUI passiveSkill_6_Lv;

    [Header("Joystic")]
    public GameObject joystic;

    private void Awake() 
    {
        GameData.Instance.uiManager = this;        
    }
  
    private void Update()
    {
        if(GameData.Instance.gameManager.gameState == GameManager.GameState.Title)
            if (Input.anyKeyDown)
            {
                GameStartButtonDown();
            }
    }
       
    public void Show_GameOver(bool set)
    {
        Time.timeScale = set ? 0 : 1;
        joystic.SetActive(!set);
        gameOverPanel.SetActive(set);

        // BGM STOP
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("BGM102");
    }

    public void Show_GameClear(bool set)
    {
        Time.timeScale = set ? 0 : 1;
        joystic.SetActive(!set);
        gameClearPanel.SetActive(set);

        // BGM STOP
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("BGM011");
    }

    public void Show_UI_Title(bool set)
    {
        Time.timeScale = set ? 0 : 1;
        joystic.SetActive(!set);
        pressGameStart.SetActive(set);          
    }

    int _ranNum1;
    int _ranNum2;
    int _ranNum3;

    public void Show_Skill_Select(bool setActive)
    {                        
        // 이미 습득한 스킬 이미지와 레벨 표시 필요

        if (setActive)
        {
            Time.timeScale = 0;

            joystic.transform.localScale = Vector3.zero;

            skillCardSelectObj.SetActive(true);
            aliveSkills.SetActive(true);

            Skill_Image_And_Star_SetActiveFalse();
            AliveSkillSetActiveTrue();

            int ranNum1;
            int ranNum2;
            int ranNum3;

            // 랜덤한 스킬 카드 번호 3가지
            ranNum1 = Random.Range(0, 9);            

            ranNum2 = Random.Range(0, 9);
            while (ranNum2 == ranNum1) ranNum2 = Random.Range(0, 9);

            ranNum3 = Random.Range(0, 15);
            while (ranNum3 == ranNum1 || ranNum3 == ranNum2) ranNum3 = Random.Range(0, 15);

            _ranNum1 = ranNum1;
            _ranNum2 = ranNum2;
            _ranNum3 = ranNum3;

            // 첫번째 제시 카드
            skillCard_1[ranNum1].SetActive(true);

            if (ranNum1 <= 8)
            {
                activeBg_1.SetActive(true);
                passiveBg_1.SetActive(false);
            }
            else
            {
                activeBg_1.SetActive(false);
                passiveBg_1.SetActive(true);
            }
                       
            for (int i = 0; i < GameData.Instance.playerContoller.skill_Lv[ranNum1]; i++)
            {
                if (GameData.Instance.playerContoller.skill_Lv[ranNum1] >= 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        star_1[j].SetActive(true);
                    }
                }
                else star_1[i].SetActive(true);
            }
            
            skillName_1.text = GetSkillName(ranNum1);
            info_1.text = GetSkillInfo(ranNum1);

                        
            // 두번째 제시 카드
            skillCard_2[ranNum2].SetActive(true);

            if (ranNum2 <= 8)
            {
                activeBg_2.SetActive(true);
                passiveBg_2.SetActive(false);
            }
            else
            {
                activeBg_2.SetActive(false);
                passiveBg_2.SetActive(true);
            }

            for (int i = 0; i < GameData.Instance.playerContoller.skill_Lv[ranNum2]; i++)
            {
                if (GameData.Instance.playerContoller.skill_Lv[ranNum2] >= 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        star_2[j].SetActive(true);
                    }
                }
                else star_2[i].SetActive(true);
            }

            skillName_2.text = GetSkillName(ranNum2);
            info_2.text = GetSkillInfo(ranNum2);


            // 세번째 제시 카드
            skillCard_3[ranNum3].SetActive(true);

            if (ranNum3 <= 8)
            {
                activeBg_3.SetActive(true);
                passiveBg_3.SetActive(false);
            }
            else
            {
                activeBg_3.SetActive(false);
                passiveBg_3.SetActive(true);
            }

            for (int i = 0; i < GameData.Instance.playerContoller.skill_Lv[ranNum3]; i++)
            {
                if (GameData.Instance.playerContoller.skill_Lv[ranNum3] >= 5)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        star_3[j].SetActive(true);
                    }
                }
                else star_3[i].SetActive(true);
            }

            skillName_3.text = GetSkillName(ranNum3);
            info_3.text = GetSkillInfo(ranNum3);
        }
        else
        {
            Skill_Image_And_Star_SetActiveFalse();
            skillCardSelectObj.SetActive(false);
            aliveSkills.SetActive(false);
        }        
    }
    
    public void Skill_Image_And_Star_SetActiveFalse()
    {       
        for (int i = 0; i < skillCard_1.Length; i++)
        {
            skillCard_1[i].SetActive(false);
        }

        for (int i = 0; i < skillCard_2.Length; i++)
        {
            skillCard_2[i].SetActive(false);
        }

        for (int i = 0; i < skillCard_3.Length; i++)
        {
            skillCard_3[i].SetActive(false);
        }
        
        for (int i = 0; i < star_1.Length; i++)
        {
            star_1[i].SetActive(false);
        }

        for (int i = 0; i < star_2.Length; i++)
        {
            star_2[i].SetActive(false);
        }

        for (int i = 0; i < star_3.Length; i++)
        {
            star_3[i].SetActive(false);
        }
    }
   
    public void SkillSelectButton1_Down()
    {
        Invoke("LvUp_ParticlePlay", 0.1f);
        
        // 효과음 
        SoundManager.Instance.PlaySe("IE007");

        Show_Skill_Select(false);
        GameData.Instance.playerContoller.SkillSelect(_ranNum1);
    
        if (_ranNum1 < 9)
        {
            for (int i = 0; i < activeSkillSaveArr.Length; i++)
            {
                if (activeSkillSaveArr[i] == _ranNum1) break;

                if (activeSkillSaveArr[i] == -1)
                {
                    activeSkillSaveArr[i] = _ranNum1;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < passiveSkillSaveArr.Length; i++)
            {
                if (passiveSkillSaveArr[i] == _ranNum1) break;

                if (passiveSkillSaveArr[i] == -1)
                {
                    passiveSkillSaveArr[i] = _ranNum1;
                    break;
                }
            }
        }
    }

    public void SkillSelectButton2_Down()
    {
        Invoke("LvUp_ParticlePlay", 0.1f);

        // 효과음 
        SoundManager.Instance.PlaySe("IE007");

        Show_Skill_Select(false);
        GameData.Instance.playerContoller.SkillSelect(_ranNum2);
        
        if(_ranNum2 < 9)
        {
            for (int i = 0; i < activeSkillSaveArr.Length; i++)
            {
                if (activeSkillSaveArr[i] == _ranNum2) break;

                if (activeSkillSaveArr[i] == -1)
                {
                    activeSkillSaveArr[i] = _ranNum2;
                    break;
                }
            }
        }
        else
        {
            for(int i = 0; i < passiveSkillSaveArr.Length; i++)
            {
                if (passiveSkillSaveArr[i] == _ranNum2) break;

                if (passiveSkillSaveArr[i] == -1)
                {
                    passiveSkillSaveArr[i] = _ranNum2;
                    break;
                }
            }
        }             
    }

    public void SkillSelectButton3_Down()
    {
        Invoke("LvUp_ParticlePlay", 0.1f);

        // 효과음 
        SoundManager.Instance.PlaySe("IE007");

        Show_Skill_Select(false);
        GameData.Instance.playerContoller.SkillSelect(_ranNum3);
        
        if (_ranNum3 < 9)
        {
            for (int i = 0; i < activeSkillSaveArr.Length; i++)
            {
                if(activeSkillSaveArr[i] == _ranNum3) break;

                if (activeSkillSaveArr[i] == -1)
                {
                    activeSkillSaveArr[i] = _ranNum3;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < passiveSkillSaveArr.Length; i++)
            {
                if (passiveSkillSaveArr[i] == _ranNum3) break;

                if (passiveSkillSaveArr[i] == -1)
                {
                    passiveSkillSaveArr[i] = _ranNum3;
                    break;
                }
            }
        }
    }

    void LvUp_ParticlePlay()
    {
        GameData.Instance.playerContoller.lvUp_Particle.Play();
    }

    public void GameStartButtonDown()
    {
        Show_UI_Title(false);
        GameData.Instance.gameManager.gameState = GameManager.GameState.GamePlay;
        GameData.Instance.gameManager.GameStart();     
    }

    public void PauseButtonDown()
    {
        Time.timeScale = 0;

        pauseCtrlObj.SetActive(true);
        aliveSkills.SetActive(true);
        joystic.transform.localScale = Vector3.zero;
        AliveSkillSetActiveTrue();
    }

    public void ResumeButtonDown()
    {
        Time.timeScale = 1;

        pauseCtrlObj.SetActive(false);
        aliveSkills.SetActive(false);
        joystic.transform.localScale = Vector3.one;
    }

    public void ExitButtonDown() // 나가기 버튼 - 누를 시, 게임 재시작 버튼을 누르는 것과 동일. 나중에 수정해야할수도 있음.
    {
        Show_GameClear(false);
        Show_GameOver(false);
        GameData.Instance.gameManager.SceneMove(0);
    }

    public void GameRestartButtonDown() // 재시작 버튼
    {
        Show_GameClear(false);
        Show_GameOver(false);     
        GameData.Instance.gameManager.SceneMove(1);
    }

    /*
            001. 윈드 애로우 [0]
            002. 서먼 위스프 [1]
            003. 라이트닝 볼트 [2]
            004. 프로즌 필드 [3]
            005. 파이어 노바 [4]
            006. 어스퀘이크 [5]
            201. 사이클론 [6]
            205. 메테오 [7]
            206. 쇼크웨이브 [8]
            
            102. 체력 강화 [9]
            103. 공격력 증가 [10]
            104. 방어 강화 [11]
            105. 민첩력 상승 [12]
            -102. 스킬 쿨타임 감소 [13]
            -053. 냉기 반격 [14]
        */

    // 여기 저장된 숫자가 무엇이냐에 따라, 어떤 스킬 이미지를 활성화 할지가 정해진다
    int[] activeSkillSaveArr = new int[6] { 0, -1, -1, -1, -1, -1 }; // UI창에 보여지는 순서대로 나열 한것, 
    int[] passiveSkillSaveArr = new int[6] { -1, -1, -1, -1, -1, -1 }; // UI창에 보여지는 순서대로 나열 한것, 
    
    // 새로운 스킬을 올렸으면, 이미지 활성화
    public void AliveSkillSetActiveTrue()
    {
        for (int i = 0; i < activeSkillSaveArr.Length; i++)
        {
            if (activeSkillSaveArr[i] == -1) break;

            switch (activeSkillSaveArr[i])
            {              
                case 0:  //text = "윈드 애로우";
                    if (i == 0)
                    {
                        activeSkill_1[0].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }
                    else if(i == 1)
                    {
                        activeSkill_2[0].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }
                    else if(i == 2)
                    {
                        activeSkill_3[0].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[0].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }
                    else if(i == 4)
                    {
                        activeSkill_5[0].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }
                    else if(i == 5)
                    {
                        activeSkill_6[0].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[0]}";
                    }                                       
                    break;
                case 1:
                    //text = "서먼 위스프";
                    if (i == 0)
                    {
                        activeSkill_1[1].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[1].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[1].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[1].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[1].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[1].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[1]}";
                    }
                    break;
                case 2:
                    //text = "라이트닝 볼트";
                    if (i == 0)
                    {
                        activeSkill_1[2].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[2].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[2].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[2].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[2].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[2].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[2]}";
                    }
                    break;
                case 3:
                    //text = "프로즌 필드";
                    if (i == 0)
                    {
                        activeSkill_1[3].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[3].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[3].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[3].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[3].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[3].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[3]}";
                    }
                    break;
                case 4:
                    //text = "파이어 노바";
                    if (i == 0)
                    {
                        activeSkill_1[4].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[4].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[4].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[4].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[4].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[4].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[4]}";
                    }
                    break;
                case 5:
                    //text = "어스퀘이크";
                    if (i == 0)
                    {
                        activeSkill_1[5].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[5].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[5].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[5].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[5].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[5].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[5]}";
                    }
                    break;
                case 6:
                    //text = "사이클론";
                    if (i == 0)
                    {
                        activeSkill_1[6].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[6].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[6].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[6].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[6].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[6].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[6]}";
                    }
                    break;
                case 7:
                    //text = "메테오";
                    if (i == 0)
                    {
                        activeSkill_1[7].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[7].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[7].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[7].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[7].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[7].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[7]}";
                    }
                    break;
                case 8:
                    //text = "쇼크웨이브";
                    if (i == 0)
                    {
                        activeSkill_1[8].SetActive(true);
                        activeSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    else if (i == 1)
                    {
                        activeSkill_2[8].SetActive(true);
                        activeSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    else if (i == 2)
                    {
                        activeSkill_3[8].SetActive(true);
                        activeSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    else if (i == 3)
                    {
                        activeSkill_4[8].SetActive(true);
                        activeSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    else if (i == 4)
                    {
                        activeSkill_5[8].SetActive(true);
                        activeSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    else if (i == 5)
                    {
                        activeSkill_6[8].SetActive(true);
                        activeSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[8]}";
                    }
                    break;               
            }
        }

        /*
         * 102. 체력 강화 [9] <- 이 인덱스는 플레이어에 있는 레벨 변수의 인덱스 ,  활성화할 이미지 인덱스 0
            103. 공격력 증가 [10]  활성화할 이미지 인덱스 1
            104. 방어 강화 [11]  활성화할 이미지 인덱스 2 
            105. 민첩력 상승 [12]  활성화할 이미지 인덱스 3
            -102. 스킬 쿨타임 감소 [13]  활성화할 이미지 인덱스 4
            -053. 냉기 반격 [14]  활성화할 이미지 인덱스 5
         */

        for (int i = 0; i < passiveSkillSaveArr.Length; i++)
        {
            if (passiveSkillSaveArr[i] == -1) break;

            switch (passiveSkillSaveArr[i])
            {               
                case 9:
                    //text = "체력 강화";
                    if (i == 0)
                    {
                        passiveSkill_1[0].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[0].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[0].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[0].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[0].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[0].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[9]}";
                    }
                    break;
                case 10:
                    //text = "공격력 증가";
                    if (i == 0)
                    {
                        passiveSkill_1[1].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[1].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[1].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[1].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[1].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[1].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[10]}";
                    }
                    break;
                case 11:
                    //text = "방어 강화";
                    if (i == 0)
                    {
                        passiveSkill_1[2].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[2].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[2].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[2].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[2].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[2].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[11]}";
                    }
                    break;
                case 12:
                    //text = "민첩력 상승";
                    if (i == 0)
                    {
                        passiveSkill_1[3].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[3].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[3].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[3].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[3].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[3].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[12]}";
                    }
                    break;
                case 13:
                    //text = "스킬 쿨타임 감소";
                    if (i == 0)
                    {
                        passiveSkill_1[4].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[4].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[4].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[4].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[4].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[4].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[13]}";
                    }
                    break;
                case 14:
                    //text = "냉기 반격";
                    if (i == 0)
                    {
                        passiveSkill_1[5].SetActive(true);
                        passiveSkill_1_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    else if (i == 1)
                    {
                        passiveSkill_2[5].SetActive(true);
                        passiveSkill_2_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    else if (i == 2)
                    {
                        passiveSkill_3[5].SetActive(true);
                        passiveSkill_3_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    else if (i == 3)
                    {
                        passiveSkill_4[5].SetActive(true);
                        passiveSkill_4_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    else if (i == 4)
                    {
                        passiveSkill_5[5].SetActive(true);
                        passiveSkill_5_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    else if (i == 5)
                    {
                        passiveSkill_6[5].SetActive(true);
                        passiveSkill_6_Lv.text = $"Lv. {GameData.Instance.playerContoller.skill_Lv[14]}";
                    }
                    break;
            }
        }        
    }
                          
    string GetSkillName(int num)
    {
        string text = "";

        switch (num)
        {
            case 0:
                text = "윈드 애로우";
                break;
            case 1:
                text = "서먼 위스프";
                break;
            case 2:
                text = "라이트닝 볼트";
                break;
            case 3:
                text = "프로즌 필드";
                break;
            case 4:
                text = "파이어 노바";
                break;
            case 5:
                text = "어스퀘이크";
                break;
            case 6:
                text = "사이클론";
                break;
            case 7:
                text = "메테오";
                break;
            case 8:
                text = "쇼크웨이브";
                break;
            case 9:
                text = "체력 강화";
                break;
            case 10:
                text = "공격력 증가";
                break;
            case 11:
                text = "방어 강화";
                break;
            case 12:
                text = "민첩력 상승";
                break;
            case 13:
                text = "스킬 쿨타임 감소";
                break;
            case 14:
                text = "냉기 반격";
                break;           
        }

        return text;
    }
   
    string GetSkillInfo(int num)
    {
        string text = "";

        switch (num)
        {
            case 0:
                text = "마법 화살을 발사한다.\n발사체 수 증가\n대미지 증가\n관통 횟수 증가\n쿨타임 감소";
                break;
            case 1:
                text = "주위의 적들에게 피해를 입히고 체력을 흡수한다.\n대미지 증가\n체력흡수량 증가\n지속시간 증가\n쿨타임 감소";
                break;
            case 2:
                text = "주위의 적들에게 피해를 입히고 감전 상태로 만든다.\n대미지 증가\n쿨타임 감소";
                break;
            case 3:
                text = "주위의 적들에게 피해를 입히고 동상 상태로 만든다.\n대미지 증가\n지속시간 증가\n쿨타임 감소";
                break;
            case 4:
                text = "주위의 적들에게 피해를 입히고 화상 상태로 만든다.\n대미지 증가\n쿨타임 감소";
                break;
            case 5:
                text = "주위의 적들에게 피해를 입히고 기절 상태로 만든다.\n대미지 증가\n쿨타임 감소";
                break;
            case 6:
                text = "적을 관통하는 회오리바람을 발사해서 광역피해를 입히고, 강하게 밀어낸다.\n대미지 증가\n쿨타임 감소";
                break;
            case 7:
                text = "적에게 운석을 떨어뜨려, 피해를 입히고 화상 상태로 만든다.\n대미지 증가\n쿨타임 감소";
                break;
            case 8:
                text = "적에게 진동을 일으켜, 피해를 입히고 기절 상태로 만든다.\n대미지 증가\n쿨타임 감소";
                break;
            case 9:
                text = "최대 체력이 20% 증가한다.\n모든 체력을 회복시킨다.";
                break;
            case 10:
                text = "모든 공격스킬의 공격력이 +20% 증가한다.";
                break;
            case 11:
                text = "받는 피해가 -10% 감소한다.\n매초당 체력 +2 회복한다.";
                break;
            case 12:
                text = "이동속도가 증가한다.\n+12% 확률로 적의 공격을 회피한다.";
                break;
            case 13:
                text = "모든 공격스킬의 쿨타임이 -5% 감소한다.\n쿨타임 감소";
                break;
            case 14:
                text = "부딪힌 적에게 +25 피해를 입히고, 동상 상태로 만든다.\n대미지 증가";
                break;
        }

        return text;
    }
}
