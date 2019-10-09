using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFMpegSegmentManager.Encoders.Video
{
    class CopyEncoder : BaseVideoEncoder
    {
        public CopyEncoder(string inputFile, string outputFileFormat, double? seekPosition, ulong? videoBandwidth) : base(inputFile, outputFileFormat, seekPosition, videoBandwidth) { }

        protected override string generateArguments()
        {
            var base_arguments = base.generateArguments();
            var codec_arguments = string.Format("-c:v copy");
            return string.Format(base_arguments, codec_arguments);
        }
    }
}
