using NaturalOrdering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileOrdering
{
    internal class Program
    {
        public enum MergeOption
        {
            None,
            MergeSubDir
        }
        
        private static string workingDir = string.Empty;

        public static void Main(string[] args)
        {
            WorkMain();
            Console.WriteLine("\n Press any key to close \n");
            Console.ReadKey();
        }
        
        private static void WorkMain()
        {
            List<string> fileList = new(), tempNameList = new();
            MergeOption option;
            try
            {
                IEnumerable<string> folders = Directory.EnumerateDirectories(Environment.CurrentDirectory);
                if (!folders.Any())
                {
                    Console.WriteLine($"Sub Directory not found\ntarget folder : {Environment.CurrentDirectory}");
                    return;
                }
                workingDir = CreateTempPath();
                foreach (string path in folders)
                {
                    if (workingDir.Equals(path)) continue;
                    try
                    {
                        Console.WriteLine($"Target folder : {path}\nContinue? Y (or any key) / N");
                        if (IsOk(ConsoleKey.N))
                        {
                            continue;
                        }
                        fileList.Clear();
                        option = MergeOption.None;
                        if (ExistsDirectory(path))
                        {
                            Console.WriteLine("Merge all files from sub Directories into a single Directory? Y / N (or any key)");
                            if (IsOk())
                            {
                                option = MergeOption.MergeSubDir;
                            }
                        }
                        AddFileNamesInFolder(path, fileList, option);
                        MoveToTempFolder(fileList, tempNameList);
                        SortingContent(path, fileList, tempNameList);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Path Error");
                        Console.WriteLine(e);
                        continue;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (Directory.Exists(workingDir))
                {
                    Directory.Delete(workingDir);
                }
            }

        }

        private static void SortingContent(string parentDir, List<string> fileList, List<string> tempNameList)
        {
            int index = 0;
            string modifypath;
            try
            {
                for (int fileIndex = 0, dirIndex = 0; index < fileList.Count; index++)
                {
                    modifypath = string.Empty;
                    if (File.Exists(tempNameList[index]))
                    {
                        fileIndex++;
                        modifypath = Path.Combine(parentDir, fileIndex + Path.GetExtension(fileList[index]));
                        Console.WriteLine($"{Path.GetFileName(fileList[index])} \t --> \t {Path.GetFileName(modifypath)}");
                        File.Move(tempNameList[index], modifypath);
                    }
                    else
                    {
                        dirIndex++;
                        modifypath = Path.Combine(parentDir, dirIndex.ToString());
                        Console.WriteLine($"{Path.GetFileName(fileList[index])} \t --> \t {Path.GetFileName(modifypath)}");
                        Directory.Move(tempNameList[index], modifypath);
                    }
                    tempNameList[index] = modifypath;
                }
                Console.WriteLine("Cancle? Y / N (or any key)");
                if (IsOk())
                {
                    CancelWorking(index, fileList, tempNameList);
                    Console.WriteLine("Cancel Complete");
                }
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine("\n Error occured. Attempting to restore file names \n");
                try
                {
                    CancelWorking(index, fileList, tempNameList);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath">folder full path</param>
        /// <param name="fileList">file names</param>
        private static void AddFileNamesInFolder(string directoryPath,List<string> fileList, MergeOption option=MergeOption.None)
        {
            IEnumerable<string>? files;
            if (option == MergeOption.MergeSubDir)
            {
                //get root files
                var root = Directory.EnumerateFiles(directoryPath, "*", new EnumerationOptions() { }).ToList();
                root.Sort(NaturalCompare.CompareOrdinal);
                fileList.AddRange(root);

                //get sub dir list without root dir
                var subDirList = Directory.EnumerateDirectories(directoryPath, "*", new EnumerationOptions() { RecurseSubdirectories = true }).ToList();
                subDirList.Sort(NaturalCompare.CompareOrdinal);
                
                //get files in dir list
                foreach (string subDir in subDirList)
                {
                    var subDirFileList = Directory.EnumerateFiles(subDir, "*", new EnumerationOptions() { }).ToList();
                    subDirFileList.Sort(NaturalCompare.CompareOrdinal);
                    fileList.AddRange(subDirFileList);
                }
            }
            else
            {
                // get root file and dir
                files = Directory.EnumerateFileSystemEntries(directoryPath, "*", new EnumerationOptions() { });
                fileList.AddRange(files);
                fileList.Sort(NaturalOrdering.NaturalCompare.CompareOrdinal);
            }
        }

        private static void MoveToTempFolder(List<string> TargetList, List<string> tempName)
        {
            tempName.Clear();
            for(int i = 0; i < TargetList.Count; i++)
            {
                if (File.Exists(TargetList[i]))
                {
                    tempName.Add(Path.Combine(workingDir, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetFileName(TargetList[i])));
                    File.Move(TargetList[i], tempName[i]);
                }
                else if (Directory.Exists(TargetList[i]))
                {
                    tempName.Add(Path.Combine(workingDir, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetFileName(TargetList[i])));
                    Directory.Move(TargetList[i], tempName[i]);
                }
            }
        }

        private static void CancelWorking(int index, List<string> fileList, List<string> tempNameList)
        {
            if (index >= fileList.Count)
            {
                index = fileList.Count - 1;
            }
            string modifyPath;
            for (int i=index; i> -1; i--)
            {
                if (File.Exists(tempNameList[i]))
                {
                    modifyPath = Path.Combine(workingDir, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetFileName(tempNameList[i]));
                    File.Move(tempNameList[i], modifyPath);
                    tempNameList[i] = modifyPath;
                }
                else if (Directory.Exists(tempNameList[index]))
                {
                    modifyPath = Path.Combine(workingDir, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetFileName(tempNameList[i]));
                    Directory.Move(tempNameList[i], modifyPath);
                    tempNameList[i] = modifyPath;
                }
            }
            for (; index > -1; index--)
            {
                if (File.Exists(tempNameList[index]))
                {
                    File.Move(tempNameList[index], fileList[index]);
                }
                else if(Directory.Exists(tempNameList[index]))
                {
                    Directory.Move(tempNameList[index], fileList[index]);
                }
            }
        }

        private static string CreateTempPath()
        {
            string result = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            Directory.CreateDirectory(result);
            return result;
        }

        /// <summary>
        /// compare input key and default
        /// </summary>
        /// <param name="defaultKey"></param>
        /// <returns></returns>
        private static bool IsOk(ConsoleKey defaultKey=ConsoleKey.Y)
        {
            ConsoleKey inputKey = Console.ReadKey().Key;
            Console.WriteLine();
            if (inputKey == defaultKey)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ExistsDirectory at path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool ExistsDirectory(string path)
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(path, "*", new EnumerationOptions() { });
            if (directories.Any())
            {
                return true;
            }
            return false;
        }
    }
}
