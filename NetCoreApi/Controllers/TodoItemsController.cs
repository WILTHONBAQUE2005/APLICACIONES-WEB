using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreApi.Data;
using NetCoreApi.Models;

namespace NetCoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TodoItemsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetAll(CancellationToken ct)
    {
        var items = await _db.TodoItems
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct);

        return items;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id, CancellationToken ct)
    {
        var item = await _db.TodoItems.FindAsync([id], ct);
        if (item is null)
        {
            return NotFound();
        }

        return item;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItemCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title es requerido.");
        }

        var item = new TodoItem
        {
            Title = request.Title.Trim()
        };

        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoItem>> Update(int id, TodoItemUpdateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title es requerido.");
        }

        var item = await _db.TodoItems.FindAsync([id], ct);
        if (item is null)
        {
            return NotFound();
        }

        item.Title = request.Title.Trim();
        item.IsDone = request.IsDone;

        await _db.SaveChangesAsync(ct);

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await _db.TodoItems.FindAsync([id], ct);
        if (item is null)
        {
            return NotFound();
        }

        _db.TodoItems.Remove(item);
        await _db.SaveChangesAsync(ct);

        return NoContent();
    }
}

public record TodoItemCreateRequest(string Title);
public record TodoItemUpdateRequest(string Title, bool IsDone);
