//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The GameObject requires a Rigidbody component

[RequireComponent(typeof(Rigidbody))]
public class Bullet : Item
{
    private Rigidbody rb;
    private bool bMove = true;

    public float speed = 50.0f;
    public float damage = 5.0f;
    public GameObject explosionEffect = null;
    public float effectDurationTime = 5.0f;
    public float damageRadius = 10.0f;
    public WeaponType bulletType = WeaponType.Default;
    public float damagePerSecond = 0.0f;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (pickable)
            return;

        if (bMove)
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    void Explosion(Collider other)
    {
        if (!explosionEffect)
            return;
        GameObject g = (GameObject)GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        switch (bulletType)
        {
            case WeaponType.Pistol:
            case WeaponType.Gun:
            case WeaponType.Laser:
                g.transform.SetParent(other.transform);
                break;
            case WeaponType.Rocket:
            default:
                break;
        }
        Destroy(g, effectDurationTime);
   }

    public override void OnTriggerEnter(Collider other)
    {
        if (pickable)
        {
            base.OnTriggerEnter(other);
            return;
        }
        switch (other.tag)
        {
            case Tags.obstacle:
                bMove = false;
                break;
            case Tags.enemy:
                if (isEnemy)
                    return;
                break;
            case Tags.player:
                if (!isEnemy)
                    return;
                break;
            default:
                //Explosion();
                //bMove = false;
                //GameObject.Destroy(gameObject);
                return;
        }
        //Debug.Log("damage object:" + other.gameObject);
        StartCoroutine("BeginDamage", other);
    }

    private IEnumerator BeginDamage(Collider other)
    {
        //Debug.Log("Called damage");

        GetComponent<Renderer>().enabled = false;
        //transform.SetParent(other.gameObject.transform);

        Actor otherActor = other.GetComponent<Actor>();
        Explosion(other);
        switch (bulletType)
        {
            case WeaponType.Pistol:
                if (!otherActor)
                    break;
                otherActor.Damage(damage);
                break;
            case WeaponType.Gun:
                //Debug.Log("Persistent");
                bMove = false;
                if (!otherActor)
                    break;
                transform.SetParent(other.transform);
                otherActor.Damage(damage);
                float t = 0.0f;
                while (t < effectDurationTime)
                {
                    t += Time.deltaTime;
                    if (otherActor)
                        otherActor.Damage(damagePerSecond * Time.deltaTime);
                    else
                        break;
                    yield return null;
                    //Debug.Log("this time:" + t + "damage:" + damagePerSecond * Time.deltaTime);
                }
                break;
            case WeaponType.Rocket:
                bMove = false;

                var cols = Physics.OverlapSphere(transform.position, damageRadius);
                var damagedActors = new List<Actor>();
                //Debug.Log("overlap" + cols.GetLength(0));
                foreach (var col in cols)
                {
                    Actor a = col.gameObject.GetComponent<Actor>();
                    if (a && !damagedActors.Contains(a))
                    {
                        float r = Vector3.Distance(a.transform.position, transform.position);
                        r = (damageRadius - r) / damageRadius;
                        r = r < 0f ? 0 : r;
                        if ((isEnemy && a.tag == Tags.player)
    || (!isEnemy && a.tag == Tags.enemy))
                            a.Damage(damage * r);
                        damagedActors.Add(a);
                        //Debug.Log("Damage" + a);
                    }
                }
                break;
            case WeaponType.Laser:
                if (!otherActor)
                    break;
                if ((isEnemy && otherActor.tag == Tags.player)
                    || (!isEnemy && otherActor.tag == Tags.enemy))
                    otherActor.Damage(damage);
                break;
            default:
                break;
        }
        GameObject.Destroy(gameObject);
    }
    override public void SetupRandomPickableItem()
    {
        base.SetupRandomPickableItem();
        switch (bulletType)
        {
            case WeaponType.Pistol:
                num = Random.Range(20, 100);
                break;
            case WeaponType.Gun:
                num = Random.Range(10, 30);
                break;
            case WeaponType.Rocket:
                num = Random.Range(5, 20);
                break;
            case WeaponType.Laser:
                num = Random.Range(20, 100);
                break;
            default:
                break;
        }
    }
}
