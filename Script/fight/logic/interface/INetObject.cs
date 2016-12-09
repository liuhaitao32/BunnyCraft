using System.Collections.Generic;

public interface INetObject:IClear {
    int netId { get; }
    Dictionary<string, object> getData();
    void setData(Dictionary<string, object> data);
    int type { get; }
}
