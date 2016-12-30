using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jatan.Models;

namespace JatanWebApp.SignalR
{
    /// <summary>
    /// Player data transfer object.
    /// </summary>
    public class PlayerDTO
    {
        public int Id { get; set; }

        public PlayerDTO(Player player)
        {
            this.Id = player.Id;
        }
    }
}