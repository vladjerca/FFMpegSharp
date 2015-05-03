# FFMpegSharp
![FFMpeg Sharp](https://media.licdn.com/mpr/mpr/jc/AAEAAQAAAAAAAAL-AAAAJDEyYTdkMjAyLTc2M2ItNDM2YS1iOTc5LTAwZTU1YWM0NjdiNQ.jpg)

FFMpegSharp is a great way to use FFMpeg encoding when writing video applications, client-side and server-side. It has wrapper methods that allow conversion to all web formats: MP4, OGV, WebM and methods of capturing screens from the videos. 

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
            
            // string ffmpegRoot = ConfigurationManager.AppSettings["ffmpegRoot"]
            // the is loaded implicitly from configuration if it is not passed as an argument to the constructor
            FFProbe infoDecoder = new FFProbe(ffmpegRoot);

            string output = infoDecoder.GetVideoInfo(inputFile).ToString();

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
            string  inputFile = "input_path_goes_here",
                    outputFile = "output_path_goes_here";
            // string ffmpegRoot = ConfigurationManager.AppSettings["ffmpegRoot"]
            // the is loaded implicitly from configuration if it is not passed as an argument to the constructor
            FFMpeg encoder = new FFMpeg();
            
            // easily track conversion progress
            encoder.OnProgress += encoder_OnProgress;
            
            // input and output strings are required
            // all other parameters are optional
            encoder.ToMP4(inputFile, outputFile, Speed.Slower, VideoSize.FullHD, true);
            encoder.ToOGV(inputFile, outputFile, VideoSize.HD, true);
            encoder.ToWebM(inputFile, outputFile, VideoSize.Original, true);
            encoder.ToTS(inputFile, outputFile);
        }
        
static void encoder_OnProgress(int percentage)
        {
            Console.WriteLine("Progress {0}%", percentage);
        }
```

Easily capture screens from your videos:
```csharp
static void Main(string[] args)
        {
            string inputFile = "input_path_goes_here",
                   outputFile = "output_path_goes_here";

            FFMpeg encoder = new FFMpeg();

            encoder.SaveThumbnail(inputFile, outputFile, new TimeSpan(0, 0, 15));
        }
```

Join video parts:
```csharp
static void Main(string[] args)
        {
            string ffmpegRoot = ConfigurationManager.AppSettings["ffmpegRoot"];
            
            FFMpeg encoder = new FFMpeg(ffmpegRoot);

            encoder.Join(@"..\joined_video.mp4", @"..\part1.mp4", @"..\part2.mp4", @"..\part3.mp4");
        }
```

Strip audio track from videos:
```csharp
static void Main(string[] args)
        {
            string inputFile = "input_path_goes_here",
                   outputFile = "output_path_goes_here";

            FFMpeg encoder = new FFMpeg(ffmpegRoot);

            encoder.Mute(inputFile, outputFile);
        }
```

Save audio track from video:
```csharp
static void Main(string[] args)
        {
            string inputVideoFile = "input_path_goes_here",
                   outputAudioFile = "output_path_goes_here";

            FFMpeg encoder = new FFMpeg(ffmpegRoot);

            encoder.SaveAudio(inputVideoFile, outputAudioFile);
        }
```

Save audio track from video:
```csharp
static void Main(string[] args)
        {
            string inputVideoFile = "input_path_goes_here",
                   inputAudioFile = "input_path_goes_here",
                   outputVideoFile = "output_path_goes_here";

            FFMpeg encoder = new FFMpeg(ffmpegRoot);

            encoder.AddAudio(inputVideoFile, inputAudioFile, outputVideoFile);
        }
```

Add poster image to audio file (good for youtube videos):
```csharp
static void Main(string[] args)
        {
            string inputImageFile = "input_path_goes_here",
                   inputAudioFile = "input_path_goes_here",
                   outputVideoFile = "output_path_goes_here";

            FFMpeg encoder = new FFMpeg(ffmpegRoot);

            encoder.AddAudio(inputImageFile, inputAudioFile, outputVideoFile);
        }
```

Control over the 'FFmpeg' process doing the job:
```csharp
static void Main(string[] args)
        {
            string inputVideoFile = "input_path_goes_here",
                   outputVideoFile = "input_path_goes_here";
                   
            FFMpeg encoder = new FFMpeg(ffmpegRoot);
            
            // start the conversion process
            Task.Run(() => {
              encoder.ToMP4(inputVideoFile, outputVideoFile);
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
