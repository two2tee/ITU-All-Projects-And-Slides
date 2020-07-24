// IFIleProvider.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.Threading.Tasks;
using LiftLog.Core.Dto.PortableDto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     IFileProvider is responsible for converting a class to byte array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFIleProvider<in T> where T : class
    {
        /// <summary>
        ///     This method will convert an input to a file
        /// </summary>
        /// <param name="input">such as portableUserDto</param>
        /// <returns>FileDto witl file name, extension and content</returns>
        Task<FileDto> MakeFileAsync(T input);
    }
}