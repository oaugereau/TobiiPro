using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tobii.Research;

namespace RecordingEyeGaze
{


    public class Record
    {
        private string filenameWithoutExtention;
        //This lists contain the lines that will be written to the output files.
        private static List<string> linesGaze = new List<string>();
        private bool recording = false;
        private IEyeTracker myEyetracker;
        float width = 0;
        float height = 0;



        public void saveFiles(int recordingSpeed = 0)
        {
            string screenshotFolder = filenameWithoutExtention + "_screenshots";
            string gazeFilename = filenameWithoutExtention + ".csv";


            //Write the headers
            File.WriteAllText(gazeFilename, "WindowsTime;Timestamp;GazeX;GazeY;PupilDiameter\n");
            //Write the files
            File.AppendAllLines(gazeFilename, linesGaze);

            //make a screenshot and save it
            //screenshot(screenshotFolder + "/01.png");
        }

        public void screenshot(string savePath)
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            // Save the screenshot to the specified path that the user has chosen.
            bmpScreenshot.Save(savePath, ImageFormat.Png);
        }

        public Record(string filenameWithoutExtention, IEyeTracker myEyeTracker)
        {
            this.filenameWithoutExtention = filenameWithoutExtention;
            myEyetracker = myEyeTracker;
            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height;
        }

        public void recordLoop()
        {
            string input = Console.ReadLine();
            if (input == "q")
                return;
            else
            {
                if (recording == false)
                {
                    startRecord();
                    recording = true;
                }
                else
                {
                    pauseRecord();
                    recording = false;
                }
                recordLoop();
            }
        }

        public void startRecord()
        {
            myEyetracker.GazeDataReceived += MyEyeTracker_GazeDataReceived;
        }

        public void pauseRecord()
        {
            myEyetracker.GazeDataReceived -= MyEyeTracker_GazeDataReceived;
        }


        private void MyEyeTracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            // Write the data to the console.
            double x = e.LeftEye.GazePoint.PositionOnDisplayArea.X;
            double y = e.LeftEye.GazePoint.PositionOnDisplayArea.Y;
            Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}", x, y, e.SystemTimeStamp);
            linesGaze.Add(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.FFF") + ";" + e.SystemTimeStamp + ";" + x * width + ";" + y * height + ";" + e.LeftEye.Pupil.PupilDiameter);
        }

        internal void Dispose()
        {
            myEyetracker.Dispose();
        }
    }
}
