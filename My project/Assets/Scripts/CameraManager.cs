using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera ActiveCamera      { get; private set; }
    public int    ActiveCameraIndex { get; private set; } = 1;

    private Camera           _mainCamera;
    private Camera[]         _wallCameras;
    private Camera           _normalCamera;
    private Camera           _midpointCamera;
    private ThirdPersonCamera _thirdPersonCam;
    private Transform        _player;
    private GameHUD          _hud;

    // One action per camera slot (index 0 = camera 1, etc.)
    private InputAction[] _cameraActions;
    private Action<InputAction.CallbackContext>[] _cameraCallbacks;

    private void Awake()
    {
        Instance = this;

        _cameraActions   = new InputAction[4];
        _cameraCallbacks = new Action<InputAction.CallbackContext>[4];

        for (int i = 0; i < _cameraActions.Length; i++)
        {
            int cameraIndex = i + 1; // capture for closure
            _cameraActions[i] = new InputAction($"SwitchToCamera{cameraIndex}", InputActionType.Button);
            _cameraActions[i].AddBinding($"<Keyboard>/{cameraIndex}");
            _cameraCallbacks[i] = _ => SwitchToCamera(cameraIndex);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < _cameraActions.Length; i++)
        {
            _cameraActions[i].performed += _cameraCallbacks[i];
            _cameraActions[i].Enable();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _cameraActions.Length; i++)
        {
            _cameraActions[i].performed -= _cameraCallbacks[i];
            _cameraActions[i].Disable();
        }
    }

    private void Start()
    {
        _mainCamera     = Camera.main;
        ActiveCamera    = _mainCamera;
        _thirdPersonCam = _mainCamera.GetComponent<ThirdPersonCamera>();
        _hud            = _mainCamera.GetComponent<GameHUD>();

        // Find all wall cameras
        GameObject arena = GameObject.Find("Arena");
        var camList = new System.Collections.Generic.List<Camera>();
        foreach (Transform child in arena.transform)
        {
            if (child.name.StartsWith("WallCam_"))
            {
                Camera cam = child.GetComponent<Camera>();
                if (cam != null) camList.Add(cam);
            }
        }
        _wallCameras = camList.ToArray();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _player = player.transform;

        GameObject normalCamObj = GameObject.Find("NormalCam");
        if (normalCamObj != null) _normalCamera = normalCamObj.GetComponent<Camera>();

        GameObject midpointCamObj = GameObject.Find("MidpointCam");
        if (midpointCamObj != null) _midpointCamera = midpointCamObj.GetComponent<Camera>();
    }

    private void Update()
    {
        // Wall cameras without their own follow behaviour track the player
        if (_player != null)
        {
            Vector3 lookTarget = _player.position + Vector3.up * 2f;
            foreach (Camera cam in _wallCameras)
            {
                if (cam.GetComponent<WallFollowCamera>() == null)
                    cam.transform.LookAt(lookTarget);
            }
        }
    }

    private void SwitchToCamera(int index)
    {
        Camera target = GetCameraForIndex(index);
        if (target == null) return;

        DisableAll();
        target.enabled = true;

        // Camera 1 also drives ThirdPersonCamera behaviour
        if (_thirdPersonCam != null)
            _thirdPersonCam.enabled = (index == 1);

        ActiveCamera      = target;
        ActiveCameraIndex = index;
        if (_hud != null) _hud.currentCameraIndex = index;
    }

    private Camera GetCameraForIndex(int index)
    {
        switch (index)
        {
            case 1:  return _mainCamera;
            case 2:  return GetNearestWallCamera();
            case 3:  return _normalCamera;
            case 4:  return _midpointCamera;
            default: return null;
        }
    }

    private Camera GetNearestWallCamera()
    {
        if (_wallCameras == null || _wallCameras.Length == 0 || _player == null) return null;

        Camera nearest     = null;
        float  nearestDist = float.MaxValue;
        foreach (Camera cam in _wallCameras)
        {
            float dist = Vector3.Distance(cam.transform.position, _player.position);
            if (dist < nearestDist) { nearestDist = dist; nearest = cam; }
        }
        return nearest;
    }

    private void DisableAll()
    {
        _mainCamera.enabled = false;
        if (_thirdPersonCam != null) _thirdPersonCam.enabled = false;

        foreach (Camera cam in _wallCameras)
            cam.enabled = false;

        if (_normalCamera   != null) _normalCamera.enabled   = false;
        if (_midpointCamera != null) _midpointCamera.enabled = false;
    }
}
