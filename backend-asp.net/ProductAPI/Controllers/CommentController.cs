using ProductAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Dtos.Comment;
using ProductAPI.Interfaces;
using ProductAPI.Model;

namespace ProductAPI.Controllers;

[Route("/comment")]
[ApiController]
public class CommentController : Controller
{
    private readonly ICommentRepository _commentRepo;
    private readonly IProductRepository _productRepo;

    public CommentController(ICommentRepository commentRepository, IProductRepository productRepository)
    {
        _commentRepo = commentRepository;
        _productRepo = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await _commentRepo.getAllAsync();

        // Select :  convert list of GameModel to IEnumerable of GetGameDto
        IEnumerable<GetCommentDto> commentDto = comments.Select(s => s.ToGetCommentDto());

        return Ok(commentDto);
    }

    [HttpPost]
    public async Task<IActionResult> PostSingleComment([FromBody] PostCommentDto postCommentDto)
    {
        // create new Game from postGameDto
        bool gameExist = await _productRepo.productExistAsync(postCommentDto.ProductId);
        if (!gameExist)
        {
            return BadRequest("Product does not exist");
        }

        CommentModel newComment = await _commentRepo.createAsync(postCommentDto);

        return Ok(newComment);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateSingleComment([FromRoute] Guid commentId, [FromBody] PutCommentDto putCommentDto)
    {

        CommentModel? commentModel = await _commentRepo.updateAsync(commentId, putCommentDto);

        if (commentModel == null)
        {
            return NotFound();
        }

        return Ok(commentModel.ToGetCommentDto());
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteSingleComment([FromRoute] Guid id)
    {

        CommentModel? commentModel = await _commentRepo.deleteAsync(id);

        if (commentModel == null)
        {
            return NotFound();
        }

        return NoContent();
    }


}
