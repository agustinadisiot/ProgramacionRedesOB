using System.IO;
using System.Threading.Tasks;

namespace Server.Utils
{
    public static class DeleteFile
    {
        public static void DeleteFileInAnotherThread(string path)
        {
            Task.Run(() => DeleteMutexFile(path));

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
