using UnityEngine;
using System.Collections;

public class ItemsCollection : MonoBehaviour {
    public ItemLevel itemLevel = ItemLevel.Level1;
    public Item[] items;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public Item GenerateRandomLevelItem()
    {
        int idx = Random.Range(0, items.Length);
        Debug.Log("Item index:"+idx+"Item level:"+itemLevel);
        return items[idx];
    }
    public Item GetItem(int idx)
    {
        return items[idx % items.Length];
    }
}
