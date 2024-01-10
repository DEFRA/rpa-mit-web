namespace EST.MIT.Web.Entities;

public class Notification
{
    public string Action { get; set; }
    public object Data { get; set; }
    public string Id { get; set; }
    public string Scheme { get; set; }
    public string ApproverEmail { get; set; }
    public string CreatorEmail { get; set; }

    public Notification(string id, string scheme, string action, string approverEmail, string creatorEmail, object data)
    {
        Id = id;
        Scheme = scheme;
        Action = action;
        ApproverEmail = approverEmail;
        CreatorEmail = creatorEmail;
        Data = data;
    }
}
