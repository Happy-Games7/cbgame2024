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
        // �̹� ������ ��ų �̹����� ���� ǥ�� �ʿ�

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

            // ������ ��ų ī�� ��ȣ 3����
            ranNum1 = Random.Range(0, 9);            

            ranNum2 = Random.Range(0, 9);
            while (ranNum2 == ranNum1) ranNum2 = Random.Range(0, 9);

            ranNum3 = Random.Range(0, 15);
            while (ranNum3 == ranNum1 || ranNum3 == ranNum2) ranNum3 = Random.Range(0, 15);

            _ranNum1 = ranNum1;
            _ranNum2 = ranNum2;
            _ranNum3 = ranNum3;

            // ù��° ���� ī��
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

                        
            // �ι�° ���� ī��
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


            // ����° ���� ī��
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
        
        // ȿ���� 
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

        // ȿ���� 
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

        // ȿ���� 
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

    public void ExitButtonDown() // ������ ��ư - ���� ��, ���� ����� ��ư�� ������ �Ͱ� ����. ���߿� �����ؾ��Ҽ��� ����.
    {
        Show_GameClear(false);
        Show_GameOver(false);
        GameData.Instance.gameManager.SceneMove(0);
    }

    public void GameRestartButtonDown() // ����� ��ư
    {
        Show_GameClear(false);
        Show_GameOver(false);     
        GameData.Instance.gameManager.SceneMove(1);
    }

    /*
            001. ���� �ַο� [0]
            002. ���� ������ [1]
            003. ����Ʈ�� ��Ʈ [2]
            004. ������ �ʵ� [3]
            005. ���̾� ��� [4]
            006. �����ũ [5]
            201. ����Ŭ�� [6]
            205. ���׿� [7]
            206. ��ũ���̺� [8]
            
            102. ü�� ��ȭ [9]
            103. ���ݷ� ���� [10]
            104. ��� ��ȭ [11]
            105. ��ø�� ��� [12]
            -102. ��ų ��Ÿ�� ���� [13]
            -053. �ñ� �ݰ� [14]
        */

    // ���� ����� ���ڰ� �����̳Ŀ� ����, � ��ų �̹����� Ȱ��ȭ ������ ��������
    int[] activeSkillSaveArr = new int[6] { 0, -1, -1, -1, -1, -1 }; // UIâ�� �������� ������� ���� �Ѱ�, 
    int[] passiveSkillSaveArr = new int[6] { -1, -1, -1, -1, -1, -1 }; // UIâ�� �������� ������� ���� �Ѱ�, 
    
    // ���ο� ��ų�� �÷�����, �̹��� Ȱ��ȭ
    public void AliveSkillSetActiveTrue()
    {
        for (int i = 0; i < activeSkillSaveArr.Length; i++)
        {
            if (activeSkillSaveArr[i] == -1) break;

            switch (activeSkillSaveArr[i])
            {              
                case 0:  //text = "���� �ַο�";
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
                    //text = "���� ������";
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
                    //text = "����Ʈ�� ��Ʈ";
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
                    //text = "������ �ʵ�";
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
                    //text = "���̾� ���";
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
                    //text = "�����ũ";
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
                    //text = "����Ŭ��";
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
                    //text = "���׿�";
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
                    //text = "��ũ���̺�";
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
         * 102. ü�� ��ȭ [9] <- �� �ε����� �÷��̾ �ִ� ���� ������ �ε��� ,  Ȱ��ȭ�� �̹��� �ε��� 0
            103. ���ݷ� ���� [10]  Ȱ��ȭ�� �̹��� �ε��� 1
            104. ��� ��ȭ [11]  Ȱ��ȭ�� �̹��� �ε��� 2 
            105. ��ø�� ��� [12]  Ȱ��ȭ�� �̹��� �ε��� 3
            -102. ��ų ��Ÿ�� ���� [13]  Ȱ��ȭ�� �̹��� �ε��� 4
            -053. �ñ� �ݰ� [14]  Ȱ��ȭ�� �̹��� �ε��� 5
         */

        for (int i = 0; i < passiveSkillSaveArr.Length; i++)
        {
            if (passiveSkillSaveArr[i] == -1) break;

            switch (passiveSkillSaveArr[i])
            {               
                case 9:
                    //text = "ü�� ��ȭ";
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
                    //text = "���ݷ� ����";
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
                    //text = "��� ��ȭ";
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
                    //text = "��ø�� ���";
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
                    //text = "��ų ��Ÿ�� ����";
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
                    //text = "�ñ� �ݰ�";
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
                text = "���� �ַο�";
                break;
            case 1:
                text = "���� ������";
                break;
            case 2:
                text = "����Ʈ�� ��Ʈ";
                break;
            case 3:
                text = "������ �ʵ�";
                break;
            case 4:
                text = "���̾� ���";
                break;
            case 5:
                text = "�����ũ";
                break;
            case 6:
                text = "����Ŭ��";
                break;
            case 7:
                text = "���׿�";
                break;
            case 8:
                text = "��ũ���̺�";
                break;
            case 9:
                text = "ü�� ��ȭ";
                break;
            case 10:
                text = "���ݷ� ����";
                break;
            case 11:
                text = "��� ��ȭ";
                break;
            case 12:
                text = "��ø�� ���";
                break;
            case 13:
                text = "��ų ��Ÿ�� ����";
                break;
            case 14:
                text = "�ñ� �ݰ�";
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
                text = "���� ȭ���� �߻��Ѵ�.\n�߻�ü �� ����\n����� ����\n���� Ƚ�� ����\n��Ÿ�� ����";
                break;
            case 1:
                text = "������ ���鿡�� ���ظ� ������ ü���� ����Ѵ�.\n����� ����\nü������� ����\n���ӽð� ����\n��Ÿ�� ����";
                break;
            case 2:
                text = "������ ���鿡�� ���ظ� ������ ���� ���·� �����.\n����� ����\n��Ÿ�� ����";
                break;
            case 3:
                text = "������ ���鿡�� ���ظ� ������ ���� ���·� �����.\n����� ����\n���ӽð� ����\n��Ÿ�� ����";
                break;
            case 4:
                text = "������ ���鿡�� ���ظ� ������ ȭ�� ���·� �����.\n����� ����\n��Ÿ�� ����";
                break;
            case 5:
                text = "������ ���鿡�� ���ظ� ������ ���� ���·� �����.\n����� ����\n��Ÿ�� ����";
                break;
            case 6:
                text = "���� �����ϴ� ȸ�����ٶ��� �߻��ؼ� �������ظ� ������, ���ϰ� �о��.\n����� ����\n��Ÿ�� ����";
                break;
            case 7:
                text = "������ ��� ����߷�, ���ظ� ������ ȭ�� ���·� �����.\n����� ����\n��Ÿ�� ����";
                break;
            case 8:
                text = "������ ������ ������, ���ظ� ������ ���� ���·� �����.\n����� ����\n��Ÿ�� ����";
                break;
            case 9:
                text = "�ִ� ü���� 20% �����Ѵ�.\n��� ü���� ȸ����Ų��.";
                break;
            case 10:
                text = "��� ���ݽ�ų�� ���ݷ��� +20% �����Ѵ�.";
                break;
            case 11:
                text = "�޴� ���ذ� -10% �����Ѵ�.\n���ʴ� ü�� +2 ȸ���Ѵ�.";
                break;
            case 12:
                text = "�̵��ӵ��� �����Ѵ�.\n+12% Ȯ���� ���� ������ ȸ���Ѵ�.";
                break;
            case 13:
                text = "��� ���ݽ�ų�� ��Ÿ���� -5% �����Ѵ�.\n��Ÿ�� ����";
                break;
            case 14:
                text = "�ε��� ������ +25 ���ظ� ������, ���� ���·� �����.\n����� ����";
                break;
        }

        return text;
    }
}
