using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Dj_Gamer; 

public static class KeyHandler {
    private static Thread _t;
    private static Thread _mouseTLeft;
    private static Thread _mouseTRight;
    private static readonly int[] _jogs = { 4268720, 4137648, 4268721, 4137649, };
    private static readonly int[] _blacklist = { 8337040, 13968, 8337041, 13961, };
    private static bool _mouseType = false;

    public static int MouseSensitivity = 5;
    public static int MouseDelay = 180;

    public static void SendInput(KeyStruct keys) {
        KeyHandler.MouseClicks(keys);
        
        if (keys.Key == VirtualKeys.VK_CHANGE_CONTROL_L) {
            KeyHandler._mouseType = false;
            Console.WriteLine("SetCursorPos ON");
        }

        if (keys.Key == VirtualKeys.VK_CHANGE_CONTROL_R) {
            KeyHandler._mouseType = true;
            Console.WriteLine("PostMessage ON");
        }
        
        if (KeyHandler._blacklist.Contains(keys.MidiKey)) {
            Console.WriteLine("Key is blacklisted!");
            return;
        }
        
        if (KeyHandler._jogs.Contains(keys.MidiKey)) {
            KeyHandler.MouseInput(keys);
            return;
        }
        
        if (keys.MidiKey > 8_000_000) {
            KeyHandler.KeyboardInput(keys, 0);
            return;
        }
        
        KeyHandler.KeyboardInput(keys, 1);
    }

    private static void MouseClicks(KeyStruct keys) {
        switch (keys.Key) {
            case VirtualKeys.VK_LBUTTON:
                Console.WriteLine("L DOWN");
                KeyHandler._mouseTLeft = new Thread(() => Winapi.ClickLeftDown(KeyHandler.MouseDelay));
                KeyHandler._mouseTLeft.Start();
                break;
            case VirtualKeys.VK_LBUTTONUP:
                Console.WriteLine("L UP");
                Winapi.ClickLeftUp();
                break;
            case VirtualKeys.VK_RBUTTON:
                Console.WriteLine("R DOWN");
                KeyHandler._mouseTRight = new Thread(() => Winapi.ClickRightDown(KeyHandler.MouseDelay));
                KeyHandler._mouseTRight.Start();
                break;
            case VirtualKeys.VK_RBUTTONUP:
                Console.WriteLine("R UP");
                Winapi.ClickRightUp();
                break;
        }
    }

    private static void KeyboardInput(KeyStruct keys, int type) {
        Winapi.InitKeys((ushort)keys.Key);
        if (keys.Delay == 0) {
            keys.Delay = 1;
        }
        
        switch (type) {
            case 0:
                Console.WriteLine("Pushing down");
                KeyHandler._t = new Thread(() => Winapi.PushDown(keys.Delay));
                KeyHandler._t.Start();
                break;
            case 1:
                Console.WriteLine("Pushing up");
                Winapi.PushUp();
                break;
            case 2:
                Winapi.ShowMessage();
                break;
        }
        Winapi.PushUp();
    }

    private static void MouseInput(KeyStruct keys) {
        Winapi.GetCursorPos(out Winapi.POINT p);
        Winapi.GetCursorPos(out Winapi.POINT pStatic);

        //Console.WriteLine(p);
        
        switch (keys.Key) {
            case VirtualKeys.VK_JOGS_LR:
                p.X -= KeyHandler.MouseSensitivity;
                if (KeyHandler._mouseType) {
                    Winapi.mouse_event(Winapi.Constants.MOUSEEVENTF_MOVE, p.X - pStatic.X, p.Y - pStatic.Y, 0, 0);
                    break;
                }
                
                Winapi.SetCursorPos(p.X,p.Y);
                break;
            case VirtualKeys.VK_JOGS_LL:
                p.X += KeyHandler.MouseSensitivity;
                if (KeyHandler._mouseType) {
                    Winapi.mouse_event(Winapi.Constants.MOUSEEVENTF_MOVE, p.X - pStatic.X, p.Y - pStatic.Y, 0, 0);
                    break;
                }
                
                Winapi.SetCursorPos(p.X,p.Y);
                break;
            case VirtualKeys.VK_JOGS_RR:
                p.Y += KeyHandler.MouseSensitivity;
                if (KeyHandler._mouseType) {
                    Winapi.mouse_event(Winapi.Constants.MOUSEEVENTF_MOVE, p.X - pStatic.X, p.Y - pStatic.Y, 0, 0);
                    break;
                }
                
                Winapi.SetCursorPos(p.X,p.Y);
                break;
            case VirtualKeys.VK_JOGS_RL:
                p.Y -= KeyHandler.MouseSensitivity;
                if (KeyHandler._mouseType) {
                    Winapi.mouse_event(Winapi.Constants.MOUSEEVENTF_MOVE, p.X - pStatic.X, p.Y - pStatic.Y, 0, 0);
                    break;
                }
                
                Winapi.SetCursorPos(p.X,p.Y);
                break;
        }
    }
    
    private static class Winapi {
        public readonly ref struct Constants {
            public const uint WM_MOUSEMOVE = 0x0200;
            public const uint WM_LBUTTONDOWN = 0x0201;
            public const uint WM_LBUTTONUP = 0x0202;
            public const int MOUSEEVENTF_MOVE = 0x0001;
            public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
            public const int INPUT_MOUSE = 0;
            public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            public const int MOUSEEVENTF_LEFTUP = 0x0004;
        }
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushDown(int delay = 1);
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushUp();
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitKeys(ushort key);

        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ShowMessage")]
        public static extern void ShowMessage();

        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClickLeftDown(int delay = 1);
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClickLeftUp();
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClickRightDown(int delay = 1);
        
        [DllImport("UnmanagedUtils.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClickRightUp();

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public override string ToString() => $"{X}x{Y}";
        }
        
        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // Define the input event structures
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public INPUTUNION inputUnion;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mouseInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}