using Microsoft.AspNetCore.Mvc;
using Ray.Notes.RE.Entity;
using Ray.Notes.RE.Repository;
using RulesEngine.Models;

namespace Ray.Notes.RE.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static string json = @"
  {
    ""WorkflowName"": ""ActivedVehicleCount"",
    ""Rules"": [
      {
        ""RuleName"": ""MoreThanZero"",
        ""Expression"": ""count > 0""
      },
      {
        ""RuleName"": ""MoreThanOne"",
        ""Expression"": ""count > 1""
      }
    ]
  }
";

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public bool Get()
    {
        var count = 1;

        var workflowRules = new List<string> { json };
        var re = new RulesEngine.RulesEngine(workflowRules.ToArray());
        var rp = new RuleParameter("count", count);

        var resultList = re.ExecuteAllRulesAsync("ActivedVehicleCount", rp).Result;

        return resultList
            .First(x => x.Rule.RuleName == "MoreThanZero")
            .IsSuccess;
    }

    [HttpPost]
    [Route("Test")]
    public bool Test([FromHeader] int count)
    {
        ConditionEntity condition = new ConditionRepository().Get(1);

        List<Rule> rules = new List<Rule>();

        Rule rule = new Rule();
        rule.RuleName = condition.Code;
        rule.Expression = condition.ExpressionStr;
        rule.RuleExpressionType = RuleExpressionType.LambdaExpression;
        rules.Add(rule);

        var workflows = new List<Workflow>();

        Workflow exampleWorkflow = new Workflow();
        exampleWorkflow.WorkflowName = condition.Type.ToString();
        exampleWorkflow.Rules = rules;

        workflows.Add(exampleWorkflow);

        var bre = new RulesEngine.RulesEngine(workflows.ToArray());

        var rp = new RuleParameter("count", count);

        var resultList = bre.ExecuteAllRulesAsync(condition.Type.ToString(), rp).Result;

        return resultList
            .First(x => x.Rule.RuleName == condition.Code)
            .IsSuccess;
    }
}
