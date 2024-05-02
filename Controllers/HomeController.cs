using System.Diagnostics;
using System.Text.Json;
using BIMagistral.Services;
using BIMagistral.Models;
using Microsoft.AspNetCore.Mvc;

namespace BIMagistral.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiProxy _apiProxy;
    private readonly LoginResult _user;

    public HomeController(ILogger<HomeController> logger, ApiProxy apiProxy)
    {
        _logger = logger;
        _apiProxy = apiProxy;

        var session = new HttpContextAccessor().HttpContext!.Session;
        if (string.IsNullOrEmpty(session.GetString("USR_DATA")))
        {
            new HttpContextAccessor().HttpContext!.Response.Redirect("../Login");
            _user = new();
        }
        else
        {
            _user = JsonSerializer.Deserialize<LoginResult>(session.GetString("USR_DATA")!);
        }
    }

    private async Task<List<Linha>> GetLinhas()
    {
        var session = new HttpContextAccessor().HttpContext!.Session;
        List<Linha> result;
        if (string.IsNullOrEmpty(session.GetString("LINHAS")))
        {
            result = await _apiProxy.GetLinhas(_user);
            session.SetString("LINHAS", JsonSerializer.Serialize(result));
        }
        else
        {
            result = JsonSerializer.Deserialize<List<Linha>>(session.GetString("LINHAS") ?? "")!;
        }

        return result;
    }

    private async Task<List<UF>> GetUFs()
    {
        var session = new HttpContextAccessor().HttpContext!.Session;
        List<UF> result;
        if (string.IsNullOrEmpty(session.GetString("UFS")))
        {
            result = await _apiProxy.GetUFs(_user);
            session.SetString("UFS", JsonSerializer.Serialize(result));
        }
        else
        {
            result = JsonSerializer.Deserialize<List<UF>>(session.GetString("UFS") ?? "")!;
        }

        return result;
    }

    
    public async Task<IActionResult> Fornecedores()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioAnual"];
        TempData.Keep("DtInicioAnual");
        ViewBag.DtFim    = TempData["DtFimAnual"];
        TempData.Keep("DtFimAnual");
        ViewBag.QtFornecedores = "10";

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.Linha = session.GetString("Linha") ?? string.Empty;
        ViewBag.UF = session.GetString("UF") ?? string.Empty;
        ViewBag.DtInicio = session.GetString("DtInicio") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim") ?? ViewBag.DtFim;
        ViewBag.QtFornecedores = session.GetString("QtFornecedores") ?? ViewBag.QtFornecedores;
        
        ViewData["AutoLoad"] = true;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Fornecedores(string dtInicio, string dtFim, string linha, string uf, string qtFornecedores)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;
        ViewBag.QtFornecedores = qtFornecedores;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("Linha", linha ?? string.Empty);
        session.SetString("UF", uf ?? string.Empty);
        session.SetString("DtInicio", dtInicio ?? string.Empty);
        session.SetString("DtFim", dtFim ?? string.Empty);
        session.SetString("QtFornecedores", qtFornecedores ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        ViewBag.FatFornecedor = await _apiProxy.GetFatFornecedor(_user, dataInicio, dataFim);
        ViewBag.TopFornecedor = await _apiProxy.GetTopFornecedor(_user, dataInicio, dataFim, qtFornecedores);
        return View();
    }
    
    public async Task<IActionResult> Brasil()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioAnual"];
        TempData.Keep("DtInicioAnual");
        ViewBag.DtFim    = TempData["DtFimAnual"];
        TempData.Keep("DtFimAnual");
        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.DtInicio = session.GetString("DtInicio_Brasil") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_Brasil") ?? ViewBag.DtFim;
        ViewBag.Linha = session.GetString("Linha_Brasil") ?? string.Empty;
        ViewBag.UF = session.GetString("UF_Brasil") ?? string.Empty;
       
        ViewData["AutoLoad"] = true;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Brasil(string dtInicio, string dtFim, string linha, string uf)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_Brasil", dtInicio);
        session.SetString("DtFim_Brasil", dtFim);
        session.SetString("Linha_Brasil", linha ?? string.Empty);
        session.SetString("UF_Brasil", uf ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        ViewBag.FatUF = await _apiProxy.GetFatUF(_user, dataInicio, dataFim);
        return View();
    }
    
    public async Task<IActionResult> Produtos()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioSemestral"];
        TempData.Keep("DtInicioSemestral");
        ViewBag.DtFim    = TempData["DtFimSemestral"];
        TempData.Keep("DtFimSemestral");
        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();
        ViewBag.QtProdutos = "20";
        ViewBag.QtFornecedores = "5"; 
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.DtInicio = session.GetString("DtInicio_Produtos") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_Produtos") ?? ViewBag.DtFim;
        ViewBag.Linha = session.GetString("Linha_Produtos") ?? string.Empty;
        ViewBag.UF = session.GetString("UF_Produtos") ?? string.Empty;
        ViewBag.QtProdutos = session.GetString("QtProdutos_Produtos") ?? ViewBag.QtProdutos;
        ViewBag.QtFornecedores = session.GetString("QtFornecedores_Produtos") ?? ViewBag.QtFornecedores;

        ViewData["AutoLoad"] = true;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Produtos(string dtInicio, string dtFim, string linha, string uf, string qtProdutos, string qtFornecedores)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;
        ViewBag.QtProdutos = qtProdutos;
        ViewBag.QtFornecedores = qtFornecedores;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_Produtos", dtInicio);
        session.SetString("DtFim_Produtos", dtFim);
        session.SetString("Linha_Produtos", linha ?? string.Empty);
        session.SetString("UF_Produtos", uf ?? string.Empty);
        session.SetString("QtProdutos_Produtos", qtProdutos ?? string.Empty);
        session.SetString("QtFornecedores_Produtos", qtFornecedores ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        ViewBag.ProdGeral = await _apiProxy.GetProdGeral(_user, dataInicio, dataFim);
        var ranking = await _apiProxy.GetProdRanking(_user, dataInicio, dataFim, qtProdutos, qtFornecedores);
        ranking.ForEach(q => {
            q.LISTAFORNECEDORES = JsonSerializer.Deserialize<List<ProdutoTopFornecedor>>(q.TOPFORNECEDORES ?? "")!;
            //q.TOPFORNECEDORESJSON = JsonSerializer.Deserialize<JsonArray>(q.TOPFORNECEDORES);
        });
        ViewBag.ProdRanking = ranking;
        return View();
    }
    
    public async Task<IActionResult> Periodo()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioAnual"];
        TempData.Keep("DtInicioAnual");
        ViewBag.DtFim    = TempData["DtFimAnual"];
        TempData.Keep("DtFimAnual");
        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.DtInicio = session.GetString("DtInicio_Periodo") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_Periodo") ?? ViewBag.DtFim;
        ViewBag.Linha = session.GetString("Linha_Periodo") ?? string.Empty;
        ViewBag.UF = session.GetString("UF_Periodo") ?? string.Empty;

        ViewData["AutoLoad"] = true;

        return View();   
    }
    
    [HttpPost]
    public async Task<IActionResult> Periodo(string dtInicio, string dtFim, string linha, string uf)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_Periodo", dtInicio);
        session.SetString("DtFim_Periodo", dtFim);
        session.SetString("Linha_Periodo", linha ?? string.Empty);
        session.SetString("UF_Periodo", uf ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        ViewBag.FatPeriodo = (await _apiProxy.GetFatPeriodo(_user, dataInicio, dataFim)).OrderByDescending(q => q.ANO)
            .ThenByDescending(q => q.MES).ToList();
        ViewBag.FatLinhas = (await _apiProxy.GetFatLinhas(_user, dataInicio, dataFim))
            .Where(q => string.IsNullOrEmpty(linha) || q.LINHA == linha).ToList();
        return View();

    }

    public async Task<IActionResult> Pagamentos()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();
        ViewBag.DtInicio = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
        ViewBag.DtFim = DateTime.Today.ToString("dd/MM/yyyy");
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.DtInicio = session.GetString("DtInicio") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim") ?? ViewBag.DtFim;
        ViewBag.Linha = session.GetString("Linha") ?? string.Empty;
        ViewBag.UF = session.GetString("UF") ?? string.Empty;
        
        ViewData["AutoLoad"] = true;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Pagamentos(string dtInicio, string dtFim, string linha, string uf)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio", dtInicio);
        session.SetString("DtFim", dtFim);
        session.SetString("Linha", linha ?? string.Empty);
        session.SetString("UF", uf ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> FiltroProduto(string descricao)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;
        return Json(await _apiProxy.GetFiltroProduto(_user, descricao));
    }
    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

   public async Task<IActionResult> ProdutosCompras()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioTrimestral"];
        TempData.Keep("DtInicioTrimestral");
        ViewBag.DtFim    = TempData["DtFimTrimestral"];
        TempData.Keep("DtFimTrimestral");
        ViewBag.QtCompras = "3";
        ViewBag.QtFornecedores = "6";
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        
        ViewBag.DtInicio = session.GetString("DtInicio_ProdutosCompras") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_ProdutosCompras") ?? ViewBag.DtFim;
        ViewBag.QtCompras = session.GetString("QtCompras_ProdutosCompras") ?? ViewBag.QtCompras;
        ViewBag.QtFornecedores = session.GetString("QtFornecedores_ProdutosCompras") ?? ViewBag.QtFornecedores;
        
        ViewData["AutoLoad"] = true;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProdutosCompras(string codigo, 
              string dtInicio, string dtFim, string qtCompras, string qtFornecedores)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.QtCompras = qtCompras;
        ViewBag.QtFornecedores = qtFornecedores;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_Compras", dtInicio);
        session.SetString("DtFim_Compras", dtFim);
        session.SetString("QtCompras_Compras", qtCompras ?? string.Empty);
        session.SetString("QtFornecedores_Produtos", qtFornecedores ?? string.Empty);

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        return Json(await _apiProxy.GetProdutosCompras(_user, codigo, dataInicio, dataFim, qtCompras, qtFornecedores));
    }
    

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;
        var VI_DtAux = DateTime.Today.AddMonths(-12);
        TempData["DtInicioAnual"]      = new DateTime(VI_DtAux.Year, VI_DtAux.Month, 1).ToString("dd/MM/yyyy");
        TempData["DtFimAnual"]         = DateTime.Today.ToString("dd/MM/yyyy");

        VI_DtAux = DateTime.Today.AddMonths(-6);
        TempData["DtInicioSemestral"]  = new DateTime(VI_DtAux.Year, VI_DtAux.Month, 1).ToString("dd/MM/yyyy");
        TempData["DtFimSemestral"]     = DateTime.Today.ToString("dd/MM/yyyy");

        VI_DtAux = DateTime.Today.AddMonths(-3);
        TempData["DtInicioTrimestral"] = new DateTime(VI_DtAux.Year, VI_DtAux.Month, 1).ToString("dd/MM/yyyy");
        TempData["DtFimTrimestral"]    = DateTime.Today.ToString("dd/MM/yyyy");
        //TempData["UFs"]              = await GetUFs();  ver pq não func.
        return View();
    }

    public async Task<IActionResult> ProdutosGestao()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioTrimestral"];
        TempData.Keep("DtInicioTrimestral");
        ViewBag.DtFim    = TempData["DtFimTrimestral"];
        TempData.Keep("DtFimTrimestral");
        ViewBag.QtCompras = "3";
        ViewBag.QtFornecedores = "6";
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        
        ViewBag.DtInicio = session.GetString("DtInicio_ProdutosGestao") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_ProdutosGestao") ?? ViewBag.DtFim;
        ViewBag.QtCompras = session.GetString("QtCompras_ProdutosGestao") ?? ViewBag.QtCompras;
        ViewBag.QtFornecedores = session.GetString("QtFornecedores_ProdutosGestao") ?? ViewBag.QtFornecedores;
        
        ViewData["AutoLoad"] = true;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProdutosGestao(string codigo, 
              string dtInicio, string dtFim, string qtCompras, string qtFornecedores)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.QtCompras = qtCompras;
        ViewBag.QtFornecedores = qtFornecedores;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_ProdutosGestao", dtInicio);
        session.SetString("DtFim_ProdutosGestao", dtFim);
        session.SetString("QtCompras_ProdutosGestao", qtCompras ?? string.Empty);
        session.SetString("QtFornecedores_ProdutosGestao", qtFornecedores ?? string.Empty);

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        return Json(await _apiProxy.GetProdutosGestao(_user, codigo, dataInicio, dataFim, qtCompras, qtFornecedores));
    }

    public async Task<IActionResult> ProdutosVariacao()
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        ViewBag.DtInicio = TempData["DtInicioSemestral"];
        TempData.Keep("DtInicioSemestral");
        ViewBag.DtFim    = TempData["DtFimSemestral"];
        TempData.Keep("DtFimSemestral");
        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();
        ViewBag.QtProdutos = "100";
        ViewBag.PtVariacao = "1"; 
        
        var session = new HttpContextAccessor().HttpContext!.Session;
        ViewBag.DtInicio = session.GetString("DtInicio_Produtos") ?? ViewBag.DtInicio;
        ViewBag.DtFim = session.GetString("DtFim_Produtos") ?? ViewBag.DtFim;
        ViewBag.Linha = session.GetString("Linha_Produtos") ?? string.Empty;
        ViewBag.UF = session.GetString("UF_Produtos") ?? string.Empty;
        ViewBag.QtProdutos = session.GetString("QtProdutos_Variacao") ?? ViewBag.QtProdutosVariacao;
        ViewBag.QtFornecedores = session.GetString("PtVariacao") ?? ViewBag.PtVariacao;

        ViewData["AutoLoad"] = true;

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ProdutosVariacao(string dtInicio, string dtFim, string linha, string uf, string qtProdutos, string ptVariacao)
    {
        if (string.IsNullOrEmpty(_user.access_token)) return Empty;

        // Filtros
        ViewBag.DtInicio = dtInicio;
        ViewBag.DtFim = dtFim;
        ViewBag.Linha = linha;
        ViewBag.UF = uf;
        ViewBag.QtProdutos = qtProdutos;
        ViewBag.PtVariacao = ptVariacao;

        var session = new HttpContextAccessor().HttpContext!.Session;
        session.SetString("DtInicio_Variacao", dtInicio);
        session.SetString("DtFim_Variacao", dtFim);
        session.SetString("Linha_Variacao", linha ?? string.Empty);
        session.SetString("UF_Variacao", uf ?? string.Empty);
        session.SetString("QtProdutos_Variacao", qtProdutos ?? string.Empty);
        session.SetString("PtVariacao", ptVariacao ?? string.Empty);

        ViewBag.Linhas = await GetLinhas();
        ViewBag.UFs = await GetUFs();

        string dataInicio = dtInicio.Replace("/", string.Empty);
        string dataFim = dtFim.Replace("/", string.Empty);

        ViewBag.ProdVariacao = await _apiProxy.GetProdVariacao(_user, dataInicio, dataFim, qtProdutos, ptVariacao);
        return View();
    }

}



