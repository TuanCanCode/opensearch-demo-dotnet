namespace OpenSearchDemo.Models
{
    public class AwsConfig
    {
        public OpenSearchConfig OpenSearch { get; set; }
        public class OpenSearchConfig
        {
            public string Url { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }
        }
    }
}
