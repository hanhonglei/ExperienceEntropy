using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum GameLevel { Level1 = 0, Level2, Level3, Level4};
public class LevelManager : MonoBehaviour {

    public GameLevel currentLevel = GameLevel.Level1;
    public ItemsCollection[] allLevelItems;

    public int killEnemyNum = 0;
    public int[][] test = new int[3][];

    private Actor player;
    private Text gameInfo;

	// Use this for initialization
	void Start () {
        Random.InitState(System.Environment.TickCount);
        player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Actor>();
        gameInfo = GameObject.Find("Text").GetComponent<Text>();
	} 
	
	// Update is called once per frame
	void Update () {
        if (!player || !gameInfo)
            return;
        Weapon w = player.weapons[player.currWeapon].GetComponent<Weapon>();
        gameInfo.text = "Kill enemies:" + killEnemyNum + "\n" + "Player health:" + (int)(player.health+0.5) + "\n"
            + "Weapon:" + w.type.ToString() +"\n"+ "Ammo:" + w.num;
	}

    public Item GenerateRandomItem(Transform dropper)
    {
        float r = Random.value;
        Debug.Log("Random number:" + r);
        int itemLevel = (int)currentLevel;
        int enemyLevel = (int)dropper.gameObject.GetComponent<EnemyDropItemProc>().enemyLevel;
        if (r < 0.8)        // There is 80% to generate the same level item
        {
            itemLevel = enemyLevel;
        }
        else if (r < 0.9)    // 10% to generate 1 higher level item
        {
            itemLevel += 1;
        }
        else if (r < 0.95)   // 5% to generate 2 higher level item
        {
            itemLevel += 2;
        }
        else if (r < 0.96)   // 1% to generate particular, means special, level item
        {
            itemLevel = (int)ItemLevel.Particular;
        }
        else
            return null;
        // if the enemy itself is special level, it has more opportunity to drop special item
        if (enemyLevel == (int)ItemLevel.Particular)
        {
            if (Random.value < 0.33)
            {
                itemLevel = (int)ItemLevel.Particular;
            }
        }
        itemLevel = (itemLevel < allLevelItems.Length) ? itemLevel : (allLevelItems.Length - 1);

        Debug.Log("Drop item level:" + itemLevel);

        Item droppedItem = allLevelItems[itemLevel].GenerateRandomLevelItem();
        Debug.Assert(droppedItem!=null);     
    
        droppedItem = (Item)GameObject.Instantiate(droppedItem, dropper.position, Quaternion.identity);
        droppedItem.SetupRandomPickableItem();

        return droppedItem;
    }
}
