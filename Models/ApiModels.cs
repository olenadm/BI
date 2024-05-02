using System.Text.Json.Nodes;

namespace BIMagistral.Models;

public record Credenciais(string email, string senha);

public class LoginResult
{
    public Usuario usuario { get; set; } = new();
    public string access_token { get; set; } = string.Empty;
    public string refresh_token { get; set; } = string.Empty;
}

public class Empresa
{
    public int CODIGO { get; set; } = 0;
    public string CNPJ { get; set; } = string.Empty;
    public string FANTASIA { get; set; } = string.Empty;
    public string DIREITOS { get; set; } = string.Empty;
}

public class Usuario
{
    public int codigo { get; set; } = 0;
    public string nome { get; set; } = string.Empty;
    public int licenca { get; set; } = 0;
    public List<Empresa> empresas { get; set; } = new();
}

public class Linha
{
    public int EMPRESA { get; set; }
    public string PAIS { get; set; }
    public string ATIVO { get; set; }
    public string CODIGO { get; set; }
    public string DESCRICAO { get; set; }
}

public class UF
{
    public int EMPRESA { get; set; }
    public string PAIS { get; set; }
    public string ATIVO { get; set; }
    public string CODIGO { get; set; }
    public string DESCRICAO { get; set; }
}

public class FatPeriodo
{
    public int ANO { get; set; }
    public int MES { get; set; }
    public double VRFATURAMENTO { get; set; }
    public double VRFATURAMENTO_ANOANTERIOR { get; set; }
    public int TRANSACOES { get; set; }
    public double TICKETMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public int PRODUTOS { get; set; }
    public int PARCELAS { get; set; }
    public int PRAZOMEDIO { get; set; }
    public string TOPLINHAS { get; set; }
}

public class FatLinha
{
    public string LINHA { get; set; }
    public string DESCRICAO { get; set; }
    public int ANO { get; set; }
    public int MES { get; set; }
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public double TICKETMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public int PRODUTOS { get; set; }
}

public class FatFornecedor
{
    public string CNPJFABR { get; set; }
    public string FORNECEDOR { get; set; }
    public int ANO { get; set; }
    public int MES { get; set; }
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public double TICKETMEDIO { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public int PRODUTOS { get; set; }
    public int PARCELAS { get; set; }
    public int PRAZOMEDIO { get; set; }
    public string TOPLINHAS { get; set; }
}

public class FatPrazo
{
    public int IDFAIXA { get; set; }
    public int VALOR { get; set; }
    public int TRANSACOES { get; set; }
    public int PARCELAS { get; set; }
    public int PRAZOMEDIO { get; set; }
}

public class Produto
{
    public int CODIGO { get; set; }
    public string DESCRICAO { get; set; }
    public string LINHA { get; set; }
}

public class TopFornecedor
{
    public string CNPJ { get; set; }
    public double VRFATURAMENTO { get; set; }
    public double PARTICIPACAO { get; set; }
    public int TRANSACOES { get; set; }
    public double TICKETMEDIO { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public string FORNECEDOR { get; set; }
    public int PRODUTOS { get; set; }
    public int PARCELAS { get; set; }
    public int PRAZOMEDIO { get; set; }
    public string TOPLINHAS { get; set; }
}

public class FatUF
{
    public int ID { get; set; }
    public string UFDEST { get; set; }
    public double VRFATURAMENTO { get; set; }
    public double PARTICIPACAO { get; set; }
    public int TRANSACOES { get; set; }
    public double TICKETMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public string ESTADO { get; set; }
    public int PRODUTOS { get; set; }
    public string TOPLINHAS { get; set; }
    public string ANOANTERIORX { get; set; }
}

public class ProdGeral
{
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public int TRANSACOESPRODUTOS { get; set; }
    public int PRODUTOS { get; set; }
    public double TICKETMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
}

public class ProdRanking
{
    public int CODIGO { get; set; }
    public string UNESTOQUE { get; set; }
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public int TRANSACOESPRODUTO { get; set; }
    public double TICKETMEDIO { get; set; }
    public double QUANTIDADE { get; set; }
    public double PRECOMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public string PRODUTO { get; set; }
    public string TOPFORNECEDORES { get; set; }
    public List<ProdutoTopFornecedor> LISTAFORNECEDORES { get; set; }

}

public class FiltroProduto
{
    public int CODIGO { get; set; }
    public string DESCRICAO { get; set; }
}


public class ProdutoCompras
{
    public int CODIGO { get; set; }
    public string UNIDADE { get; set; }
    public string LINHA { get; set; }
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public int TRANSACOESPRODUTO { get; set; }
    public double TICKETMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public double PRECOMEDIO { get; set; }
    public double PRECOMENOR { get; set; }
    public double VRBONIFICACAO { get; set; }
    public double QTBONIFICACAO { get; set; }
    public double QUANTIDADE { get; set; }
    public string PRODUTO { get; set; }
    public string TOPCOMPRAS { get; set; }
    public string TOPFORNECEDORES { get; set; }
}
    
public class ProdutoTopFornecedor
{
    public string CNPJ { get; set; }
    public string FORNECEDOR { get; set; }
    public double VRFATURAMENTO { get; set; }
    public double QUANTIDADE { get; set; }
    public string UNESTOQUE { get; set; }
    public int TRANSACOES { get; set; }
    public int TRANSACOESPRODUTO { get; set; }
    public double TICKETMEDIO { get; set; }
    public double PRECOMEDIO { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
}

public class ProdVariacao
{
    public int CODIGO { get; set; }
    public string UNESTOQUE { get; set; }
    public double VRFATURAMENTO { get; set; }
    public int TRANSACOES { get; set; }
    public int TRANSACOESPRODUTO { get; set; }
    public double TICKETMEDIO { get; set; }
    public double QUANTIDADE { get; set; }
    public double PRECOMEDIO { get; set; }
    public int FORNECEDORES { get; set; }
    public int CLIENTES { get; set; }
    public int UFS { get; set; }
    public string PRODUTO { get; set; }
    public double PTVARIACAO { get; set; }
    
    public string TOPFORNECEDORES { get; set; }
    public List<ProdutoTopFornecedor> LISTAFORNECEDORES { get; set; }
}
