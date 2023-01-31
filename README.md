Flickers on top 🖥️
=======================
[![DOI](https://zenodo.org/badge/483661450.svg)](https://zenodo.org/badge/latestdoi/483661450)

🪧 Overview
------------------
✨ FlickersOnTop is a program written in C# that offers SSVEP and code-VEP (using m-sequence) flickers implementation trough a Graphical User Interface for Windows only 🪟.   
No need to code ! Just use the software to define flickers and place them. Use the sequencer to enable play and pauses in the flickering. The flickers can be displayed on top of any interface or with a black background for pyschopysics experiments. 🧑‍🔬  
It provides [Lab Streaming Layer (LSL)](https://github.com/sccn/labstreaminglayer) integration to send corresponding markers at the start of the flickering.

🧠 It was developped in the [Human-Factors department](https://personnel.isae-supaero.fr/neuroergonomie-et-facteurs-humains-dcas?lang=en) of ISAE-Supaero (France) by the team under the supervision of [Frédéric Dehais](https://personnel.isae-supaero.fr/frederic-dehais/). 

📚 You can cite our work using the DOI on top or in the [`CITATION.cff`](https://github.com/ludovicdmt/FlickersOnTop/blob/main/CITATION.cff) file. 

⚠️ This project is still under-development and this is a beta version. If you experience some issue/bug, you can share it with us in the [GitHub issues webpage](https://github.com/ludovicdmt/FlickersOnTop/issues) and we would be very happy to work on it ! It can also be used to suggest further improvements. 

![FlickerOnTop](https://user-images.githubusercontent.com/19227268/215775405-28b916f5-372c-40a2-89b9-6d5bd565292f.PNG)

👩‍💻 How to use it
---------------
📥 Download the compiled version in the [Release](https://github.com/ludovicdmt/FlickersOnTop/releases) section and then click on `Interface2App.exe`. It will launch the GUI and allows you to create some flickers.   

1. Click on `New` to add a flicker. You can set a name, X and Y positions, width, height, frequency and phase (for SSVEP). To place and set dimensions of flickers you can use the information of the `Mouse position`, on the bottom right of the interface. It is also possible to change the color of the flicker (default to white) and the amplitude depth of the flicker (default from 0 to 100%).  

2. By default it would use a a black background but you can put the flickers on top of your current interface by a click in the checkbox: `Add a Black Screen background when running`.

3. You can also load an image (as for now only in `BMP` format, you can find some [online converter](https://image.online-convert.com/fr/convertir-en-bmp)) to replace the rectangle by an image (a checkerboard for instance).  

🏃 You can then click on `TEST` to make the flicker run for 30s, you can stop it before with an `Escape` press. If you click on `RUN` it will run until you press `Escape`. One LSL marker per flicker (with the corresponding information) is send at the start to synchronize the EEG recording.   

💾 The configuration files of the flickers is saved automatically in `XML` format. You can use it to inspect later your design or to load it in the GUI (`Import` button) to run the same configuraton.

⏯️ Sequencing
---------------
You can click on `Sequencing` to define a specific sequence of pauses and flickering for each stimulus. Then, every time a flicker stops or restarts a new LSL marker is sent. This functionality is still under development so the ergonomy could be improved.  
Please send us your [recommendation](https://github.com/ludovicdmt/FlickersOnTop/issues)! 🙏 

📁 Organization of the repo
---------------
The code is divided in two parts : 

* Interface2App: A GUI to design and place flickers. It creates an XML files that would be used as input for the second part.
* VisualSimuli: Use directly the XML file without GUI to create and animate flickers.

🗜️ Make your own build
---------------
 If you want to build the software by our self instead of using the `.exe`, here are some hints: 
- All files are coded in C# and runing in VisualStudio.  
- Download VisualStudio 2022: <https://visualstudio.microsoft.com/vs/>  
- There are 2 application extension: 
    * `SDL2.dll`
    * `LSL.dll`
- For most cases, it will automatically be added to project when you do download. However, if you meet problem like [BadImageFormatException: An attempt was made to load a program with an incorrect format. (Exception from HRESULT: 0x8007000B)] when trying to complite program
You should make sure the two extentions are in x64 bits version and you can download them there:

 - https://www.libsdl.org/download-2.0.php
 - https://github.com/sccn/liblsl/releases

 
And then, you must go to project's properties -> Build -> change Platform target to `x64`.
![Select plateform](https://user-images.githubusercontent.com/102971418/176470491-9454a7da-a7c8-4472-b526-578e37f3c928.png)

🆘 Help
--------------

If you experience issues during installation and/or use of this program, you can post a new issue on the [GitHub issues webpage](https://github.com/ludovicdmt/FlickersOnTop/issues). We will reply to you as soon as possible and are very interested in to the improvement of this tool !  
