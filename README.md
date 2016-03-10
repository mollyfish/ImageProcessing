# ImageProcessing


As part of the lecture offered by Kal from Kal academy, this project was provided as an exercise. I borrowed heavily from Vishnu's code (https://github.com/vishsram/ImageProcessing) so that I could follow along and understand the problem and one possible solution before mucking about in it myself.

Many thanks, Vishnu!

## Place image file in the correct directory:

Find an appropriate TIFF image to manipulate.  Place it in MyApp/bin/Debug (the same directory as MyApp.exe).  If you do not place your file here, even if you pass in a relative path so the program can locate your file, the program will error out because it will try to include the given path as part of the new filename, not a path. Since we are not supposed to alter Sobel.cs and Blur.cs, this is not fixable.

## To blur an image:

Open the Project > Properties window, and navigate to the Application tab.  Select the Blur file from the Startup Object menu.  This will tell VS to run the main() from that file.

To pass in command line argumants, navigate to the Debug tab.  Enter the arguments you want to use in the Command line argments field (ex: original.tiff 3).  The first argument is the filename and the second is the blur iterations.

The new image will be saved as blur_filename.tiff.

## To edge an image:

Open the Project > Properties window, and navigate to the Application tab.  Select the Sobel file from the Startup Object menu.  This will tell VS to run the main() from that file.

To pass in command line argumants, navigate to the Debug tab.  Enter the arguments you want to use in the Command line argments field (ex: original.tiff 3 edge).  The first argument is the filename and the second is the blur iterations, and the third (which can be anything) will trigger the edging program.

The new image will be saved as edge_filename.tiff.