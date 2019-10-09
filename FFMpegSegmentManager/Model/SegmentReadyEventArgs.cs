using System;

namespace FFMpegSegmentManager.Model
{
    public class SegmentReadyEventArgs : EventArgs
    {
        public string SegmentFileName { get; }
        public double StartTime { get; }
        public double EndTime { get; }
        public SegmentReadyEventArgs(string SegmentFileName, double StartTime, double EndTime)
        {
            this.SegmentFileName = SegmentFileName;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
        }
    }
}