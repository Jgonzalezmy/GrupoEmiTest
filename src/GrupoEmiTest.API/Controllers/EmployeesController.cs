using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace GrupoEmiTest.API.Controllers;

/// <summary>
/// Manages CRUD operations and paginated queries for the Employee resource.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initialises a new instance of <see cref="EmployeesController"/>.
    /// </summary>
    /// <param name="employeeService">The service that handles employee business logic.</param>
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Returns a keyset-paginated page of employees with full related data.
    /// Pass the <c>cursor</c> value from the previous response to advance to the next page.
    /// </summary>
    /// <param name="cursor">The ID of the last employee seen. Omit or pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of employees per page (default: 10).</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>200 OK</c> with a <see cref="PagedResult{T}"/> containing employees and pagination metadata.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? cursor,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetAllAsync(
            new PageRequest(pageSize, cursor),
            cancellationToken);

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves a single employee by ID including department, position history, and projects.
    /// </summary>
    /// <param name="id">The employee's primary key.</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>200 OK</c> with the <see cref="Application.DTOs.Response.EmployeeResponse"/>.<br/>
    /// <c>404 Not Found</c> if no employee with the given ID exists.
    /// </returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] IdRequest idRequest, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetByIdAsync(idRequest.Id);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="request">The employee data.</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>201 Created</c> with the created <see cref="Application.DTOs.Response.EmployeeResponse"/>.<br/>
    /// <c>400 Bad Request</c> if validation fails.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.CreateAsync(request);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="request">The updated employee data.</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>200 OK</c> with the updated <see cref="Application.DTOs.Response.EmployeeResponse"/>.<br/>
    /// <c>400 Bad Request</c> if validation fails.<br/>
    /// <c>404 Not Found</c> if the employee does not exist.
    /// </returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] IdRequest idRequest, [FromBody] EmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.UpdateAsync(
            id: idRequest.Id,
            request: request);

        if (result.IsFailure)
        {
            return result.Error.Type == GrupoEmiTest.Domain.Enums.ErrorType.NotFound
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Returns a keyset-paginated page of employees that belong to the specified department
    /// and are assigned to at least one project.
    /// Pass the <c>cursor</c> value from the previous response to advance to the next page.
    /// </summary>
    /// <param name="departmentIdRequest">The department's primary key.</param>
    /// <param name="cursor">The ID of the last employee seen. Omit or pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of employees per page (default: 10).</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>200 OK</c> with a <see cref="PagedResult{T}"/> containing employees and pagination metadata.<br/>
    /// <c>400 Bad Request</c> if <paramref name="departmentIdRequest"/> is invalid.
    /// </returns>
    [HttpGet("by-department/{departmentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDepartment(
        [FromRoute] IdRequest departmentIdRequest,
        [FromQuery] int? cursor,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.GetByDepartmentWithProjectsAsync(
            departmentId: departmentIdRequest.Id,
            request: new PageRequest(pageSize, cursor),
            cancellationToken: cancellationToken);

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes an employee by ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <param name="cancellationToken">Propagated by the framework when the client disconnects.</param>
    /// <returns>
    /// <c>204 No Content</c> on success.<br/>
    /// <c>404 Not Found</c> if the employee does not exist.
    /// </returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] IdRequest idRequest, CancellationToken cancellationToken = default)
    {
        var result = await _employeeService.DeleteAsync(idRequest.Id);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return NoContent();
    }


}
