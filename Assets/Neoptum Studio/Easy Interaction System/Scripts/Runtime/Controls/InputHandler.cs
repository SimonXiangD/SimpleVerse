using EIS.Runtime.Misc;
using UnityEngine;

namespace EIS.Runtime.Controls
{
    public class InputHandler : Singleton<InputHandler>
    {
        public Vector2 GetRotationInput()
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        public float GetZoomInput()
        {
            return Input.mouseScrollDelta.y;
        }

        public bool GetChangeButtonPressDown()
        {
            return Input.GetButtonDown("Change");
        }
    }
}