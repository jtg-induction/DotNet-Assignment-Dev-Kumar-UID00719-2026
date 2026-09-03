using System.Collections.Generic;

namespace dotNetAssignment.Models.DTO
{
    public class ApiResponseDto<T>
    {
        /// <summary>
        /// Indicates whether the API request was successful or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Provides additional information about the API response, such as error messages or success messages.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Contains the data returned by the API request. This can be any type of object.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Contains the error information, if the API request failed.
        /// </summary>
        public Dictionary<string, List<string>> Error { get; set; }
    }
}
