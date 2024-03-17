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

    private void Update()
    {
        _rotVAmount = _joystick.Vertical;
        _rotHAmount = _joystick.Horizontal;
        _parentCam.transform.Rotate(0, _rotHAmount * RotationSpeed * Time.deltaTime, 0);
        _cam.transform.Rotate(-_rotVAmount * RotationSpeed * Time.deltaTime, 0, 0);
    }
}
