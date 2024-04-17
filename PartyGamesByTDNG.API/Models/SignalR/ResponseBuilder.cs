
using System.ComponentModel.DataAnnotations;

namespace PartyGamesByTDNG.API.Models.SignalR
{
    public class ResponseBuilder
    {
        public static string Build(Response response)
        {
            return $"{(response.Result == ResponseCode.Success ? "OK" : "ERR")}~{response.ResultTitle ?? ""}~{response.ResultMessage}";
        }
    }
    public class Response
    {

        public required ResponseCode Result { get; set; }

        public string? ResultTitle { get; set; }

        public required string ResultMessage { get; set; }
    }

    public enum ResponseCode
    {
        Success,
        Failed
    }



}
