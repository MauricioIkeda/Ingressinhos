using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Generic.Domain.Entities
{
    public class MessageReturn
    {
        public string Message { get; set; }
        public bool Error { get; set; }
        [JsonIgnore]
        public Exception Exception { get; set; }

        public MessageReturn() { }

        public MessageReturn(string message, bool error = false)
        {
            Message = message;
            Error = error;
        }

        public MessageReturn(Exception exception)
        {
            Exception = exception;
            Message = exception.Message;
            Error = true;
        }
    }

    public class ListMessages : List<MessageReturn>
    {
        public void Add(Exception exception)
        {
            Add(new MessageReturn(exception));
        }

        public void Add(string message, bool error = false)
        {
            Add(new MessageReturn(message, error));
        }

        public bool HasError()
        {
            return this.Any(m => m.Error);
        }
    }
}
