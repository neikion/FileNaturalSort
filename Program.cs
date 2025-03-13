using NaturalOrdering;
namespace FileOrdering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WorkMain();
            Console.WriteLine("\n Press any key to close \n");
            Console.ReadKey();
        }

        public static void WorkMain()
        {
            List<string> fileList;
            int index = 0;
            ConsoleKey inputKey;
            IEnumerable<string> folders = Directory.EnumerateDirectories(Environment.CurrentDirectory);
            if (folders.Count() == 0)
            {
                Console.WriteLine($"Subfolders not found\ntarget folder : {Environment.CurrentDirectory}");
                return;
            }
            foreach (string path in folders)
            {
                try
                {
                    Console.WriteLine($"Target folder : {path}\nContinue? Y (or any key) / N");
                    inputKey = Console.ReadKey().Key;
                    Console.WriteLine();
                    if (inputKey == ConsoleKey.N)
                    {
                        continue;
                    }
                    GetFileNamesInFolder(path, out fileList);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Path Error");
                    Console.WriteLine(e);
                    continue;
                }


                try
                {
                    for (index = 0; index < fileList.Count; index++)
                    {
                        string modifypath = Path.GetDirectoryName(fileList[index]) + "\\" + +index + Path.GetExtension(fileList[index]);
                        Console.WriteLine($"{Path.GetFileName(fileList[index])} \t --> \t {Path.GetFileName(modifypath)}");
                        File.Move(fileList[index], modifypath);
                    }
                    Console.WriteLine("Cancle? Y / N (or any key)");
                    inputKey = Console.ReadKey().Key;
                    if (inputKey == ConsoleKey.Y)
                    {
                        CancelWorking(index, fileList);
                        Console.WriteLine("Cancel Complete");
                    }
                    Console.WriteLine();
                }
                catch
                {
                    Console.WriteLine("\n Error occured. Attempting to restore file names \n");
                    try
                    {
                        CancelWorking(index, fileList);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath">folder full path</param>
        /// <param name="fileList">file names</param>
        public static void GetFileNamesInFolder(string directoryPath,out List<string> fileList)
        {
            IEnumerable<string> files = Directory.EnumerateFileSystemEntries(directoryPath, "*", new EnumerationOptions() { });
            fileList = files.ToList();
            fileList.Sort(NaturalOrdering.NaturalCompare.CompareOrdinal);
        }

        public static void CancelWorking(int index, List<string> fileList)
        {
            if (index >= fileList.Count)
            {
                index = fileList.Count - 1;
            }
            for (; index > -1; index--)
            {
                string modifypath = Path.GetDirectoryName(fileList[index]) + "\\" + +index + Path.GetExtension(fileList[index]);
                Console.WriteLine($"{Path.GetFileName(modifypath)} \t --> \t {Path.GetFileName(fileList[index])}");
                File.Move(modifypath, fileList[index]);
            }
        }
    }
}
