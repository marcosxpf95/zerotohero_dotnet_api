
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> ReateMovieAsync(Guid movieId, int rating, Guid userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(new CommandDefinition(""" 
            insert into ratings(userId, movieId, rating)
            values (@userId, @movieId, @rating)
            on conflict (userId, movieId) do update
                set rating = @rating
            """, new { userId, movieId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(""" 
            select round(avg(r.rating), 1) from ratings r
            where movieId = @movieId
            """, new { movieId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId = default, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition(""" 
            select round(avg(r.rating), 1),
                   (select rating
                   from rating
                   where movieid = @movieId
                        and userId = @userId
                   limit 1)
            from ratings
            where movieId = @movieId
            """, new { movieId, userId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingASync(Guid movieId, Guid userId, CancellationToken token)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        var result = await connection.ExecuteAsync(new CommandDefinition(""" 
            delete from ratings
            where movieid = @movieId
            and userid = @userId
            """, new { userId, movieId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);

        return await connection.QueryAsync<MovieRating>(new CommandDefinition(""" 
            select r.rating, r.movieid, m.slug
            from ratings
            inner join movies m on r.movieid = m.id
            where userid = @userId
            """, new { userId }, cancellationToken: token));
    }
}
