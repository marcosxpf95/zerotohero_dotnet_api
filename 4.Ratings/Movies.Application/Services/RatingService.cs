using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;

    public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieASync(Guid movieId, int rating, Guid userId = default, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException("Rating must be between 1 and 5.");
        }

        var movieExists = await _movieRepository.ExistsByIdAsync(movieId);
        if (!movieExists)
        {
            return false;
        }

        return await _ratingRepository.ReateMovieAsync(movieId, rating, userId, token);
    }

    public Task<bool> DeleteRatingASync(Guid movieId, Guid userId, CancellationToken token)
    {
        return _ratingRepository.DeleteRatingASync(movieId, userId, token);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, token);
    }
}
