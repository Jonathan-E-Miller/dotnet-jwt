﻿namespace TokenAuthenticator;

public class InvalidConfigurationException : Exception
{
    public InvalidConfigurationException(string message) : base(message)
    {
    }
}