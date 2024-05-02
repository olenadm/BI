namespace BIMagistral.Helpers;

public class Calc
{
    public static double VariacaoPercentual(double atual, double anterior)
    {
        if (anterior == 0) return 100;
        var variacao = atual / anterior - 1;
        return variacao * 100;
    }

    public static double VariacaoAbsoluta(double atual, double anterior)
    {
        return atual - anterior;
    }
}