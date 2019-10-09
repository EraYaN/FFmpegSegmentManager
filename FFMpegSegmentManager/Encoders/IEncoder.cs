using System;
using System.Collections.Generic;
using System.Text;

namespace FFMpegSegmentManager.Encoders
{
    interface IEncoder
    {
        string Arguments { get; }
    }
}
