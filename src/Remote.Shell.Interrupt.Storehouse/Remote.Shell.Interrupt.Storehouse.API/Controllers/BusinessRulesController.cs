using Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;
using Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class BusinessRulesController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
    ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<BusinessRule>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetBusinessRules(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetBusinessRulesQuery(),
                             cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(BusinessRule), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetBusinessRuleById(Guid id,
                                                       CancellationToken cancellationToken)
  => Ok(await _sender.Send(new GetBusinessRuleByExpressionQuery(x => x.Id == id),
                           cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateBusinessRule([FromBody] CreateBusinessRuleDTO createBusinessRuleDTO,
                                                      CancellationToken cancellationToken)
    => Ok(await _sender.Send(new CreateBusinessRuleCommand(createBusinessRuleDTO),
                             cancellationToken));

  [HttpPut("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateBusinessRule(Guid id,
                                                      [FromBody] UpdateBusinessRule updateBusinessRule,
                                                      CancellationToken cancellationToken)
    => Ok(await _sender.Send(new UpdateBusinessRuleCommand(id,
                                                           updateBusinessRule),
                             cancellationToken));

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteBusinessRuleById(Guid id,
                                                          CancellationToken cancellationToken)
    => Ok(await _sender.Send(new DeleteBusinessRuleByExpressionCommand(x => x.Id == id),
                             cancellationToken));
}
