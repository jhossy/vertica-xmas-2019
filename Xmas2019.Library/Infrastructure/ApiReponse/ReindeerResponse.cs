using System;
using System.Collections.Generic;
using System.Text;

namespace Xmas2019.Library.Infrastructure.ApiReponse
{
    public class ReindeerResponse
    {
        public string ToyDistributionXmlUrl { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return ToyDistributionXmlUrl;
        }
    }
}
