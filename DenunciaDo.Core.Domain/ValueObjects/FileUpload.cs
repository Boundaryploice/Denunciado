using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenunciaDo.Domain.ValueObjects
{
    public class FileUpload
    {
        public Stream Content { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public long Size { get; private set; }

        public FileUpload(Stream content, string fileName, string contentType, long size)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Size = size;
        }
    }
}
