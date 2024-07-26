using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<bool> ReateMovieAsync(Guid movieId, int rating, Guid userId= default, CancellationToken token = default);
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default);
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId = default, CancellationToken token = default);
    Task<bool> DeleteRatingASync(Guid movieId, Guid userId, CancellationToken token);
    Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
