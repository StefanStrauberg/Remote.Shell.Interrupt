using System.Text;

namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

public class GenericRepository<TDocument> : IGenericRepository<TDocument> where TDocument : BaseEntity
{
  readonly IMongoCollection<TDocument> _collection;

  public GenericRepository(IMongoDbSettings settings)
  {
    var client = new MongoClient(settings.ConnectionString);
    var database = client.GetDatabase(settings.DatabaseName);
    var sb = new StringBuilder();

    sb.Append(typeof(TDocument).Name);
    sb.Append('s');

    var collectionName = sb.ToString();

    _collection = database.GetCollection<TDocument>(name: collectionName);
  }

  public virtual async Task DeleteManyAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                            CancellationToken cancellationToken)
    => await _collection.DeleteManyAsync(filter: filterExpression,
                                         cancellationToken: cancellationToken);

  public virtual async Task DeleteOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                           CancellationToken cancellationToken)
    => await _collection.FindOneAndDeleteAsync(filter: filterExpression,
                                               cancellationToken: cancellationToken);

  public virtual async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                              CancellationToken cancellationToken)
    => await _collection.CountDocumentsAsync(filter: filterExpression,
                                             cancellationToken: cancellationToken) > 0;

  public async virtual Task<TDocument> FindOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                                    CancellationToken cancellationToken)
    => await _collection.Find(filter: filterExpression)
                        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

  public virtual async Task<List<TDocument>> GetAllAsync(CancellationToken cancellationToken)
   => await _collection.Find(filter: FilterDefinition<TDocument>.Empty)
                       .ToListAsync(cancellationToken: cancellationToken);

  public async virtual Task InsertManyAsync(IEnumerable<TDocument> documents,
                                            CancellationToken cancellationToken)
    => await _collection.InsertManyAsync(documents: documents,
                                         options: null,
                                         cancellationToken: cancellationToken);

  public virtual async Task InsertOneAsync(TDocument document,
                                     CancellationToken cancellationToken)
    => await _collection.InsertOneAsync(document: document,
                                        options: null,
                                        cancellationToken: cancellationToken);

  public virtual async Task ReplaceOneAsync(System.Linq.Expressions.Expression<Func<TDocument, bool>> filterExpression,
                                      TDocument document,
                                      CancellationToken cancellationToken)
    => await _collection.ReplaceOneAsync(filter: filterExpression, replacement: document,
                                        options: new ReplaceOptions { IsUpsert = true },
                                        cancellationToken: cancellationToken);
}
