using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class JoystickController : MonoBehaviour
{
    public static event EventHandler OnEnemyDead;

    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _parentCam;

    private float _rotHAmount;
    private float _rotVAmount;
    public float RotationSpeed = 45;

    // Enemy Move 
    private float Speed = 4;
    public static float Step;

    // Shooting
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private Animator _armsAnim;
    [SerializeField] private AudioSource _gunShotSound;
    [SerializeField] private LayerMask _targetLayer;
    private WaitForSeconds _zombieDeathTime = new WaitForSeconds(2); // otherwise it constantly create new waitforsecond object which makes to work GC in some place and decrease the performance
    private WaitForSeconds _batDeathTime = new WaitForSeconds(1);
    private WaitForSeconds _ghostDeathTime = new WaitForSeconds(1); 
    private RaycastHit _hit;

    // Particles
    [SerializeField] private GameObject _particleFXZombie;
    [SerializeField] private GameObject _particleFXBat;
    [SerializeField] private GameObject _particleFXGhost;

    // Player Damage
    public bool ZombieAttack = false;
    public bool GhostAttack = false;
    public bool BatAttack = false;

    // Reloading
    [SerializeField] private GameObject _reloadButton;
    [SerializeField] private List<GameObject> _bullets = new List<GameObject>();
    [SerializeField] private List<GameObject> _deactiveBullets = new List<GameObject>();
    WaitForSeconds ReloadPause = new WaitForSeconds(2);
    private int _index = 0;

    // Sound Effects
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _reloadFX;

    // Timer
    [SerializeField] private TextMeshProUGUI _timer;
    WaitForSeconds TimeDelay = new WaitForSeconds(1);
    private int _levelTimeLimit = 30;
    private int _levelCurrentTime;

    // Level Up
    public static int LevelNumber = 1;
    [SerializeField] private GameObject _levelUp;
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Awake()
    {
        _levelUp.SetActive(false);

        for (int i = 0; i < _bullets.Count; i++)
        {
            _bullets[i].gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        _levelText.text = "Level " + LevelNumber.ToString();
        Speed = Speed *= LevelNumber / 2;
        _levelCurrentTime = _levelTimeLimit;
        StartCoroutine(TimeCountDown());
    }

    private void Update()
    {
        _rotVAmount = _joystick.Vertical;
        _rotHAmount = _joystick.Horizontal;
        _parentCam.transform.Rotate(0, _rotHAmount * RotationSpeed * Time.deltaTime, 0);
        _cam.transform.Rotate(-_rotVAmount * RotationSpeed * Time.deltaTime, 0, 0);

        Step = Speed * Time.deltaTime;
    }

    public void Shoot()
    {
        if (_deactiveBullets.Count < 8)
        {
            if (_index > 7)
            {
                _index = 0;
            }

            if (_index <= 7)
            {
                _armsAnim.SetTrigger("Fire");
                _bullets[_index].gameObject.SetActive(false);
                _deactiveBullets.Add(_bullets[_index]);
                _gunShotSound.Play();

                Ray ray = Camera.main.ScreenPointToRay(_crosshair.transform.position);

                if (Physics.Raycast(ray, out _hit, 200, _targetLayer))
                {
                    if (_hit.transform.CompareTag("Zombie"))
                    {
                        OnEnemyDead?.Invoke(this, EventArgs.Empty);
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXZombie.transform.position = _hit.transform.position;
                        _particleFXZombie.SetActive(true);
                        StartCoroutine(ZombieDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 6);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }

                    if (_hit.transform.CompareTag("Bat"))
                    {
                        OnEnemyDead?.Invoke(this, EventArgs.Empty);
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXBat.transform.position = _hit.transform.position;
                        _particleFXBat.SetActive(true);
                        StartCoroutine(BatDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 6);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }

                    if (_hit.transform.CompareTag("Ghost"))
                    {
                        OnEnemyDead?.Invoke(this, EventArgs.Empty);
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXGhost.transform.position = _hit.transform.position;
                        _particleFXGhost.SetActive(true);
                        StartCoroutine(GhostDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 6);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }
                }

                _index++;
            }
        }

        if (_deactiveBullets.Count == 8)
        {
            _reloadButton.SetActive(true);
            Debug.Log("There is no ammo");
        }
    }

    public void Reload()
    {
        _armsAnim.SetTrigger("Reload");
        _source.clip = _reloadFX;
        _source.Play();
        StartCoroutine(WaitForReload());
        _reloadButton.SetActive(false);
        _deactiveBullets.Clear();
    }

    IEnumerator ZombieDeathTime()
    {
        yield return _zombieDeathTime;
        _hit.transform.gameObject.SetActive(false);
    }

    IEnumerator GhostDeathTime()
    {
        yield return _ghostDeathTime;
        _hit.transform.gameObject.SetActive(false);
    }

    IEnumerator BatDeathTime()
    {
        yield return _batDeathTime;
        _hit.transform.gameObject.SetActive(false);
    }

    IEnumerator WaitForReload()
    {
        yield return ReloadPause;

        for (int i = 0;  i < _bullets.Count; i++)
        {
            _bullets[i].gameObject.SetActive(true);
        }
    }

    IEnumerator TimeCountDown()
    {
        if (_levelCurrentTime > 0)
        {
            _levelCurrentTime -= 1;
            _timer.text = _levelCurrentTime.ToString();
            yield return TimeDelay;
        }

        if (_levelCurrentTime < 2)
        {
            _levelUp.SetActive(true);
        }

        if (_levelCurrentTime <= 0)
        {
            LevelNumber++;
            SceneManager.LoadScene(1);
        }
    }
}
