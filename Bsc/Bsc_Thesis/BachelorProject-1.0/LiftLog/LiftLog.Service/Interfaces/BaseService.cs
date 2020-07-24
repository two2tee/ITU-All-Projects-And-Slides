// BaseService.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using Microsoft.Extensions.Logging;

namespace LiftLog.Service.Interfaces
{
    /// <summary>
    ///     This abstract class serve as a base service with logging functionality.
    /// </summary>
    public abstract class BaseService
    {
        protected ILogger Logger;
        protected string Servicetag;


        /// <summary>
        ///     Log an error message with exception
        /// </summary>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected void LogError(string method, string message, Exception exception)
        {
            Logger.LogError($"{Servicetag} : {DateTime.Now} : " +
                            $"\n{method} - {message}" +
                            $"\nStacktrace: {exception.Message}");
        }

        /// <summary>
        ///     Log an error message without any exceptions
        /// </summary>
        /// <param name="method"></param>
        /// <param name="m"></param>
        protected void LogError(string method, string m)
        {
            Logger.LogError($"{Servicetag} : {DateTime.Now} : " +
                            $"\n{m}");
        }

        /// <summary>
        ///     Log a warning
        /// </summary>
        /// <param name="method"></param>
        /// <param name="m"></param>
        protected void LogWarning(string method, string m)
        {
            Logger.LogWarning($"{Servicetag} : {DateTime.Now} : " +
                              $"\n{m}");
        }

        /// <summary>
        ///     Log a critical action
        /// </summary>
        /// <param name="method"></param>
        /// <param name="m"></param>
        protected void LogCritical(string method, string m)
        {
            Logger.LogCritical($"{Servicetag} : {DateTime.Now} : " +
                               $"\n{m}");
        }

        /// <summary>
        ///     Log trivial information
        /// </summary>
        /// <param name="method"></param>
        /// <param name="m"></param>
        protected void LogInformation(string method, string m)
        {
            Logger.LogInformation($"{Servicetag} : {DateTime.Now} : " +
                                  $"\n{m}");
        }

        /// <summary>
        ///     Logs error when an id is invalid
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        protected void LogInvalidIdError(string method, int id)
        {
            LogError(method, $"Provided id was invalid : {id}");
        }

        /// <summary>
        ///     Logs error when given user with provided id does not exist
        /// </summary>
        /// <param name="method"></param>
        /// <param name="id"></param>
        protected void LogNonExistingUserError(string method, int id)
        {
            LogError(method, $"User with id {id} did not exist in database.");
        }
    }
}