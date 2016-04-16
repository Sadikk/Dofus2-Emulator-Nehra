using System;
using System.IO;

namespace Stump.Core.IO.Watchers
{
    public class FileWatcher : FileSystemWatcher
    {
        private DateTime m_lastModification;
        public WatchAction Action
        {
            get;
            set;
        }
        public string FullPath
        {
            get;
            set;
        }
        public WatcherType Type
        {
            get;
            set;
        }
        public bool Watching
        {
            get;
            set;
        }

        public FileWatcher(string path, WatcherType type, WatchAction action)
            : base(System.IO.Path.GetDirectoryName(path))
        {
            this.FullPath = System.IO.Path.GetFullPath(path);
            this.Type = type;
            this.Action = action;
            if (path.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()) || path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                base.IncludeSubdirectories = true;
            }
            base.Deleted += new FileSystemEventHandler(this.OnDeleted);
            base.Created += new FileSystemEventHandler(this.OnCreated);
            base.Changed += new FileSystemEventHandler(this.OnChanged);
            base.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!(System.IO.Path.GetFullPath(e.FullPath) != this.FullPath) && this.Watching && this.Type == WatcherType.Modification && (DateTime.Now - this.m_lastModification).TotalMilliseconds > 100.0)
            {
                this.m_lastModification = DateTime.Now;
                base.EnableRaisingEvents = false;
                this.Watching = false;
                this.Action(this.FullPath);
                this.Watching = true;
                base.EnableRaisingEvents = true;
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == this.FullPath && this.Watching && this.Watching && this.Type == WatcherType.Creation)
            {
                this.Watching = false;
                this.Action(this.FullPath);
                this.Watching = true;
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (!(e.FullPath != this.FullPath) && this.Watching && this.Watching && this.Type == WatcherType.Deletion)
            {
                this.Watching = false;
                this.Action(this.FullPath);
                this.Watching = true;
            }
        }
    }
}