using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFMpegSegmentManager.Encoders.Video
{
    public abstract class BaseVideoEncoder : BaseEncoder
    {
        public ulong? VideoBandwidth { get; }

        public BaseVideoEncoder(string inputFile, string outputFileFormat, double? seekPosition, ulong? videoBandwidth) : base(inputFile, outputFileFormat, seekPosition)
        {
            VideoBandwidth = videoBandwidth;
        }

        protected override string generateArguments()
        {
            var base_arguments = base.generateArguments();
            var builder = new StringBuilder();
            builder.Append("{0}"); // for encoder variables
            if (VideoBandwidth.HasValue)
            {
                builder.AppendFormat(" -b:v {0}", VideoBandwidth);
            }
            return string.Format(base_arguments, builder.ToString());            
        }

    }
}
