using System;

namespace Server
{
    public class NameAlreadyExistsException : Exception
    {
        public override string Message => "Ya existe un usuario con ese nombre";
    }
}
