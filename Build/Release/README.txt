Rahul Vaidya
Terrence Giang
NEATRacing

This project implements an evolving NEAT neural network on top of the Racing Game XNA sample.
Uses the SharpNEAT framework to implement the network.

Quick start information for CHECKING OUT SOURCE:

1.  Install VS2005 with C#
2.  Update to Service Pack 1
3.  Install XNA Game Studio 2.0
4.  Create a new XNA solution called "NEATRacing" using the "Racing Game Windows Starter Kit".  Exit out of Visual Studio.
5.  Go into the project folder for NEATRacing (My Documents\Visual Studio 2005\Projects\NEATRacing\NEATRacing\RacingGame) and delete all of the folders except for "Content"
6.  Get TortoiseSVN: http://tortoisesvn.tigris.org/
7.  Go to the NEATRacing project folder (My Documents\Visual Studio 2005\Projects\NEATRacing), right click, and click SVN Checkout.
8.  Use "http://neatracing.googlecode.com/svn/trunk/" as the URL of the repository.
8b. Alternatively, if you're using svn command line, use "svn checkout http://neatracing.googlecode.com/svn/trunk/ neatracing-read-only".
9.  That should be it, it should download all of the relevant project stuff.

Guide for RUNNING SHARPNEAT BINARY:
1.  These components are required for running the project:
       a. .NET Framework 2.0 http://www.microsoft.com/downloads/details.aspx?FamilyID=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5&displaylang=en
       b. DirectX 9 http://www.microsoft.com/downloads/details.aspx?familyid=2da43d38-db71-4c1b-bc6a-9b6652cd92a3&displaylang=en
       c. XNA 2.0 Redistributable http://www.microsoft.com/downloads/details.aspx?FamilyID=15fb9169-4a25-4dca-bf40-9c497568f102&displaylang=en
2.  Run SharpNEAT.exe to run the NEAT GUI.  There are two "experiments":
	a.  "Training" - USE THIS OPTION! This contains a fixed training set for the network, obtained from driving around on the track.
	b.  "Interactive" - WILL NOT WORK! The NEAT neural network obtains its parameters straight from the Racing Game itself.  Useful after training data is generated.
3.  If you want to initialize a genome as the seed genome, go to Initialize Population -> Load -> Load Seed Genome.  There are genomes in the genomes directory.
3b. Alternatively, you can start from a fresh "empty genome", using Initialize Population -> Auto Generate.
4.  Click on Page 2 and modify the NEAT run parameters to your liking.  For our executions, we increased the max species threshold to 1000, and the add neural probability to 0.01
    Weight fixing was also enabled
5.  If you want the application to save the best genome so far each time, check the box "Save Genome on Improvement" and enter a filename prefix (ie. "genomes/4_" to follow our naming convention).
    This will save the optimal genome for each run.
6.  That's it!  Click Start/Continue and it will run.  You can select multiple visualization options in the Visualization menu.

Guide for RUNNING RACINGGAME BINARY
1.  The above components are required here as well.
2.  Run RacingGame.exe to run the game.  It also spawns a NEAT GUI as a thread with shared memory.
	The "Interactive" experiment mode is USABLE during this.  It requires a connected RacingGame instance to run, and the previous steps only launched SharpNEAT.
3.  Select a car in the game, then follow the above steps to start the network.  Once the network is started select a track to start the game.
4.  To enable the car to be controlled by the neural network, press F2.
5.  Enjoy watching the car crash into the rails, as the training set (and the starter set) are not sufficient enough for intelligent driving.  Suffers heavily from local maxima.

