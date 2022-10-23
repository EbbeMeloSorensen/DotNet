using Microsoft.AspNetCore.Mvc;
using Glossary.Domain.Entities;
using Glossary.Web.Application.Records;

namespace Glossary.Web.API.Controllers;

public class RecordsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRecords([FromQuery] RecordParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecord(Guid id)
    {
        return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecord(Record record)
    {
        return HandleResult(await Mediator.Send(new Create.Command { Record = record }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditRecord(Guid id, Record record)
    {
        record.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command { Record = record }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecord(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }
}
