using System;
using System.Collections.Generic;
using System.Text;

namespace FFMpegSegmentManager.Model
{
    public class FFMpegProgressUpdate
    {

        public long FrameNumber => _frameNumber;
        public double FramesPerSecond => _framesPerSecond;
        public double BitRate => _bitRate;
        public long TotalSize => _totalSize;
        public long OutTime => _outTime;
        public long DuplicatedFrames => _duplicatedFrames;
        public long DroppedFrames => _droppedFrames;
        public double Speed => _speed;
        public bool IsEnd => _progress.Equals("end", StringComparison.OrdinalIgnoreCase);

        private long _frameNumber;
        private double _framesPerSecond;
        private double _bitRate;
        private long _totalSize;
        private long _outTime;
        private long _duplicatedFrames;
        private long _droppedFrames;
        private double _speed;
        private string _progress;

        //StdErr: frame=137
        //StdErr: fps=36.15
        //StdErr: stream_0_0_q=-1.0
        //StdErr: bitrate= 483.2kbits/s
        //StdErr: total_size=337561
        //StdErr: out_time_us=5589333
        //StdErr: out_time_ms=5589333
        //StdErr: out_time=00:00:05.589333
        //StdErr: dup_frames=0
        //StdErr: drop_frames=0
        //StdErr: speed=1.47x
        //StdErr: progress=continue

        public FFMpegProgressUpdate(Dictionary<string, string> lines)
        {
            if (lines.ContainsKey("frame"))
            {
                long.TryParse(lines["frame"], out _frameNumber);
            }

            if (lines.ContainsKey("fps"))
            {
                double.TryParse(lines["fps"], out _framesPerSecond);
            }

            if (lines.ContainsKey("bitrate"))
            {
                double.TryParse(lines["bitrate"], out _bitRate);
            }

            if (lines.ContainsKey("total_size"))
            {
                long.TryParse(lines["total_size"], out _totalSize);
            }

            if (lines.ContainsKey("out_time_us"))
            {
                long.TryParse(lines["out_time_us"], out _outTime);
            }

            if (lines.ContainsKey("dup_frames"))
            {
                long.TryParse(lines["dup_frames"], out _duplicatedFrames);
            }

            if (lines.ContainsKey("drop_frames"))
            {
                long.TryParse(lines["drop_frames"], out _droppedFrames);
            }

            if (lines.ContainsKey("speed"))
            {
                
                double.TryParse(lines["speed"].Replace('x', '0'), out _speed);
            }

            if (lines.ContainsKey("progress"))
            {
                _progress = lines["progress"];
            }
        }
    }
}
