using Balo.Data.DataAccess;
using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Mapster;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Service.Core
{
    public interface IBoardService
    {
        Task<ResultModel> Get(Guid id);
        Task<ResultModel> AddAsync(BoardCreateModel model);
        Task Update(Guid id, Board model);
        Task Delete(Guid id);

        Task<PagingModel<Board>> GetPagingData(int? pageIndex = 0, int? pageSize = 10);
    }
    public class BoardService : IBoardService
    {
        private readonly AppDbContext _dbContext;

        public BoardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> AddAsync(BoardCreateModel model)
        {
            var resultModel = new ResultModel();
            try
            {
                //var board = new Board { Name = model.Name, Description = model.Description };
                var board = model.Adapt<Board>();
                await _dbContext.Boards.InsertOneAsync(board);
                resultModel.Succeed = true;
                resultModel.Data = board.Id;

            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }

            return resultModel;
        }

        public async Task Delete(Guid id)
        {
            var deleteFilter = Builders<Board>.Filter.Eq(x => x.Id, id);
            await _dbContext.Boards.DeleteOneAsync(deleteFilter);
        }

        public async Task<ResultModel> Get(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var board = await _dbContext.Boards.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (board == null)
                {
                    throw new Exception("Cant find any board!");
                }
                result.Data = board;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public async Task Update(Guid id, Board model)
        {
            await _dbContext.Boards.ReplaceOneAsync(x => x.Id == id, model);
        }

        public async Task<PagingModel<Board>> GetPagingData(int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<Board>.Filter.Empty;
            var query = _dbContext.Boards.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<Board>
            {
                TotalPages = (int)Math.Ceiling((double)count / pageSize.Value),
                TotalRows = count,
                Data = await query
                        .Skip(pageIndex * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
            };
        }
    }
}
