using System;
using System.IO;
using System.Text;

namespace NPractices.Mvc
{
    public class CaptureOutputStream : Stream
    {
        private readonly Stream _base;
        private readonly string _outputPath;

        public CaptureOutputStream(Stream responseStream, string outputPath)
        {
            if (responseStream == null)
                throw new ArgumentNullException("responseStream");
            _base = responseStream;
            _outputPath = outputPath;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            _base.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string html = Encoding.UTF8.GetString(buffer, offset, count);

            var fi = new FileInfo(_outputPath);

            if (fi.Exists)
                fi.Delete();
            if (!fi.Directory.Exists)
                fi.Directory.Create();

            using (var writer = fi.CreateText())
            {
                writer.Write(html);
            }

            //buffer = System.Text.Encoding.UTF8.GetBytes(HTML);
            //_base.Write(buffer, 0, buffer.Length);
            _base.Write(buffer, offset, count);
        }
    }
}