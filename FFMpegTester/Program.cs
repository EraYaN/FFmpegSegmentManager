using FFMpegSegmentManager;
using FFMpegSegmentManager.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFMpegTesting
{
    class Program
    {
        static Dictionary<string, string> progressLines;
        static FFMpegProgressUpdate fFMpegProgress;
        static SegmentStore sstore;

        static string segmentExtension = ".ts";
        static string sourceFile = @"\\STORAGE\media\MakeMKV\GIRLS_GENERATION_THE_BEST\title05.mkv";
        static string tempDirectory = @"F:\Users\Erwin\Videos\segments_test\";
        static string storeDirectory = @"F:\Users\Erwin\Videos\segments_store\";

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            progressLines = new Dictionary<string, string>();
            sstore = new SegmentStore(storeDirectory, segmentExtension, 100000000); //100MB max cache size.
            
            // request comes in for seek position 300.5 seconds with profile 10Mbit.
            Job job = new Job("MAIN", sourceFile, tempDirectory, segmentExtension, 300.5, 10000000);
            job.SegmentReady += Job_SegmentReady;

            job.Run();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                Console.WriteLine("Gracefully quitting ffmpeg process...");
                if (!job.IsComplete)
                    job.Stop();
            };

            job.Wait();
        }

        private static ulong GetSegmentHash(ulong startTime, ulong endTime, string parameters)
        {
            //init with versions and maybe magic constants.
            List<byte> byteContents = new List<byte>() { 0x00, 0x50 };
            byteContents.AddRange(BitConverter.GetBytes(startTime));
            byteContents.AddRange(BitConverter.GetBytes(endTime));
            byteContents.AddRange(Encoding.UTF8.GetBytes(parameters));
            var hash = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashText = hash.ComputeHash(byteContents.ToArray());

            return BitConverter.ToUInt64(hashText, 0);
        }

        private static void Job_SegmentReady(object sender, SegmentReadyEventArgs e)
        {
            var job = sender as Job;
            ulong startTime = Convert.ToUInt64(Math.Round(e.StartTime * 1e6));
            ulong endTime = Convert.ToUInt64(Math.Round(e.EndTime * 1e6));
            ulong hash = GetSegmentHash(startTime, endTime, job.Identifier);
            Console.WriteLine("Created segment {0} for timestamps {1:F2} to {2:F2} for a length of {3:F2} with hash {4:X16}", e.SegmentFileName, e.StartTime, e.EndTime, e.EndTime - e.StartTime, hash);
            //first parameter is essentially a hash of the input variables.
            sstore.Store(hash, e.SegmentFileName, startTime, endTime);
            Console.WriteLine("Current cache size {0:F1} MB", sstore.CacheSize/1e6);
        }        
    }
}
