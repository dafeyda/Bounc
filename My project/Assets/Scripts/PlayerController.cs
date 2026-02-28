using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Tilt")]
    [Range(10f, 360f)]
    [SerializeField] private float rotateSpeed = 150f; // degrees per second

    private CharacterController _controller;
    private Camera _camera;

    private InputAction _moveAction;
    private InputAction _rotateAction;
    private InputAction _ascendAction;
    private InputAction _descendAction;
    private InputAction _respawnAction;
    private InputAction[] _actions;

    private float _tiltX;
    private float _tiltY;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;

        _moveAction = new InputAction("Move", InputActionType.Value);
        _moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        _moveAction.AddBinding("<Gamepad>/leftStick");

        _rotateAction = new InputAction("Rotate", InputActionType.Value);
        _rotateAction.AddCompositeBinding("Dpad")
            .With("Up",    "<Keyboard>/upArrow")
            .With("Down",  "<Keyboard>/downArrow")
            .With("Left",  "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        _rotateAction.AddBinding("<Gamepad>/rightStick");

        _ascendAction = new InputAction("Ascend", InputActionType.Button);
        _ascendAction.AddBinding("<Keyboard>/space");
        _ascendAction.AddBinding("<Gamepad>/buttonSouth");

        _descendAction = new InputAction("Descend", InputActionType.Button);
        _descendAction.AddBinding("<Keyboard>/leftShift");
        _descendAction.AddBinding("<Gamepad>/buttonEast");

        _respawnAction = new InputAction("Respawn", InputActionType.Button);
        _respawnAction.AddBinding("<Keyboard>/r");

        _actions = new[] { _moveAction, _rotateAction, _ascendAction, _descendAction, _respawnAction };
    }

    private void OnEnable()
    {
        foreach (var a in _actions) a.Enable();
    }

    private void OnDisable()
    {
        foreach (var a in _actions) a.Disable();
    }

    private void Start()
    {
        _camera = Camera.main;
        SetupBouncyCollider();
        SetupShadowCaster();
        gameObject.AddComponent<DirectionArrow>();
        gameObject.AddComponent<BallTouchCooldown>();
    }

    private void SetupShadowCaster()
    {
        GameObject shadowCaster = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shadowCaster.name = "PlayerShadowCaster";
        shadowCaster.transform.SetParent(transform, false);
        shadowCaster.transform.localPosition = Vector3.zero;
        shadowCaster.transform.localScale = Vector3.one;
        shadowCaster.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        Destroy(shadowCaster.GetComponent<BoxCollider>());
    }

    private void SetupBouncyCollider()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null)
            box = gameObject.AddComponent<BoxCollider>();
        box.enabled = true;

        box.material = PhysicsHelper.CreateBouncyMaterial("PlayerBounce");
    }

    private void Update()
    {
        if (_respawnAction.WasPressedThisFrame())
        {
            Respawn();
            return;
        }

        Move();
        Rotate();
    }

    public void Respawn()
    {
        // CharacterController must be disabled before teleporting
        _controller.enabled = false;
        transform.position = _spawnPosition;
        transform.rotation = _spawnRotation;
        _controller.enabled = true;

        // Reset tilt accumulators
        _tiltX = 0f;
        _tiltY = 0f;
    }

    private void Move()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        Camera activeCam = CameraManager.Instance != null
            ? CameraManager.Instance.ActiveCamera
            : _camera;

        Vector3 moveDir = Vector3.zero;

        if (input.sqrMagnitude > 0.01f)
        {
            bool isCamera1 = CameraManager.Instance == null
                || CameraManager.Instance.ActiveCameraIndex == 1;

            if (isCamera1)
            {
                // Camera 1: W/S = north/south (+/-Z), A/D = west/east (-/+X)
                moveDir = new Vector3(input.x, 0f, input.y).normalized;
            }
            else
            {
                Vector3 camForward = activeCam.transform.forward;
                Vector3 camRight   = activeCam.transform.right;
                camForward.y = 0f;
                camRight.y   = 0f;
                camForward.Normalize();
                camRight.Normalize();

                moveDir = (camForward * input.y + camRight * input.x).normalized;
            }
        }

        // Vertical movement: Space = up, Shift = down
        float vertical = 0f;
        if (_ascendAction.IsPressed()) vertical += 1f;
        if (_descendAction.IsPressed()) vertical -= 1f;
        moveDir.y = vertical;

        _controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        Vector2 input = _rotateAction.ReadValue<Vector2>();

        _tiltX += input.x * rotateSpeed * Time.deltaTime;
        _tiltY += input.y * rotateSpeed * Time.deltaTime;

        // World-space rotation only — no camera dependency.
        // Left/Right arrows: yaw (Y rotation).  Up/Down arrows: pitch (X rotation).
        transform.rotation = Quaternion.Euler(-_tiltY, _tiltX, 0f);
    }
}
