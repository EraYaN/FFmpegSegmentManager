using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFMpegSegmentManager.Encoders
{
    public abstract class BaseEncoder : IEncoder
    {
        const double SEGMENT_TIME = 10;

        public string InputFile { get; }
        public string OutputFileFormat { get; }
        public double? SeekPosition { get; }
        public string Arguments => generateArguments();
        public BaseEncoder(string inputFile, string outputFileFormat, double? seekPosition)
        {
            InputFile = inputFile;
            OutputFileFormat = outputFileFormat;
            SeekPosition = seekPosition;
        }

        protected virtual string generateArguments()
        {
            var builder = new StringBuilder();
            builder.Append("-report -loglevel warning -hide_banner -progress pipe:2 -vsync passthrough -noaccurate_seek -copyts -start_at_zero");
            if (SeekPosition.HasValue)
            {
                builder.AppendFormat(" -ss {0}", SeekPosition.Value);
            }
            builder.AppendFormat(" -i \"{0}\"", InputFile);
            builder.Append(" {0}"); // for video variables            
            builder.AppendFormat(" -segment_time {0} -map 0 -segment_list pipe:1 -segment_list_type csv -f ssegment", SEGMENT_TIME);
            builder.AppendFormat(" \"{0}\"", OutputFileFormat);
            return builder.ToString();
        }

    }
}
