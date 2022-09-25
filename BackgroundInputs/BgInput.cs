using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.BackgroundInputs
{
    internal class BgInput
    {
        private static string FF_WND_NAME = "FINAL FANTASY XIV";
        private static IntPtr hWnd = FindFFXIVWindow();

        #region FindFFXIVWindow
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private static string GetWindowTextFromHandle(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd);
            if (len > 0)
            {
                var builder = new StringBuilder(len + 1);
                GetWindowText(hWnd, builder, len + 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private static IntPtr FindFFXIVWindow()
        {
            List<IntPtr> wins = new();
            Regex rex = new(FF_WND_NAME);

            EnumWindows((hWnd, lParam) =>
                {
                    try
                    {
                        string t = GetWindowTextFromHandle(hWnd);
                        Match m = rex.Match(t);
                        if (m.Success == true)
                        {
                            wins.Add(hWnd);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return true;
                }, IntPtr.Zero);
            return wins.Where(hWnd => {
                _ = GetWindowThreadProcessId(hWnd, out uint wpid);
                return wpid == Process.GetCurrentProcess().Id;
            }).FirstOrDefault((IntPtr)0);
        }
        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        private const uint WM_KEYUP = 0x101;
        private const uint WM_KEYDOWN = 0x100;

        public static void KeyDown(VirtualKey vk)
        {
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
        }
        public static void KeyUp(VirtualKey vk)
        {
            SendMessage(hWnd, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
        }

        public static void KeyPress(VirtualKey vk)
        {
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
            Thread.Sleep(10);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
        }
    }
}
