using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieASync(Guid id, int rating, Guid userId = default, CancellationToken token = default);
    Task<bool> DeleteRatingASync(Guid movieId, Guid userId, CancellationToken token);
    Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
