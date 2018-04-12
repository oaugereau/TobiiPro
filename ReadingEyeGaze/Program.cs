using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnalazingEyeGaze
{
    public class Program
    {

        static List<Color> colors = new List<Color>() { Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Yellow, Color.Orange, Color.Cyan, Color.DarkBlue, Color.HotPink };

        static void Main(string[] args)
        {

            List<string> fileToProcess = new List<string>();

            //Process all file from a folder or open a single file
            Console.WriteLine("Process all the recordings from a folder (A) or a single file (S) ?");
            string process = Console.ReadLine().ToUpper();
            //Folder
            if (process == "A")
            {
                Console.WriteLine("Write the path of the folder");
                string path = Console.ReadLine();
                //Take all csv files
                string[] filenames = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
                //Take only files which first line is "Timestamp;GazeX;GazeY;LeftEye;RightEye"
                foreach (var item in filenames)
                {
                    if (File.ReadAllLines(item)[0].Contains("GazeX;GazeY"))
                        //if (File.ReadAllLines(item)[0] == "Timestamp;GazeX;GazeY;LeftEye;RightEye")
                        fileToProcess.Add(item.Replace(".csv", ""));
                }
            }
            //File
            else
            {
                Console.WriteLine("Write the name of the file to open (without extention)");
                fileToProcess.Add(Console.ReadLine());
            }

            foreach (var item in fileToProcess)
            {
                processOneFile(item);
            }
        }

        public static string processOneFile(string filenameWithoutExtention, string screenShot = "")
        {
            Console.WriteLine("Processing the file: " + filenameWithoutExtention);
            GazeData gd = new GazeData(filenameWithoutExtention);
            //writeImage(gd, gd.pngOriginal); //Display the raw gazes in a picture
            //writeVideo(gd, gd.aviOriginal); //Create a video of the gazes

            //Compute the fixations 
            gd.gazes = GazeData.fixationBusher2008(gd.gazes);
            writeText(gd, gd.csvFixations); //save the fixations in a file
            writeImage(gd, gd.pngFixations, screenShot); //Display the fixations in a picture
                                                         //writeVideo(gd, gd.aviFixations);

            ////Line break detection
            ////gd.rowDataLineBreak(-40, 3, 2); //not usually used
            //gd.lines = GazeData.lineBreakDetectionSimple(ref gd.gazes, -500);
            ////gd.lineBreakDetectionOkoso(); //not usually used
            //writeText(gd, gd.csvFixations); //save the fixations in a file
            //writeImage(gd, gd.pngLineBreak, false);
            ////writeVideo(gd, gd.aviLineBreak);

            //Align gaze with lines
            //align(gd, "doc.layout");
            //writeImage(gd, gd.pngFixations);
            //writeVideo(gd, gd.aviFixations);

            return gd.csvFixations;
        }


        public static void writeImage(GazeData gd, string outputFile, string backGroundImage)
        {
            // quick hack because some people though it was a good idea to put tobii on a second screen but we have no idea what is the size of the first screen!!!

            Bitmap image = new Bitmap(backGroundImage);
            Graphics g = Graphics.FromImage(image);
            SolidBrush brush = new SolidBrush(Color.Red);
            for (int i = 0; i < gd.gazes.Count; i++)
            {
                g.FillEllipse(brush, new Rectangle((int)gd.gazes[i].gazeX - 5, (int)gd.gazes[i].gazeY - 5, 10, 10));
            }
            g.Save();
            image.Save(outputFile);
            Console.WriteLine("The image " + outputFile + " file has been written");

        }

        public static void writeText(GazeData gd, string outputFile)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            List<string> textLines = new List<string>();
            //header
            textLines.Add("timestamp;gazeX;gazeY;duration;idLine;pupilDiameter");
            //content
            foreach (var gaze in gd.gazes)
            {
                textLines.Add(gaze.timestamp + ";" + gaze.gazeX + ";" + gaze.gazeY + ";" + gaze.duration + ";" + gaze.idLine + ";" + gaze.pupilDiameter);
            }

            File.WriteAllLines(outputFile, textLines.ToArray());
        }
    }
}
