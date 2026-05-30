namespace VendinhaUnimar.Utils
{
    public static class DocumentoUtils
    {
        public static string SomenteNumeros(string texto)
        {
            return new string(texto.Where(char.IsDigit).ToArray());
        }

        public static bool CpfValido(string cpf)
        {
            cpf = SomenteNumeros(cpf);

            if (cpf.Length != 11)
            {
                return false;
            }

            if (cpf.All(c => c == cpf[0]))
            {
                return false;
            }

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cpf[9].ToString() == digito1.ToString() && cpf[10].ToString() == digito2.ToString();
        }
    }
}
