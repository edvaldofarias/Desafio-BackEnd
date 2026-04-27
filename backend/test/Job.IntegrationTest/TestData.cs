namespace Job.IntegrationTest;

internal static class TestData
{
    private static readonly Random Rng = new();

    /// <summary>
    /// Gera uma CNH válida (11 dígitos) compatível com Job.Domain.Commons.CnhValidation.
    /// </summary>
    public static string GenerateValidCnh()
    {
        while (true)
        {
            var prefix = string.Concat(Enumerable.Range(0, 9).Select(_ => Rng.Next(0, 10).ToString()));
            // evita "todos os dígitos iguais"
            if (prefix.Distinct().Count() == 1) continue;

            var d1 = GerarDigitoVerificador(prefix, false);
            var diferencial = 0;
            if (d1 == 10) { d1 = 0; diferencial = 2; }

            var d2 = GerarDigitoVerificador(prefix, true);
            d2 = d2 == 10 ? 0 : d2 - diferencial;
            if (d2 < 0) continue;

            return prefix + d1 + d2;
        }
    }

    private static int GerarDigitoVerificador(string digitos, bool crescente)
    {
        var soma = 0;
        var multiplicador = crescente ? 1 : 9;
        for (var i = 0; i < digitos.Length; i++)
        {
            soma += int.Parse(digitos.Substring(i, 1)) * multiplicador;
            multiplicador += crescente ? 1 : -1;
        }
        return soma % 11;
    }
}
