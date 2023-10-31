using EST.MIT.Web.Entities;

namespace EST.MIT.Web.Builders;

public class NotificationBuilder
{
    private string _id = default!;
    private string _scheme = default!;
    private string _action = default!;
    private object _data = default!;

    public NotificationBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public NotificationBuilder WithScheme(string scheme)
    {
        _scheme = scheme;
        return this;
    }

    public NotificationBuilder WithAction(Enum action)
    {
        _action = action.ToString();
        return this;
    }

    public NotificationBuilder WithData(object data)
    {
        _data = data;
        return this;
    }

    public Notification Build()
    {
        if (string.IsNullOrEmpty(_id))
        {
            throw new InvalidOperationException("Id cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(_scheme))
        {
            throw new InvalidOperationException("Scheme cannot be null or empty.");
        }

        if (_action == null)
        {
            throw new InvalidOperationException("Action cannot be null.");
        }

        return new Notification(_id, _scheme, _action, _data);
    }
}