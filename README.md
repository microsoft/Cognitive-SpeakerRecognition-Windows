# Microsoft Speaker Recognition API: Windows Client Library & Sample
This repo contains the Windows client library & sample for the Microsoft Speaker Recognition API, an offering within [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services), formerly known as Project Oxford.
* [Learn about the Speaker Recognition API](https://www.microsoft.com/cognitive-services/en-us/speaker-recognition-api)
* [Read the documentation](https://www.microsoft.com/cognitive-services/en-us/speaker-recognition-api/documentation)
* [Find more SDKs & Samples](https://www.microsoft.com/cognitive-services/en-us/SDK-Sample?api=speaker%20recognition)


## The sample
The sample includes three applications:
 1. two Windows WPF applications to demonstrate the use of identification and verification features of Speaker Recognition API for single-speaker short audios.
 2. a Windows WPF application to demonstrate an approach to use identification on potentially longer audios that contain multiple speakers by streaming a few seconds at a time.

### Build the sample
 1. Starting in the folder where you clone the repository (this folder)
 2. In a git command line tool, type `git submodule init` (or do this through a UI)
 3. Pull in the shared Windows code by calling `git submodule update`
 4. Start Microsoft Visual Studio 2015 and select `File > Open > Project/Solution`.
 5. For speaker identification, starting in the folder where you clone the repository, go to
    `SpeakerRecognition > Windows > Identification` folder.
    For speaker verification, starting in the folder where you clone the repository, go to
    `SpeakerRecognition > Windows > Verification` folder.
    For speaker steaming, starting in the folder where you clone the repository, go to
    `SpeakerRecognition > Windows > Streaming` folder.
 6. Double-click the Visual Studio 2015 Solution (.sln) file.
 7. Press Ctrl+Shift+B, or select `Build > Build Solution`.

### Run the sample
After the build is complete, press F5 to run the sample.

First, you must obtain a Speaker Recognition API subscription key by [following the instructions on our website](<https://www.microsoft.com/cognitive-services/en-us/sign-up>).

Locate the text edit box saying "Paste your subscription key here to start". Paste your subscription key. You can choose to persist your subscription key in your machine by clicking the "Save Key" button. When you want to delete the subscription key from the 
machine, click "Delete Key" to remove it from your machine.

Click on "Select Scenario" to use samples of different scenarios, and
follow the instructions on screen.

### Streaming Audio File
 1. Start with the "Enroll Speakers" scenario to prepare the speakers you will identify against.
 2. In the "Stream File" scenario, Press "Load File" button and load your audio file.
 3. Select the profiles  you want to use as candidate speakers.
 4. Control the number of seconds used with each identification request by tuning the "window size".
 5. Control the number of seconds between each identification request through tuning the "step size". 
 6. Press "Stream" button and monitor the results of the streaming process.
 
 Note: 
 Make sure that the number of requests per minute resulting from tunning the step size won't exceed your subscription's rate limit.
 For example, applying a step size of 1 on an audio file of size 1 minute will result in 60 requests. Applying a step size of 2 on the same audio file will result in 30 requests.
 For your convenience, we have provided sample audios to enroll 2 speakers and a sample audio for streaming. These audios are found under `SpeakerRecognition\Windows\Streaming\SPIDStreamingAPI-WPF-Samples\SampleAudios`.



## Contributing
We welcome contributions. Feel free to file issues and pull requests on the repo and we'll address them as we can. Learn more about how you can help on our [Contribution Rules & Guidelines](</CONTRIBUTING.md>). 

You can reach out to us anytime with questions and suggestions using our communities below:
 - **Support questions:** [StackOverflow](<https://stackoverflow.com/questions/tagged/microsoft-cognitive>)
 - **Feedback & feature requests:** [Cognitive Services UserVoice Forum](<https://cognitive.uservoice.com>)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## License
All Microsoft Cognitive Services SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
