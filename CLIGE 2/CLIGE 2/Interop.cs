using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace CLIGE_2
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InteropCoord
    {
        public short X;
        public short Y;

        public InteropCoord(short x, short y)
        {
            X = x;
            Y = y;
        }

        public InteropCoord(int x, int y)
        {
            X = (short)x;
            Y = (short)y;
        }

        public InteropCoord(Coord coord)
        {
            X = (short)coord.x;
            Y = (short)coord.y;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public char AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
        [FieldOffset(0)] public CharUnion CharUnion;
        [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    public class Interop
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int MAXIMIZE = 3;

        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_SIZE = 0xF000;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_RESTORE = 0xF120;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            InteropCoord dwBufferSize,
            InteropCoord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        private static SafeFileHandle handle;
        public static SmallRect rect;

        public static void FullscreenWindow()
        {
            Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(GetConsoleWindow(), MAXIMIZE);
        }

        public static void DisableResizing()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_RESTORE, MF_BYCOMMAND);
        }

        public static void SetConsoleHandle()
        {
            handle = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (handle.IsInvalid)
            {
                throw new Exception("Cannot create a handle to the console window.");
            }
        }

        public static void Draw(ScreenRegion region)
        {
            region.rect.RefreshSides();
            rect = new SmallRect() { Left = (short)region.rect.left, Top = (short)region.rect.top, Right = (short)(region.rect.right + 1), Bottom = (short)(region.rect.bottom + 1) };
            WriteConsoleOutput(handle, ConvertToCharInfo(region), new InteropCoord(region.rect.size), new InteropCoord(0, 0), ref rect);
        }

        private static CharInfo[] ConvertToCharInfo(ScreenRegion region)
        {
            CharInfo[] convertedContent = new CharInfo[region.rect.size.x * region.rect.size.y];

            for (int y = 0; y < region.rect.size.y; y++)
            {
                for (int x = 0; x < region.rect.size.x; x++)
                {
                    convertedContent[(y * region.rect.size.x) + x].Attributes = (byte)((byte)region.grid[y][x].foreground | ((byte)region.grid[y][x].background << 4));
                    convertedContent[(y * region.rect.size.x) + x].CharUnion.AsciiChar = region.grid[y][x].texture;
                }
            }

            return convertedContent;
        }
    }
}
