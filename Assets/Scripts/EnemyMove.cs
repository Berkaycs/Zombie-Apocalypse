using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private Transform _target;
    public NavMeshAgent Agent;
    public bool IsZombie;
    public bool IsBat;
    public bool IsGhost;
    //public bool IsEnemyAlive;

    //[SerializeField] private float speed;

    private void Awake()
    {
        //JoystickController.OnEnemyDead += JoystickController_OnEnemyDead;
        _target = GameObject.Find("Parent").GetComponentInChildren<Transform>();
    }

    /*
    private void JoystickController_OnEnemyDead(object sender, EventArgs e)
    {
        IsEnemyAlive = false;
    }
    */

    private void OnEnable()
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
            //transform.LookAt(transform.position + _target.position);
        }
    }

    void MoveTo()
    {
        Agent.destination = _target.position;
    }
}
