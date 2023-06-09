using System.Text;

LogWriter logWriter = new LogWriter();
logWriter.WriteLine("123456");


class LogWriter : TextWriter {
  public override Encoding Encoding => Encoding.Default;

  public override void WriteLine(string? value) {
    Console.WriteLine(value);
  }
}