using System;
using System.Collections.Generic;
using System.Text;

namespace FFMpegSegmentManager.Model
{
    public class Segment
    {
        public ulong Id { get; set; }
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public DateTime? LastAccessed { get; set; }
        public DateTime Added { get; set; }
    }
}
