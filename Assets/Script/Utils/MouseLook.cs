using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 100f;
    private float _mouseY;
    private float _mouseX;
    private bool _isLockedCursor;

    private void Awake()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        _mouseX = euler.y;
        _mouseY = euler.x;
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _isLockedCursor == false)
        {
            LockCursor();
        }

        const float MIN_X = -360f;
        const float MAX_X = 360f;
        const float MIN_Y = -90f;
        const float MAX_Y = 90f;

        _mouseX += Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        if (_mouseX < MIN_X) _mouseX += MAX_X;
        else if (_mouseX > MAX_X) _mouseX -= MAX_X;

        _mouseY -= Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        _mouseY = Mathf.Clamp(_mouseY, MIN_Y, MAX_Y);

        transform.rotation = Quaternion.Euler(_mouseY, _mouseX, 0.0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isLockedCursor = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isLockedCursor = false;
    }
}
