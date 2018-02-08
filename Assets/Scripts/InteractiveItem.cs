using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { TreasureBox, Cockroach, Vase, Pedestrian, Fruit, Spirit, Portal };

public class InteractiveItem : MonoBehaviour
{
    public Type itemType;
    public GameObject explosionEffect = null;
    public float effectDurationTime = 5.0f;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag != Tags.player)
            return;
        switch (itemType)
        {
            case Type.TreasureBox:

            case Type.Cockroach:

            case Type.Fruit:

            case Type.Vase:
                if (explosionEffect)
                {
                    GameObject g = (GameObject)GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
                    Destroy(g, effectDurationTime);
                }
                GameObject.Destroy(gameObject);
                break;
            case Type.Pedestrian:
                transform.Translate(other.transform.forward * 3);
                break;
            case Type.Portal:
                break;
            case Type.Spirit:
                break;

            default:
                break;
        }

        other.gameObject.GetComponent<PlayerControl>().PickupItem(gameObject);
        Destroy(gameObject);
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
