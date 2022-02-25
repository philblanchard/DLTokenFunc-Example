using System.Collections.Generic;

namespace DirectLineTokenFuncProj;


public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class DirectLineRequest
{
    public User User { get; set; }
    public List<string> TrustedOrigins { get; set; }
}
