namespace RGSS{
 using System;
 using System.Windows.Forms;
 using System.Runtime.InteropServices;
 using System.Text;
 using System.Threading;

 public class RGSS{
  [DllImport("RGSS301", CharSet=CharSet.Unicode, EntryPoint = "RGSSGameMain")]
  public static extern void GameMain(IntPtr a, string b, string c);
  [DllImport("RGSS301", EntryPoint = "RGSSInitialize3")]
  public static extern void Initialize(int a);
  [DllImport("RGSS301", CharSet=CharSet.Unicode, EntryPoint = "RGSSSetupRTP")]
  public static extern void SetupRTP(string ini, StringBuilder error, int errlen);
  [DllImport("RGSS301", CharSet=CharSet.Ansi, EntryPoint = "RGSSEval")]
  public static extern int Eval(string text);

  [DllImport("msvcrt", EntryPoint = "malloc", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr alloc(int length);
  [DllImport("msvcrt", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl)]
  public static extern void free(IntPtr ptr);

  [DllImport("Kernel32", EntryPoint = "InitializeCriticalSection")]
  public static extern IntPtr LockInit(IntPtr lockvar);
  [DllImport("Kernel32", EntryPoint = "DeleteCriticalSection")]
  public static extern IntPtr LockFree(IntPtr lockvar);
  [DllImport("Kernel32", EntryPoint = "EnterCriticalSection")]
  public static extern IntPtr LockLock(IntPtr lockvar);
  [DllImport("Kernel32", EntryPoint = "LeaveCriticalSection")]
  public static extern IntPtr LockUnlock(IntPtr lockvar);
 };

 public class Port{
   public IntPtr Buffer, Lock;

   public Port(int size = 1024){
     Buffer = RGSS.alloc(size);
     Lock   = RGSS.alloc(128);
     RGSS.LockInit(Lock);
   }

   ~Port(){
     RGSS.LockUnlock(Lock);
     RGSS.free(Lock);
     RGSS.free(Buffer);
   }
 }
 public class RGSSPlayer : Form{
  Port RPort, WPort;

  [STAThread]
  public static void Main(String[] Args){
    new RGSSPlayer();         //Application.Run()
  }

  RGSSPlayer(){
    RGSS.Initialize(0);
    RPort = new Port();
    WPort = new Port(); 
    Show();
    Thread th = new Thread(this.Thing);
    th.Start();
    Run();
    th.Join();
  }

  void Thing(){
    while(true){Thread.Sleep(10);} // Should replaced by C# calling request
  }
  
  void Run(){
    StringBuilder error = new StringBuilder(512);
    RGSS.SetupRTP(System.Environment.CurrentDirectory + "/Game.ini", error , error.Capacity);
    RGSS.Eval(String.Format("WriterLock = {0}\nWriteBuffer = {1}\n",  RPort.Lock.ToString(), RPort.Buffer.ToString()));
    RGSS.Eval(String.Format("ReaderLock = {0}\nReaderBuffer = {1}\n", WPort.Lock.ToString(), WPort.Buffer.ToString()));
    RGSS.GameMain(Handle, "Data/Scripts.rvdata2", "\0\0\0\0");
  } 
 }


}