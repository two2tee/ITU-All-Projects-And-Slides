// FileDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

namespace LiftLog.Core.Dto.PortableDto
{
    /// <summary>
    /// Dto with file content
    /// </summary>
    public class FileDto
    {
        public string FileName { get; set; }

        public string FileExtention { get; set; }

        public byte[] FileContent { get; set; }
    }
}