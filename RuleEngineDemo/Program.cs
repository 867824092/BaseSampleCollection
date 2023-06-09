using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RulesEngine.Models;


var re = new RulesEngine.RulesEngine(Deserialize());

var rp1 = new RuleParameter("country","india");
var rp2 = new RuleParameter("loyalityFactor", 1);
var rp3 = new RuleParameter("totalPurchasesToDate",500);
// Declare input1,input2,input3 
var resultList  = await re.ExecuteAllRulesAsync("Discount", rp1,rp2,rp3);

//Check success for rule
foreach(var result in resultList){
    Console.WriteLine($"Rule - {result.Rule.RuleName}, IsSuccess - {result.IsSuccess}");
}

Workflow[] Deserialize() {
    using Stream strem = File.OpenRead("Rules.json");
    return JsonSerializer.Deserialize<Workflow[]>(strem);
}


public class WorkflowModels {
    public string WorkflowName { get; set; }
    public WorkflowRule[] Rules { get; set; }
}

public class WorkflowRule {
    public string RuleName  { get; set; }
    public string Expression { get; set; }
}