using shared.Entities;

namespace console_consumer.Helpers
{
    public static class CsvParser
    {
        public static List<User> ParseUsers(string[] lines)
        {
            var users = new List<User>();

            foreach (var line in lines.Skip(1)) //pulando o cabeçalho do arquivo
            {
                var fields = line.Split(',');

                if (fields.Length < 6) continue;

                var user = new User
                {
                    FirstName = fields[0],
                    LastName = fields[1],
                    Age = int.TryParse(fields[2], out var age) ? age : 0,
                    Email = fields[3],
                    City = fields[4],
                    State = fields[5],
                    Enable = bool.TryParse(fields[6], out var enable) && enable
                };

                users.Add(user);
            }

            return users;
        }
    }
}
