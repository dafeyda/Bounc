using UnityEngine;

public class GameHUD : MonoBehaviour
{
    public static GameHUD Instance { get; private set; }

    public int currentCameraIndex = 1;
    public int playerScore        = 0;
    public int opponentScore      = 0;

    [Header("Score Bar")]
    [SerializeField] private float scoreWidth  = 160f;
    [SerializeField] private float scoreHeight =  32f;
    [SerializeField] private float scorePadX   =  14f;
    [SerializeField] private float scorePadY   =   6f;
    [SerializeField] private float scoreY      =   8f;

    [Header("Camera Label")]
    [SerializeField] private float camLabelWidth  =  80f;
    [SerializeField] private float camLabelHeight =  24f;
    [SerializeField] private float camLabelRight  =  10f;
    [SerializeField] private float camLabelTop    =  10f;

    [Header("Ball Speed")]
    [SerializeField] private float mphScale      = 10f;  // unity units/sec → mph
    [SerializeField] private float speedWidth    = 140f;
    [SerializeField] private float speedHeight   =  28f;
    [SerializeField] private float speedPadB     =  16f;  // gap from bottom edge

    private GUIStyle  _scoreStyle;
    private GUIStyle  _cameraStyle;
    private GUIStyle  _speedStyle;
    private Texture2D _bgTex;
    private bool      _stylesInitialized;

    private Rigidbody _ballRb;

    private void Awake()
    {
        Instance = this;
    }

    private void OnGUI()
    {
        if (!_stylesInitialized) InitStyles();

        // ── Score bar ─────────────────────────────────────────────────────────
        float scoreX = (Screen.width - scoreWidth) * 0.5f;

        // Semi-transparent pill background
        GUI.color = new Color(0f, 0f, 0f, 0.5f);
        GUI.DrawTexture(new Rect(scoreX - scorePadX, scoreY - scorePadY,
                                 scoreWidth + scorePadX * 2f, scoreHeight + scorePadY * 2f), _bgTex);
        GUI.color = Color.white;

        GUI.Label(new Rect(scoreX, scoreY, scoreWidth, scoreHeight),
                  $"{playerScore}  —  {opponentScore}", _scoreStyle);

        // ── Camera indicator (top-right, subtle) ──────────────────────────────
        GUI.Label(new Rect(Screen.width - camLabelWidth - camLabelRight, camLabelTop, camLabelWidth, camLabelHeight),
                  $"Cam {currentCameraIndex}", _cameraStyle);

        // ── Ball speed (bottom-centre) ─────────────────────────────────────────
        if (_ballRb == null)
        {
            Transform ball = SceneHelper.FindBall();
            if (ball != null) _ballRb = ball.GetComponent<Rigidbody>();
        }
        if (_ballRb != null)
        {
            float mph  = _ballRb.linearVelocity.magnitude * mphScale;
            float sx   = (Screen.width - speedWidth) * 0.5f;
            float sy   = Screen.height - speedHeight - speedPadB;
            GUI.Label(new Rect(sx, sy, speedWidth, speedHeight), $"{mph:F0} mph", _speedStyle);
        }
    }

    private void InitStyles()
    {
        _bgTex = new Texture2D(1, 1);
        _bgTex.SetPixel(0, 0, Color.white);
        _bgTex.Apply();

        _scoreStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        _scoreStyle.normal.textColor = Color.white;

        _cameraStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 13,
            alignment = TextAnchor.MiddleRight
        };
        _cameraStyle.normal.textColor = new Color(1f, 1f, 1f, 0.55f);

        _speedStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 17,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        _speedStyle.normal.textColor = new Color(1f, 1f, 1f, 0.80f);

        _stylesInitialized = true;
    }
}
