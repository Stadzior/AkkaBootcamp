using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Akka.Actor;
using WinTail.Messages;

namespace WinTail.Actors
{    
    /// <summary>
    /// Monitors the file at <see cref="_filePath"/> for changes and sends
    /// file updates to console.
    /// </summary>
    public class TailActor : UntypedActor
    {
        private readonly string _filePath;
        private readonly IActorRef _reporterActor;
        private FileObserver _observer;
        private Stream _fileStream;
        private StreamReader _fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case FileWrite _:
                {
                    // move file cursor forward
                    // pull results from cursor to end of file and write to output
                    // (this is assuming a log file type format that is append-only)
                    var text = _fileStreamReader.ReadToEnd();
                    if (!string.IsNullOrEmpty(text))
                    {
                        _reporterActor.Tell(text);
                    }

                    break;
                }
                case FileError error:
                {
                    var fe = error;
                    _reporterActor.Tell($"Tail error: {fe.Reason}");
                    break;
                }
                case InitialRead read:
                {
                    var ir = read;
                    _reporterActor.Tell(ir.Text);
                    break;
                }
            }
        }

        /// <summary>
        /// Initialization logic for actor that will tail changes to a file.
        /// </summary>
        protected override void PreStart()
        {
            // start watching file for changes
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            // open the file stream with shared read/write permissions
            // (so file can be written to while open)
            _fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(_fileStream, Encoding.UTF8);

            // read the initial contents of the file and send it to console as first msg
            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(_filePath, text));
        }

        /// <summary>
        /// Cleanup OS handles for <see cref="_fileStreamReader"/> 
        /// and <see cref="FileObserver"/>.
        /// </summary>
        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            _fileStreamReader.Close();
            _fileStreamReader.Dispose();
            base.PostStop();
        }
    }
}
