using System.Text;

namespace ConsoleApp;

public class Product
{
    public string Link { get; set; }
    public string Title { get; set; }   
    public string Price { get; set; }


    public override string ToString()
    {
        return GetType().GetProperties()
        .Select(info => (info.Name, Value: info.GetValue(this, null) ?? "(null)"))
        .Aggregate(
            new StringBuilder(),
            (sb, pair) => sb.AppendLine($"{pair.Name}: {pair.Value}"),
            sb => sb.ToString());
    }
}
