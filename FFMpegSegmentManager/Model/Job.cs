using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FFMpegSegmentManager.Encoders;
using FFMpegSegmentManager.Encoders.Video;
using FFMpegSegmentManager.Model;

namespace FFMpegSegmentManager.Model
{
    public class Job
    {
        
        public event EventHandler<SegmentReadyEventArgs> SegmentReady;
        Process process;
        ProcessStartInfo startInfo;
        string _sourceFile;
        string _outputDirectory;
        string _outputFilenameFormat;
        string _id;
        bool _isRunning;

        Dictionary<string, string> progressLines = new Dictionary<string, string>();

        IEncoder encoder;

        public string Identifier {
            get {
                if (encoder != null)
                    return encoder.Arguments;
                else
                    return string.Empty;
            }
        }

        public DateTime StartTime => process.StartTime;
        public bool IsRunning => _isRunning;

        public bool IsComplete => !_isRunning && process.HasExited && process.ExitCode == 0;

        public bool HasError => !_isRunning && process.HasExited && process.ExitCode != 0;

        public Job(string Id, string SourceFile, string OutputDirectory, string segmentExtension = ".ts", double? seekPosition = null, ulong? videoBitrate = 20000000)
        {
            _id = Id;
            _sourceFile = SourceFile;
            _outputDirectory = OutputDirectory;
            _outputFilenameFormat = Path.Combine(_outputDirectory, string.Format("segment-{0}-%12d{1}", _id, segmentExtension));
            process = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.CreateNoWindow = true;

            startInfo.UseShellExecute = false;
            
            encoder = new Libx264Encoder(_sourceFile, _outputFilenameFormat, seekPosition, videoBitrate);
            //encoder = new Libx265Encoder(_sourceFile, _outputFilenameFormat, seekPosition, videoBitrate);
            startInfo.Arguments = encoder.Arguments;
            startInfo.FileName = "ffmpeg";

            process.StartInfo = startInfo;

            process.Exited += Process_Exited;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            string[] values = e.Data.Split(',');
            if (values.Length == 3)
            {
                //SegmentReady
                SegmentReady(this, new SegmentReadyEventArgs(Path.Combine(_outputDirectory,values[0]), double.Parse(values[1]), double.Parse(values[2])));
            }            
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            if (e.Data.Contains("="))
            {                
                var items = e.Data.Split('=');
                if (items.Length == 2)
                {
                    var key = items[0];
                    var val = items[1];
                    if (key == "frame")
                    {
                        progressLines.Clear();
                    }
                    progressLines.Add(key, val);
                    if (key == "progress")
                    {
                        var fFMpegProgress = new FFMpegProgressUpdate(progressLines);
                        Console.WriteLine("[{4}] Progress: {0:F2}, Speed: {1:F1} fps ({3}x), IsEnd: {2}", fFMpegProgress.OutTime/1e6, fFMpegProgress.FramesPerSecond, fFMpegProgress.IsEnd, fFMpegProgress.Speed, _id);
                    }
                }
            }
        }

        public void Run()
        {
            process.Start();
            _isRunning = true;

            
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void Wait()
        {
            if (_isRunning)
                process.WaitForExit();
        }

        public void Stop()
        {
            if(!process.HasExited && _isRunning)
                process.StandardInput.Write('q');
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            _isRunning = false;
            process.CancelErrorRead();
            process.CancelOutputRead();
        }
    }
}
