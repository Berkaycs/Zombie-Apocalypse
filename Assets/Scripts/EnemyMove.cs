using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private Transform _target;
    public NavMeshAgent Agent;
    public bool IsZombie;
    public bool IsBat;
    public bool IsGhost;

    //[SerializeField] private float speed;

    private void Awake()
    {
        _target = GameObject.Find("Parent").GetComponentInChildren<Transform>();
    }

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

        if (IsBat == true || IsGhost == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, JoystickController.Step);
            transform.LookAt(_target);
        }
    }

    void MoveTo()
    {
        Agent.destination = _target.position;
    }
}
