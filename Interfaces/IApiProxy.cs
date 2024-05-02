using BIMagistral.Models;

namespace BIMagistral.Interfaces;

public interface IApiProxy
{
    Task<LoginResult?> Login(Credenciais credenciais);
    Task<List<Linha>> GetLinhas(LoginResult login);
    Task<List<Produto>> GetProdutos(LoginResult login);
    Task<List<UF>> GetUFs(LoginResult login);
    Task<List<FatOverview>> GetFatPeriodo(LoginResult login, string dtInicio, string dtFim);
    Task<List<FatLinha>> GetFatLinhas(LoginResult login, string dtInicio);
    Task<List<FatFornecedor>> GetFatFornecedor(LoginResult login, string dtInicio);
    Task<List<FatPrazo>> GetFatPrazo(LoginResult login, string dtInicio, string dtFim);
}