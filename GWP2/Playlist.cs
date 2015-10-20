using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GWP2
{
    public class Playlist
    {
        private bool _shuffle = false;
        private bool _repeat = false;
        private bool _paused = false;
        private bool isPlaying = false;

        public bool IsPlaying
        {
            get { return this.isPlaying; }
            set { this.isPlaying = value;}
        }

        private ObservableCollection<Media> medias;
        private MediaElement player;


        public Playlist(MediaElement mediaElement)
        {
            this.medias = new ObservableCollection<Media>();
            this.player = mediaElement;
        }

        public int ActiveIndex { get; set; }

        public ObservableCollection<Media> Medias
        {
            get { return this.medias; }
        }

        public void AddMedia(string path)
        {
            if (System.IO.File.Exists(path))
            {
                this.medias.Add(new Media(System.IO.Path.GetFileNameWithoutExtension(path), path));
                
            }
        }

        public void DeleteSong() 
        {
            if (this.medias.Count > this.ActiveIndex)
            {
                this.medias.RemoveAt(this.ActiveIndex);
            }
        }

        public void Play()
        {
            if (!this._paused && this.isPlaying)
            {
                this.Pause();
            }
            else
            {
                if (!this._paused && this.medias.Count > this.ActiveIndex)
                {
                    Media mediaToPlay = this.medias[this.ActiveIndex];
                    this.player.Source = new Uri(mediaToPlay.Path);
                }
                this.player.Play();
                this._paused = false;
                this.isPlaying = true;
                
            }
        }

        public void Pause()
        {
            this._paused = true;
            this.isPlaying = false;
            this.player.Pause();
        }

        public void Stop()
        {
            this.player.Stop();
            this.player.Position = new TimeSpan(0);
            this.isPlaying = false;
            this._paused = false;
        }
        public void Next()
        {
            this.Stop();
            this.ActiveIndex++;
            if (this.ActiveIndex >= this.medias.Count)
            {
                this.ActiveIndex = 0;
            }
            this.Play();
        }

        public void Previous()
        {
            this.Stop();
            this.ActiveIndex--;
            if (this.ActiveIndex < 0)
            {
                this.ActiveIndex = this.medias.Count - 1;
            }
            this.Play();
        }

        public void ToggleRepeat()
        {
            this._repeat = !this._repeat;
        }

        public void ToggleShuffle()
        {
            this._shuffle = !this._shuffle;
        }

        public void Clear()
        {
            this.player.Stop();
            medias.Clear();
            ActiveIndex = 0;
        }
    }
}