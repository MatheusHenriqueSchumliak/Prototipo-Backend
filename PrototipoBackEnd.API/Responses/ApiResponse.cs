using System.Text.Json.Serialization;

namespace PrototipoBackEnd.API.Responses
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(string message, T? data)
        {
            Message = message;
            Data = data;
        }
    }

    public class FieldError
    {
        public string Field { get; set; }
        public string Message { get; set; }

        public FieldError(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<FieldError>? Errors { get; set; }

        public ErrorResponse(string message, IReadOnlyList<FieldError>? errors = null)
        {
            Message = message;
            Errors = errors;
        }
    }
}
