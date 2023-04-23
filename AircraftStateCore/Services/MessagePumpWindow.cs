using System.Reflection;
using System.Runtime.InteropServices;

namespace AircraftStateCore.Services;

#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
public sealed class MessagePumpWindow : IDisposable
{
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WNDCLASS
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr CreateWindowExW(
       uint dwExStyle,
       [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
       [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
       uint dwStyle,
       int x,
       int y,
       int nWidth,
       int nHeight,
       IntPtr hWndParent,
       IntPtr hMenu,
       IntPtr hInstance,
       IntPtr lpParam
    );

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    static extern IntPtr DispatchMessage(ref MSG lpmsg);

    [DllImport("user32.dll")]
    static extern bool TranslateMessage(ref MSG lpMsg);

    private const int ERROR_CLASS_ALREADY_EXISTS = 1410;

    public bool Disposed { get; private set; } = false;
    public IntPtr Hwnd { get; private set; }

    public void Dispose()
    {
        if (!Disposed)
        {
            // Dispose unmanaged resources
            if (Hwnd != IntPtr.Zero)
            {
                DestroyWindow(Hwnd);
                Hwnd = IntPtr.Zero;
            }
            Disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    static MessagePumpWindow instance = null;
    public static MessagePumpWindow GetWindow()
    {
        instance ??= new MessagePumpWindow();
        return instance;
    }

    private MessagePumpWindow()
    {
        var className = Assembly.GetExecutingAssembly().GetName().Name;
        wndProcDelegate = CustomWndProc;
        WNDCLASS windClass = new() { lpszClassName = className, lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProcDelegate) };
        ushort classAtom = RegisterClassW(ref windClass);
        int lastError = Marshal.GetLastWin32Error();
        if (classAtom == 0 && lastError != ERROR_CLASS_ALREADY_EXISTS)
            throw new System.Data.DuplicateNameException();
        //Hwnd = CreateWindowExW(0, className, "MessagePump", 0, 0, 0, 10, 10, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        Hwnd = CreateWindowExW(0, className, "MessagePump", 0, 10, 10, 10, 10, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    private IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        WndProcHandle?.Invoke(hWnd, msg, wParam, lParam);
        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    public static void MessageLoop()
    {
        _ = GetWindow(); // Ensure an instance has been created.
        var msg = new MSG();
        // Standard WIN32 message loop
        while (GetMessage(out msg, IntPtr.Zero, 0, 0) > 0 && !GetWindow().Disposed)
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }
    }

    public event WndProc WndProcHandle;

    private readonly WndProc wndProcDelegate;
}
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

