﻿namespace PowerAPI.Dto
{
    public class RefreshTokenDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
