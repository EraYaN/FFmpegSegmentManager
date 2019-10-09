namespace FFMpegSegmentManager.Encoders.Video
{
    class Libx264Encoder : BaseVideoEncoder
    {
        public Libx264Encoder(string inputFile, string outputFileFormat, double? seekPosition, ulong? videoBandwidth) : base(inputFile, outputFileFormat, seekPosition, videoBandwidth) { }

        protected override string generateArguments()
        {
            var base_arguments = base.generateArguments();
            var codec_arguments = string.Format("-c:v libx264 -preset veryfast");
            return string.Format(base_arguments, codec_arguments);
        }
    }
}
