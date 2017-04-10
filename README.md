# Microsoft Speaker Recognition API: Windows Client Library & Sample
This repo contains the Windows client library & sample for the Microsoft Speaker Recognition API, an offering within [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services), formerly known as Project Oxford.
* [Learn about the Speaker Recognition API](https://www.microsoft.com/cognitive-services/en-us/speaker-recognition-api)
* [Read the documentation](https://www.microsoft.com/cognitive-services/en-us/speaker-recognition-api/documentation)
* [Find more SDKs & Samples](https://www.microsoft.com/cognitive-services/en-us/SDK-Sample?api=speaker%20recognition)


## The sample
The sample is a Windows WPF application to demonstrate the use of Speaker Recognition API. It demonstrates the speaker identification and speaker verification features. 
Also, it demonstarates the way to stream audio and the usage of identification features in identifying speakers contributing in this streamed audio.

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

Locate the text edit box saying "Paste your subscription key here to start". Paste your subscription key. You can choose to persist your subscription key in your machine by clicking "Save Key" button. When you want to delete the subscription key from the
machine, click "Delete Key" to remove it from your machine.

Click on "Select Scenario" to use samples of different scenarios, and
follow the instructions on screen.

Microsoft will receive the audio files you upload and may use them to improve
Speaker Recognition API and related services. By submitting an audio, you confirm
you have consent from everyone in it.

### Streaming Audio File
 1. Starting in the "Stream File" Window, Press "Load File" button and load your audio file.
 2. Select the profiles of the speakers who should contribute in the identification process
     during streaming the audio.
 3. Control number of look-back seconds included with each newly-streamed second to be identified
     through tunning the "widnow size".
 4. Control the number of requests sent in streaming your audio through tunning the "step size".
     If the step size is x, then you identify streamed seconds every other x seconds.
 5. Press "stream" button and wait for the results of the streaming process.



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


## Developer Code of Conduct
Developers using Cognitive Services, including this client library & sample, are expected to follow the “Developer Code of Conduct for Microsoft Cognitive Services”, found at [http://go.microsoft.com/fwlink/?LinkId=698895](http://go.microsoft.com/fwlink/?LinkId=698895).
