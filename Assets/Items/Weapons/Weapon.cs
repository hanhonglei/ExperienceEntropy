using UnityEngine;
using System.Collections;


public class Weapon : Item {
    public GameObject bullet = null;
    public Transform bulletPos = null;
    public WeaponType type = WeaponType.Default;
    public float freezeTime = 1f;

    private float lastShootTime = 0f;
    private GameObject fireParticle = null;

	// Use this for initialization
	void Start () {
        if (!pickable)
            gameObject.GetComponent<SphereCollider>().enabled = false;
        lastShootTime = freezeTime;
        if (bulletPos.childCount > 0)
        {
            fireParticle = bulletPos.GetChild(0).gameObject;
        }
        StopFire();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
        if (pickable)
            return;
        if (lastShootTime < freezeTime)
        {
            lastShootTime += Time.deltaTime;
        }
	}

    public void Fire(Vector3 target)
    {
        if (lastShootTime < freezeTime)
            return;
        Vector3 dir = target - bulletPos.position;
        Vector3 xyProject = Vector3.ProjectOnPlane(dir, Vector3.up);
        if (num <= 0)
            return;
        Vector3 bulletStartPos = bulletPos.position;
        bulletStartPos.y = 1;
        GameObject g = (GameObject)GameObject.Instantiate(bullet, bulletStartPos, Quaternion.FromToRotation(Vector3.forward, xyProject.normalized));
        g.GetComponent<Bullet>().isEnemy = isEnemy;
        if(!isEnemy)
            num--;
        lastShootTime = 0f;
        if (fireParticle)
            fireParticle.SetActive(true);
    }
    public void StopFire()
    {
        if (fireParticle)
            fireParticle.SetActive(false);

    }
    override public void SetupRandomPickableItem()
    {
        base.SetupRandomPickableItem();
        switch (type)
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
