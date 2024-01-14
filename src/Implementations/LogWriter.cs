using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestReponseInterceptor.Implementations;

public class LogWriter
{
    public void WriteLog(string logMessage)
    {
        // Obtenha o diretório corrente
        string currentDirectory = Directory.GetCurrentDirectory();

        // Crie um subdiretório para o mês atual
        string monthDirectory = Path.Combine(currentDirectory, "logs");

        // Verifique se o subdiretório já existe, se não, crie
        if (!Directory.Exists(monthDirectory))
        {
            Directory.CreateDirectory(monthDirectory);
        }

        // Crie um nome de arquivo com base na data e hora atual
        string fileName = Path.Combine(monthDirectory, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");

        // Escreva o log no arquivo, fazendo um append ao conteúdo existente
        File.AppendAllText(fileName, logMessage);
    }
}
