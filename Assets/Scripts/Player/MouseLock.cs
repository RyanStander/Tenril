using UnityEngine;

namespace Player
{
    public class MouseLock : MonoBehaviour
    {
        [SerializeField] private bool mouseLockEnabled;
        // locks the mouse if its enabled
        private void Start()
        {
            MouseLockCheck();
        }

        private void MouseLockCheck()
        {
            if (mouseLockEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                return;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
