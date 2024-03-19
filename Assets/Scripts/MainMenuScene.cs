using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _buttonSFX;

    private void Start()
    {
        _source.clip = _buttonSFX;
    }
    public void Play()
    {
        _source.Play();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        _source.Play();
        Application.Quit();
    }
}
