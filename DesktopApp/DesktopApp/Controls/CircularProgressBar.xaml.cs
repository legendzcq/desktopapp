//  --------------------------------
//  Copyright (c) Huy Pham. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.opensource.org/licenses/ms-pl.html
//  ---------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularProgressBar"/> class.
        /// </summary>
        public CircularProgressBar()
        {
            InitializeComponent();
        }

        private void CircularProgressBar_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var s0 = Resources["MetroLoadingAnimation"] as Storyboard;
            var s1 = Resources["MetroLoadingAnimation1"] as Storyboard;
            var s2 = Resources["MetroLoadingAnimation2"] as Storyboard;
            var s3 = Resources["MetroLoadingAnimation3"] as Storyboard;
            var s4 = Resources["MetroLoadingAnimation4"] as Storyboard;
            if (Visibility == Visibility.Visible)
            {
                s0.Begin();
                s1.Begin();
                s2.Begin();
                s3.Begin();
                s4.Begin();
            }
            else
            {
                s0.Stop();
                s1.Stop();
                s2.Stop();
                s3.Stop();
                s4.Stop();
            }
        }
    }
}