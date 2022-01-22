using UnityEngine;

public class MouseLock : MonoBehaviour
{
    [SerializeField] private bool mouseLockEnabled=false;
    // locks the mouse if its enabled
    void Start()
    {
        if (mouseLockEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
