using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QwiqClient.Services
{
    public class MemoryIO
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        private IntPtr _hProc;

        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        public MemoryIO(Process process)
        {
            _hProc = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, process.Id);
        }

        public void WriteMemory(int address, byte[] buffer)
        {
            WriteProcessMemory((int)_hProc, address, buffer, buffer.Length, out int read);
        }

        public byte[] ReadMemory(int address, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            var read = 0;
            ReadProcessMemory((int)_hProc, address, buffer, buffer.Length, ref read);

            return buffer;
        }

        private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
