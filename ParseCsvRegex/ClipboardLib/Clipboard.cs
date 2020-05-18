using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParseCsvRegex.ClipboardLib
{
    /// <summary>
    /// Provides methods to place text on and retrieve text from the system Clipboard.
    /// </summary>
    public static class Clipboard
    {
        static Func<CancellationToken, Task<string?>> getAsyncFunc = CreateAsyncGet();
        static Func<string?> getFunc = CreateGet();

        /// <summary>
        /// Retrieves text data from the Clipboard.
        /// </summary>
        public static Task<string?> GetTextAsync(CancellationToken cancellation = default)
        {
            return getAsyncFunc(cancellation);
        }

        /// <summary>
        /// Retrieves text data from the Clipboard.
        /// </summary>
        public static string? GetText()
        {
            return getFunc();
        }

        static Func<string, CancellationToken, Task> setAsyncAction = CreateAsyncSet();
        static Action<string> setAction = CreateSet();

        /// <summary>
        /// Clears the Clipboard and then adds text data to it.
        /// </summary>
        public static Task SetTextAsync(string text, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(text, nameof(text));
            return setAsyncAction(text, cancellation);
        }

        /// <summary>
        /// Clears the Clipboard and then adds text data to it.
        /// </summary>
        public static void SetText(string text)
        {
            Guard.AgainstNull(text, nameof(text));
            setAction(text);
        }




        #region Set
        static Func<string, CancellationToken, Task> CreateAsyncSet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.SetTextAsync;
            }

           

            return (s, c) => throw new NotSupportedException();
        }

        static Action<string> CreateSet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.SetText;
            }


            return s => throw new NotSupportedException();
        }
        #endregion




        #region Get


        static Func<CancellationToken, Task<string?>> CreateAsyncGet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.GetTextAsync;
            }

            

            throw new NotSupportedException();
        }

        static Func<string?> CreateGet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.GetText;
            }


            throw new NotSupportedException();
        }





        #endregion
    }

    static class Guard
    {
        public static void AgainstNull(object value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }


    }

    static class WindowsClipboard
    {
        public static async Task SetTextAsync(string text, CancellationToken cancellation)
        {
            await TryOpenClipboardAsync(cancellation);

            InnerSet(text);
        }

        public static void SetText(string text)
        {
            TryOpenClipboard();

            InnerSet(text);
        }

        static void InnerSet(string text)
        {
            EmptyClipboard();
            IntPtr hGlobal = default;
            try
            {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);

                if (hGlobal == default)
                {
                    ThrowWin32();
                }

                var target = GlobalLock(hGlobal);

                if (target == default)
                {
                    ThrowWin32();
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(cfUnicodeText, hGlobal) == default)
                {
                    ThrowWin32();
                }

                hGlobal = default;
            }
            finally
            {
                if (hGlobal != default)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }
        }

        static async Task TryOpenClipboardAsync(CancellationToken cancellation)
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                {
                    break;
                }

                if (--num == 0)
                {
                    ThrowWin32();
                }

                await Task.Delay(100, cancellation);
            }
        }

        static void TryOpenClipboard()
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                {
                    break;
                }

                if (--num == 0)
                {
                    ThrowWin32();
                }

                Thread.Sleep(100);
            }
        }

        public static async Task<string?> GetTextAsync(CancellationToken cancellation)
        {
            if (!IsClipboardFormatAvailable(cfUnicodeText))
            {
                return null;
            }
            await TryOpenClipboardAsync(cancellation);

            return InnerGet();
        }

        public static string? GetText()
        {
            if (!IsClipboardFormatAvailable(cfUnicodeText))
            {
                return null;
            }
            TryOpenClipboard();

            return InnerGet();
        }

        static string? InnerGet()
        {
            IntPtr handle = default;

            IntPtr pointer = default;
            try
            {
                handle = GetClipboardData(cfUnicodeText);
                if (handle == default)
                {
                    return null;
                }

                pointer = GlobalLock(handle);
                if (pointer == default)
                {
                    return null;
                }

                var size = GlobalSize(handle);
                var buff = new byte[size];

                Marshal.Copy(pointer, buff, 0, size);

                return Encoding.Unicode.GetString(buff).TrimEnd('\0');
            }
            finally
            {
                if (pointer != default)
                {
                    GlobalUnlock(handle);
                }

                CloseClipboard();
            }
        }

        const uint cfUnicodeText = 13;

        static void ThrowWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("User32.dll", SetLastError = true)]
        static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern int GlobalSize(IntPtr hMem);
    }
}

