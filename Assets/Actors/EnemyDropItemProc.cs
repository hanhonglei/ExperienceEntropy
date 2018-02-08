using UnityEngine;
using System.Collections;

public class EnemyDropItemProc : MonoBehaviour {
    public ItemLevel enemyLevel = ItemLevel.Level1;
    public LevelManager levelManager = null;
    public float dropItemPer = 0.1f;                         // 掉落物品的比例，比例越高越能够掉落物品

    // Use this for initialization
    void Start () {
        if (!levelManager)
        {
            levelManager = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LevelManager>();
        }	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public Item DropItem()
    {
        if (Random.Range(0f, 1f) < dropItemPer)
        {
            return levelManager.GenerateRandomItem(transform);
        }
        return null;
    }
}
