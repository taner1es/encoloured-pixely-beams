using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace k32
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Attributes
            string fileOutputName = "golgede_kalmayalim";
            int w = 32000;
            int h = 18000;
            var pixels = new byte[w * h * 3];

            var t0 = Environment.TickCount64;
            
            // Pixelize scale factor
            int interval = 200;

            // Pre-produced randomized and scaled through interval value
            byte[,] random = new byte[w / interval, h / interval];

            for(int y = 0; y < h / interval; y++)
            {
                for(int x = 0; x < w / interval; x++)
                {
                    random[x, y] = (byte)new Random().Next(100,255);
                }
            }
            #endregion

            #region Drawing
            Parallel.For(0, h, j =>
            {
                var t = j * w * 3;

                for (var i = 0; i < w; i++)
                {
                    double r = 0, g = 0, b = 0;

                    // Corner points
                    if (i % interval == 0 && j % interval == 0)
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                    // Horizontal and vertical dividers
                    else if ((i % interval == 0 || j % interval == 0 || i == w - 1))
                    {
                        r = 255;
                        g = 255;
                        b = 255;
                    }
                    else
                    {
                        // Flooring 
                        int floorX = i - (i % interval);
                        int floorY = j - (j % interval);

                        // Have a random value
                        int rnd = random[floorX / interval, floorY / interval];
                        
                        // Encolor the quarter circle (top-left)
                        if((i * i) + (j * j) <= w / 8 * w / 8)
                        {
                            r = rnd;
                            g = rnd;
                        }
                        // Encolor the rays
                        else if(i <= j / 16)
                        {
                            g = rnd;
                            b = rnd;
                        }
                        else if(i <= j / 8)
                        {
                            r = rnd;
                            b = rnd;
                        }
                        else if(i <= j / 4)
                        {
                            r = rnd;
                            g = rnd / 2;
                        }
                        else if(i <= j / 2)
                        {
                            r = rnd / 2;
                            b = rnd;
                        }
                        else if (j >= i)
                        {
                            r = rnd;
                        }
                        else if (j >= i / 2)
                        {
                            b = rnd;
                        }
                        else if (j >= i / 4)
                        {
                            g = rnd;
                        }
                        else if (j >= i / 8)
                        {
                            r = rnd;
                            b = rnd / 2;
                        }
                        else if (j >= i / 16)
                        {
                            g = rnd;
                            b = rnd / 2;
                        }
                        else if (j >= i / 32)
                        {
                            r = rnd;
                            g = rnd / 4;
                        }
                        else
                        {
                            g = rnd / 2;
                            b = rnd;
                        }
                    }

                    pixels[t++] = (byte)b;
                    pixels[t++] = (byte)g;
                    pixels[t++] = (byte)r;
                }
            });
            #endregion

            #region Output
            // Print time
            Console.WriteLine("Time Elapsed: {0}ms", Environment.TickCount64 - t0);

            // Save output File
            var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            var bitmap = new Bitmap(w, h, w * 3, PixelFormat.Format24bppRgb, handle.AddrOfPinnedObject());
            bitmap.Save(fileOutputName + ".png", ImageFormat.Png);
            bitmap.Save(fileOutputName + ".jpg", ImageFormat.Jpeg);

            // Print saved files
            Console.WriteLine("Saved Output Files:");
            Console.WriteLine("PNG: " + Environment.CurrentDirectory + @"/" + fileOutputName + ".png");
            Console.WriteLine("JPG: " + Environment.CurrentDirectory + @"/" + fileOutputName + ".jpg");

            // View output file
            var p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = fileOutputName + ".png",
                WindowStyle = ProcessWindowStyle.Maximized,
                UseShellExecute = true
            };
            p.Start();

            // Print opened file
            Console.WriteLine("\nOpen File: " + Environment.CurrentDirectory + @"/" + fileOutputName + ".png");
            #endregion
        }
    }
}
