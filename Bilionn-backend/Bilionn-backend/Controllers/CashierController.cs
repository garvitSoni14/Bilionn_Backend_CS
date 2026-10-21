namespace Bilionn_backend.Controllers;

using System.Security.Claims;
using Bilionn_backend.DTOs.Cashiers;
using Bilionn_backend.Models;
using Bilionn_backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

[ApiController]
[Route("api/admin/cashiers")]
[Authorize(Roles = AppRoles.Admin)]
public class CashiersController : ControllerBase
{
    private readonly ICashierService _service;
    private readonly IValidator<CreateCashierRequest> _createValidator;
    private readonly IValidator<UpdateCashierRequest> _updateValidator;

    public CashiersController(
        ICashierService service,
        IValidator<CreateCashierRequest> createValidator,
        IValidator<UpdateCashierRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCashierRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        //var adminUserId = User.FindFirst("sub")?.Value;   // the logged-in admin, from the JWT
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);   // the logged-in admin, from the cookie
        if (adminUserId is null) return Unauthorized();

        var created = await _service.CreateAsync(request, adminUserId);
        return CreatedAtAction(nameof(GetByCode), new { employeeCode = created.EmployeeCode }, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{employeeCode}")]
    public async Task<ActionResult<EmployeeResponse>> GetByCode(string employeeCode) =>
        Ok(await _service.GetByCodeAsync(employeeCode));

    [HttpPut("{employeeCode}")]
    public async Task<IActionResult> Update(string employeeCode, UpdateCashierRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

        return Ok(await _service.UpdateAsync(employeeCode, request));
    }

    [HttpDelete("{employeeCode}")]
    public async Task<IActionResult> Delete(string employeeCode)
    {
        await _service.DeleteAsync(employeeCode);
        return NoContent();
    }
}
