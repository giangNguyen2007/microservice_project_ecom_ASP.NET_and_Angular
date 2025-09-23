using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Dtos;
using ProductAPI.Dtos.Comment;
using ProductAPI.Interfaces;
using ProductAPI.Model;

namespace ProductAPI.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly ProductDBContext _context;

     public CommentRepository(ProductDBContext context)
    {
        _context = context;
    }

  

    public async Task<List<CommentModel>> getAllAsync()
    {
        return await _context.Comments.ToListAsync();
    }

    public async Task<CommentModel?> getByIdAsync(Guid id)
    {
        CommentModel? commentModel = await _context.Comments.FindAsync(id);
        return commentModel;
    }
    
    public async Task<CommentModel> createAsync(PostCommentDto postCommentDto)
    {
        CommentModel newComment = postCommentDto.ToCommentModel();
        await _context.Comments.AddAsync(newComment);
        await _context.SaveChangesAsync();

        return newComment;
    }

 

    public async Task<CommentModel?> updateAsync(Guid commentId, PutCommentDto putCommentDto)
    {
        CommentModel? commentModel = await _context.Comments.FirstOrDefaultAsync(g => g.Id == commentId);
        if (commentModel == null)
        {
            return null;
        }

        if (commentModel.Content != null)
        {
            commentModel.Content = putCommentDto.Content;
        }

        if (commentModel.Rating != null)
        {
            commentModel.Rating = putCommentDto.Rating;
        }

        await _context.SaveChangesAsync();

        return commentModel;
    }
    
    public async Task<CommentModel?> deleteAsync(Guid id)
    {
        CommentModel? commentModel = await _context.Comments.FirstOrDefaultAsync(g => g.Id == id);
        if (commentModel == null)
        {
            return null;
        }

        // Note: remove is not an async function, leave it like this
        _context.Comments.Remove(commentModel);
        await _context.SaveChangesAsync();

        return commentModel;
    }
    
    
    
    
}
