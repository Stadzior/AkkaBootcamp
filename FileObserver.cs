using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Akka.Actor;
using WinTail.Actors;
using WinTail.Messages;

namespace WinTail
{
    /// <summary>
    /// Turns <see cref="FileSystemWatcher"/> events about a specific file into
    /// messages for <see cref="TailActor"/>.
    /// </summary>
    public class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;
        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _fileDir = Path.GetDirectoryName(absoluteFilePath);
            _fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        /// <summary>
        /// Begin monitoring file.
        /// </summary>
        public void Start()
        {
            // Need this for Mono 3.12.0 workaround
            // uncomment next line if you're running on Mono!
            // Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "enabled");

            // make watcher to observe our specific file
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            // watch our file for changes to the file name,
            // or new messages being written to file

            // assign callbacks for event types
            _watcher.Changed += OnFileChanged;
            _watcher.Error += OnFileError;

            // start watching
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stop monitoring file.
        /// </summary>
        public void Dispose()
            => _watcher.Dispose();

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file error events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnFileError(object sender, ErrorEventArgs e)
            => _tailActor.Tell(new FileError(_fileNameOnly, e.GetException().Message), ActorRefs.NoSender);

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file change events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
                // here we use a special ActorRefs.NoSender
                // since this event can happen many times,
                // this is a little microoptimization
                _tailActor.Tell(new FileWrite(e.Name), ActorRefs.NoSender);
        }
    }
}
