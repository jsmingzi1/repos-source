using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenRecorderLib;

namespace ScreenRecorder
{
    class SRecorder
    {
        Recorder _rec;
        Stream _outStream;
        public void CreateRecording(string path)
        {
            _rec = Recorder.CreateRecorder();
            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;

            _rec.Record(path);
            //..Or to a stream
            //_outStream = new MemoryStream();
            //_rec.Record(_outStream);
            
        }
        public void EndRecording()
        {
            
            _rec.Stop();
            _rec.Dispose();
        }
        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            Console.WriteLine("Rec_OnRecordingComplete");
            //Get the file path if recorded to a file
            string path = e.FilePath;
            //or do something with your stream
            //... something ...
            _outStream?.Dispose();
        }
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            Console.WriteLine("Rec_OnRecordingFailed");
            string error = e.Error;
            _outStream?.Dispose();
        }
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            Console.WriteLine("Rec_OnStatusChanged");
            RecorderStatus status = e.Status;
        }
    }
}
