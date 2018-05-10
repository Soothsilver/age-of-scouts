using Age.Core;
using Auxiliary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.World
{
    class MoveViewport
    {
        private static int MOUSE_MOVE_MARGIN = 10;

        internal static void UpdateMove(Session session, float elapsedSeconds)
        {
            if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) ||
                Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                KeyboardMove(-1, 0, elapsedSeconds, session);
            }
            if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) ||
            Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                KeyboardMove(1, 0, elapsedSeconds, session);
            }
            if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up) ||
            Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                KeyboardMove(0, -1, elapsedSeconds, session);
            }
            if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down) ||
            Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                KeyboardMove(0, 1, elapsedSeconds, session);
            }
            if (Root.Mouse_NewState.X >= 0 && Root.Mouse_NewState.X <= MOUSE_MOVE_MARGIN)
            {
                MouseMove(-1, 0, elapsedSeconds, session);
            }
            if (Root.Mouse_NewState.X >= Root.ScreenWidth - MOUSE_MOVE_MARGIN && Root.Mouse_NewState.X <= Root.ScreenWidth)
            {
                MouseMove(1, 0, elapsedSeconds, session);
            }
            if (Root.Mouse_NewState.Y >= 0 && Root.Mouse_NewState.Y <= MOUSE_MOVE_MARGIN)
            {
                MouseMove(0,-1, elapsedSeconds, session);
            }
            if (Root.Mouse_NewState.Y >= Root.ScreenHeight - MOUSE_MOVE_MARGIN && Root.Mouse_NewState.Y <= Root.ScreenHeight)
            {
                MouseMove(0, 1, elapsedSeconds, session);
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                session.ZoomLevel /= 2;
            }
            if (Root.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                session.ZoomLevel *= 2;
            }
            if (Root.Mouse_NewState.ScrollWheelValue > Root.Mouse_OldState.ScrollWheelValue)
            {
                session.ZoomLevel *= 2;
            }
            if (Root.Mouse_NewState.ScrollWheelValue < Root.Mouse_OldState.ScrollWheelValue)
            {
                session.ZoomLevel /= 2;
            }
            if (SmartCenterRemainingSeconds > 0)
            {
                if (SmartCenterRemainingSeconds < elapsedSeconds)
                {
                    elapsedSeconds = SmartCenterRemainingSeconds;
                }
                SmartCenterRemainingSeconds -= elapsedSeconds;
                session.CenterOfScreenInStandardPixels += SmartCenterSpeed * elapsedSeconds;
            }
        }

        private static Vector2 SmartCenterSpeed;
        private static float SmartCenterRemainingSeconds = 0;
        internal static void SmartCenterTo(Session session, Vector2 target)
        {
            SmartCenterRemainingSeconds = 0.5f;
            SmartCenterSpeed = (target - session.CenterOfScreenInStandardPixels) / SmartCenterRemainingSeconds;
        }

        private static void MouseMove(int x, int y, float elapsedSeconds, Session session)
        {
            session.CenterOfScreenInStandardPixels += new Microsoft.Xna.Framework.Vector2(x, y) * elapsedSeconds * Settings.Instance.MouseMoveSpeed;
        }

        private static void KeyboardMove(int x, int y, float elapsedSeconds, Session session)
        {
            session.CenterOfScreenInStandardPixels += new Microsoft.Xna.Framework.Vector2(x, y) * elapsedSeconds * Settings.Instance.KeyboardMoveSpeed;
        }
    }
}
