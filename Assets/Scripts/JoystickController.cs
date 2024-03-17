using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _parentCam;

    private float _rotHAmount;
    private float _rotVAmount;

    public float RotationSpeed = 35;

    // Enemy Move 
    private float Speed = 1;
    public static float Step;

    // Shooting
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private Animator _armsAnim;
    [SerializeField] private AudioSource _gunShotSound;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
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
        _armsAnim.SetTrigger("Fire");
        _gunShotSound.Play();
    }
}
