using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 *  The PixImage class represents an image, which is a rectangular grid of
 *  color pixels.  Each pixel has red, green, and blue intensities in the range
 *  0...255.  Descriptions of the methods you must implement appear below.
 *  They include a constructor of the form
 *
 *      public PixImage(int width, int height);
 *
 *  that creates a black (zero intensity) image of the specified width and
 *  height.  Pixels are numbered in the range (0...width - 1, 0...height - 1).
 *
 *  All methods in this class must be implemented to complete Part I.
 */


namespace ImageProcessing
{
    public class PixImage
    {
        // image object
        private class Pixel
        {
            public short red;
            public short green;
            public short blue;
        }
        private int width;
        private int height;
        private Pixel[,] myImage;
        // creates myImage object with given width and height, and all pixels set to 0,0,0 (black)
        public PixImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            // creates new myImage
            this.myImage = new Pixel[width, height];
            for(int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    setPixel(x, y, 0, 0, 0);
                }
            }
        }
        // returns the width of the image.
        public virtual int Width
        {
            get
            {
                return this.width;
            }
        }
        // returns the height of the image.
        public virtual int Height
        {
            get
            {
                return this.height;
            }
        }
        // returns the red intensity of the pixel at coordinate (x, y).
        public virtual short getRed(int x, int y)
        {
            return myImage[x, y].red;
        }
        // does the same for green
        public virtual short getGreen(int x, int y)
        {
            return myImage[x, y].green;
        }
        // does the same for blue
        public virtual short getBlue(int x, int y)
        {
            return myImage[x, y].blue;
        }
        // sets the pixel at coordinate (x, y) to specified red, green, or blue value
        public virtual void setPixel(int x, int y, short red, short green, short blue)
        {
            if (myImage[x, y] == null)
            {   
                // if no values are passed, then no values are changed
                myImage[x, y] = new Pixel();
            }
            // selects the color at a given location, sets it equal to the new value passed in as a parameter
            this.myImage[x, y].red = red;
            this.myImage[x, y].green = green;
            this.myImage[x, y].blue = blue;
        }

        // returns a String representation of 'this' PixImage.  Not used in implementation, for debugging only.
        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for(int y = 0; y < height; y++)
        //    {
        //        for(int x = 0; x < width; x++)
        //        {
        //            sb.Append(myImage[x, y].red.ToString() + ":" + myImage[x, y].green.ToString() + ":" + myImage[x, y].blue.ToString() + "\n");
        //        }
        //    }
        //    return sb.ToString();
        //}

        private Pixel getBlurValue(PixImage img, int x, int y)
        {
            // create new image to work on
            Pixel blurP = new Pixel();
            // ID the neighbors:
            // set range start for x values
            int startx = (x - 1 != -1) ? x - 1 : x;
            // set range start for y values
            int starty = (y - 1 != -1) ? y - 1 : y;
            // set range end for x values
            int endx = (x + 1 < width) ? x + 1 : x;
            // set range end for y values
            int endy = (y + 1 < height) ? y + 1 : y;
            // set these to zero to begin
            int counter = 0;
            int redTotal = 0;
            int greenTotal = 0;
            int blueTotal = 0;
            // for each pixel, work through the array
            for(int i = starty; i <= endy; i++)
            {
                for(int j = startx; j <= endx; j++)
                {
                    // add the color value to the running total
                    redTotal += img.myImage[j, i].red;
                    greenTotal += img.myImage[j, i].green;
                    blueTotal += img.myImage[j, i].blue;
                    // iterate the counter
                    counter++;
                }
            }
            // average the color values
            blurP.red = Convert.ToInt16(redTotal / counter);
            blurP.green = Convert.ToInt16(greenTotal / counter);
            blurP.blue = Convert.ToInt16(blueTotal / counter);
            // return the averaged color values
            return blurP;
        }
        public virtual PixImage boxBlur(int numIterations)
        {
            // if no blur iterations are wanted, abort and return the image as-is
            if (numIterations <= 0) return this;
            // save the original image before messing with it
            PixImage prevBlur = this;
            // loop thru iterations as set by passed-in parameter
            for(int i = 0; i < numIterations; i++)
            {
                // create a new image for the blur loop to write to
                PixImage blurImage = new PixImage(width, height);
                // loop thru every pixel
                for(int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        // this function is defined above - it averages color values, which dictate how to blur the pixel
                        Pixel bPixel = getBlurValue(prevBlur, x, y);
                        // set new pixel values to match the blur values
                        blurImage.setPixel(x, y, bPixel.red, bPixel.green, bPixel.blue);
                    }
                }
                // sets the newly created blur image to the previous image, so if we loop again we will blur again, not re-blur the original iamge
                prevBlur = blurImage;
            }
            // return the blurred image after all iterations have run
            return prevBlur;
        }

        // this method came as-is and was not changed. It shifts the image to grayscale and restricts the value range to 0-255 (rgb)
        private static short mag2gray(long mag)
        {
            short intensity = (short)(30.0 * Math.Log(1.0 + (double)mag) - 256.0);
            // Make sure the returned intensity is in the range 0...255, regardless of the input value.
            if (intensity < 0)
            {
                intensity = 0;
            }
            else if (intensity > 255)
            {
                intensity = 255;
            }
            return intensity;
        }

        /// <summary>
        /// sobelEdges() applies the Sobel operator, identifying edges in "this"
        /// image.  The Sobel operator computes a magnitude that represents how
        /// strong the edge is.  We compute separate gradients for the red, blue, and
        /// green components at each pixel, then sum the squares of the three
        /// gradients at each pixel.  We convert the squared magnitude at each pixel
        /// into a grayscale pixel intensity in the range 0...255 with the logarithmic
        /// mapping encoded in mag2gray().  The output is a grayscale PixImage whose
        /// pixel intensities reflect the strength of the edges.
        ///   
        /// See http://en.wikipedia.org/wiki/Sobel_operator#Formulation for details.
        /// </summary>
        /// <returns> a grayscale PixImage representing the edges of the input image.
        /// Whiter pixels represent stronger edges. </returns>
        /// 

        // Matrix is the matrix used in the Sobol filter
        private long[] getGradValue(int[,] Matrix, int x, int y)
        {
            long red = 0;
            long green = 0;
            long blue = 0;
            // create array to store rgb values
            long[] pixel = new long[3];
            // set x and y value ranges
            int startx = (x - 1 != -1) ? x - 1 : x;
            int starty = (y - 1 != -1) ? y - 1 : y;
            int endx = (x + 1 < width) ? x + 1 : x;
            int endy = (y + 1 < width) ? y + 1 : y;
            // loop through the pixels, and through the Matrix matrix
            for (int i = startx, l = 0; i <= endx; i++, l++)
            {
                for (int j = starty, m = 0; j <= endy; j++, m++)
                {
                    red = red + (Matrix[l, m] * this.myImage[i, j].red);
                    green = green + (Matrix[l, m] * this.myImage[i, j].green);
                    blue = blue + (Matrix[l, m] * this.myImage[i, j].blue);
                }
            }
            // store the rgb values in the array created above. Instead of averaging the color values, 
            // we want to see how different the color value is from it's neighbor so we can judge the strength of the edge:
            // a large difference equals a strong edge (large change in color value), while a small difference equals 
            // a weak edge (no change in color value)
            pixel[0] = red;
            pixel[1] = green;
            pixel[2] = blue;
            // return the array
            return pixel;
        }

        public virtual PixImage sobelEdges()
        {
            // create new image to work with
            PixImage img = new PixImage(width, height);
            // these are the matrices used by the Sobel filter (from https://en.wikipedia.org/wiki/Sobel_operator#Formulation)
            int[,] xMatrix =
            {
                {1, 0, -1 },
                {2, 0, -2 },
                {1, 0, -1 }
            };
            int[,] yMatrix =
            {
                {1, 2, 1 },
                {0, 0, 0 },
                {-1, -2, -1 }
            };
            // x result after the filter
            long[] gx;
            // y result after the filter
            long[] gy;
            // energy is calculated by summing the squares of each color value gradient for the pixel (both x and y); 
            // that value is specific to each pixel
            long[,] energy = new long[width, height];
            // loop through the pixels
            for(int x = 0; x < width; x++)
            {
                for(int y =0; y < height; y++)
                {
                    // get the gradient values for x and y for each pixel
                    gx = getGradValue(xMatrix, x, y);
                    gy = getGradValue(yMatrix, x, y);
                    // calculate the energy for the pixel
                    energy[x, y] = (gx[0] * gx[0]) + (gy[0] * gy[0]) + (gx[1] * gx[1]) + (gy[1] * gy[1]) + (gx[2] * gx[2]) + (gy[2] * gy[2]);
                    // set the pixel values by converting energy to grayscale (r=g=b) using the given method mag2gray 
                    img.setPixel(x, y, mag2gray(energy[x, y]), mag2gray(energy[x, y]), mag2gray(energy[x, y]));
                }
            }
            // return the image
            return img;
        }

        
        // CODE BELOW THIS POINT CAME AS-IS AND WAS NOT CHANGED OR USED

        /// <summary>
        /// TEST CODE:  YOU DO NOT NEED TO FILL IN ANY METHODS BELOW THIS POINT.
        /// You are welcome to add tests, though.  Methods below this point will not
        /// be tested.  This is not the autograder, which will be provided separately.
        /// </summary>


        /// <summary>
        /// doTest() checks whether the condition is true and prints the given error
        /// message if it is not.
        /// </summary>
        /// <param name="b"> the condition to check. </param>
        /// <param name="msg"> the error message to print if the condition is false. </param>
        private static void doTest(bool b, string msg)
        {
            if (b)
            {
                Console.WriteLine("Good.");
            }
            else
            {
                Console.Error.WriteLine(msg);
            }
        }

        /// <summary>
        /// array2PixImage() converts a 2D array of grayscale intensities to
        /// a grayscale PixImage.
        /// </summary>
        /// <param name="pixels"> a 2D array of grayscale intensities in the range 0...255. </param>
        /// <returns> a new PixImage whose red, green, and blue values are equal to
        /// the input grayscale intensities. </returns>
        private static PixImage array2PixImage(int[][] pixels)
        {
            int width = pixels.Length;
            int height = pixels[0].Length;
            PixImage image = new PixImage(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.setPixel(x, y, (short)pixels[x][y], (short)pixels[x][y], (short)pixels[x][y]);
                }
            }

            return image;
        }

        /// <summary>
        /// equals() checks whether two images are the same, i.e. have the same
        /// dimensions and pixels.
        /// </summary>
        /// <param name="image"> a PixImage to compare with "this" PixImage. </param>
        /// <returns> true if the specified PixImage is identical to "this" PixImage. </returns>
        public virtual bool Equals(PixImage image)
        {
            int width = Width;
            int height = Height;

            if (image == null || width != image.Width || height != image.Height)
            {
                return false;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!(getRed(x, y) == image.getRed(x, y) && getGreen(x, y) == image.getGreen(x, y) && getBlue(x, y) == image.getBlue(x, y)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// main() runs a series of tests to ensure that the convolutions (box blur
        /// and Sobel) are correct.
        /// </summary>
        public static void Main(string[] args)
        {
            // Be forwarned that when you write arrays directly in C# as below,
            // each "row" of text is a column of your image--the numbers get
            // transposed.
            PixImage image1 = array2PixImage(new int[][]
            {
        new int[] {0, 10, 240},
        new int[] {30, 120, 250},
        new int[] {80, 250, 255}
            });
            Console.WriteLine("Testing getWidth/getHeight on a 3x3 image.  " + "Input image:");
            Console.Write(image1);
            doTest(image1.Width == 3 && image1.Height == 3, "Incorrect image width and height.");

            Console.WriteLine("Testing blurring on a 3x3 image.");
            doTest(image1.boxBlur(1).Equals(array2PixImage(new int[][]
            {
        new int[] {40, 108, 155},
        new int[] {81, 137, 187},
        new int[] {120, 164, 218}
            })), "Incorrect box blur (1 rep):\n" + image1.boxBlur(1));
            doTest(image1.boxBlur(2).Equals(array2PixImage(new int[][]
            {
        new int[] {91, 118, 146},
        new int[] {108, 134, 161},
        new int[] {125, 151, 176}
            })), "Incorrect box blur (2 rep):\n" + image1.boxBlur(2));
            doTest(image1.boxBlur(2).Equals(image1.boxBlur(1).boxBlur(1)), "Incorrect box blur (1 rep + 1 rep):\n" + image1.boxBlur(2) + image1.boxBlur(1).boxBlur(1));

            Console.WriteLine("Testing edge detection on a 3x3 image.");
            doTest(image1.sobelEdges().Equals(array2PixImage(new int[][]
            {
        new int[] {104, 189, 180},
        new int[] {160, 193, 157},
        new int[] {166, 178, 96}
            })), "Incorrect Sobel:\n" + image1.sobelEdges());


            PixImage image2 = array2PixImage(new int[][]
            {
        new int[] {0, 100, 100},
        new int[] {0, 0, 100}
            });
            Console.WriteLine("Testing getWidth/getHeight on a 2x3 image.  " + "Input image:");
            Console.Write(image2);
            doTest(image2.Width == 2 && image2.Height == 3, "Incorrect image width and height.");

            Console.WriteLine("Testing blurring on a 2x3 image.");
            doTest(image2.boxBlur(1).Equals(array2PixImage(new int[][]
            {
        new int[] {25, 50, 75},
        new int[] {25, 50, 75}
            })), "Incorrect box blur (1 rep):\n" + image2.boxBlur(1));

            Console.WriteLine("Testing edge detection on a 2x3 image.");
            doTest(image2.sobelEdges().Equals(array2PixImage(new int[][]
            {
        new int[] {122, 143, 74},
        new int[] {74, 143, 122}
            })), "Incorrect Sobel:\n" + image2.sobelEdges());

            Console.ReadLine();
        }

    }
}
