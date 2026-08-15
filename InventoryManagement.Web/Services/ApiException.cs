namespace InventoryManagement.Web.Services
{
    /// <summary>
    /// Thrown when the backend API returns a non-success status code.
    /// The message is extracted from the API's problem+json response when possible.
    /// </summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
