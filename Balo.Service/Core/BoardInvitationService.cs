using Balo.Data.DataAccess;
using Balo.Data.Enums;
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

    public interface IBoardInvitationService
    {
        Task<ResultModel> AddAsync(string username, BoardInvitationCreate model);
        Task<ResultModel> GetInvitationForUserAsync(string username);
        Task<ResultModel> GetReceivedInvitationForUserAsync(string username);
        Task<PagingModel<BoardInviation>> GetAsync(Guid? id, Guid? receiveId, Guid? senderId, int? pageIndex = 0, int? pageSize = 10);
        Task<ResultModel> PutInvitationStatus(string username, Guid invitationId, InvitationStatus status);
        Task DeleteAsync(Guid? id);
    }
    public class BoardInvitationService : IBoardInvitationService
    {

        private readonly AppDbContext _dbContext;

        public BoardInvitationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResultModel> AddAsync(string username, BoardInvitationCreate model)
        {
            var resultModel = new ResultModel();
            var session = _dbContext.StartSession(); session.StartTransaction();
            try
            {
                var sender = await _dbContext.Users.Find(x => x.UserName == username).FirstOrDefaultAsync();
                var receiver = await _dbContext.Users.Find(x => x.Id == model.ReceiverId).FirstOrDefaultAsync();
                var board = await _dbContext.Boards.Find(x => x.Id == model.BoardId).FirstOrDefaultAsync();
                if (sender != null && receiver != null && board != null)
                {
                    //find board invitation existed in db
                    var exBoardInvitation = await _dbContext.BoardInviations
                        .Find(x => x.Receiver.Id == model.ReceiverId && x.Board.Id == model.BoardId).FirstOrDefaultAsync();
                    if (exBoardInvitation == null)
                    {
                        var invitation = model.Adapt<BoardInviation>();
                        invitation.Status = InvitationStatus.Pending;
                        invitation.Sender = sender;
                        invitation.Receiver = receiver;
                        invitation.Board = board;
                        await _dbContext.BoardInviations.InsertOneAsync(invitation);
                        resultModel.Succeed = true;
                        resultModel.Data = invitation.Id;
                        await session.CommitTransactionAsync();
                    }
                    else
                    {
                        throw new Exception("The invitation already existed");
                    }


                }
                else
                {
                    throw new Exception("Can not find data");
                }


            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }

            return resultModel;
        }

        public async Task DeleteAsync(Guid? id)
        {
            var deleteFilter = Builders<BoardInviation>.Filter.Eq(x => x.Id, id);
            await _dbContext.BoardInviations.DeleteOneAsync(deleteFilter);
        }

        public async Task<PagingModel<BoardInviation>> GetAsync(Guid? id, Guid? receiveId, Guid? senderId, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<BoardInviation>.Filter.Empty;
            if (id != null)
            {
                filters &= Builders<BoardInviation>.Filter.Eq(x => x.Id, id);
            }
            if (receiveId != null)
            {
                filters &= Builders<BoardInviation>.Filter.Eq(x => x.Receiver.Id, receiveId);
            }
            if (senderId != null)
            {
                filters &= Builders<BoardInviation>.Filter.Eq(x => x.Sender.Id, senderId);
            }


            var query = _dbContext.BoardInviations.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<BoardInviation>
            {
                TotalPages = (int)Math.Ceiling((double)count / pageSize.Value),
                TotalRows = count,
                Data = await query
                        .Skip(pageIndex * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
            };
        }

        public async Task<ResultModel> GetInvitationForUserAsync(string username)
        {
            var resultModel = new ResultModel();
            try
            {
                var user = await _dbContext.Users.Find(x => x.UserName == username).FirstOrDefaultAsync();
                if (user != null)
                {
                    var userInvitation = await _dbContext.BoardInviations.Find(x => x.Sender.Id == user.Id).ToListAsync();
                    resultModel.Succeed = true;
                    resultModel.Data = userInvitation;
                }
                else
                {
                    throw new Exception("The user not existed");
                }
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> GetReceivedInvitationForUserAsync(string username)
        {
            var resultModel = new ResultModel();
            try
            {
                var user = await _dbContext.Users.Find(x => x.UserName == username).FirstOrDefaultAsync();
                if (user != null)
                {
                    var userInvitation = await _dbContext.BoardInviations.Find(x => x.Receiver.Id == user.Id).ToListAsync();
                    resultModel.Succeed = true;
                    resultModel.Data = userInvitation;
                }
                else
                {
                    throw new Exception("The user not existed");
                }
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }
            return resultModel;
        }

        public async Task<ResultModel> PutInvitationStatus(string username, Guid invitationId, InvitationStatus status)
        {
            var resultModel = new ResultModel();
            try
            {
                var exInvitation = await _dbContext.BoardInviations.Find(x => x.Id == invitationId).FirstOrDefaultAsync();
                if (exInvitation != null)
                {
                    exInvitation.Status = status;
                    await _dbContext.BoardInviations.ReplaceOneAsync(x => x.Id == invitationId, exInvitation);
                    var exBoard = await _dbContext.Boards.Find(x => x.Id == exInvitation.Board.Id).FirstOrDefaultAsync();
                    if (exBoard != null)
                    {
                        var isMemberAlreadyExist = exBoard.Members.Where(x => x.UserName == username).FirstOrDefault() != null;
                        if (!isMemberAlreadyExist)
                        {
                            if (status == InvitationStatus.Accepted)
                            {
                                var receiverUser = await _dbContext.Users.Find(x => x.Id == exInvitation.Receiver.Id).FirstOrDefaultAsync();
                                var boardMember = receiverUser.Adapt<User>();
                                boardMember.Role = Role.Member;
                                exBoard.Members.Add(boardMember);
                                await _dbContext.Boards.ReplaceOneAsync(x => x.Id == exInvitation.Board.Id, exBoard);
                                resultModel.Succeed = true;
                                resultModel.Data = exInvitation;
                            }
                            else
                            {
                                await _dbContext.BoardInviations.DeleteOneAsync(x => x.Id == invitationId);
                                resultModel.Succeed = true;
                                resultModel.Data = "";
                            }
                        }
                        else
                        {
                            throw new Exception("The board not existed");
                        }

                    }
                    else
                    {
                        throw new Exception("The board not existed");

                    }
                }
                else
                {
                    throw new Exception("The invitation not existed");

                }
            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }
            return resultModel;
        }
    }
}
