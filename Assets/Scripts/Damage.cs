using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Damage : MonoBehaviour
{
    [SerializeField] private JoystickController _controller;
    [SerializeField] private AudioSource _damageSound;
    [SerializeField] private AudioClip _batBite, _ghostHowl, _zombieBite;
    [SerializeField] private Image _batImage, _ghostImage, _zombieImage;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private GameObject _pumpkinTip;

    private float _zombieDamage = 0.080f;
    private float _ghostDamage = 0.060f;
    private float _batDamage = 0.050f;
    private bool _isFirstDamage = false;
    private bool _isFirstDamageHandled = false;

    private WaitForSeconds _graphicPauseTime = new WaitForSeconds(0.3f);
    private WaitForSeconds _tipTime = new WaitForSeconds(15);

    private void Awake()
    {
        _gameOver.SetActive(false);
    }

    void CheckForGameOver()
    {
        if (_controller._healthBar.fillAmount <= 0 && _controller._levelCurrentTime > 2)
        {
            _controller.IsGameOver = true;
            _gameOver.SetActive(true);
            Time.timeScale = 0;
            JoystickController.LevelNumber = 1;
            _controller.Speed = 2;
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isFirstDamage && !_isFirstDamageHandled)
        {
            StartCoroutine(PumpkinTip());
            _isFirstDamageHandled = true;
        }

        if (other.CompareTag("Bat"))
        {
            _damageSound.clip = _batBite;
            _damageSound.Play();
            _controller._healthBar.fillAmount -= _batDamage;
            _batImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 5);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
            _isFirstDamage = true;
            CheckForGameOver();
        }

        if (other.CompareTag("Ghost"))
        {
            _damageSound.clip = _ghostHowl;
            _damageSound.Play();
            _controller._healthBar.fillAmount -= _ghostDamage;
            _ghostImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 5);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
            _isFirstDamage = true;
            CheckForGameOver();
        }

        if (other.CompareTag("Zombie"))
        {
            _damageSound.clip = _zombieBite;
            _damageSound.Play();
            _controller._healthBar.fillAmount -= _zombieDamage;
            _zombieImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 5);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);
            _isFirstDamage = true;
            CheckForGameOver();
        }

        other.gameObject.SetActive(false);
    }

    IEnumerator HideImage()
    {
        yield return _graphicPauseTime;
        _zombieImage.gameObject.SetActive(false);
        _ghostImage.gameObject.SetActive(false);
        _batImage.gameObject.SetActive(false);
    }

    IEnumerator PumpkinTip()
    {
        _pumpkinTip.gameObject.SetActive(true);
        yield return _tipTime;
        _pumpkinTip.gameObject.SetActive(false);
    }
}
