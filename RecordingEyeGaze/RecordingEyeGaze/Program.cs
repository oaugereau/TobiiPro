using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using ModelUtpal;
using AnalazingEyeGaze;
using Tobii.Research;

namespace RecordingEyeGaze
{
    public class Program
    {


        static void Main(string[] args)
        {
            Console.WriteLine("\nSearching for all eye trackers");
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            foreach (IEyeTracker eyeTracker in eyeTrackers)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}", eyeTracker.Address, eyeTracker.DeviceName, eyeTracker.Model, eyeTracker.SerialNumber, eyeTracker.FirmwareVersion);
            }
            var myEyeTracker = eyeTrackers.First();
            string license = @"license_key_IS404-100106240232";
            ApplyLicense(myEyeTracker, license);

            Console.WriteLine("\nWrite the output file name (without extension): ");
            string filenameWithoutExtention = Console.ReadLine();

            Record record = new Record(filenameWithoutExtention, myEyeTracker);

            Console.WriteLine("Listening for gaze data, press 'q' to quit or any key to start/stop the recording...");
            // Let it run until a key is pressed.
            record.recordLoop();
            
            //Save the files
            record.saveFiles();

            //Dispose
            record.Dispose();

            string screenShot = filenameWithoutExtention + ".png";
            record.screenshot(screenShot);
            //Process the fix file
            string fileOutput = AnalazingEyeGaze.Program.processOneFile(filenameWithoutExtention, screenShot);
            ModelUtpal.Program.processOnefile(fileOutput);
        }

        private static void ApplyLicense(IEyeTracker eyeTracker, string licensePath)
        {
            // Create a collection with the license.
            var licenseCollection = new LicenseCollection(
                new System.Collections.Generic.List<LicenseKey>
                {
           new LicenseKey(System.IO.File.ReadAllBytes(licensePath))
                });
            // See if we can apply the license.
            FailedLicenseCollection failedLicenses;
            if (eyeTracker.TryApplyLicenses(licenseCollection, out failedLicenses))
            {
                Console.WriteLine(
                    "Successfully applied license from {0} on eye tracker with serial number {1}.",
                    licensePath, eyeTracker.SerialNumber);
            }
            else
            {
                Console.WriteLine(
                    "Failed to apply license from {0} on eye tracker with serial number {1}.\n" +
                    "The validation result is {2}.",
                    licensePath, eyeTracker.SerialNumber, failedLicenses[0].ValidationResult);
            }
            // Clear any applied license.
            //eyeTracker.ClearAppliedLicenses();
        }
    }
}
