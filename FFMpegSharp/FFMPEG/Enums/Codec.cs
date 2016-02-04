namespace FFMpegSharp.FFMPEG.Enums
{
    internal enum VideoCodec
    {
        LibX264,
        LibVPX,
        LibTheora,
        PNG,
        MpegTS
    }

    internal enum AudioCodec
    {
        AAC,
        LibVorbis
    }

    internal enum Filter
    {
        H264_MP4ToAnnexB,
        AAC_ADTSToASC
    }

    internal enum Channel
    {
        Audio,
        Video,
        Both
    }
}
