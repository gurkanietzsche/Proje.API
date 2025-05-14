﻿namespace Proje.UI.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}