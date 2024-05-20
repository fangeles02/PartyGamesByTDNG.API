
using System.ComponentModel.DataAnnotations;

namespace PartyGamesByTDNG.API.Models.SignalR
{
    public class ResponseBuilder
    {
        public static string Build(Response response)
        {
            return $"{response.Recipient.ToString()}~{(response.Result == ResponseCode.Success ? "OK" : "ERR")}~{response.ResultTitle ?? ""}~{response.ResultMessage}~{response.ResponseParams}";
        }
    }
    public class Response
    {
        public required Recipient Recipient { get; set; }

        public required ResponseCode Result { get; set; }

        public string? ResultTitle { get; set; }

        public required string ResultMessage { get; set; }
        public string? ResponseParams { get; set; }
    }

    public enum ResponseCode
    {
        Success,
        Failed
    }


    public enum Recipient
    {
        Self,
        Group
    }



}
