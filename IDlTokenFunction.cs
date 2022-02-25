using System.Threading.Tasks;

namespace DirectLineTokenFuncProj;

public interface IDlTokenFunction
{
    Task<DirectLineToken> GetToken();
}