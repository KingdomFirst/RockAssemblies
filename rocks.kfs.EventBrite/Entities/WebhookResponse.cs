namespace rocks.kfs.Eventbrite.Entities
{
    public class WebhookResponse
    {
        public string Api_Url { get; set; }
        public WebhookConfig Config { get; set; }
    }

    public class WebhookConfig
    {
        public long Webhook_Id { get; set; }
        public string Action { get; set; }
        public string Endpoint_Url { get; set; }
        public long User_Id { get; set; }

    }
}
