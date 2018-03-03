using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour {
    private EnemyBehavior enemy = null;

    // give the player options: next level, or stay in this level?
    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Enemy")
            return;       
        Debug.Log("Staying"+other.name);

        EnemyBehavior e = other.gameObject.GetComponent<EnemyBehavior>();
        if (!e)
            return;
        enemy = e;
    }

    public EnemyBehavior CurrentEnemy()
    {
        return enemy;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Enemy")
            return;

        EnemyBehavior e = other.gameObject.GetComponent<EnemyBehavior>();
        if (e == enemy)
            enemy = null;
        Debug.Log("Exit!");
    }
        // Use this for initialization
        void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
