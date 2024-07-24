using api.Dtos.comment;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepo.GetAllAsync();
            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
                return NotFound();

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, [FromBody] CreateCommentRequestDto commentDto)
        {
            if (!await _stockRepo.StockExists(stockId))
                return BadRequest("Stock does not exists");

            var commentModel = commentDto.ToCommentFromCreateDto(stockId);
            await _commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdateDto());

            if (comment == null)
                return NotFound("Comment not found");

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentModel = await _commentRepo.DeleteAsync(id);

            if (commentModel == null)
                return NotFound("Comment does not exist");

            return Ok(commentModel);
        }

    }
}