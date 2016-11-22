using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using DBGExtensionLibrary;
using OpenTK;

namespace TKSprites
{
    public class InputManager
    {
        private bool[] KeysDown = new bool[140];
        private bool mouseLeft = false, mouseRight = false;
        private Vector2 mousePos = new Vector2(0, 0);

        public InputManager()
        {
            KeysDown.Populate(false);
        }

        
        public void KeyDown(Key k) { KeyDown((int)k); }
        public void KeyDown(int key)
        {
            if(key >= 0 || key < KeysDown.Length)
            {
                KeysDown[key] = true;
            }
        }

        public void KeyUp(Key k) { KeyUp((int)k); }
        public void KeyUp(int key)
        {
            if (key >= 0 || key < KeysDown.Length)
            {
                KeysDown[key] = false;
            }
        }

        public bool IsDown(Key ka, Key kb) { return IsDown((int)ka, (int)kb, false); }
        public bool IsDown(Key ka, Key kb, bool both) { return (IsDown((int)ka, (int)kb, both)); }
        public bool IsDown(int ka, int kb) { return IsDown(ka, kb, false); }

        public bool IsDown(int ka, int kb, bool both)
        {
            if (both) return (IsDown(ka) && IsDown(kb));
            else return (IsDown(ka) || IsDown(kb));
        }

        public bool IsDown(Key k) { return IsDown((int)k); }
        public bool IsDown(int key)
        {
            return KeysDown[key];
        }

        public bool MouseLeft { get { return mouseLeft; } set { mouseLeft = value; } }
        public bool MouseRight { get { return mouseLeft; } set { mouseRight = value; } }

        public Vector2 MousePosition { get { return mousePos; } }

        public void UpdateMousePos(float x, float y)
        {
            mousePos = new Vector2(x, y);
        }
    }
}
