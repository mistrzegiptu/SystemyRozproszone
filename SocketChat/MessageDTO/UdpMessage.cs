using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MessageDTO
{
    public class UdpMessage
    {
        public required string ClientNickname { get; set; }
        public int ClientId { get; set; }
        public required string Message { get; set; }

        public List<int>? TargetIds { get; set; }

        public override string ToString()
        {
            return $"{ClientNickname}({ClientId}): \n {Message}";
        }

        public byte[] GetBytes()
        {
            return JsonSerializer.SerializeToUtf8Bytes(this);
        }

        public static UdpMessage? Parse(byte[] bytes)
        {
            return JsonSerializer.Deserialize<UdpMessage>(bytes);
        }
    }
}
