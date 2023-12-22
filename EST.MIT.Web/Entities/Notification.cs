namespace EST.MIT.Web.Entities;

public class Notification
{
    public string Action { get; set; }
    public object Data { get; set; }
    public string Id { get; set; }
    public string Scheme { get; set; }
    public string EmailRecipient { get; set; }

    public Notification(string id, string scheme, string action, string emailReciepent, object data)
    {
        Id = id;
        Scheme = scheme;
        Action = action;
        EmailRecipient = emailReciepent;
        Data = data;
    }
}
