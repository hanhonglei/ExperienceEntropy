using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public Transform weaponPos = null;
    public float health = 100.0f;
    public bool ikActive = false;
    public Transform rightHandObj = null;

    public GameObject[] weapons;
    public ArrayList test;
    public int currWeapon = 0;

    private Animator animator;

    protected Vector3 desPos;
    protected Animator m_Animator;


    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {

                // Set the look target position, if one has been assigned
                //if (desPos)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(desPos);
                }

                // Set the right hand target position and rotation, if one has been assigned
                //if (rightHandObj != null)
                //{
                //    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                //    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                //    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                //    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                //}

            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }    

    // Use this for initialization
    void Start()
    {
        if (!weaponPos)
            weaponPos = gameObject.transform;
        animator = GetComponent<Animator>();
        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = (GameObject)Instantiate(weapons[i], weaponPos.position, Quaternion.identity);
            if (gameObject.tag == Tags.enemy)
                weapons[i].SendMessage("SetAsEnemy", true);
            weapons[i].transform.SetParent(weaponPos);
            weapons[i].SendMessage("SetUsable", i == currWeapon);
            weapons[i].gameObject.SetActive(i==currWeapon);
        }
        m_Animator = GetComponent<Animator>();

    }
    protected virtual void Die()
    {

    }

    // called by the bullet script, cause damage to this object
    virtual public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0f;
            Die();
        }
    }

    public void Fire(Vector3 target)
    {       
        if (weapons.Length > 0)
        {
            weapons[currWeapon].SendMessage("Fire", target);

        }
    }
    public void StopFire()
    {
        if (weapons.Length > 0)
        {
            weapons[currWeapon].SendMessage("StopFire");

        }
    }
}
