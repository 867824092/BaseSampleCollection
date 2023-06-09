using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace WriterDemo; 

public class Utf8JsonDemo {
    public static void JsonRun() {
        string jsonString =
            @"{
  ""Date"": ""2019-08-01T00:00:00"",
  ""Temperature"": 25,
  ""Summary"": ""Hot"",
  ""DatesAvailable"": [
    ""2019-08-01T00:00:00"",
    ""2019-08-02T00:00:00""
  ],
  ""TemperatureRanges"": {
      ""Cold"": {
          ""High"": 20,
          ""Low"": -10
      },
      ""Hot"": {
          ""High"": 60,
          ""Low"": 20
      }
  }
}
";
        // Create a JsonNode DOM from a JSON string.
        JsonNode forecastNode = JsonNode.Parse(jsonString)!;

        // Write JSON from a JsonNode
        var options = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(forecastNode!.ToJsonString(options));
        
         // Get value from a JsonNode.
        JsonNode temperatureNode = forecastNode!["Temperature"]!;
        Console.WriteLine($"Type={temperatureNode.GetType()}");
        Console.WriteLine($"JSON={temperatureNode.ToJsonString()}");
        //output:
        //Type = System.Text.Json.Nodes.JsonValue`1[System.Text.Json.JsonElement]
        //JSON = 25

        // Get a typed value from a JsonNode.
        int temperatureInt = (int)forecastNode!["Temperature"]!;
        Console.WriteLine($"Value={temperatureInt}");
        //output:
        //Value=25

        // Get a typed value from a JsonNode by using GetValue<T>.
        temperatureInt = forecastNode!["Temperature"]!.GetValue<int>();
        Console.WriteLine($"TemperatureInt={temperatureInt}");
        //output:
        //Value=25

        // Get a JSON object from a JsonNode.
        JsonNode temperatureRanges = forecastNode!["TemperatureRanges"]!;
        Console.WriteLine($"Type={temperatureRanges.GetType()}");
        Console.WriteLine($"JSON={temperatureRanges.ToJsonString()}");
        //output:
        //Type = System.Text.Json.Nodes.JsonObject
        //JSON = { "Cold":{ "High":20,"Low":-10},"Hot":{ "High":60,"Low":20} }

        // Get a JSON array from a JsonNode.
        JsonNode datesAvailable = forecastNode!["DatesAvailable"]!;
        Console.WriteLine($"Type={datesAvailable.GetType()}");
        Console.WriteLine($"JSON={datesAvailable.ToJsonString()}");
        //output:
        //datesAvailable Type = System.Text.Json.Nodes.JsonArray
        //datesAvailable JSON =["2019-08-01T00:00:00", "2019-08-02T00:00:00"]

        // Get an array element value from a JsonArray.
        JsonNode firstDateAvailable = datesAvailable[0]!;
        Console.WriteLine($"Type={firstDateAvailable.GetType()}");
        Console.WriteLine($"JSON={firstDateAvailable.ToJsonString()}");
        //output:
        //Type = System.Text.Json.Nodes.JsonValue`1[System.Text.Json.JsonElement]
        //JSON = "2019-08-01T00:00:00"

        // Get a typed value by chaining references.
        int coldHighTemperature = (int)forecastNode["TemperatureRanges"]!["Cold"]!["High"]!;
        Console.WriteLine($"TemperatureRanges.Cold.High={coldHighTemperature}");
        //output:
        //TemperatureRanges.Cold.High = 20

        // Parse a JSON array
        var datesNode = JsonNode.Parse(@"[""2019-08-01T00:00:00"",""2019-08-02T00:00:00""]");
        JsonNode firstDate = datesNode![0]!.GetValue<DateTime>();
        Console.WriteLine($"firstDate={ firstDate}");
    }

    public static void Utf8JsonWrite() {
        var options = new JsonWriterOptions()
        { Indented = true };
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream,options);
        writer.WriteStartObject();
        {
            // writer.WriteString("date",DateTimeOffset.UtcNow);
            // writer.WriteNumber("temp",42);
        }
        {
            writer.WriteStartArray("defaultJsonFormatting");
            foreach (double number in new double[]{50.4,51}) {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
                writer.WriteNumberValue(number);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            
            writer.WriteStartArray("customJsonFormatting");
            foreach (double result in new double[] { 50.4, 51 })
            {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
                writer.WriteRawValue(
                    FormatNumberValue(result), skipInputValidation: true);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            
            writer.WriteStartArray("array");
            foreach (double result in new double[]
                     { 50.4, 51 }) {
                writer.WriteRawValue(FormatNumberValue(result));
            }
            writer.WriteEndArray();
        }
        writer.WriteEndObject();
        writer.Flush();

        Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
    }

    public static void Uft8JsonReade() {
     byte[] s_nameUtf8 = "name"u8.ToArray();
     ReadOnlySpan<byte> Utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
     // ReadAllBytes if the file encoding is UTF-8:
     string fileName = "UniversitiesUtf8.json";
     ReadOnlySpan<byte> jsonReadOnlySpan = File.ReadAllBytes(fileName);

     // Read past the UTF-8 BOM bytes if a BOM exists.
     if (jsonReadOnlySpan.StartsWith(Utf8Bom))
     {
         jsonReadOnlySpan = jsonReadOnlySpan.Slice(Utf8Bom.Length);
     }

     // Or read as UTF-16 and transcode to UTF-8 to convert to a ReadOnlySpan<byte>
     //string fileName = "Universities.json";
     //string jsonString = File.ReadAllText(fileName);
     //ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(jsonString);

     int count = 0;
     int total = 0;

     var reader = new Utf8JsonReader(jsonReadOnlySpan);

     while (reader.Read())
     {
         JsonTokenType tokenType = reader.TokenType;

         switch (tokenType)
         {
             case JsonTokenType.StartObject:
                 total++;
                 break;
             case JsonTokenType.PropertyName:
                 if (reader.ValueTextEquals(s_nameUtf8))
                 {
                     // Assume valid JSON, known schema
                     reader.Read();
                     if (reader.GetString()!.EndsWith("University"))
                     {
                         count++;
                     }
                 }
                 break;
         }
     }
     Console.WriteLine($"{count} out of {total} have names that end with 'University'");
    }
    
    static string FormatNumberValue(double numberValue)
    {
        return numberValue == Convert.ToInt32(numberValue) ? 
            numberValue.ToString() + ".0" : numberValue.ToString();
    }
}