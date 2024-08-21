using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using MonsterType = MonsterController.MonsterType;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        ExpGemSmall, // 일반 몬스터가 드랍
        ExpGemMedium, // 엘리트 몬스터가 드랍
        ExpGemLarge, // 보스 몬스터가 드랍  
        Magnetic, // 게임시간이 특정 시간이 되면, 어느 한 몬스터가 드랍 (예. 1~2분마다?)
        Boom, // 게임시간이 특정 시간이 되면, 어느 한 몬스터가 드랍 
        Potion, // 게임시간이 특정 시간이 되면, 어느 한 몬스터가 드랍 
        Treasure, // 엘리트 몬스터를 죽였을때만 나온다
    }
           
    public ItemType itemType { get; private set; }    
    CircleCollider2D circleCollider;
    SpriteRenderer spriteRenderer;

    // GameManager에서 게임시간이 특정 시간이 되면, true로 만들어준다    
    public static bool canDropMagnetic = false;
    public static bool canDropBoom = false;
    public static bool canDropPotion = false;   
    
    public bool MoveExpGem { get; set; } = false;

    private void OnEnable()
    {
        MoveExpGem = false;
        circleCollider.enabled = true;
    }

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }   

    private void Update()
    {       
        if (MoveExpGem)
        {
            if (itemType == ItemType.ExpGemSmall || itemType == ItemType.ExpGemMedium || itemType == ItemType.ExpGemLarge)
            {
                Vector2 dir = (GameData.Instance.playerContoller.transform.position - transform.position).normalized;
                transform.Translate(dir * 11f * Time.deltaTime);
            }
        }        
    }

    void SetItemRange()
    {
        switch (GameData.Instance.playerContoller.itemRange_Lv)
        {
            case 1:
                // circleCollider.radius =
                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
        }
    }
        
    public void MagneticItemInit()
    {
        // SetItemRange();        
        if (canDropMagnetic)
        {
            canDropMagnetic = false;
            itemType = ItemType.Magnetic;            
            ItemSpriteChange(itemType);            
        }
        else gameObject.SetActive(false);
    }

    public void BoomItemInit()
    {
        // SetItemRange();        
        if (canDropBoom)
        {
            canDropBoom = false;
            itemType = ItemType.Boom;
            ItemSpriteChange(itemType);           
        }
        else gameObject.SetActive(false);
    }

    public void PotionItemInit()
    {
        // SetItemRange();        
        if (canDropPotion)
        {
            canDropPotion = false;
            itemType = ItemType.Potion;
            ItemSpriteChange(itemType);            
        }
        else gameObject.SetActive(false);
    }

    public void TreasureItemInit()
    {
        // SetItemRange();           
        itemType = ItemType.Treasure;
        ItemSpriteChange(itemType);                           
    }

    public bool ExpItemInit(MonsterType mt)
    {
        // SetItemRange();        
        switch (mt)
        {
            case MonsterType.Normal:
                itemType = ItemType.ExpGemSmall;
                break;
            case MonsterType.Elite:
                itemType = ItemType.ExpGemMedium;
                break;
            case MonsterType.Boss:
                itemType = ItemType.ExpGemLarge;
                break;            
        }

        ItemSpriteChange(itemType);
        return true;
    }

    void ItemSpriteChange(ItemType it)
    {
        switch (it)
        {
            case ItemType.ExpGemSmall:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Gem_Green");
                spriteRenderer.sortingOrder = 2;
                break;
            case ItemType.ExpGemMedium:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Gem_Blue");
                spriteRenderer.sortingOrder = 3;
                break;
            case ItemType.ExpGemLarge:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Gem_Purple");
                spriteRenderer.sortingOrder = 4;
                break;
            case ItemType.Magnetic:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Magnetic");
                spriteRenderer.sortingOrder = 4;
                break;
            case ItemType.Boom:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Boom");
                spriteRenderer.sortingOrder = 4;
                break;
            case ItemType.Potion:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Potion");
                spriteRenderer.sortingOrder = 4;
                break;
            case ItemType.Treasure:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprite/Item/Treasure");
                spriteRenderer.sortingOrder = 4;
                break;
        }
    }

    void EatItem(ItemType it)
    {
        switch (it)
        {
            case ItemType.ExpGemSmall:
                GameData.Instance.playerContoller.ExpUp(2f);
                break;
            case ItemType.ExpGemMedium:
                GameData.Instance.playerContoller.ExpUp(50f);
                break;
            case ItemType.ExpGemLarge:
                GameData.Instance.playerContoller.ExpUp(300f);
                break;
            case ItemType.Magnetic:                
                GameData.Instance.playerContoller.EatMagnetic();
                break;
            case ItemType.Boom:
                GameData.Instance.playerContoller.EatBoom();
                break;
            case ItemType.Potion:
                GameData.Instance.playerContoller.EatPotion();
                break;
            case ItemType.Treasure:
                GameData.Instance.playerContoller.EatTreasure();
                break;
        }
    }          
       
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            circleCollider.enabled = false;
            StartCoroutine(BeforeEat());
        }
    }
    
    IEnumerator BeforeEat()
    {
        Vector2 dir = (transform.position - GameData.Instance.playerContoller.transform.position).normalized;

        float time = 0;

        // 밀어내기
        if (MoveExpGem == false)
        {
            while (time < 0.2f)
            {
                transform.Translate(dir * 9.5f * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
        }

        // 다시 당기기
        while (Vector2.Distance(transform.position, GameData.Instance.playerContoller.transform.position) >= 0.1f)
        {            
            Vector2 dirOpposite = (GameData.Instance.playerContoller.transform.position - transform.position).normalized;            
            transform.Translate(dirOpposite * 11f * Time.deltaTime);
            yield return null;
        }

        if (Vector2.Distance(transform.position, GameData.Instance.playerContoller.transform.position) <= 0.2f)
        { 
            MatchPlayerPosition();
            yield break;
        }
    }

    void MatchPlayerPosition()
    {
        MoveExpGem = false;
        spriteRenderer.sprite = null;
        EatItem(itemType);
        circleCollider.enabled = true;
        gameObject.SetActive(false);
    }
}
