using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NotWallpapers
{
    class Program
    {
        public static string[] extensions = { ".png", ".jpg", ".jpeg" };

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool system(string str);

        public static void ScanDirectory(String path, String output, double ratio, bool recursive)
        {
            if (recursive)
            {
                String[] directories = Directory.GetDirectories(path);
                foreach (String directory in directories)
                {
                    ScanDirectory(directory, output, ratio, recursive);
                }
            }
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                string filename = Path.GetFileName(file);
                if (Program.extensions.Contains<string>(ext))
                {
                    FileStream s = File.Open(file, FileMode.Open);
                    Image i = Image.FromStream(s, false, false);
                    s.Close();
                    if (ratio != (double)i.Height / (double)i.Width)
                    {
                        try
                        {
                            File.Move(file, output + "/" + filename);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            system("pause");
                            return;
                        }
                        Console.WriteLine(filename + " moved to output directory.");
                    }
                }
                else
                {
                    Console.WriteLine(filename + " is not an image.");
                }
            }
        }

        static void Main(string[] args)
        {
            String path;
            String output;
            double ratio;
            bool recursive;

            //set path
            if (args.Length >= 1)
            {
                path = args[0];
            }
            else
            {
                path = Directory.GetCurrentDirectory();
            }

            //set output directory
            if (args.Length >= 2)
            {
                output = args[1];
            }
            else
            {
                output = @"../notwallpapers";
            }

            //recursive
            if (args.Length >= 3)
            {
                recursive = Convert.ToBoolean(args[2]);
            }
            else
            {
                recursive = false;
            }

            //set aspect ratio
            if (args.Length >= 4)
            {
                ratio = Convert.ToDouble(args[3]);
            }
            else
            {
                ratio = 0.5625;
            }

            //check for valid path
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid path: " + path);
                system("pause");
                return;
            }

            //check that output directory exists if not create it
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
                Console.WriteLine("Output directory " + output + " not found.");
                if (Directory.Exists(output))
                {
                    Console.WriteLine("Successfully created output directory.");
                }
                else
                {
                    Console.WriteLine("Failed to created output directory.");
                    system("pause");
                    return;
                }
            }

            ScanDirectory(path, output, ratio, recursive);

            system("pause");
        }
    }
}
