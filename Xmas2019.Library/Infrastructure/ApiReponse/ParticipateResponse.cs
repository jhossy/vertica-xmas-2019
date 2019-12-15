using System;

namespace Xmas2019.Library.Infrastructure.ApiReponse
{
    public class ParticipateResponse
    {
        public Guid Id { get; set; }
        public Credentials Credentials { get; set; }

        public static ParticipateResponse Empty()
        {
            return new ParticipateResponse() { Id = Guid.Empty, Credentials = new Credentials() { Username = string.Empty, Password = string.Empty } };
        }
    }
}
