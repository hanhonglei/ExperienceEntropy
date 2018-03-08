using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { Default, TreasureBox, Stepable, Uninteractable, Pedestrian, Fruit, Spirit, Portal };

public class InteractiveItem : MonoBehaviour
{
    public Type itemType;
    public GameObject explosionEffect = null;
    public float effectDurationTime = 5.0f;
    private Animator anim = null;
    private int interactionTime = 0;
    private bool rotating = false;
    private Quaternion origQ;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (tag == Tags.enemy)
        {
            if (other.tag != "AttackZone")
                return;
        }
        else if (other.tag != Tags.player)
            return;
        if (explosionEffect)
        {
            GameObject g = (GameObject)GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(g, effectDurationTime);
        }
        switch (itemType)
        {
            case Type.TreasureBox:

            case Type.Fruit:
                if (anim)
                {
                    anim.SetTrigger("rotate");
                }
                break;

            case Type.Uninteractable:
                break;
            case Type.Stepable:
            case Type.Pedestrian:
                //transform.Translate(other.transform.forward * 3);
                if (anim)
                {
                    anim.SetTrigger("DeathTrigger");
                }
                break;
            case Type.Portal:
                break;
            case Type.Spirit:
                break;

            default:
                break;
        }
        interactionTime++;
        PlayerControl pc = other.gameObject.GetComponent<PlayerControl>();
        if (pc)
            pc.PickupItem(gameObject);
    }
    // give the player options: next level, or stay in this level?
    void OnTriggerStay(Collider other)
    {
        if (other.tag != Tags.player)
            return;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag != Tags.player)
            return;
    }
    public int InteractionTimes()
    {
        return interactionTime;
    }
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        origQ = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
