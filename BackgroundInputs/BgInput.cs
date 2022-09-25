﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

namespace CottonCollector.BackgroundInputs
{
    internal class BgInput
    {

        #region user32 imports
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, 
            WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);
        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        #endregion

        #region delegates
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, 
            int idChild, int dwEventThread, int dwmsEventTime);
        #endregion

        #region const and enums
        private const string FF_WND_NAME = "FINAL FANTASY XIV";

        private const uint WM_KEYUP = 0x101;
        private const uint WM_KEYDOWN = 0x100;
        private const int EVENT_SYSTEM_FOREGROUND = 0x0003;

        private enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }
        #endregion

        #region window utils
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

        private static IntPtr hWndGame = FindFFXIVWindow();
        private static IntPtr hFocusHook = IntPtr.Zero;
        private static WinEventProc listener = new(ForegroundChangedCallback);
        private static bool gameIsFocused = true;
        private static HashSet<VirtualKey> pressedKeys = new();

        private static void ForegroundChangedCallback(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, 
            int idChild, int dwEventThread, int dwmsEventTime)
        {
            if (hWnd == hWndGame)
            {
                PluginLog.Verbose("BAKA!! Re-Focused FFXIV!");
                gameIsFocused = true;
            }
            else if (gameIsFocused)
            {
                PluginLog.Verbose("BAKA!! Un-Focused FFXIV!");
                gameIsFocused = false;
                foreach (VirtualKey pressedKey in  pressedKeys)
                {
                    KeyDown(pressedKey);
                }
            }
        }

        public static void Initialize()
        {
            PluginLog.Log("Initializing BgInput");
            hFocusHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, listener, 0, 0,
                SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
        }

        public static void Dispose()
        {
            UnhookWinEvent(hFocusHook); 
        }

        public static void KeyDown(VirtualKey vk)
        {
            pressedKeys.Add(vk);
            SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
        }

        public static void KeyUp(VirtualKey vk)
        {
            pressedKeys.Remove(vk);
            SendMessage(hWndGame, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
        }

        public static void KeyPress(VirtualKey vk)
        {
            SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
            Thread.Sleep(10);
            SendMessage(hWndGame, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
        }
    }
}
