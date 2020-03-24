using System;
using System.IO;

namespace Leaderboard.Areas.Uploads.Models
{

    public class FileContent
    {
        public string Name { get; }
        public Stream ContentStream { get; }

        public FileContent(string name, Stream contentStream)
        {
            Name = name;
            ContentStream = contentStream;
        }
    }

    public class FileUploadModel
    {
        public FileContent File { get; set; }
    }
}
