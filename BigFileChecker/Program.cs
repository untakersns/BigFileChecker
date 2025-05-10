namespace BigFileChecker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            string path;
            Console.WriteLine("BigFileChecker(v1)\n");
            Console.WriteLine(
                "Выберите функцию:\n 1.Информация о хранилище\n 2.Анализ конкретных файлов\n 3.Анализ конкретных папок\n");
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

        static void ShowDriveInfo()
        {
            Console.WriteLine("Информация о дисках:");
            foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                Console.WriteLine($"\nДиск: {drive.Name}");
                Console.WriteLine($"Тип: {drive.DriveType}");
                Console.WriteLine($"Формат: {drive.DriveFormat}");
                Console.WriteLine($"Общий размер: {FormatSize(drive.TotalSize)}");
                Console.WriteLine($"Свободно: {FormatSize(drive.AvailableFreeSpace)}");
            }
        }

        static void AnalyzeFiles(string path)
        {
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
        }

        static void AnalyzeDirectories(string path)
        {
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
            string[] suffixes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
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