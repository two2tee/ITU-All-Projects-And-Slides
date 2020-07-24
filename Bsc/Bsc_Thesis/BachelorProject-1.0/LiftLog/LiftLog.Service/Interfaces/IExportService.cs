// IExportService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading.Tasks;
using LiftLog.Core.Dto.PortableDto;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     The export service is reponsible for creating exportable formates of the data stored in the
    ///     system
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        ///     This method returns all data of user in a portable DTO ready to be transformed to other formats
        ///     such as JSON.
        /// </summary>
        /// <param name="userToken">UID of user</param>
        /// <returns>Portable DTO</returns>
        Task<PortableUserDto> ExportToPortableObjectAsync(Guid userToken);

        /// <summary>
        ///     This method returns a byteArray that represents a PDF with all user informations
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>byte array</returns>
        Task<FileDto> ExportToFileAsync(int userId);

        /// <summary>
        ///     This method returns a token used to export data from api
        /// </summary>
        /// <param name="userId">uid of user</param>
        /// <returns>Token as a string</returns>
        Task<string> GetExportTokenAsync(int userId);
    }
}