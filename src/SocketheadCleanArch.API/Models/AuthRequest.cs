﻿namespace SocketheadCleanArch.API.Models;

public class AuthRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}