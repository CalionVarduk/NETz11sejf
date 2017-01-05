using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETz11sejf
{
    public delegate void PlayerControlsKeyHandler(Keys key);

    public static class PlayerControls
    {
        public static Player player = null;
        public static bool Up = false;
        public static bool Down = false;
        public static bool Left = false;
        public static bool Right = false;

        public static event PlayerControlsKeyHandler KeyPressed;

        public static void setKeyboardEvents(Control control)
        {
            control.KeyDown += Event_KeyDown;
            control.KeyUp += Event_KeyUp;
        }

        private static void setPlayerMovement()
        {
            if (player != null)
            {
                float x = 0, y = 0;
                if (Up) y -= 1;
                if (Down) y += 1;
                if (Left) x -= 1;
                if (Right) x += 1;
                player.Velocity = (x != 0 || y != 0) ? 1.6f : 0;
                player.setDirection(x, y);
            }
        }

        private static void Event_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    PlayerControls.Up = true; break;

                case Keys.A:
                case Keys.Left:
                    PlayerControls.Left = true; break;

                case Keys.S:
                case Keys.Down:
                    PlayerControls.Down = true; break;

                case Keys.D:
                case Keys.Right:
                    PlayerControls.Right = true; break;
            }
            setPlayerMovement();
            if (KeyPressed != null) KeyPressed(e.KeyCode);
        }

        private static void Event_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    PlayerControls.Up = false; break;

                case Keys.A:
                case Keys.Left:
                    PlayerControls.Left = false; break;

                case Keys.S:
                case Keys.Down:
                    PlayerControls.Down = false; break;

                case Keys.D:
                case Keys.Right:
                    PlayerControls.Right = false; break;
            }
            setPlayerMovement();
        }
    }
}
