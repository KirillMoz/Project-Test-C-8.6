using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TestHW03
{

    class Program
    {
        struct Student
        {
            public string Name;
            public string Group;
            public DateTime DateOfBirth;
            public decimal AvgBall;

            public Student(string Name, string Group, DateTime DateOfBirth, decimal AvgBall)
            {
                this.Name = Name;
                this.Group = Group;
                this.DateOfBirth = DateOfBirth;
                this.AvgBall = AvgBall;
            }
        }
        struct PathFile
        {
            public string Path;
            public bool IsFolder;

            public PathFile(string Path, bool IsFolder)
            {
                this.Path = Path;
                this.IsFolder = IsFolder;
            }
        }
        private List<PathFile> GetFoldersAndFiles(string Path)
        {
            List<PathFile> ListPuth = new List<PathFile>();
            try
            {
                string[] PuthFolders = Directory.GetDirectories(Path);
                foreach (string folder in PuthFolders)
                {
                    ListPuth.Add(new PathFile(folder, true));
                    ListPuth.AddRange(GetFoldersAndFiles(folder));
                }

                string[] PuthFiles = Directory.GetFiles(Path);
                foreach (string filename in PuthFiles)
                {
                    ListPuth.Add(new PathFile(filename, false));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ListPuth;
        }

        private void DeleteFolderOrFile(string Path, bool IsFolder)
        {
            if (IsFolder)
            {
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(Path);
                    if (DateTime.Now.Subtract(directory.LastAccessTime).TotalMinutes >= 30)
                    {
                        directory.Delete(true);
                        Console.WriteLine($"Каталог ({Path}) успешно удален со всеми вложенными файлами и папками");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при удалении каталога!!! Ошибка: {e.Message}");
                }
            }
            else
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(Path);
                    if (DateTime.Now.Subtract(fileInfo.LastAccessTime).TotalMinutes >= 30)
                    {
                        fileInfo.Delete();
                        Console.WriteLine($"Файл ({Path}) успешно удален!!!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при удалении файла!!! Ошибка: {e.Message}");
                }
            }
        }


        public void ClearFolder(string Path)
        {
            if (Directory.Exists(Path))
            {
                try 
                {
                    List<PathFile> ListFoldersAndFiles = GetFoldersAndFiles(Path);
                    foreach (var puth in ListFoldersAndFiles)
                    {
                        if (puth.IsFolder)
                        {
                            Console.WriteLine($"Путь до папки: {puth.Path}");
                        }
                        else
                        {
                            Console.WriteLine($"Путь до файла: {puth.Path}");
                        }

                        DeleteFolderOrFile(puth.Path, puth.IsFolder);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Не удалось сформировать список файлов. Код ошибки: {e.Message}");
                }
                   
            }
            else
                Console.WriteLine($"Каталога {Path} не существует!!! Пожалуйста, проверьте правильность пути к каталогу!");
        }

        public double GetBytes(string Path)
        {
            List<PathFile> ListFiles = GetFoldersAndFiles(Path);
            if (ListFiles == null || ListFiles.Count == 0)
                return 0;
            else
            {
                double AllBytes = 0;
                foreach (var file in ListFiles)
                {
                    if (!file.IsFolder)
                    {
                        FileInfo fileInfo = new FileInfo(file.Path);
                        if (fileInfo.Exists)
                            AllBytes += fileInfo.Length;
                    }
                }
                return AllBytes;
            }
        }

        static async Task Main(string[] args)
        {
            double sizeFolder = 0;
            string path = "";
            var ControlDelete = new Program();
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("Задание 1 (HW-03)");
            Console.WriteLine("Укажите путь до папки, которую нужно удалить: ");
            path = Console.ReadLine();
            try
            {
                ControlDelete.ClearFolder(path);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось удалить папку: {e.Message}");
            }
            Console.ReadKey();
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("Задание 2 (HW-03)");
            Console.WriteLine("Укажите путь до папки: ");
            path = Console.ReadLine();
            try
            {
                sizeFolder = ControlDelete.GetBytes(path);
                Console.WriteLine($"Размер папки ({path}): " + sizeFolder.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не вычислить размер папки: {e.Message}");
            }
            Console.ReadKey();
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("Задание 3 (HW-03)");
            Console.WriteLine("Укажите путь до папки: ");
            path = Console.ReadLine();
            try
            {
                sizeFolder = ControlDelete.GetBytes(path);
                Console.WriteLine($"Размер папки до удаления ({path}): " + sizeFolder.ToString());
                ControlDelete.ClearFolder(path);
                sizeFolder = ControlDelete.GetBytes(path);
                Console.WriteLine($"Размер папки после удаления ({path}): " + sizeFolder.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось удалить папку: {e.Message}");
            }
            Console.ReadKey();
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine("Задание 4 (HW-03)");
            List<Student> students = new List<Student>();
            Console.WriteLine("Считывание файла students.dat...");
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open("students.dat", FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        string Name = reader.ReadString();
                        string Group = reader.ReadString();
                        long DateOfBirth = reader.ReadInt64();
                        decimal AvgBall = reader.ReadDecimal();
                        students.Add(new Student(Name, Group, DateTime.FromBinary(DateOfBirth), AvgBall));
                    }
                }
                Console.WriteLine("Считывание файла students.dat успешно завершено");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Считывание файла students.dat не удалось: {e.Message}");
                Console.ReadKey();
            }

            if (students.Count > 0)
            {
                try
                {
                    Console.WriteLine("Создание папки Students");
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");
                    if (Directory.Exists(path))
                        Console.WriteLine("Папка Students уже создана");
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Не удалось создать папку Students: {e.Message}");
                        }

                    }
                    foreach (var student in students)
                    {
                        string Filename = Path.Combine(path, student.Group);
                        using (StreamWriter writer = new StreamWriter(Filename + ".txt", true))
                        {
                            string writedstring = student.Name + ", " + student.DateOfBirth.ToString() + ", " + student.AvgBall.ToString();
                            await writer.WriteLineAsync(writedstring);
                        }
                    }
                    Console.WriteLine($"Парсинг файла успешно завершен: файлы лежат в папке {path}");
                    Console.WriteLine("------------------------------------------------------------------------------------------");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Не удалось создать папку Students: {e.Message}");
                }
            }
  
        }
    }
}
