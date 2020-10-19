using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MultiplayerSeeChess.Game;
using MultiplayerSeeChess.Game.Models;
using MultiplayerSeeChess.Models;
using MultiplayerSeeChess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {

        private IGameService gameService;
        private const char PlayerOneSym = 'X';
        private const char PlayerTwoSym = 'O';
        public GameHub(IGameService gameService)
        {
            this.gameService = gameService;
        }


        public async Task Send(string massage)
        {
            await this.Clients.All.SendAsync("NewMessage",
                          new Massage() { Content = massage });

        }

        public async Task Turn(string gameId, string cordinates)
        {
            var token = cordinates.Split("-");
            var x = byte.Parse(token[0]);
            var y = byte.Parse(token[1]);
            var user = this.Context.User.Identity.Name;
            var game = gameService.GetGameByID(gameId);
            if (!game.IsTurn(user))
            {
                await this.Clients.Caller.SendAsync("statusLine", new Massage() { Content = "It`s not your turn!" });
            }
            else
            {
                var currentStatus = game.Turn(user, x, y);
                var opponent = game.GetOpponentUsernameAndConnId(user);
                if (currentStatus == StatusGame.Ok)
                {
                    await this.Clients.Client(opponent.ConnectionId).SendAsync("turn", new Point(x, y, PlayerTwoSym));
                    await this.Clients.Caller.SendAsync("turn", new Point(x, y, PlayerOneSym));
                }
                else if (currentStatus == StatusGame.Winner)
                {
                    await this.Clients.Client(opponent.ConnectionId).SendAsync("lose", new Point(x, y, PlayerTwoSym));
                    await this.Clients.Caller.SendAsync("win", new Point(x, y, PlayerOneSym));
                    game.ClearGame();
                }
                else if (currentStatus == StatusGame.End)
                {
                    await this.Clients.Client(opponent.ConnectionId).SendAsync("end", new Point(x, y, PlayerTwoSym));
                    await this.Clients.Caller.SendAsync("end", new Point(x, y, PlayerOneSym));
                    game.ClearGame();
                }
                else if (currentStatus == StatusGame.Error)
                {
                    await this.Clients.Caller.SendAsync("statusLine", new Massage() { Content = "Error!" });
                }
            }


        }

        public override Task OnConnectedAsync()
        {
            var userName = this.Context.User.Identity.Name;
            var userConnId = this.Context.ConnectionId;
            var gameId = gameService.StartGame(userName, userConnId);

            if (!string.IsNullOrEmpty(gameId))
            {
                var game = gameService.GetGameByID(gameId);
                var opponent = game.GetOpponentUsernameAndConnId(userName);
                this.Clients.All.SendAsync("NewMessage", new Massage() { Content = userName + " StartGame!" });
                this.Clients.Caller.SendAsync("NewGame", new Massage() { Content = gameId });
                this.Clients.Client(opponent.ConnectionId).SendAsync("NewGame", new Massage() { Content = gameId });
            }
            else
            {
                this.Clients.Caller.SendAsync("NewMessage", new Massage() { Content = userName + " Waiting for player!" });
            }


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userName = this.Context.User.Identity.Name;
            if (!gameService.RemoveWaitingPlayer(userName))
            {
                KeyValuePair<string, NewGame> gameIdandObject = gameService.GetGameByPlayer(userName);
                var gameId = gameIdandObject.Key;
                var game = gameIdandObject.Value;
                var opponent = game.GetOpponentUsernameAndConnId(userName);
                this.Clients.Client(opponent.ConnectionId).SendAsync("connClose", new Massage() { Content=$"Player {userName} discconect!!!"});
                gameService.RemoveGame(gameId);

            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
