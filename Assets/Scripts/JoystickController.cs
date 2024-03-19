using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _parentCam;

    private float _rotHAmount;
    private float _rotVAmount;
    private float RotationSpeed = 50;

    public bool IsGameOver = false;

    // Enemy Move 
    public float Speed = 1.8f;
    private float _speedMultiplier = 0.2f;
    public static float Step;

    // Shooting
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private Animator _armsAnim;
    [SerializeField] private AudioSource _gunShotSound;
    [SerializeField] private LayerMask _targetLayer;
    private WaitForSeconds _zombieDeathTime = new WaitForSeconds(0.6f); // otherwise it constantly create new waitforsecond object which makes to work GC in some place and decrease the performance
    private WaitForSeconds _batDeathTime = new WaitForSeconds(0.5f);
    private WaitForSeconds _ghostDeathTime = new WaitForSeconds(0.3f); 
    private RaycastHit _hit;

    // Particles
    [SerializeField] private GameObject _particleFXZombie;
    [SerializeField] private GameObject _particleFXBat;
    [SerializeField] private GameObject _particleFXGhost;
    [SerializeField] private GameObject _particleFXPumpkin;

    // Player Damage
    public bool ZombieAttack = false;
    public bool GhostAttack = false;
    public bool BatAttack = false;
    public Image _healthBar;

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
    private WaitForSeconds _timeDelay = new WaitForSeconds(1);
    private int _levelTimeLimit = 45;
    public int _levelCurrentTime;

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
        if (IsGameOver == true)
        {
            LevelNumber = 1;
            IsGameOver = false;
        }
        _levelText.text = "Level " + LevelNumber.ToString();
        Speed += LevelNumber * _speedMultiplier;
        _levelCurrentTime = _levelTimeLimit;
        TimeCountDown();
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
        if (_deactiveBullets.Count < 8 && !IsGameOver)
        {
            if (_index > 7)
            {
                _index = 0;
            }

            if (_index <= 7)
            {
                _armsAnim.SetTrigger("Fire");
                _gunShotSound.Play();
                _bullets[_index].gameObject.SetActive(false);
                _deactiveBullets.Add(_bullets[_index]);

                Ray ray = Camera.main.ScreenPointToRay(_crosshair.transform.position);

                if (Physics.Raycast(ray, out _hit, 200, _targetLayer))
                {
                    if (_hit.transform.CompareTag("Zombie"))
                    {
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXZombie.transform.position = _hit.transform.position;
                        _particleFXZombie.SetActive(true);
                        StartCoroutine(ZombieDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 5);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }

                    if (_hit.transform.CompareTag("Bat"))
                    {
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXBat.transform.position = _hit.transform.position;
                        _particleFXBat.SetActive(true);
                        StartCoroutine(BatDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 5);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }

                    if (_hit.transform.CompareTag("Ghost"))
                    {
                        _hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Die");
                        _particleFXGhost.transform.position = _hit.transform.position;
                        _particleFXGhost.SetActive(true);
                        StartCoroutine(GhostDeathTime());

                        int enemyTpye = Random.Range(0, 3);
                        int spawnIndex = Random.Range(0, 5);
                        EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
                    }

                    if (_hit.transform.CompareTag("Pumpkin"))
                    {
                        _particleFXPumpkin.transform.position = _hit.transform.position;
                        _particleFXPumpkin.SetActive(true);
                        _healthBar.fillAmount += 0.3f;
                        _hit.transform.gameObject.SetActive(false);
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

    void TimeCountDown()
    {
        if (_levelCurrentTime > 0)
        {
            _levelCurrentTime -= 1;
            _timer.text = _levelCurrentTime.ToString();
            StartCoroutine(TimeDelay());
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

    IEnumerator TimeDelay()
    {
        yield return _timeDelay;
        TimeCountDown();
    }
}
