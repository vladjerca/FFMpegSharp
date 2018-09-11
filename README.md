# FFMpegSharp
![FFMpeg Sharp](https://media.licdn.com/mpr/mpr/jc/AAEAAQAAAAAAAAL-AAAAJDEyYTdkMjAyLTc2M2ItNDM2YS1iOTc5LTAwZTU1YWM0NjdiNQ.jpg)

FFMpegSharp is a great way to use FFMpeg encoding when writing video applications, client-side and server-side. It has wrapper methods that allow conversion to all web formats: MP4, OGV, TS and methods of capturing screens from the videos. 

### Getting started

Setup your app config (ffmpeg files can be found in the 'Resources' folder):

```xml
  <appSettings>
    <add key="ffmpegRoot" value="C:\ffmpeg\bin\" />
  </appSettings>
```

### FFProbe

FFProbe is used to gather video information
```csharp
static void Main(string[] args)
{
    string inputFile = "G:\\input.mp4";

    // loaded from configuration
    var video = new VideoInfo(inputFile);

    string output = video.ToString();

    Console.WriteLine(output);
}
```

Sample output:
```csharp
Video Path : G:\input.mp4
Video Root : G:\\
Video Name: input.mp4
Video Extension : .mp4
Video Duration : 00:00:09
Audio Format : none
Video Format : h264
Aspect Ratio : 16:9
Framerate : 30fps
Resolution : 1280x720
Size : 2.88 Mb
```

### FFMpeg
Convert your video files to web ready formats:

```csharp
static void Main(string[] args)
{
    string inputFile = "input_path_goes_here";
    var encoder = new FFMpeg();
    FileInfo outputFile = new FileInfo("output_path_goes_here");

    var video = VideoInfo.FromPath(inputFile);

    // easily track conversion progress
    encoder.OnProgress += (percentage) => Console.WriteLine("Progress {0}%", percentage);

    // MP4 conversion
    encoder.Convert(
        video,
        outputFile,
        VideoType.Mp4, 
        Speed.UltraFast,
        VideoSize.Original,
        AudioQuality.Hd,
        true
    );
    // OGV conversion
    encoder.Convert(
        video,
        outputFile,
        VideoType.Ogv,
        Speed.UltraFast,
        VideoSize.Original,
        AudioQuality.Hd,
        true
    );
    // TS conversion
    encoder.Convert(
        video,
        outputFile,
        VideoType.Ts
    );
}
```

Easily capture screens from your videos:
```csharp
static void Main(string[] args)
{
    string inputFile = "input_path_goes_here";
    FileInfo output = new FileInfo("output_path_goes_here");

    var video = VideoInfo.FromPath(inputFile);

    new FFMpeg()
        .Snapshot(
            video, 
            output, 
            new Size(200, 400), 
            TimeSpan.FromMinutes(1)
        );
}
```

Join video parts:
```csharp
static void Main(string[] args)
{
    FFMpeg encoder = new FFMpeg();

    encoder.Join(
        new FileInfo(@"..\joined_video.mp4"),
        VideoInfo.FromPath(@"..\part1.mp4"),
        VideoInfo.FromPath(@"..\part2.mp4"),
        VideoInfo.FromPath(@"..\part3.mp4")
    );
}
```

Join image sequences: 
```csharp
static void Main(string[] args)
{
    FFMpeg encoder = new FFMpeg();

    encoder.JoinImageSequence(
        new FileInfo(@"..\joined_video.mp4"),
        1, // FPS
        ImageInfo.FromPath(@"..\1.png"),
        ImageInfo.FromPath(@"..\2.png"),
        ImageInfo.FromPath(@"..\3.png")
    );
}
```

Strip audio track from videos:
```csharp
static void Main(string[] args)
{
    string inputFile = "input_path_goes_here",
       outputFile = "output_path_goes_here";

    new FFMpeg()
        .Mute(
            VideoInfo.FromPath(inputFile),
            new FileInfo(outputFile)
        );
}
```

Save audio track from video:
```csharp
static void Main(string[] args)
{
    string inputVideoFile = "input_path_goes_here",
       outputAudioFile = "output_path_goes_here";

    new FFMpeg()
        .ExtractAudio(
            VideoInfo.FromPath(inputVideoFile), 
            new FileInfo(outputAudioFile)
        );
}
```

Add audio track to video:
```csharp
static void Main(string[] args)
{
    string inputVideoFile = "input_path_goes_here",
       inputAudioFile = "input_path_goes_here",
       outputVideoFile = "output_path_goes_here";

    FFMpeg encoder = new FFMpeg();

    new FFMpeg()
        .ReplaceAudio(
            VideoInfo.FromPath(inputVideoFile), 
            new FileInfo(inputAudioFile), 
            new FileInfo(outputVideoFile)
        );
}
```

Add poster image to audio file (good for youtube videos):
```csharp
static void Main(string[] args)
{
    string inputImageFile = "input_path_goes_here",
       inputAudioFile = "input_path_goes_here",
       outputVideoFile = "output_path_goes_here";

    FFMpeg encoder = new FFMpeg();

    ((Bitmap)Image.FromFile(inputImageFile))
        .AddAudio(
            new FileInfo(inputAudioFile), 
            new FileInfo(outputVideoFile)
        );

    /* OR */

    new FFMpeg()
        .PosterWithAudio(
            inputImageFile,
            new FileInfo(inputAudioFile),
            new FileInfo(outputVideoFile)
        );
}
```

Control over the 'FFmpeg' process doing the job:
```csharp
static void Main(string[] args)
{
    string inputVideoFile = "input_path_goes_here",
       outputVideoFile = "input_path_goes_here";

    FFMpeg encoder = new FFMpeg();

    // start the conversion process
    Task.Run(() => {
        encoder.Convert(new VideoInfo(inputVideoFile), new FileInfo(outputVideoFile));
    });

    // stop encoding after 2 seconds (only for example purposes)
    Thread.Sleep(2000);
    encoder.Stop();
}
```

Video Size enumeration:

```csharp
public enum VideoSize
{
    HD,
    FullHD,
    ED,
    LD,
    Original
}
```

Speed enumeration:

```csharp
public enum Speed
{
    VerySlow,
    Slower,
    Slow,
    Medium,
    Fast,
    Faster,
    VeryFast,
    SuperFast,
    UltraFast
}
```
