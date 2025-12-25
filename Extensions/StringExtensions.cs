namespace MedicalTriageSystem.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
        }

        public static string FormatPhoneNumber(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 10)
            {
                return $"{digits.Substring(0, 2)} {digits.Substring(2, 2)} {digits.Substring(4, 2)} {digits.Substring(6, 2)} {digits.Substring(8, 2)}";
            }

            return phone;
        }

        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static string ToInitials(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return "?";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }

            return name[0].ToString().ToUpper();
        }

        public static string MaskEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return email;

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return $"{username[0]}***@{domain}";

            return $"{username[0]}***{username[^1]}@{domain}";
        }
    }
}