using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Leaderboard.Areas.Uploads.Services
{
    public interface ICreatableFileProvider : IFileProvider
    {
        IWriteStreamFactory GetWriteStreamFactory(string subPath);
    }

    public interface IWriteStreamFactory
    {
        string Root { get; }
        Stream CreateStream(string fileName);
    }

    public class CreatablePhysicalFileProvider : PhysicalFileProvider, ICreatableFileProvider
    {
        public CreatablePhysicalFileProvider(string root) : base(root)
        {
        }

        public CreatablePhysicalFileProvider(string root, ExclusionFilters filters) : base(root, filters)
        {
        }

        public IWriteStreamFactory GetWriteStreamFactory(string subPath)
            => new CreatablePhysicalWriteStreamFactory(subPath);
    }

    public class CreatablePhysicalWriteStreamFactory : IWriteStreamFactory
    {
        public string Root { get; set; }

        public CreatablePhysicalWriteStreamFactory(string root)
        {
            Root = root;
        }

        public Stream CreateStream(string fileName) => File.Create(Path.Join(Root, fileName));
    }
}
