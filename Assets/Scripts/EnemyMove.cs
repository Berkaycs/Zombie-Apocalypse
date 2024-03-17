using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform _target;
    public NavMeshAgent Agent;
    public bool IsZombie;
    public bool IsBat;
    public bool IsGhost;

    //[SerializeField] private float speed;

    private void Start()
    {
        if (IsZombie == true)
        {
            MoveTo();
        }
    }

    private void Update()
    {
        //float step = speed * Time.deltaTime; -> try to use the computations in the one update for optimization in mobile

        if(IsBat == true || IsGhost == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, JoystickController.Step);
        }   
    }

    void MoveTo()
    {
        Agent.destination = _target.position;
    }
}
