namespace BaseBot;

public class LogService
{
    private object _lock = new object();

    public void AddLog(string mes)
    {
        lock (_lock)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now} -- {mes}\r\n");
        }
    }
}