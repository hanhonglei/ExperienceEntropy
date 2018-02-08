using UnityEngine;
using System.Collections;

public enum ItemType { Weapon, Bullet, Intelligence, Power, Speed, Health };


public enum WeaponType { Default, Pistol, Gun, Rocket, Laser };

public enum ItemLevel { Level1 = 0, Level2, Level3, Level4, Particular }

public class Item : MonoBehaviour {
    public bool pickable = false;
    public float rotSpeed = 10.0f;
    public bool usable = true;
    public bool isEnemy = false;
    public float num = 1f;
    public float numRandomRange = 20f;

    public ItemType itemType;
    public ItemLevel level;
    

	// Update is called once per frame
	public virtual void Update () {
        if (pickable)
            transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
	}
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag != Tags.player || !pickable)
            return;
        other.gameObject.GetComponent<PlayerControl>().PickupItem(gameObject);
        Destroy(gameObject);
    }
    virtual public void SetupRandomPickableItem()
    {
        pickable = true;
        num = Random.Range(num, num+numRandomRange);
    }
    public void SetAsEnemy(bool e = true)
    {
        isEnemy = e;
    }
    public void SetUsable(bool u = true)
    {
        usable = u;
    }

    public void PickUp(Item it)
    {
        if (usable)
            num += it.num;
        else
            num = it.num;
    }

}
