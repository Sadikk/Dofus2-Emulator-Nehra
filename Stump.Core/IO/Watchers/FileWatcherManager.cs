using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stump.Core.IO.Watchers
{
    public static class FileWatcherManager
    {
        private static readonly List<FileWatcher> Watchers = new List<FileWatcher>();

        public static void Watch(string path, WatcherType watcherType, Action action)
        {
            FileWatcherManager.Watch(path, watcherType, delegate(string entry)
            {
                action();
            });
        }

        public static void Watch(string path, WatcherType watcherType, WatchAction action)
        {
            FileWatcher item = new FileWatcher(Path.GetFullPath(path), watcherType, action)
            {
                Watching = true
            };
            FileWatcherManager.Watchers.Add(item);
        }

        public static void StopWatching(string path)
        {
            FileWatcher[] array = (
                from entry in FileWatcherManager.Watchers
                where entry.FullPath == Path.GetFullPath(path)
                select entry).ToArray<FileWatcher>();
            FileWatcher[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                FileWatcher fileWatcher = array2[i];
                fileWatcher.Watching = false;
                fileWatcher.Dispose();
                FileWatcherManager.Watchers.Remove(fileWatcher);
            }
        }
    }
}