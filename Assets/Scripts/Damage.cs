using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Damage : MonoBehaviour
{
    public event EventHandler OnPlayerDead;

    [SerializeField] private AudioSource _damageSound;
    [SerializeField] private AudioClip _batBite, _ghostHowl, _zombieBite;
    [SerializeField] private Image _batImage, _ghostImage, _zombieImage;
    [SerializeField] private Image _healthBar;

    private float _zombieDamage = 0.015f;
    private float _ghostDamage = 0.010f;
    private float _batDamage = 0.05f;

    private WaitForSeconds _graphicPauseTime = new WaitForSeconds(0.3f);

    private void Start()
    {
        OnPlayerDead += JoystickController_OnPlayerDead;
    }

    private void JoystickController_OnPlayerDead(object sender, System.EventArgs e)
    {
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if (_healthBar.fillAmount <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bat"))
        {
            _damageSound.clip = _batBite;
            _damageSound.Play();
            _healthBar.fillAmount -= _batDamage;
            _batImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 6);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);

            OnPlayerDead?.Invoke(this, EventArgs.Empty);
        }

        if (other.CompareTag("Ghost"))
        {
            _damageSound.clip = _ghostHowl;
            _damageSound.Play();
            _healthBar.fillAmount -= _ghostDamage;
            _ghostImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 6);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);

            OnPlayerDead?.Invoke(this, EventArgs.Empty);
        }

        if (other.CompareTag("Zombie"))
        {
            _damageSound.clip = _zombieBite;
            _damageSound.Play();
            _healthBar.fillAmount -= _zombieDamage;
            _zombieImage.gameObject.SetActive(true);
            StartCoroutine(HideImage());

            int enemyTpye = Random.Range(0, 3);
            int spawnIndex = Random.Range(0, 6);
            EnemyPool.Instance.GetEnemies(enemyTpye, spawnIndex);

            OnPlayerDead?.Invoke(this, EventArgs.Empty);
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
}
