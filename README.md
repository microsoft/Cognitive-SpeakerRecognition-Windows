Microsoft Cognitive Services Speaker Recognition Windows client library
==================

This repo contains the client libraries that demonstrate Microsofts cloud-based speaker recognition algorithms to identify and verify speakers.

Speaker Identification
----------------

Identify a speaker from a group of given speakers.

Speaker verification
----------------

Check a speaker's identity using his voice print.

The sample
==========

The sample is a Windows WPF application to demonstrate the use of Speaker Recognition API.

It demonstrates the speaker identification and speaker verification features.

Build the sample
----------------


1.  Starting in the folder where you clone the repository (this folder)

2.  In a git command line tool, type `git submodule init` (or do this through a UI)

3.  Pull in the shared Windows code by calling `git submodule update`

4.  Start Microsoft Visual Studio 2015 and select `File > Open >
    Project/Solution`.

5.  For speaker identification, starting in the folder where you clone the repository, go to
    `SpeakerRecognition > Windows > Identification` folder.
    For speaker identification, starting in the folder where you clone the repository, go to
    `SpeakerRecognition > Windows > Verification` folder.

6.  Double-click the Visual Studio 2015 Solution (.sln) file.

7.  Press Ctrl+Shift+B, or select `Build > Build Solution`.

Run the sample
--------------

After the build is complete, press F5 to run the sample.

First, you must obtain a Speaker Recognition API subscription key by following instructions in [Microsoft Cognitive Services subscription](<https://www.microsoft.com/cognitive-services/en-us/sign-up>).

Locate the text edit box saying "Paste your subscription key here to start". Paste
your subscription key. You can choose to persist your subscription key in your machine
by clicking "Save Key" button. When you want to delete the subscription key from the
machine, click "Delete Key" to remove it from your machine.

Click on "Select Scenario" to use samples of different scenarios, and
follow the instructions on screen.

Microsoft will receive the audio files you upload and may use them to improve
Speaker Recognition API and related services. By submitting an audio, you confirm
you have consent from everyone in it.

Contributing
============
We welcome contributions and are always looking for new SDKs, input, and
suggestions. Feel free to file issues on the repo and we'll address them as we can. You can also learn more about how you can help on the [Contribution
Rules & Guidelines](</CONTRIBUTING.md>).

For questions, feedback, or suggestions about Microsoft Cognitive Services, feel free to reach out to us directly.

-   [Cognitive Services UserVoice Forum](<https://cognitive.uservoice.com>)

License
=======

All Microsoft Cognitive Services SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
