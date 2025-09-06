namespace CatalogoBackend.Models.Responses
{
    public enum ResponseCode { Ok, NoContent, BadRequest, NotFound, CreatedAtAction ,ServerError };
    public class GeneralResponse<T>
    {
        public T? Data { get; set; }
        public string? Msg { get; set; }
        public bool Success {  get; set; }
        public ResponseCode Code { get; set; }

        public static GeneralResponse<T> Ok(T? data, string? msg = null, ResponseCode code = ResponseCode.Ok) =>
            new GeneralResponse<T> { Data = data, Msg = msg, Success = true, Code = code };

        public static GeneralResponse<T> Fail(T? data, string? msg = null, ResponseCode code = ResponseCode.BadRequest) =>
            new GeneralResponse<T> { Data = data, Msg = msg, Success = false, Code = code };
    }
}
