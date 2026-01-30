namespace SalesPro.Domain.Validation;

public static class EcuadorCedula
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var ced = value.Trim();
        if (ced.Length != 10) return false;
        if (!ced.All(char.IsDigit)) return false;

        var province = int.Parse(ced.Substring(0, 2));
        if (province < 1 || province > 24) return false;

        var third = ced[2] - '0';
        if (third > 5) return false;

        int[] coef = [2, 1, 2, 1, 2, 1, 2, 1, 2];
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = ced[i] - '0';
            int p = d * coef[i];
            if (p >= 10) p -= 9;
            sum += p;
        }

        int mod = sum % 10;
        int check = (mod == 0) ? 0 : 10 - mod;

        return check == (ced[9] - '0');
    }
}
