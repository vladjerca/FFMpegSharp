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
		FileInfo outputFile = new FileInfo("output_path_goes_here");

		var video = VideoInfo.FromPath(inputFile);
				
		// easily track conversion progress
		video.OnConversionProgress += video_OnConversionProgress;

		// input and output strings are required
		// all other parameters are optional
		video.ConvertTo(VideoType.Mp4, outputFile, Speed.UltraFast,
			VideoSize.Original,
			AudioQuality.Hd, 
			true, 
			false);
		video.ConvertTo(VideoType.Ogv, outputFile, Speed.UltraFast,
			VideoSize.Original,
			AudioQuality.Hd,
			true,
			false);
		video.ConvertTo(VideoType.WebM, outputFile, Speed.UltraFast,
			VideoSize.Original,
			AudioQuality.Hd,
			true,
			false);
		video.ConvertTo(VideoType.Ts, outputFile, Speed.UltraFast,
			VideoSize.Original,
			AudioQuality.Hd,
			true,
			false);
	}

	static void video_OnConversionProgress(double percentage)
	{
		Console.WriteLine("Progress {0}%", percentage);
	}
```

Easily capture screens from your videos:
```csharp
static void Main(string[] args)
        {
            string inputFile = "input_path_goes_here";
            FileInfo output = new FileInfo("output_path_goes_here");

            var video = VideoInfo.FromPath(inputFile);

            video.Snapshot(output, new Size(200, 400), TimeSpan.FromMinutes(1));
        }
```

Join video parts:
```csharp
static void Main(string[] args)
        {
            FFMpeg encoder = new FFMpeg();

            encoder.Join(new FileInfo(@"..\joined_video.mp4"), 
                VideoInfo.FromPath(@"..\part1.mp4"),
                VideoInfo.FromPath(@"..\part2.mp4"),
                VideoInfo.FromPath(@"..\part3.mp4"));
        }
```

Strip audio track from videos:
```csharp
static void Main(string[] args)
        {
            string inputFile = "input_path_goes_here",
                   outputFile = "output_path_goes_here";
            
            VideoInfo.FromPath(inputFile).Mute(new FileInfo(outputFile));
        }
```

Save audio track from video:
```csharp
static void Main(string[] args)
        {
            string inputVideoFile = "input_path_goes_here",
                   outputAudioFile = "output_path_goes_here";
                        
            VideoInfo.FromPath(inputVideoFile).ExtractAudio(new FileInfo(outputAudioFile));
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

            VideoInfo.FromPath(inputVideoFile).ReplaceAudio(new FileInfo(inputAudioFile), new FileInfo(outputVideoFile));
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

            var image = (Bitmap)Image.FromFile(inputImageFile);
            image.AddAudio(new FileInfo(inputAudioFile), new FileInfo(outputVideoFile);
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
                encoder.ToMp4(new VideoInfo(inputVideoFile), new FileInfo(outputVideoFile));
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
