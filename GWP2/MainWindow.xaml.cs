﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GEMMP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Playlist brain;
        private bool _isDragging = false;
        TimeSpan _position;
        private DispatcherTimer _timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(ticktock);
            _timer.Start();
            this.DataContext = this;
            this.brain = new Playlist(vplayer);
            btnPlay.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        public void ticktock(object sender, EventArgs e)
        {
            if (!_isDragging)
            {
                pgbProgress.Value = vplayer.Position.TotalSeconds;
            }

        }


        public void mnuLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        this.brain.AddMedia(filename);
                    }
                }
                btnPlay.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnStop.IsEnabled = true;
            }
        }

        public void vplayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            brain.Next();
        }



        public void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (brain.IsPlaying == true)
            {
                btnPlay.Content = FindResource("Play");
                brain.Play();
            }
            else
            {
                btnPlay.Content = FindResource("Pause");
                brain.Play();
            }
        }

        public void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            this.brain.Previous();
            //lstPlaylist.SelectedIndex = brain.ActiveIndex;
        }

        public void btnNext_Click(object sender, RoutedEventArgs e)
        {
            this.brain.Next();
            //lstPlaylist.SelectedIndex = brain.ActiveIndex;
        }

        public ObservableCollection<Media> Medias
        {
            get { return this.brain.Medias; }
        }

        private void Player_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            vplayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            brain.Stop();
            btnPlay.Content = FindResource("Play");

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            brain.Stop();
            vplayer.Source = null;
            btnPlay.Content = FindResource("Play");
            btnPlay.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnPrevious.IsEnabled = false;
            btnStop.IsEnabled = false;
            vplayer.Source = null;
            brain.Clear();
            brain.Medias.Clear();
        }

        private void lstPlaylist_Selected(object sender, RoutedEventArgs e)
        {
            //lstPlaylist.SelectedIndex = brain.ActiveIndex;
        }

        private void vplayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            _position = vplayer.NaturalDuration.TimeSpan;
            pgbProgress.Minimum = 0;
            pgbProgress.Maximum = _position.TotalSeconds;
        }

        private void pgbProgress_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (vplayer.Source != null)
            {
                long videoPositon = Convert.ToInt64(vplayer.NaturalDuration.TimeSpan.Ticks * (pgbProgress.Value / pgbProgress.Maximum));
                vplayer.Position = new TimeSpan(videoPositon);
            }

            _isDragging = false;
        }

        private void Player_MouseEnter(object sender, MouseEventArgs e)
        {
            pgbProgress.Visibility = System.Windows.Visibility.Visible;
            pgbVolume.Visibility = System.Windows.Visibility.Visible;
            btnPlay.Visibility = System.Windows.Visibility.Visible;
            btnPrevious.Visibility = System.Windows.Visibility.Visible;
            btnStop.Visibility = System.Windows.Visibility.Visible;
            btnNext.Visibility = System.Windows.Visibility.Visible;
        }
        private void Player_MouseLeave(object sender, MouseEventArgs e)
        {
            pgbProgress.Visibility = System.Windows.Visibility.Hidden;
            pgbVolume.Visibility = System.Windows.Visibility.Hidden;
            btnPlay.Visibility = System.Windows.Visibility.Hidden;
            btnPrevious.Visibility = System.Windows.Visibility.Hidden;
            btnStop.Visibility = System.Windows.Visibility.Hidden;
            btnNext.Visibility = System.Windows.Visibility.Hidden;
        }

        //private void pgbProgress_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    pgbProgress.Height = 10;
        //}

        //private void pgbProgress_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    pgbProgress.Height = 3;
        //}

        private void pgbProgress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            double mousePosition = e.GetPosition(pgbProgress).X;
            setProgressBarPosition(mousePosition);
            _isDragging = true;

        }

        private void setProgressBarPosition(double mousePosition)
        {
            if (vplayer.Source != null)
            {
                double progressBarPosition = mousePosition / pgbProgress.ActualWidth * pgbProgress.Maximum;

                pgbProgress.Value = progressBarPosition;
            }
        }

        private void File_DropEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filename in files)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        this.brain.AddMedia(filename);
                    }
                }
                btnPlay.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnStop.IsEnabled = true;
            }
        }

        public void mnuClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MediaFilter_DragOver(object sender, DragEventArgs e)
        {
            bool dropEnabled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                foreach (string filename in filenames)
                {
                    if (System.IO.Path.GetExtension(filename).ToUpperInvariant() == ".MP3"
                        || System.IO.Path.GetExtension(filename).ToUpperInvariant() == ".MKV"
                        || System.IO.Path.GetExtension(filename).ToUpperInvariant() == ".MP4"
                        || System.IO.Path.GetExtension(filename).ToUpperInvariant() == ".FLV"
                        || System.IO.Path.GetExtension(filename).ToUpperInvariant() == ".FLAC")
                    {
                        dropEnabled = true;
                    }
                    else
                    {
                        dropEnabled = false;
                    }

                    if (!dropEnabled)
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                }
            }
        }





    }
}