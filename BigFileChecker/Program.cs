namespace BigFileChecker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Program
{
    private static string lang;
    static void Main(string[] args)
    {
        Console.WriteLine("ru/en?");
        lang=Console.ReadLine();
        while(lang!="ru"&&lang!="en")
        {
            Console.WriteLine("ru/en?");
            lang=Console.ReadLine();
        }
        switch (lang)
        {
            case "ru":
                while (true)
                {
                    string path;
                    Console.WriteLine("\nBigFileChecker(v1)\n");
                    Console.WriteLine("Выберите функцию:\n 1.Информация о хранилище\n 2.Анализ конкретных файлов\n 3.Анализ конкретных папок\n");
                    int func = int.Parse(Console.ReadLine());
                    switch (func)
                    {
                        case 1:
                            ShowDriveInfo();
                            break;
                        case 2:
                            Console.Write("\nВведите путь для анализа (например, C:\\): ");
                            path = Console.ReadLine();
                            if (!Directory.Exists(path))
                            {
                                Console.WriteLine("Указанная директория не существует!");
                                return;
                            }

                            AnalyzeFiles(path);
                            break;
                        case 3:
                            Console.Write("\nВведите путь для анализа (например, C:\\): ");
                            path = Console.ReadLine();
                            if (!Directory.Exists(path))
                            {
                                Console.WriteLine("Указанная директория не существует!");
                                return;
                            }

                            AnalyzeDirectories(path);
                            break;
                    }
                }
            case "en":
                while (true)
                {
                    string path;
                    Console.WriteLine("BigFileChecker(v1)\n");
                    Console.WriteLine("Choose function:\n 1.Storage info\n 2.File size analyze\n 3.Folder size analyze\n");
                    int func = int.Parse(Console.ReadLine());
                    switch (func)
                    {
                        case 1:
                            ShowDriveInfo();
                            break;
                        case 2:
                            Console.Write("\nEnter the path to be analyzed (e.g. C:\\): ");
                            path = Console.ReadLine();
                            if (!Directory.Exists(path))
                            {
                                Console.WriteLine("The specified directory does not exist!");
                                return;
                            }
                            AnalyzeFiles(path);
                            break;
                        case 3:
                            Console.Write("\nEnter the path to be analyzed (e.g. C:\\): ");
                            path = Console.ReadLine();
                            if (!Directory.Exists(path))
                            {
                                Console.WriteLine("The specified directory does not exist!");
                                return;
                            }
                            AnalyzeDirectories(path);
                            break;
                    }
                }
                    
        }

        static void ShowDriveInfo()
        {
            switch (lang)
            {
                case "ru":
                    Console.WriteLine("Информация о дисках:");
                    foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                    {
                        Console.WriteLine($"\nДиск: {drive.Name}");
                        Console.WriteLine($"Тип: {drive.DriveType}");
                        Console.WriteLine($"Формат: {drive.DriveFormat}");
                        Console.WriteLine($"Общий размер: {FormatSize(drive.TotalSize)}");
                        Console.WriteLine($"Свободно: {FormatSize(drive.AvailableFreeSpace)}");
                    }
                    break;
                case "en":
                    Console.WriteLine("Drive's info:");
                    foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                    {
                        Console.WriteLine($"\nDrive: {drive.Name}");
                        Console.WriteLine($"Type: {drive.DriveType}");
                        Console.WriteLine($"Format: {drive.DriveFormat}");
                        Console.WriteLine($"Total size: {FormatSize(drive.TotalSize)}");
                        Console.WriteLine($"Free space: {FormatSize(drive.AvailableFreeSpace)}");
                    }
                    break;
            }
        }

        static void AnalyzeFiles(string path)
        {
            switch (lang)
            {
                case "ru":
                    Console.WriteLine("\nАнализ файлов...");
                    var files = new List<FileInfo>();
                    var directoryQueue = new Queue<string>();
                    directoryQueue.Enqueue(path);
                    while (directoryQueue.Count > 0)
                    {
                        string currentDir = directoryQueue.Dequeue();
                        try
                        {
                            foreach (string file in Directory.GetFiles(currentDir))
                            {
                                try
                                {
                                    files.Add(new FileInfo(file));
                                }
                                catch (UnauthorizedAccessException)
                                {
                                }
                            }
                            foreach (string dir in Directory.GetDirectories(currentDir))
                                directoryQueue.Enqueue(dir);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                    var topFiles = files.OrderByDescending(f => f.Length).Take(10);
                    Console.WriteLine("\nТоп 10 самых больших файлов:");
                    int counter = 1;
                    foreach (var file in topFiles)
                    {
                        Console.WriteLine($"{counter++}. {file.FullName} - {FormatSize(file.Length)}");
                    }
                    break;
                case "en":
                    Console.WriteLine("\nAnalyzing files...");
                    files = new List<FileInfo>();
                    directoryQueue = new Queue<string>();
                    directoryQueue.Enqueue(path);
                    while (directoryQueue.Count > 0)
                    {
                        string currentDir = directoryQueue.Dequeue();
                        try
                        {
                            foreach (string file in Directory.GetFiles(currentDir))
                            {
                                try
                                {
                                    files.Add(new FileInfo(file));
                                }
                                catch (UnauthorizedAccessException)
                                {
                                }
                            }
                            foreach (string dir in Directory.GetDirectories(currentDir))
                                directoryQueue.Enqueue(dir);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                    topFiles = files.OrderByDescending(f => f.Length).Take(10);
                    Console.WriteLine("\n10 Most large files:");
                    counter = 1;
                    foreach (var file in topFiles)
                    {
                        Console.WriteLine($"{counter++}. {file.FullName} - {FormatSize(file.Length)}");
                    }
                    break;
            }
        }

        static void AnalyzeDirectories(string path)
        {
            switch (lang)
            {
                case "ru":
                    Console.WriteLine("\nАнализ папок...");
                    var dirSizes = new Dictionary<string, long>();
                    var directoryQueue = new Queue<string>();
                    directoryQueue.Enqueue(path);
                    while (directoryQueue.Count > 0)
                    {
                        string currentDir = directoryQueue.Dequeue();
                        try
                        {
                            long size = CalculateDirectorySize(currentDir);
                            dirSizes[currentDir] = size;
    
                            foreach (string dir in Directory.GetDirectories(currentDir))
                                directoryQueue.Enqueue(dir);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                    var topDirs = dirSizes.OrderByDescending(d => d.Value).Take(10);
                    Console.WriteLine("\nТоп 10 самых больших папок:");
                    int counter = 1;
                    foreach (var dir in topDirs)
                    {
                        Console.WriteLine($"{counter++}. {dir.Key} - {FormatSize(dir.Value)}");
                    } 
                    break;
                case "en":
                    Console.WriteLine("\nAnalyzing folders...");
                    dirSizes = new Dictionary<string, long>();
                    directoryQueue = new Queue<string>();
                    directoryQueue.Enqueue(path);
                    while (directoryQueue.Count > 0)
                    {
                        string currentDir = directoryQueue.Dequeue();
                        try
                        {
                            long size = CalculateDirectorySize(currentDir);
                            dirSizes[currentDir] = size;
    
                            foreach (string dir in Directory.GetDirectories(currentDir))
                                directoryQueue.Enqueue(dir);
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                    topDirs = dirSizes.OrderByDescending(d => d.Value).Take(10);
                    Console.WriteLine("\n10 Most large folders:");
                    counter = 1;
                    foreach (var dir in topDirs)
                    {
                        Console.WriteLine($"{counter++}. {dir.Key} - {FormatSize(dir.Value)}");
                    } 
                    break;
            }
        }

        static long CalculateDirectorySize(string path)
        {
            long size = 0;
            try
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                foreach (string dir in Directory.GetDirectories(path))
                    size += CalculateDirectorySize(dir);
            }
            catch (UnauthorizedAccessException)
            {
            }

            return size;
        }

        static string FormatSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblBytes = bytes;

            while (dblBytes >= 1024 && i < suffixes.Length - 1)
            {
                dblBytes /= 1024;
                i++;
            }

            return $"{dblBytes:0.##} {suffixes[i]}";
        }
    }
}