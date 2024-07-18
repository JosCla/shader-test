using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace shader_test
{
    public static class InputUtils
    {
        private static Point absMousePos = Point.Zero;
        private static Vector2 mousePos = Vector2.Zero;
        private static Vector2 boundedMousePos = Vector2.Zero;

        private static InputBit mouseState = new InputBit();

        public static void Update()
        {
            absMousePos = Mouse.GetState().Position;
            mousePos = new Vector2(
                (float)absMousePos.X / (float)Game1.SCREEN_RECT.Width,
                (float)absMousePos.Y / (float)Game1.SCREEN_RECT.Height
            );
            boundedMousePos = new Vector2(
                Math.Clamp(mousePos.X, 0.0f, 1.0f),
                Math.Clamp(mousePos.Y, 0.0f, 1.0f)
            );

            mouseState.Update(Mouse.GetState().LeftButton == ButtonState.Pressed);
        }

        public static Point GetAbsMousePos()
        {
            return absMousePos;
        }

        public static Vector2 GetMousePos()
        {
            return mousePos;
        }

        public static Vector2 GetBoundedMousePos()
        {
            return boundedMousePos;
        }

        public static bool IsMouseHeld()
        {
            return mouseState.IsHeld();
        }

        public static bool IsMouseOnPress()
        {
            return mouseState.IsOnPress();
        }
    }

    public class InputBit
    {
        private bool isHeld;
        private bool isOnPress;

        public InputBit()
        {
            this.isHeld = false;
            this.isOnPress = false;
        }

        public void Update(bool currPressed)
        {
            this.isOnPress = (!isHeld && currPressed);
            this.isHeld = currPressed;
        }

        public bool IsHeld()
        {
            return isHeld;
        }

        public bool IsOnPress()
        {
            return isOnPress;
        }
    }
}