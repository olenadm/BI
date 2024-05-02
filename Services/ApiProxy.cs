using System.Net;
using System.Text;
using System.Text.Json;
using BIMagistral.Models;
using RestSharp;

namespace BIMagistral.Services;

public class ApiProxy
{
    private readonly ILogger<ApiProxy> _logger;
    private readonly IConfiguration _configuration;

    public ApiProxy(ILogger<ApiProxy> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;
    }

    #region Clients

    private RestClient AuthClient()
    {
        var options = new RestClientOptions($"{_configuration.GetSection("APIAuth").Value}");
        var client = new RestClient(options);
        client.AddDefaultHeader("Content-Type", "application/json");
        client.AddDefaultHeader("Accept", "application/json");
        return client;
    }

    private RestClient CadastrosClient(LoginResult usuario)
    {
        var options = new RestClientOptions($"{_configuration.GetSection("APICadastros").Value}");
        var client = new RestClient(options);
        client.AddDefaultHeader("Content-Type", "application/json");
        client.AddDefaultHeader("Authorization", $"Bearer {usuario.access_token}");
        client.AddDefaultHeader("Accept", "application/json");
        return client;
    }

    private RestClient BIClient(LoginResult usuario)
    {
        var options = new RestClientOptions($"{_configuration.GetSection("APIBIMagistral").Value}");
        var client = new RestClient(options);
        client.AddDefaultHeader("Content-Type", "application/json");
        client.AddDefaultHeader("Authorization", $"Bearer {usuario.access_token}");
        client.AddDefaultHeader("Accept", "application/json");
        return client;
    }

    #endregion

    #region Request Bodies

    private string LoginRequestBody(Credenciais credenciais)
    {
        var body = $$"""
                     {
                         "email": "{{credenciais.email}}",
                         "senha": "{{credenciais.senha}}"
                     }
                     """;
        return body;
    }
    
    private string FiltroProdutoRequestBody(string descricao)
    {
        var itens = new StringBuilder();
        var produtos = descricao.Split(",").ToList();
        var ok = false;
        foreach (var p in produtos)
        {
            if (p.Trim().Length >= 3)
            {
                if(ok) itens.Append(",");
                ok = true;
                itens.AppendLine($$"""
                   {"descricaoproduto": "{{p.Trim()}}"}
                """);
            }
        }

        if (ok)
        {
            var body = $$"""
             [
                {{itens.ToString()}}
             ]
             """;
            return body;
        }

        return string.Empty;
    }
    
    private string ProdutoRequestBody(string codigo)
    {
        var itens = new StringBuilder();
        var produtos = codigo.Split(",").ToList();
        var ok = false;
        foreach (var p in produtos)
        {
            if (p.Trim().Length >= 0)
            {
                if(ok) itens.Append(",");
                ok = true;
                itens.AppendLine($$"""
                      {"codigoproduto": {{p.Trim()}}}
                   """);
            }
        }

        if (ok)
        {
            var body = $$"""
             [
                {{itens.ToString()}}
             ]
             """;
            return body;
        }

        return string.Empty;
    }

    #endregion

    public async Task<LoginResult?> Login(Credenciais credenciais)
    {
        var client = AuthClient();

        var request = new RestRequest("auth/v1/login", Method.Post);
        request.AddStringBody(LoginRequestBody(credenciais), DataFormat.Json);

        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<LoginResult>(response.Content!) : null;
    }

    public async Task<List<Linha>> GetLinhas(LoginResult login)
    {
        var client = CadastrosClient(login);

        var request =
            new RestRequest($"/cadastros/v1/linhas/lista/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<Linha>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<Produto>> GetProdutos(LoginResult login)
    {
        var client = CadastrosClient(login);

        var request =
            new RestRequest($"/cadastros/v1/itens/lista/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<Produto>>(response.Content!) ?? new() : new();
    }

    public async Task<List<UF>> GetUFs(LoginResult login)
    {
        var client = CadastrosClient(login);

        var request =
            new RestRequest($"/cadastros/v1/ufs/lista/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<UF>>(response.Content!) ?? new() : new();
    }

    public async Task<List<FatPeriodo>> GetFatPeriodo(LoginResult login, string dtInicio, string dtFim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/anomes/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful
            ? JsonSerializer.Deserialize<List<FatPeriodo>>(response.Content!) ?? new()
            : new();
    }

    public async Task<List<FatLinha>> GetFatLinhas(LoginResult login, string dtInicio, string dtFim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/linha/anomes/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<FatLinha>>(response.Content!) ?? new() : new();
    }

    public async Task<List<FatFornecedor>> GetFatFornecedor(LoginResult login, string dtInicio, string dtfim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/fornecedor/anomes/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtfim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful
            ? JsonSerializer.Deserialize<List<FatFornecedor>>(response.Content!) ?? new()
            : new();
    }

    public async Task<List<FatPrazo>> GetFatPrazo(LoginResult login, string dtInicio, string dtFim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/prazopagamento/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<FatPrazo>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<TopFornecedor>> GetTopFornecedor(LoginResult login, string dtInicio, string dtFim, string qtFornecedores)
    {

        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/fornecedor/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}/{qtFornecedores}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<TopFornecedor>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<FatUF>> GetFatUF(LoginResult login, string dtInicio, string dtFim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/uf/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<FatUF>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<ProdGeral>> GetProdGeral(LoginResult login, string dtInicio, string dtFim)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/produto/geral/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<ProdGeral>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<ProdRanking>> GetProdRanking(LoginResult login, string dtInicio, string dtFim, string qtProdutos, string qtFornecedores)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/produto/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}/{qtProdutos}/{qtFornecedores}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<ProdRanking>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<FiltroProduto>> GetFiltroProduto(LoginResult login, string descricao)
    {
        var body = FiltroProdutoRequestBody(descricao);
        if(body == string.Empty) return new();

        var client = CadastrosClient(login);
        var request =
            new RestRequest(
                $"/cadastros/v1/itens/filtrados/lista/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}",
                Method.Post);
        request.AddStringBody(body, DataFormat.Json);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<FiltroProduto>>(response.Content!) ?? new() : new();
    }
    
    public async Task<List<ProdutoCompras>> GetProdutosCompras(LoginResult login, string codigo, string dtInicio, string dtFim, string qtCompras, string qtFornecedores)
    {
        var body = ProdutoRequestBody(codigo);
        if(body == string.Empty) return new();

        var client = BIClient(login);
        
        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/produto/gestao/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}/{qtCompras}/{qtFornecedores}",
                Method.Post);
        request.AddStringBody(body, DataFormat.Json);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<ProdutoCompras>>(response.Content!) ?? new() : new();
    }

    public async Task<List<ProdutoCompras>> GetProdutosGestao(LoginResult login, string codigo, string dtInicio, string dtFim, string qtCompras, string qtFornecedores)
    {
        var body = ProdutoRequestBody(codigo);
        if(body == string.Empty) return new();

        var client = BIClient(login);
        
        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/produto/gestao/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}/{qtCompras}/{qtFornecedores}",
                Method.Post);
        request.AddStringBody(body, DataFormat.Json);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<ProdutoCompras>>(response.Content!) ?? new() : new();
    }

    public async Task<List<ProdVariacao>> GetProdVariacao(LoginResult login, string dtInicio, string dtFim, string qtProdutos, string ptVariacao)
    {
        var client = BIClient(login);

        var request =
            new RestRequest(
                $"/bimagistral/v1/faturamento/produto/variacao/{login.usuario.licenca}/{login.usuario.empresas[0].CODIGO}/{dtInicio}/{dtFim}/{qtProdutos}/{ptVariacao}",
                Method.Get);
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful ? JsonSerializer.Deserialize<List<ProdVariacao>>(response.Content!) ?? new() : new();
    }
    
}

