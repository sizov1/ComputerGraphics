﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab_work_1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourseImage, int x, int y);

        virtual public Bitmap processImage (Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            for(int i = 0; i < sourseImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourseImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp (int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
    }

    //Точечные фильтры

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int intensity = Convert.ToInt32(sourceColor.R * 0.299 + sourceColor.G * 0.587 + sourceColor.B * 0.114);
            intensity = Clamp(intensity, 0, 255);
            Color resultColor = Color.FromArgb(intensity,
                                               intensity,
                                               intensity);
            return resultColor;
        }
    }
    
    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            double k = 60.0;
            double intensity = 0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B;

            Color resultColor = Color.FromArgb(Clamp((int)(intensity + 2.0 * k), 0, 255), 
                                                  Clamp((int)(intensity + k * 0.5), 0, 255), 
                                                    Clamp((int)(intensity - 1.0 * k), 0, 255));
            return resultColor;
        }
    }
    
    class TransferFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            if (x + 50 > sourseImage.Width - 1)
                return Color.Transparent;
            else
            {
                Color resultColor = sourseImage.GetPixel(x + 50, y);
                return resultColor;
            }
        }
    }
    
    class TurnFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int x0 = (int)(sourseImage.Width / 2), y0 = (int)(sourseImage.Height / 2);
            double w = Math.PI / 4.0;
            int xx = (int)((x - x0) * Math.Cos(w) - (y - y0) * Math.Sin(w) + x0);
            int yy = (int)((x - x0) * Math.Sin(w) + (y - y0) * Math.Cos(w) + y0);

            if((xx < sourseImage.Width - 1)&&(yy < sourseImage.Height - 1)&&(xx > 0)&&(yy > 0))
            {
                Color resultColor = sourseImage.GetPixel(xx, yy);
                return resultColor;
            }
            return Color.Transparent;
        }
    }
    
    class WavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int xx = (int)(x + 20*Math.Sin(2.0 * Math.PI * y / 60.0));
            int yy = y;

            if ((xx < sourseImage.Width - 1) && (yy < sourseImage.Height - 1) && (xx > 0) && (yy > 0))
            {
                Color resultColor = sourseImage.GetPixel(xx, yy);
                return resultColor;
            }
            return Color.Transparent;
        }
    }

    class GrayWorldFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }

        public override Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);

            double R = 0.0, G = 0.0, B = 0.0;
            double Avg = 0.0;

            Color temp;
            for (int i = 0; i < sourseImage.Width; i++)
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    temp = sourseImage.GetPixel(i, j);
                    R += temp.R; G += temp.G; B += temp.B;
                }
            R = (double)R / (sourseImage.Width * sourseImage.Height);
            G = (double)G / (sourseImage.Width * sourseImage.Height);
            B = (double)B / (sourseImage.Width * sourseImage.Height);
            Avg = (R + G + B) / 3.0d;

            Color sourceColor, resultColor;

            for (int i = 0; i < sourseImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourceColor = sourseImage.GetPixel(i, j);
                    resultColor = Color.FromArgb(Clamp((int)(sourceColor.R * Avg / R), 0, 255),
                                                Clamp((int)(sourceColor.G * Avg / G), 0, 255),
                                                  Clamp((int)(sourceColor.B * Avg / B), 0, 255));

                    resultImage.SetPixel(i, j, resultColor);
                }
            }
            return resultImage;
        }
    }

    class PerfectReflectorFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }

        public override Bitmap processImage(Bitmap sourseImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);

            double Rm = 0.0, Gm = 0.0, Bm = 0.0;

            Color temp;
            for (int i = 0; i < sourseImage.Width; i++)
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    temp = sourseImage.GetPixel(i, j);
                    if (temp.R > Rm)
                        Rm = temp.R;
                    if (temp.G > Gm)
                        Gm = temp.G;
                    if (temp.B > Bm)
                        Bm = temp.B;
                }

            Color sourceColor, resultColor;

            for (int i = 0; i < sourseImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourceColor = sourseImage.GetPixel(i, j);
                    resultColor = Color.FromArgb(Clamp((int)(sourceColor.R * 255 / Rm), 0, 255),
                                                    Clamp((int)(sourceColor.G * 255 / Gm), 0, 255),
                                                        Clamp((int)(sourceColor.B * 255 / Bm), 0, 255));

                    resultImage.SetPixel(i, j, resultColor);
                }
            }
            return resultImage;
        }
    }


    //Матричные фильтры

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float [,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for(int l = -radiusY; l <= radiusY; l++)
                for(int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);
                    Color neighColor = sourseImage.GetPixel(idX, idY);
                    resultR += neighColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
        }
    }
    
    class GaussianFilter : MatrixFilter
    {
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
        public void createGaussianKernel(int radius, float sigma)
        {
            //определяем размер ядра
            int size = 2 * radius + 1;
            //создаем ядро фильтра
            kernel = new float[size, size];
            //коэффициент нормировки ядра
            float norm = 0;
            //рассчитываем ядро линейного фильтра
            for(int i = -radius; i <= radius; i++)
                for(int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            //нормируем ядро
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }
    
    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    if((i == 1)&&(j == 1))
                    {
                        kernel[i, j] = 9.0f;
                    }
                    else
                        kernel[i, j] = -1.0f;
                }
        }
    }
    
    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter()
        {
            int sizeX = 9;
            int sizeY = 9;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    if(i == j)
                        kernel[i, j] = 1.0f / (float)(sizeY);
                    else
                        kernel[i, j] = 0.0f;
                }
        }
    }

}

  