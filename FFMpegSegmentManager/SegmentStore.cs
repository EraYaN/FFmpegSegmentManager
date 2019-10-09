using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FFMpegSegmentManager.Exceptions;
using FFMpegSegmentManager.Model;

namespace FFMpegSegmentManager
{
    public class SegmentStore
    {
        long _maxSize;
        long _currentSize = 0;
        DirectoryInfo _basePath;
        string _segmentExtension;
        Dictionary<ulong, Segment> _segments;

        public long CacheSize => _currentSize;

        public SegmentStore(string BasePath, string SegmentExtension = ".ts", long MaxSize = 2147483648)
        {
            _maxSize = MaxSize;
            _basePath = new DirectoryInfo(BasePath);
            _segmentExtension = SegmentExtension;
            _segments = new Dictionary<ulong, Segment>();
            if (!_basePath.Exists)
            {
                throw new ArgumentException("Given path does not exist.", nameof(BasePath));
            }
        }

        public void Clear()
        {
            foreach (var seg in _segments)
            {
                Remove(seg.Key);
            }
            _segments.Clear();
            _currentSize = 0;
        }

        string CreateInternalFilename(ulong Id)
        {
            return Path.Join(_basePath.FullName, Id.ToString("X16") + _segmentExtension);
        }

        public void Store(ulong Id, string SegmentPath, ulong StartTime, ulong EndTime)
        {
            if (File.Exists(SegmentPath))
            {
                var length = new System.IO.FileInfo(SegmentPath).Length;
                //maybe store this file size in the segment class.
                var extension = Path.GetExtension(SegmentPath);
                if (_segmentExtension != extension)
                {
                    throw new ArgumentException("Given segment does not have the correct file extension.", nameof(SegmentPath));
                }
                var internalFilename = CreateInternalFilename(Id);
                if (File.Exists(internalFilename))
                    File.Delete(internalFilename);
                File.Move(SegmentPath, internalFilename);
                _segments.Add(Id, new Segment() { Id = Id, StartTime = StartTime, EndTime = EndTime, Added = DateTime.Now });

                _currentSize += length;
                while (_currentSize > _maxSize)
                {
                    var removalKey = _segments.OrderBy(x => x.Value.LastAccessed).ThenBy(x => x.Value.Added).Select(x => x.Key).First();
                    Console.WriteLine("Removing oldest segment {0:X16} because new cache size of {1:F1} MB is larger than the limit {2:F1} MB...", removalKey, _currentSize / 1e6, _maxSize / 1e6);
                    Remove(removalKey);
                }
            }
        }

        public FileInfo Get(ulong Id)
        {
            if (_segments.ContainsKey(Id))
            {
                _segments[Id].LastAccessed = DateTime.Now;
                return new FileInfo(CreateInternalFilename(Id));
            }
            else
            {
                throw new SegmentNotFoundException();
            }
        }

        public void Remove(ulong Id)
        {
            if (_segments.ContainsKey(Id))
            {
                var filename = CreateInternalFilename(Id);
                var length = new System.IO.FileInfo(filename).Length;
                File.Delete(filename);
                _segments.Remove(Id);
                _currentSize -= length;
            }
            else
            {
                throw new SegmentNotFoundException();
            }
        }
    }
}
