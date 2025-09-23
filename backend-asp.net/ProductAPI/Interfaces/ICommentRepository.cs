using ProductAPI.Dtos.Comment;
using ProductAPI.Model;

namespace ProductAPI.Interfaces;

public interface ICommentRepository
{
     Task<List<CommentModel>> getAllAsync();

    // note : "?" => meaning could be null, in case not found
    Task<CommentModel?> getByIdAsync(Guid id);
    Task<CommentModel> createAsync(PostCommentDto postCommentDto);
    
    Task<CommentModel?> updateAsync(Guid id, PutCommentDto putCommentDto);
    Task<CommentModel?> deleteAsync(Guid id);
}
