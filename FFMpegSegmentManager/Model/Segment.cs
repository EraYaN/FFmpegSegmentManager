using System;

namespace FFMpegSegmentManager.Model
{
    public class Segment
    {
        public ulong Id { get; set; }
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public DateTime? LastAccessed { get; set; }
    }
}
