using System.IO;
using System.Threading;

namespace Server.Utils
{
    public static class DeleteFile
    {
        public static void DeleteFileInAnotherThread(string path)
        {
            Thread deletingFileThread = new Thread(() => DeleteMutexFile(path));
            deletingFileThread.Start();

        }

        private static void DeleteMutexFile(string pathToDelete)
        {
            lock (pathToDelete)
            {
                File.Delete(pathToDelete);
            }

        }
    }
}
