namespace FFMpegSegmentManager.Encoders.Video
{
    class Libx265Encoder : BaseVideoEncoder
    {
        public Libx265Encoder(string inputFile, string outputFileFormat, double? seekPosition, ulong? videoBandwidth) : base(inputFile, outputFileFormat, seekPosition, videoBandwidth) { }

        protected override string generateArguments()
        {
            var base_arguments = base.generateArguments();
            var codec_arguments = string.Format("-c:v libx265 -preset veryfast");
            return string.Format(base_arguments, codec_arguments);
        }
    }
}
