using Common.Domain;
using Common.Protocol;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class AdminGrpcService : Admin.AdminBase
    {
        private readonly ILogger<AdminGrpcService> _logger;
        public AdminGrpcService(ILogger<AdminGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<MessageReply> PostGame(GameDTO request, ServerCallContext context)
        {
            BusinessLogicGameCRUD cud = BusinessLogicGameCRUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            try
            {
                Game game = new Game()
                {
                    Title = request.Title,
                    ESRBRating = (Common.ESRBRating)request.EsrbRating,
                    CoverFilePath = request.CoverFilePath,
                    Genre = request.Genre,
                    Publisher = utils.GetUser(request.PublisherId),
                    Synopsis = request.Synopsis,
                    Reviews = new List<Review>()
                };
                string message = cud.PublishGame(game);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (TitleAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<GamesResponseList> GetGames(GamesRequest request, ServerCallContext context)
        {
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            try
            {
                List<Game> games = crud.GetGames();
                List<GameDTO> gamesDto = games.ConvertAll(g => new GameDTO()
                {
                    Id = g.Id,
                    Title = g.Title,
                    Synopsis = g.Synopsis,
                    Genre = g.Genre,
                    EsrbRating = (int)g.ESRBRating,
                    PublisherId = g.Publisher.Id,
                    CoverFilePath = g.CoverFilePath,
                });
                GamesResponseList gamesResp = new GamesResponseList();
                gamesDto.ForEach(g => gamesResp.Games.Add(g));
                return Task.FromResult(gamesResp);
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<GameDTO> GetGameById(Id request, ServerCallContext context)
        {
            BusinessLogicGameCRUD crud = BusinessLogicGameCRUD.GetInstance();
            try
            {
                Game game = crud.GetGameById(request.Id_);
                GameDTO gamedto = new GameDTO
                {
                    Id = game.Id,
                    Title = game.Title,
                    Synopsis = game.Synopsis,
                    Genre = game.Genre,
                    EsrbRating = (int)game.ESRBRating,
                    PublisherId = game.Publisher.Id,
                    CoverFilePath = game.CoverFilePath,
                };
                return Task.FromResult(gamedto);
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<MessageReply> UpdateGame(GameDTO request, ServerCallContext context)
        {
            BusinessLogicGameCRUD cud = BusinessLogicGameCRUD.GetInstance();
            try
            {
                Game game = new Game()
                {
                    Id = request.Id,
                    Title = request.Title,
                    ESRBRating = (Common.ESRBRating)request.EsrbRating,
                    CoverFilePath = request.CoverFilePath,
                    Genre = request.Genre,
                    Synopsis = request.Synopsis
                };
                string message = cud.ModifyGame(request.Id, game);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (TitleAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<MessageReply> DeleteGame(Id request, ServerCallContext context)
        {
            BusinessLogicGameCRUD cud = BusinessLogicGameCRUD.GetInstance();
            bool couldDelete = cud.DeleteGame(request.Id_);
            string message = couldDelete ? "Juego eliminado corretamente" : "No se pudo eliminar el juego";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            string message = session.CreateUser(request.Name);
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<UsersResponseList> GetUsers(UsersRequest request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            try
            {
                List<User> users = session.GetUsers();
                List<UserDTO> usersDto = users.ConvertAll(u => new UserDTO()
                {
                    Id = u.Id,
                    Name = u.Name
                });
                UsersResponseList usersResp = new UsersResponseList();
                usersDto.ForEach(u => usersResp.Users.Add(u));
                return Task.FromResult(usersResp);
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<UserDTO> GetUserById(Id request, ServerCallContext context)
        {
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            try
            {
                User user = utils.GetUser(request.Id_);
                UserDTO userdto = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name
                };
                return Task.FromResult(userdto);
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<MessageReply> UpdateUser(UserDTO request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            try
            {
                User modifiedUser = new User()
                {
                    Id = request.Id,
                    Name = request.Name
                };
                string message = session.ModifyUser(request.Id, modifiedUser);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (NameAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<MessageReply> DeleteUser(Id request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            bool couldDelete = session.DeleteUser(request.Id_);
            string message = couldDelete ? "Usuario eliminado correctamente" : "No se pudo eliminar usuario";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> AssociateGameWithUser(Purchase request, ServerCallContext context)
        {
            BusinessLogicGameInfo info = BusinessLogicGameInfo.GetInstance();
            try
            {
                bool couldBuy = info.AssociateGameToUser(request.IdGame, request.IdUser);
                string message = couldBuy ? "Juego comprado correctamente" : "No se pudo comprar juego";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<MessageReply> DisassociateGameWithUser(Purchase request, ServerCallContext context)
        {
            BusinessLogicGameInfo info = BusinessLogicGameInfo.GetInstance();
            try
            {
                bool couldReturn = info.ReturnGame(request.IdGame, request.IdUser);
                string message = couldReturn ? "Se retorno el juego correctamente" : "No se pudo retornar el juego";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }
    }
}
