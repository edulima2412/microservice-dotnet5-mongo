using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
  [ApiController]
  [Route("api/items")]
  public class ItemsController : ControllerBase
  {
    private readonly IItemsRepository _repository;

    public ItemsController(IItemsRepository repository)
    {
      _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
    {
      var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());

      return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
    {
      var item = await _repository.GetItemAsync(id);
      if (item is null)
        return NotFound();

      return Ok(item.AsDto());
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
    {
      Item item = new()
      {
        Id = Guid.NewGuid(),
        Name = itemDto.Name,
        Price = itemDto.Price,
        CreatedDate = DateTimeOffset.UtcNow
      };

      await _repository.CreateItemAsync(item);

      return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemDto>> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
    {
      var existingItem = await _repository.GetItemAsync(id);
      if (existingItem is null)
        return NotFound();

      Item updateItem = existingItem with
      {
        Name = itemDto.Name,
        Price = itemDto.Price,
      };

      await _repository.UpdateItemAsync(updateItem);

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ItemDto>> DeleteItemAsync(Guid id)
    {
      var existingItem = await _repository.GetItemAsync(id);
      if (existingItem is null)
        return NotFound();

      await _repository.DeleteItemAsync(id);

      return NoContent();
    }
  }
}
