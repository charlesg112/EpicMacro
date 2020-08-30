using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace EpicMacro.Res
{
    public static class UserControl
    {

        public struct ScreenPos
        {
            public int X, Y;
            public ScreenPos(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        /// <summary>
        /// type : 0 -> mouse, 1 -> keyboard, 2 -> hardware
        /// </summary>
        private struct INPUT
        {
            public uint type;
            public UNION union;
        }

        #region INPUT substructs

        internal struct UNION
        {
            public MOUSEINPUT mi;
            public KEYBDINPUT ki;
            public HARDWAREINPUT hi;
        }

        public struct MOUSEINPUT
        {
            public long dx;
            public long dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
            public MOUSEINPUT(long _dx, long _dy, uint _mouseData, uint _dwFlags, uint _time, UIntPtr _dwExtraInfo) => (dx, dy, mouseData, dwFlags, time, dwExtraInfo) = (_dx, _dy, _mouseData, _dwFlags, _time, _dwExtraInfo);
        }

        internal struct KEYBDINPUT
        {
            public VIRTUALKEY wVk;
            public VIRTUALKEY wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        internal struct HARDWAREINPUT
        {
            public uint uMsg;
            public int wParamL;
            public int wParamH;
        }

        #endregion

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out ScreenPos sp);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize);

        public static void Mouse_LeftClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((int)MOUSE.LeftDown, x, y, 0, 0);
            mouse_event((int)MOUSE.LeftUp, x, y, 0, 0);
        }

        public static void Mouse_LongLeftClick(int x, int y, int ms)
        {
            SetCursorPos(x, y);
            mouse_event((int)MOUSE.LeftDown, x, y, 0, 0);
            Thread.Sleep(ms);
            mouse_event((int)MOUSE.LeftUp, x, y, 0, 0);
        }

        public static void Mouse_MoveCursor(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static ScreenPos Mouse_GetCursorPos()
        {
            ScreenPos output = new ScreenPos(0, 0);
            GetCursorPos(out output);
            return output;
        }

        public static void Keyboard_PressKey(VIRTUALKEY vk)
        {
            KEYBDINPUT kbinput = new KEYBDINPUT() {wVk = vk, wScan = vk, dwFlags = 0, time = 0, dwExtraInfo = new UIntPtr()};
            UNION un = new UNION() { ki = kbinput};
            INPUT input = new INPUT() { type = 1, union = un };
            INPUT[] inputs = { input };
            SendInput(1, inputs, Marshal.SizeOf(input));
        }

        /*
            public VIRTUALKEY wVk;
            public VIRTUALKEY wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        */
    }
}
