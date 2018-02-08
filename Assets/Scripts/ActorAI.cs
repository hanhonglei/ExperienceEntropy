using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Tutorial
{
    public class ActorAI : MonoBehaviour
    {

        public Transform destPoint;
        private NavMeshAgent nav;

        void Awake()
        {
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (!nav)
                return;
            Init();
        }
        public void Init()
        {
            nav.SetDestination(destPoint.position);     // set nav destination
        }

    }
}
