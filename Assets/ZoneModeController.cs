using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public enum ZoneModePoses { MID, UP, DOWN, LEFT, RIGHT, UP_RIGHT, UP_LEFT, DOWN_RIGHT, DOWN_LEFT }

public class ZoneModeController : MonoBehaviour
{
    [SerializeField] private GameObject CinemachineCameraTarget = null;
    [SerializeField] private CinemachineVirtualCamera vCam1 = null;
    [SerializeField] private CinemachineVirtualCamera vCam2 = null;
    [SerializeField] private Camera cam1 = null;
    [SerializeField] private Camera cam2 = null;
    [SerializeField] private float transitionSpeed = 0.5f;
    [SerializeField] private Vector3 cameraTargetDir = new Vector3(0, 0, 1);

    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float sprintSpeed = 2.0f;
    [SerializeField] private float speedChangeRate = 1.0f;
    [SerializeField] private float velocityChangeRate = 1.0f;

    [SerializeField] private float ascendAndDescend_maxSpeed = 1.0f;
    [SerializeField] private float ascendAndDescend_accelerate = 1.0f;
    [SerializeField] private float ascendAndDescend_decelerate = 1.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public bool transitioning = false;

    private Animator _animator;
    private CharacterController _controller;
    private ThirdPersonController _control;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private Vector2 _screenSize = Vector2.zero;
    private Transform boundsCheckerXpos, boundsCheckerXneg, boundsCheckerYpos, boundsCheckerYneg;


    void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();
        _control = GetComponent<ThirdPersonController>();
        _input = GetComponent<StarterAssetsInputs>();

        boundsCheckerXpos = transform.Find("zoneModeBoundsCheckers")?.GetChild(0);
        boundsCheckerXneg = transform.Find("zoneModeBoundsCheckers")?.GetChild(1);
        boundsCheckerYpos = transform.Find("zoneModeBoundsCheckers")?.GetChild(2);
        boundsCheckerYneg = transform.Find("zoneModeBoundsCheckers")?.GetChild(3);  
    }

    
    public void InitZoneMode()
    {
        _animator.CrossFade("zone_mid", 0.5f);
        _screenSize = new Vector2(Screen.width, Screen.height);
        StartCoroutine(TransitionCamera());
    }

    public void UpdateZoneMode()
    {
        UpdatePose();
        Move();
    }

    public void UpdateZoneCamera() //called in LateUpdate
    {
        if (transitioning)
            return;

    }


    IEnumerator TransitionCamera()
    {
        Quaternion startRot = CinemachineCameraTarget.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(cameraTargetDir);
        transitioning = true;
        float t = 0;
        while (t < 1)
        {
            float perc = t * t * t * (t * (6f * t - 15f) + 10f);
            CinemachineCameraTarget.transform.rotation = Quaternion.Slerp(startRot, targetRot, perc);
            t += Time.deltaTime * transitionSpeed;
            yield return null;
        }
        CinemachineCameraTarget.transform.rotation = targetRot;
        t = 0;
        Quaternion startRotation = transform.rotation;
        while (t < 1)
        {
            float perc = t * t * t * (t * (6f * t - 15f) + 10f);
            transform.localRotation = Quaternion.Slerp(startRotation, targetRot, perc);
            t += Time.deltaTime * transitionSpeed;
            yield return null;
        }
        transform.localRotation = targetRot;
    }


    public void ChangeCamera()
    {
        if (vCam1.gameObject.activeSelf)
        {
            cam1.GetComponent<Skybox>().material = cam2.GetComponent<Skybox>().material;
            vCam1.gameObject.SetActive(false);
            vCam2.gameObject.SetActive(true);
            CinemachineCameraTarget.transform.parent = null;
        }
    }
    /*
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }*/


    private void Move()
    {
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentSpeed = new Vector3(_controller.velocity.x, _controller.velocity.y, 0).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, _input.move.y, 0).normalized;
        //Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // Check for bounds
        if (inputDirection.x > 0 && cam1.WorldToScreenPoint(boundsCheckerXpos.position).x >= _screenSize.x) inputDirection.x *= 0;
        else if (inputDirection.x < 0 && cam1.WorldToScreenPoint(boundsCheckerXneg.position).x <= 0) inputDirection.x *= 0;
        if (inputDirection.y > 0 && cam1.WorldToScreenPoint(boundsCheckerYpos.position).y >= _screenSize.y) inputDirection.y *= 0;
        else if (inputDirection.y < 0 && cam1.WorldToScreenPoint(boundsCheckerYneg.position).y <= 0) inputDirection.y *= 0;

        inputDirection = inputDirection.normalized;
        Vector3 oldVelo = _controller.velocity * Time.deltaTime;
        Vector3 newVelo = Vector3.MoveTowards(oldVelo, inputDirection * (_speed * Time.deltaTime), velocityChangeRate);// + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

        _controller.Move(newVelo);//targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        //_controller.
    }

    [SerializeField] private float fadeSpeed = 0.1f;

    public ZoneModePoses playerCurrentPose = ZoneModePoses.MID;
    void UpdatePose()
    {
        if (_control.IsCurrentDeviceMouse)
        {
            if (_input.attack2 && playerCurrentPose != ZoneModePoses.MID)
            {
                playerCurrentPose = ZoneModePoses.MID;
                _animator.CrossFade("zone_mid", fadeSpeed, 0, 0);
                _input.attack2 = false;
            }
            if (_input.look.sqrMagnitude >= _control._threshold)
            {
                DetermineZoneDir();
            }
            
        }
        else
        {
            if (_input.look.sqrMagnitude >= _control._threshold)
            {
                DetermineZoneDir();
            }
            else if (playerCurrentPose != ZoneModePoses.MID)
            {
                playerCurrentPose = ZoneModePoses.MID;
                _animator.CrossFade("zone_mid", fadeSpeed, 0, 0);
            }
        }   
    }

    void DetermineZoneDir()
    {
        float x = _input.look.x;
        float y = _input.look.y;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0 && playerCurrentPose != ZoneModePoses.LEFT)
            {
                playerCurrentPose = ZoneModePoses.LEFT;
                _animator.CrossFade("zone_left", fadeSpeed, 0, 0);
            }
            else if (x > 0 && playerCurrentPose != ZoneModePoses.RIGHT)
            {
                playerCurrentPose = ZoneModePoses.RIGHT;
                _animator.CrossFade("zone_right", fadeSpeed, 0, 0);
            }
        }
        else if (Mathf.Abs(y) > Mathf.Abs(x))
        {
            if (y < 0 && playerCurrentPose != ZoneModePoses.UP)
            {
                playerCurrentPose = ZoneModePoses.UP;
                _animator.CrossFade("zone_up", fadeSpeed, 0, 0);
            }
            else if (y > 0 && playerCurrentPose != ZoneModePoses.DOWN)
            {
                playerCurrentPose = ZoneModePoses.DOWN;
                _animator.CrossFade("zone_down", fadeSpeed, 0, 0);
            }
        }
    }
}
