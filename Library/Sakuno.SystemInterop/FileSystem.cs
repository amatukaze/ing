namespace Sakuno.SystemInterop
{
    public static class FileSystem
    {
        public static bool Unblock(string rpFileName)
        {
            return NativeMethods.Kernel32.DeleteFileW(rpFileName + ":Zone.Identifier");
        }
    }
}
