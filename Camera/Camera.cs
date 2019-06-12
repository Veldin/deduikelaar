﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LogSystem;
using System.Windows;

namespace CameraSystem
{
    public class Camera
    {
        Canvas gameCanvas;                  // The Canvas that is used in the engine
        Window mainWindow;                  // The Window where the gameCanvas is in

        private float fromLeft;             // The fromLeft position of the camera
        private float fromTop;              // The fromTop posistion of the camera
        
        private float prevFromLeft;         // The previous fromLeft position, the one before the current one
        private float prevFromTop;          // The previous fromTop position, the one before the current one

        private float width;               // The estimated width of the camera (can be between 0 and 15 units off)
        private float height;              // The estimated height of the camera (can be between 0 and 15 units off)

        public Camera(Canvas gameCanvas, Window mainWindow)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = mainWindow;
            fromTop = 0;
            fromLeft = 0;
            prevFromLeft = 0;
            prevFromTop = 0;

            width = 0;
            height = 0;

            GenerateHeightAndWidth();
        }
        
        /*****************************************************************
         * GETTERS
         * *************************************************************/

        /// <summary>
        /// Get the fromLeft value
        /// </summary>
        /// <returns></returns>
        public float GetFromLeft()
        {
            return fromLeft;
        }

        /// <summary>
        /// Get the fromTop value
        /// </summary>
        /// <returns></returns>
        public float GetFromTop()
        {
            return fromTop;
        }

        /// <summary>
        /// Get the previous fromLeft value
        /// </summary>
        /// <returns></returns>
        public float GetPrevFromLeft()
        {
            return prevFromLeft;
        }

        /// <summary>
        /// Get the previous fromTop value
        /// </summary>
        /// <returns></returns>
        public float GetPrevFromTop()
        {
            return prevFromTop;
        }

        /// <summary>
        /// Get the width
        /// </summary>
        /// <returns></returns>
        public float GetWidth()
        {
            return width;
        }

        /// <summary>
        /// Get the height
        /// </summary>
        /// <returns></returns>
        public float GetHeight()
        {
            return height;
        }
         
        /*****************************************************************
         * SETTERS
         * **************************************************************/

         /// <summary>
         /// Set the fromLeft value
         /// </summary>
         /// <param name="value"></param>
        public void SetFromLeft(float value)
        {
            prevFromLeft = fromLeft;
            fromLeft = value;
        }

        /// <summary>
        /// Set the fromTop value
        /// </summary>
        /// <param name="value"></param>
        public void SetFromTop(float value)
        {
            prevFromTop = fromTop;
            fromTop = value;
        }

        /*****************************************************************
         * Methods
         * **************************************************************/

        /// <summary>
        /// This method generates the height and the width of the camera based on the size of the window and canvas.
        /// Note that this method only works when the viewBox in xaml is on stretch="UniformToFill".
        /// The heigth and width generated by this function are estimates and can be off by 0 to 15 units
        /// </summary>
        public void GenerateHeightAndWidth()
        {
            // Get the height and width of the window without the borders
            double windowHeightBorderless = mainWindow.ActualHeight - SystemParameters.WindowCaptionHeight  - SystemParameters.ResizeFrameHorizontalBorderHeight * 2;
            double windowWidthBorderless = mainWindow.ActualWidth -  SystemParameters.ResizeFrameVerticalBorderWidth * 2;
            
            // Calc size factor between the window and the canvas
            double widthFac = windowWidthBorderless / gameCanvas.ActualWidth;
            double heightFac = windowHeightBorderless / gameCanvas.ActualHeight;

            if (Double.IsNaN(widthFac) || Double.IsNaN(heightFac))
            {
                width = 0;
                height = 0;
                Log.Warning("Camera Width or Height is NaN");
                return;
            }
            // Check which factor is the highest and set the heigth and width based on that
            if (widthFac > heightFac)
            {
                width = (float) gameCanvas.Width;
                height = (float) (gameCanvas.Height / widthFac * heightFac);
            } else
            {
                height = (float) gameCanvas.Height;
                width = (float) (gameCanvas.Width / heightFac * widthFac);
            }

            //// Useless debbuging
            //Log.Debug(width);
            //Log.Debug(height);
        }
    }
}
